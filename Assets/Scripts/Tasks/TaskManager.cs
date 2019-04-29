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

    uint NumTasks = 8;

    // Time before a new task is created. (Seconds)
    static float TaskBetweenTime = 1f;
    float TimeToNewTasks = TaskBetweenTime;

    static int MaxNumberLines = 2;
    int currentLine = 0;
    private bool isLevelEnded = false;

    void Start() {
        timer = gameObject.AddComponent<Timer>();
        timer.DurationSeconds = LevelDurationSeconds;
        timer.Reset();

        navigator = FindObjectOfType<EasyNavigator>();
        if (navigator == null) throw new Exception("Couldn't find an EasyNavigator, probably a bug in the OneScriptToRuleThemAll or it's prefab");
        
        Score.ResetScore();
    }
    
    void Update()
    {
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
            task.CompleteStep(type, completer);

            if (task.IsComplete()) {
                Destroy(go);
            }
        }
    }

    public void CreateTaskGameObj(){
        GameObject newTaskObj = new GameObject();
        var task = newTaskObj.AddComponent(typeof(Task)) as Task;
        TaskBuilder.CreateRandomTask(task);
        task.SomeNpc = Instantiate(Npcs.PickOneAtRandom());
        newTaskObj.name = Task.GetTaskName(task.type);
        newTaskObj.tag = TAG;

        if (task.lineTask) {
            
            Queue<TaskStep> getInLineSteps = new Queue<TaskStep>();
            BoardManager bm = FindObjectOfType<BoardManager>();
            MaxNumberLines = bm.board.lineLocations.Count;
            task.lineNumber = currentLine;
            currentLine++;
            if(currentLine >= MaxNumberLines) currentLine = 0;
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
}
