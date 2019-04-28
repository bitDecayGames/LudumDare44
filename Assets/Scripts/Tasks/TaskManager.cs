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

    public float LevelDurationSeconds = 10000;
    private Timer timer;
    private EasyNavigator navigator;

    uint NumTasks = 1;

    // Time before a new task is created. (Seconds)
    static float TaskBetweenTime = 1f;
    float TimeToNewTasks = TaskBetweenTime;

    bool TaskResentlyComplete = false;

    static int MaxNumberLines = 0;
    int currentLine = 0;

    void Start() {
        timer = gameObject.AddComponent<Timer>();
        timer.DurationSeconds = LevelDurationSeconds;
        timer.Reset();

        navigator = FindObjectOfType<EasyNavigator>();
        if (navigator == null) throw new Exception("Couldn't find an EasyNavigator, probably a bug in the OneScriptToRuleThemAll or it's prefab");
    }
    
    void Update()
    {
        if (!TaskResentlyComplete)
        {
            var numCurrentTasks = GetComponentsInChildren<Task>().Length;
            if (numCurrentTasks < NumTasks) {
                CreateTaskGameObj();
            }
        } else {
            TimeToNewTasks -= Time.deltaTime;
            if (TimeToNewTasks <= 0) {
                TimeToNewTasks = TaskBetweenTime;
                TaskResentlyComplete = false;
            }
        }

        if (timer.complete) {
            EndLevel();
        }
    }

    public void EndLevel() {
        Score.LevelName = SceneManager.GetActiveScene().name;
        // TODO: MW
        Score.Money = 1000;
        Score.Happiness = 10;
        Score.TotalTasks = 30;
        Score.CompletedTasks = 20;
        Score.FailedTasks = 10;
        Score.NextLevelName = "Somehow get the next level name?";
        
        navigator.GoToScene("Score");
    }

    public void CompleteTaskStep(TaskStepType type, bool npcStep)
    {
        GameObject[] taskObjs = GameObject.FindGameObjectsWithTag(TAG);
        foreach (GameObject go in taskObjs)
        {
            Task task = go.GetComponent<Task>();
            task.CompleteStep(type, npcStep);

            if (task.IsComplete()) {
                Destroy(go);
                TaskResentlyComplete = true;
            }
        }
    }

    public void CreateTaskGameObj(){
        GameObject newTaskObj = new GameObject();
        var task = newTaskObj.AddComponent(typeof(Task)) as Task;
        TaskBuilder.CreateRandomTask(task);
        task.SomeNpc = Npcs.PickOneAtRandom();
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
                TaskStep getInLine = new TaskStep();
                getInLine.type = TaskStepType.GetInLine;
                getInLine.npcStep = true;
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
}
