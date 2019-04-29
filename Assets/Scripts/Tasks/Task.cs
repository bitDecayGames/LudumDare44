using System;
using System.Collections;
using System.Collections.Generic;
using Board;
using Boo.Lang.Runtime;
using Menus;
using UnityEngine;

public enum TaskType {
    DepositMoney,
    FillCashRegister,
    EmptyCashRegister,
    OpenBankDoor,
    ChangeIntoCash,
    VacuumTubeDeposit,
    ATMDeposit
}

public class Task : MonoBehaviour
{
    public GameObject SomeNpc;
    public TaskManager taskManager;
    
    List<GameObject> Icons = new List<GameObject>();
    
    public TaskType type;
    public Queue<TaskStep> steps;
    List<TaskStep> completedSteps;
    NpcController npcController;
    public int lineNumber;
    public bool lineTask = false;
    public int numSteps = 0;
    private bool initialized = false;
    private bool failed;
    private bool success;

    public static string GetTaskName(TaskType type)
    {
        return System.Enum.GetName(typeof(TaskType), type);
    }

    public static string GetTaskNameReadable(TaskType type)
    {
        string typeName = System.Enum.GetName(typeof(TaskType), type);
        return System.Text.RegularExpressions.Regex.Replace(typeName, "(\\B[A-Z])", " $1");
    }

    public Task(){
        steps = new Queue<TaskStep>();
        completedSteps = new List<TaskStep>();
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        if (HaveNpcSteps())
        {
            InitNpc();
            npcController.AssignTask(this);
            if(steps.Peek().npcStep)
            {
                npcController.AssignStep(steps.Peek());
            }
        }
        TryAssignNodeToStep(steps.Peek());
        CreateIconsForStep(steps.Peek());
        taskManager = GetComponentInParent<TaskManager>();
        initialized = true;
    }

    private void InitNpc() {
        npcController = SomeNpc.GetComponent<NpcController>();
        if (npcController == null)
        {
            throw new RuntimeException("NPC did not have NPC Controller on prefab");
        }
        npcController.Init();
        var npcBoardPos = SomeNpc.GetComponentInChildren<BoardPosition>();
        var npcOccupier = SomeNpc.GetComponentInChildren<Board.Board.Occupier>();
        var board = FindObjectOfType<BoardManager>();
            
        if (board.board.poiLocations.ContainsKey("npcSpawn"))
        {
            // TODO make sure we spawn these thigns reasonably
            var mySpawn = pickSpawnLocation(board.board);
            if (mySpawn == null) throw new Exception("I tried and tried to find a suitable spawn location, but it couldn't be found.  I'm guessing there must be a bunch of collision objects around the spawn locations");
            npcBoardPos.X = mySpawn.x;
            npcBoardPos.Y = mySpawn.y;
            board.board.Set(npcOccupier, mySpawn.x, mySpawn.y);

        } else {
            throw new Exception("No Npc spawner has been is found");
        }
    }

    private Board.Board.Node pickSpawnLocation(Board.Board board) {
        var spawnPoints = board.poiLocations["npcSpawn"].FindAll(p => p != null && p.myNode != null).ConvertAll(p => p.myNode);
        List<Board.Board.Node> possibleSpawns = spawnPoints.FindAll(n => n.occupier == null && !n.npcOffLimits);
        if (possibleSpawns.Count > 0) return possibleSpawns[UnityEngine.Random.Range(0, possibleSpawns.Count)];
        possibleSpawns.Clear();
        spawnPoints.ForEach(n => {
            possibleSpawns.AddRange(expandNode(n.up));
            possibleSpawns.AddRange(expandNode(n.right));
            possibleSpawns.AddRange(expandNode(n.down));
            possibleSpawns.AddRange(expandNode(n.left));
        });
        possibleSpawns = possibleSpawns.FindAll(n => n != null && n.occupier == null && !n.npcOffLimits);
        if (possibleSpawns.Count > 0) return possibleSpawns[UnityEngine.Random.Range(0, possibleSpawns.Count)];
        return null;
    }

