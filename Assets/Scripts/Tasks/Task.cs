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
    ATMDeposit,
    OpenAccount,
    CheckCashing,
    VacuumTubeDeposit,
    VacuumTubeCoinChange
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
                npcController.AssignStep(this, steps.Peek());
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
            
        if (board.board.poiLocations.ContainsKey("npcSpawn".ToLower()))
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
        var spawnPoints = board.poiLocations["npcSpawn".ToLower()].FindAll(p => p != null && p.myNode != null).ConvertAll(p => p.myNode);
        List<Board.Board.Node> possibleSpawns = spawnPoints.FindAll(n => n.occupier == null && !n.npcOffLimits);
        if (possibleSpawns.Count > 0) return possibleSpawns[UnityEngine.Random.Range(0, possibleSpawns.Count)];
        possibleSpawns.Clear();
        spawnPoints.ForEach(n => {
            possibleSpawns.AddRange(ExpandNode(n.up));
            possibleSpawns.AddRange(ExpandNode(n.right));
            possibleSpawns.AddRange(ExpandNode(n.down));
            possibleSpawns.AddRange(ExpandNode(n.left));
        });
        possibleSpawns = possibleSpawns.FindAll(n => n != null && n.occupier == null && !n.npcOffLimits);
        if (possibleSpawns.Count > 0) return possibleSpawns[UnityEngine.Random.Range(0, possibleSpawns.Count)];
        return null;
    }

    public static List<Board.Board.Node> ExpandNode(Board.Board.Node node) {
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
        MaybeAddCar(step);
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
            GameObject iconObj = IconManager.GetLocalReference().CreateIconForNonPerson(step.icon, step.node.occupier.transform, offset);
            Icons.Add(iconObj);
        } else {


            Board.BoardManager boardManager = FindObjectOfType<Board.BoardManager>();
            string lowerName = TaskStep.GetStepName(step.type).ToLower();
            if (!boardManager.board.stepLocations.ContainsKey(lowerName)) throw new Exception("Couldn't find task step type in the board: " + lowerName);
            List<Board.Board.Occupier> locations = boardManager.board.stepLocations[lowerName];
            foreach (var loc in locations)
            {
                GameObject iconObj = IconManager.GetLocalReference().CreateIconForNonPerson(step.icon, loc.gameObject.transform, offset);
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

            List<Board.Board.Node> allStepTypeNodes = new List<Board.Board.Node>();
            var allStepTypeOccupiers = bm.board.stepLocations.ContainsKey(step.type.ToString().ToLower()) ? bm.board.stepLocations[step.type.ToString().ToLower()] : new List<Board.Board.Occupier>();
            foreach (var occupier in allStepTypeOccupiers)
            {
                allStepTypeNodes.Add(occupier.myNode);
            }

            List<Board.Board.POI> pois;
            if (allStepTypeNodes.Count == 0) {
                pois = bm.board.poiLocations.ContainsKey(step.type.ToString().ToLower()) ? bm.board.poiLocations[step.type.ToString().ToLower()] : new List<Board.Board.POI>();
                foreach (var poi in pois)
                {
                    allStepTypeNodes.Add(poi.myNode);
                }
                
                if (pois.Count == 0) throw new Exception("No nodes found on the map for step type: " + step.type);
            }

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

    void MaybeAddCar(TaskStep step)
    {
        if (!step.addCar) {
            return;
        }

        CarController cc = FindObjectOfType<CarController>();
        if (cc != null) {
            cc.CreateCar();
        }
    }

    void MaybeRemoveCar(TaskStep step)
    {
        if (!step.removeCar) {
            return;
        }

        RemoveCar();
    }

    void RemoveCar()
    {
        CarController cc = FindObjectOfType<CarController>();
        if (cc != null) {
            cc.RemoveCar();
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

    void FeedbackPositive(string msg)
    {
       if (taskManager != null && taskManager.Feedback != null)
        {
            var player = FindObjectOfType<PlayerTaskController>();
            taskManager.Feedback.Positive(msg, player.transform);
        }
    }

    void FeedbackNegative(string msg)
    {
        FMODSoundEffectsPlayer.Instance.PlaySoundEffect(SFX.CustomerLeft);
        if (taskManager != null && taskManager.Feedback != null)
        {
            var player = FindObjectOfType<PlayerTaskController>();
            taskManager.Feedback.Negative(msg, player.transform);
        }
    }

    public void Fail()
    {
        steps.Clear();
        ClearIcons();
        failed = true;

        Score.FailedTasks++;
        Score.TotalTasks++;

        var player = FindObjectOfType<PlayerTaskController>();
        FeedbackNegative("Customer left");
        RemoveCar();
        
        TaskStep leaveStep = 
            TaskStep.Create()
                .Type(TaskStepType.LeaveBuilding)
                .SetIcon(Icon.Angry)
                .NPC();

        AddStep(leaveStep);
        npcController.AssignStep(this, leaveStep);
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

//TODO make so npc can only commplete npc steps

        if (!completer.tag.Equals("Player") && SomeNpc != completer)
        {
            // Debug.Log("Wrong NPC tried to complete a task");
            return;
        }

        if (completer.tag.Equals("Player") && currentStep.npcStep) return;
        if (!completer.tag.Equals("Player") && !currentStep.npcStep) return;

        steps.Dequeue();
        currentStep.complete = true;
        completedSteps.Add(currentStep);

        MaybeRemoveCar(currentStep);
        if (steps.Count > 0)
        {
            MaybeAddCar(steps.Peek());
        }

        if (currentStep.lastStepForSuccess) {
            Score.CompletedTasks++;
            Score.TotalTasks++;
        }
        if (currentStep.SFX != null) {
            FMODSoundEffectsPlayer.Instance.PlaySoundEffect(currentStep.SFX);

            switch (currentStep.type)
            {
                case TaskStepType.Safe:
                    FeedbackPositive("Cash Grabbed");
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
                    if (!currentStep.lastStepForSuccess && completer.tag.Equals("Player") && this.type == TaskType.DepositMoney)
                    {
                        FMODSoundEffectsPlayer.Instance.PlaySoundEffect(SFX.GreetCustomer);
                        
                        FeedbackPositive("Customer Greeted");
                    }
                    else if (currentStep.lastStepForSuccess && completer.tag.Equals("Player"))
                    {
                        FMODSoundEffectsPlayer.Instance.PlaySoundEffect(SFX.ByeCustomer);                    
                        
                        FeedbackPositive("Happy Customer!");
                    } 
                    else if (this.type == TaskType.OpenBankDoor && completer.tag.Equals("Player"))
                    {
                        FMODSoundEffectsPlayer.Instance.PlaySoundEffect(SFX.CashRegister);                    
                        
                        FeedbackPositive("Register Filled");
                        
                        Destroy(GameObject.Find("TutorialCanvas"));
                    } 
                    else if (this.type == TaskType.ChangeIntoCash && completer.tag.Equals("Player"))
                    {
                        FMODSoundEffectsPlayer.Instance.PlaySoundEffect(SFX.Coinstar);                    
                        
                        FeedbackPositive("Coins converted to bills");
                    } 
                    break;
            }
        }

        // Show feedback for different tasks, without a bunch of other crazy logic
        switch (currentStep.type)
        {
            case TaskStepType.AccountComputer:

                var player = FindObjectOfType<PlayerTaskController>();
                if (taskManager != null && taskManager.Feedback != null)
                {
                    if (!currentStep.npcStep)
                    {
                        FMODSoundEffectsPlayer.Instance.PlaySoundEffect(SFX.ByeCustomer);   
                        taskManager.Feedback.Positive("New Account Opened!", player.transform);
                    }
                }
                break;
        }

        ClearIcons();

        if (IsComplete() || IsFailed())
        {
            return;
        }

        TaskStep nextStep = steps.Peek();
        if(nextStep.npcStep)
        {
            npcController.AssignStep(this, nextStep);
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