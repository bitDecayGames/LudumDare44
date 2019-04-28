using System;
using System.Collections;
using System.Collections.Generic;
using Board;
using Menus;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class TaskManager : MonoBehaviour
{
    const string TAG = "Task";

    public NpcObjects Npcs;

    public float LevelDurationSeconds = 10;
    private Timer timer;
    private EasyNavigator navigator;

    uint NumTasks = 1;

    // Time before a new task is created. (Seconds)
    static float TaskBetweenTime = 1f;
    float TimeToNewTasks = TaskBetweenTime;

    bool TaskResentlyComplete = false;

    static int MaxNumberLines = 3;
    static int LineLength = 3;
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
            task.lineNumber = currentLine;
            currentLine++;
            if(currentLine > MaxNumberLines) currentLine = 0;

            List<TaskStep> newSteps;
            // TODO get nodes for line.
            // make lots of steps with those nodes for the lines.
            // add other steps and stuffs


        }

        newTaskObj.transform.SetParent(this.transform);
    }
}