    private List<Board.Board.Node> expandNode(Board.Board.Node node) {
        var tmp = new List<Board.Board.Node>();
        if (node != null) {
            tmp.Add(node);
            if (node.up != null) tmp.Add(node.up);
            if (node.right != null) tmp.Add(node.right);
            if (node.down != null) tmp.Add(node.down);
            if (node.left != null) tmp.Add(node.left);
        }
        return tmp;
    }
    
    void Update()
    {
        if(!initialized) Init();
        numSteps = steps.Count;

    }

    void OnDestroy()
    {
        if (SomeNpc != null && SomeNpc.gameObject != null) {
            // MW had to do this because npcController would sometimes be null here... NPE!!!!
            var ctrl = SomeNpc.GetComponent<NpcController>();
            if (ctrl != null) ctrl.Kill();
        }
    }

    public void AddStep(TaskStep step)
    {
        steps.Enqueue(step);
    }

    void CreateIconsForStep(TaskStep step)
    {
        // TODO Move this offset somewhere else.
        Vector3 offset = new Vector3(-0.25f, 1f, 0f);
        if (step.icon == Icon.Empty) {
            // Debug.Log("Skipping icon for " + step);
            return;
        }

        // if the step has a node, use that location to set the icon. Otherwise set it fucking everywhere.
        if (step.node != null) {
            GameObject iconObj = IconManager.GetLocalReference().CreateIcon(step.icon, step.node.occupier.transform, offset);
            Icons.Add(iconObj);
        } else {


            Board.BoardManager boardManager = FindObjectOfType<Board.BoardManager>();
            string lowerName = TaskStep.GetStepName(step.type).ToLower();
            if (!boardManager.board.stepLocations.ContainsKey(lowerName)) throw new Exception("Couldn't find task step type in the board: " + lowerName);
            List<Board.Board.Occupier> locations = boardManager.board.stepLocations[lowerName];
            foreach (var loc in locations)
            {
                GameObject iconObj = IconManager.GetLocalReference().CreateIcon(step.icon, loc.gameObject.transform, offset);
                Icons.Add(iconObj);
            }
        }
    }

    void TryAssignNodeToStep(TaskStep step)
    {
        if (step.node == null)
        {
            Board.Board.Node newStepNode = null;
            Board.BoardManager bm = FindObjectOfType<Board.BoardManager>();

            if (!bm.board.stepLocations.ContainsKey(step.type.ToString().ToLower())) {
                throw new Exception("Tried to assign node to step, but couldn't find any step locations with: " + step.type);
            }
            var allStepTypeOccupiers = bm.board.stepLocations[step.type.ToString().ToLower()];
            List<Board.Board.Node> allStepTypeNodes = new List<Board.Board.Node>();
            foreach (var occupier in allStepTypeOccupiers)
            {
                allStepTypeNodes.Add(occupier.myNode);
            }
            if (allStepTypeNodes.Count == 0) throw new Exception("No nodes found on the map for step type: " + step.type);

            // If npc not null then attempt to set node based on the npc's position and stepType
            if (SomeNpc != null)
            {
                var npcPosition = SomeNpc.GetComponent<Board.BoardPosition>();
                var npcNode = bm.board.Get(npcPosition.X, npcPosition.Y);

                foreach(Board.Board.Node node in allStepTypeNodes)
                {
                    if (node.Equals(npcNode.up)) newStepNode = npcNode.up;
                    else if (node.Equals(npcNode.left)) newStepNode = npcNode.left;
                    else if (node.Equals(npcNode.right)) newStepNode = npcNode.right;
                    else if (node.Equals(npcNode.down)) newStepNode = npcNode.down;
                    else if (node.Equals(npcNode)) newStepNode = npcNode;
                }
            } 

            // If npc is null then attempt to set node based on stepType only
            // Currently randomly picking it. may do something else later. this could be dumb.
            if (newStepNode == null)
            {
                System.Random rand = new System.Random();
                int randomStepNode = rand.Next(allStepTypeNodes.Count);
                newStepNode = allStepTypeNodes[randomStepNode];
            }

            step.node = newStepNode;
        }
    }

