using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    const string TAG = "Task";

    uint NumTasks = 2;

    void Start()
    {
        for (uint i = 0; i < NumTasks; i++)
        {
            GameObject newTaskObj = new GameObject();
            newTaskObj.name = Task.GetTaskName(TaskType.DepositMoney);
            newTaskObj.tag = TAG;
            newTaskObj.transform.SetParent(this.transform);

            Task task = newTaskObj.AddComponent(typeof(Task)) as Task;
            task.type = TaskType.DepositMoney;
        }
    }
    
    void Update()
    {

    }

    public void CompleteStep(TaskStepType type)
    {
        GameObject[] taskObjs = GameObject.FindGameObjectsWithTag(TAG);
        foreach (GameObject go in taskObjs)
        {
            Task task = go.GetComponent<Task>();
            task.CompleteStep(type);

            if (task.IsComplete()) {
                Destroy(go);
            }
        }
    }
}
