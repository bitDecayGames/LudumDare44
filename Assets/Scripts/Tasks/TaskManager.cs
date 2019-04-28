using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    const string TAG = "Task";

    uint NumTasks = 2;

    // Time before a new task is created. (Seconds)
    static float TaskBetweenTime = 1f;
    float TimeToNewTasks = TaskBetweenTime;

    bool TaskResentlyComplete = false;

    void Start()
    {

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

    }

    public void CompleteTaskStep(TaskStepType type)
    {
        GameObject[] taskObjs = GameObject.FindGameObjectsWithTag(TAG);
        foreach (GameObject go in taskObjs)
        {
            Task task = go.GetComponent<Task>();
            task.CompleteStep(type);

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
        newTaskObj.name = Task.GetTaskName(task.type);
        newTaskObj.tag = TAG;
        newTaskObj.transform.SetParent(this.transform);
    }
}