    void ClearIcons()
    {
        foreach (GameObject icon in Icons)
        {
            Destroy(icon);
        }
        Icons.Clear();
    }

    public void Fail()
    {
        steps.Clear();
        ClearIcons();
        failed = true;

        Score.FailedTasks++;
        Score.TotalTasks++;

        //TODO Play fail sound here
        
        TaskStep leaveStep = 
            TaskStep.Create()
                .Type(TaskStepType.LeaveBuilding)
                .SetIcon(Icon.Angry)
                .NPC();

        AddStep(leaveStep);
        npcController.AssignStep(leaveStep);
    }

    public void CompleteStep(TaskStepType type, GameObject completer) {
        if (steps.Count <= 0)
        {
            Debug.Log("BEHAVIOR UNKNOWN: Tried to complete a task step with no steps remaining");
            return;
        }
        TaskStep currentStep = steps.Peek();
        if (currentStep.type != type)
        {
            // Debug.LogWarning("Step " + type + " cannot be completed");
            return;
        }

        if (completer != null && SomeNpc != completer)
        {
            // Debug.Log("Wrong NPC tried to complete a task");
            return;
        }

        steps.Dequeue();
        currentStep.complete = true;
        completedSteps.Add(currentStep);
        if (currentStep.lastStepForSuccess) {
            Score.CompletedTasks++;
            Score.TotalTasks++;
        }
        if (currentStep.SFX != null) {
            FMODSoundEffectsPlayer.Instance.PlaySoundEffect(currentStep.SFX);

            switch (currentStep.type)
            {
                case TaskStepType.Safe:
                    var player = FindObjectOfType<PlayerTaskController>();
                    if (taskManager != null && taskManager.Feedback != null) taskManager.Feedback.Positive("Cash Grabbed", player.transform);
                    break;
            }
        }
        else
        {
            switch (currentStep.type)
            {
                case TaskStepType.Safe:
                    FMODSoundEffectsPlayer.Instance.PlaySoundEffect(SFX.Bills);
                    break;
                case TaskStepType.CashRegister:
                    if (!currentStep.lastStepForSuccess && completer == null && this.type == TaskType.DepositMoney)
                    {
                        FMODSoundEffectsPlayer.Instance.PlaySoundEffect(SFX.GreetCustomer);
                        
                        Debug.Log("task type: " + this.type);
                        
                        var player = FindObjectOfType<PlayerTaskController>();
                        if (taskManager != null && taskManager.Feedback != null) taskManager.Feedback.Positive("Customer Greeted", player.transform);
                    }
                    else if (currentStep.lastStepForSuccess && completer == null)
                    {
                        FMODSoundEffectsPlayer.Instance.PlaySoundEffect(SFX.ByeCustomer);                    
                        
                        var player = FindObjectOfType<PlayerTaskController>();
                        if (taskManager != null && taskManager.Feedback != null) taskManager.Feedback.Positive("Happy Customer!", player.transform);
                    } 
                    else if (this.type == TaskType.OpenBankDoor && completer == null)
                    {
                        FMODSoundEffectsPlayer.Instance.PlaySoundEffect(SFX.CashRegister);                    
                        
                        var player = FindObjectOfType<PlayerTaskController>();
                        if (taskManager != null && taskManager.Feedback != null) taskManager.Feedback.Positive("Register Filled", player.transform);
                        
                        Destroy(GameObject.Find("TutorialCanvas"));
                    } 

                    break;
            }
        }

        ClearIcons();

        if (IsComplete() || IsFailed())
        {
            return;
        }

        TaskStep nextStep = steps.Peek();
        if(nextStep.npcStep)
        {
            npcController.AssignStep(nextStep);
        }
        TryAssignNodeToStep(nextStep);
        CreateIconsForStep(nextStep);
    }

    public bool IsComplete()
    {
        return steps.Count <= 0;
    }

    public bool IsFailed()
    {
        return failed;
    }

    private bool HaveNpcSteps(){
        bool haveNpcSteps = false;
        foreach (TaskStep step in steps)
        {
            if(step.npcStep)
            {
                haveNpcSteps = true;
                break;
            }
        }
        return haveNpcSteps;
    }
}