using System;
using System.Collections;
using System.Collections.Generic;
using Board;
using Managers;
using Menus;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class TaskManager : MonoBehaviour
{
    const string TAG = "Task";

    public NpcObjects Npcs;
    public FeedbackManager Feedback;

    public float LevelDurationSeconds = 120;
    private Timer timer;
    private EasyNavigator navigator;

    uint NumTasks = 5;
    int NumRandomTasks;

    // Time before a new task is created. (Seconds)
    static float TaskBetweenTime = 1f;
    float TimeToNewTasks = TaskBetweenTime;

    static int MaxNumberLines = 1;
    int currentLine = 0;
    private bool isLevelEnded = false;

    List<String> possibleStepList;
    List<TaskType> possibleTaskList;
    private Task levelStartTask;

    void Start() {
        timer = gameObject.AddComponent<Timer>();
        timer.DurationSeconds = LevelDurationSeconds;
        timer.Reset();

        navigator = FindObjectOfType<EasyNavigator>();
        if (navigator == null) throw new Exception("Couldn't find an EasyNavigator, probably a bug in the OneScriptToRuleThemAll or it's prefab");
        
        Score.ResetScore();

        // Create the list of possible tasks to create in current level
        possibleStepList = GetPossibleStepList();
        possibleTaskList = CreatePossibleTaskList();
        if(possibleTaskList.Count == 0) throw new Exception ("Couldn't create any possible tasks. Something has gone wrong");
        NumRandomTasks = possibleTaskList.Count;

        // Create the initial task for the player to start the level
        GameObject newTaskObj = new GameObject();
        var task = newTaskObj.AddComponent(typeof(Task)) as Task;
        TaskBuilder.GetStartingTask(task, "");
        newTaskObj.transform.SetParent(this.transform);
        newTaskObj.name = Task.GetTaskName(task.type);
        newTaskObj.tag = TAG;
        levelStartTask = task;

    }
    
    void Update()
    {
        if (levelStartTask != null)
        {
            if (!levelStartTask.IsComplete())
            {
                // wait for player to finish the start task
                return;
            }
            else
            {
                FMODSoundEffectsPlayer.Instance.PlaySoundEffect(SFX.FemaleAngry);
                levelStartTask = null;
            }
        }
        
        
        var numCurrentTasks = GetComponentsInChildren<Task>().Length;

        if (numCurrentTasks < NumTasks)
        {
            TimeToNewTasks -= Time.deltaTime;
            
            if (TimeToNewTasks <= 0) {
                CreateTaskGameObj();
                TimeToNewTasks = TaskBetweenTime;
            }
        }

        if (timer.complete && !isLevelEnded) {
            EndLevel();
        }
    }

    public void EndLevel() {
        isLevelEnded = true;
        Score.LevelName = SceneManager.GetActiveScene().name;
        var rnd = new System.Random();
        for (int i = 0; i < Score.CompletedTasks; i++) {
            Score.Money += 5 * rnd.Next(1, 10);
        }
        Score.Happiness = Score.TotalTasks == 0 ? 0 : (int)Math.Round(Score.CompletedTasks / (float) Score.TotalTasks * 10);
        Score.NextLevelName = "Somehow get the next level name?";
        
        navigator.GoToScene("Score");
    }

    public void CompleteTaskStep(TaskStepType type, GameObject completer)
    {
        GameObject[] taskObjs = GameObject.FindGameObjectsWithTag(TAG);
        foreach (GameObject go in taskObjs)
        {
            Task task = go.GetComponent<Task>();

            TaskStep nextStep = task.steps.Peek();

            BoardManager bm = FindObjectOfType<BoardManager>();
            BoardPosition completerPos = completer.GetComponent<BoardPosition>();
            Board.Board.Node completerNode = bm.board.Get(completerPos.X, completerPos.Y);

            var expandedNextStepNode = Task.ExpandNode(nextStep.node);
            foreach (var node in expandedNextStepNode)
            {
                if (node.Equals(completerNode)){
                    task.CompleteStep(type, completer);
                    if (task.IsComplete()) {
                        Destroy(go);
                    }
                }

                break;
            }
        }
    }

    public void NpcCompleteTaskStep(Task task, TaskStep step, GameObject completer) {
        task.CompleteStep(step.type, completer);
        if (task.IsComplete()) {
            Destroy(task.gameObject);
        }
    }

    public void CreateTaskGameObj(){
        GameObject newTaskObj = new GameObject();
        var task = newTaskObj.AddComponent(typeof(Task)) as Task;
        CreateRandomTask(task);
        task.SomeNpc = Instantiate(Npcs.PickOneAtRandom());
        newTaskObj.name = Task.GetTaskName(task.type);
        newTaskObj.tag = TAG;

        if (task.lineTask) {
            
            Queue<TaskStep> getInLineSteps = new Queue<TaskStep>();
            BoardManager bm = FindObjectOfType<BoardManager>();
            MaxNumberLines = bm.board.lineLocations.Count;
            
            // find valid line numbers by looping through tasks and line locations for matching meta strings
            List<int> validLineNumbers = new List<int>();
            string stepMeta = null;
            foreach (var step in task.steps) {
                if (step.meta != null) {
                    stepMeta = step.meta;
                }
            }
            if (stepMeta == null) throw new Exception("There was no step meta on any of the task steps.  You are probably missing a .Meta() call on your task builder for this task: " + task.type);
            stepMeta = stepMeta.ToLower();
            foreach (var line in bm.board.lineLocations) {
                var lineNum = line.Key;
                var meta = line.Value[0].Meta;
                if (meta != null && meta.ToLower() == stepMeta) validLineNumbers.Add(lineNum);
            }
            if (validLineNumbers.Count == 0) throw new Exception(string.Format("Could not find any valid lines with the meta ({0}).  You are probably missing some line configuration on this tiled map.", stepMeta));
            task.lineNumber = validLineNumbers[UnityEngine.Random.Range(0, validLineNumbers.Count)];
            Debug.Log("Picked line number: " + task.lineNumber);
            
            foreach (Board.Board.POI poi in bm.board.lineLocations[task.lineNumber])
            {
                TaskStep getInLine = TaskStep.Create().Type(TaskStepType.GetInLine).NPC(true);
                getInLine.node = poi.myNode;
                getInLineSteps.Enqueue(getInLine);
            }

            Queue<TaskStep> newSteps = new Queue<TaskStep>();
            foreach (TaskStep oldStep in task.steps)
            {
                if(oldStep.type != TaskStepType.GetInLine)
                {
                    newSteps.Enqueue(oldStep);
                } else {
                    foreach (TaskStep ts in getInLineSteps)
                    {
                        newSteps.Enqueue(ts);
                    };
                }
            }

            task.steps = newSteps;
        }

        newTaskObj.transform.SetParent(this.transform);
    }

    public List<Board.Board.Node> GetNodesFromStepType(TaskStepType stepType)
    {
        List<Board.Board.Node> stepNodes = new List<Board.Board.Node>();
        var tasks = GetComponentsInChildren<Task>();
        foreach (var task in tasks)
        {
            var nextStep = task.steps.Peek();
            if(nextStep.type == stepType)
            {
                stepNodes.Add(nextStep.node);
            }
        }
        return stepNodes;
    }

    public void CreateRandomTask(Task task){
        var rand = new System.Random();
        int taskNumber = rand.Next(NumRandomTasks);
        // Debug.Log("Rand task number to spawn: " + taskNumber);
        task.type = possibleTaskList[taskNumber];
        TaskBuilder.CreateTask(task);
    }

    public List<String> GetPossibleStepList()
    {
        List<String> possibleStepList = new List<String>();
        BoardManager bm = FindObjectOfType<BoardManager>();
        var allStepLocations = bm.board.stepLocations.Keys;
        foreach (String key in allStepLocations)
        {
            if (!possibleStepList.Contains(key))
            {
                possibleStepList.Add(key);
            }
        }

        return possibleStepList;
    }

    public List<TaskType> CreatePossibleTaskList()
    {
        List<TaskType> possibleTaskList = new List<TaskType>();
        var allTaskAndStepDetails = TaskBuilder.GetAllTaskAndStepDetails();
        foreach (var taskStepCombo in allTaskAndStepDetails)
        {
            bool taskPossible = true;
            foreach (var step in taskStepCombo.Value)
            {
                if (!possibleStepList.Contains(step))
                {
                    Debug.Log("Couldn't create task "+taskStepCombo.Key+", no step found for " + step);
                    taskPossible = false;
                    break;
                }
            }

            if (taskPossible) possibleTaskList.Add(taskStepCombo.Key);
        }

        return possibleTaskList;
    }
}
