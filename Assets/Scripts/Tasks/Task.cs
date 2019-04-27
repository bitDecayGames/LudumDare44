using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskType {
    DepositMoney,

}

public class Task : MonoBehaviour
{
    public TaskType type;
    List<TaskStep> steps;
    GameObject npc;

    public static string GetTaskName(TaskType type)
    {
        switch (type) {
            case TaskType.DepositMoney:
                return "Deposit Money";
        }

        return "Unkown Task Type";
    }

    void Start()
    {
        steps = new List<TaskStep>();
        TaskStep moveToSafeStep = new TaskStep();
        moveToSafeStep.type = TaskStepType.MoveToSafe;
        steps.Add(moveToSafeStep);
    }
    
    void Update()
    {
        
    }

    public void CompleteStep(TaskStepType type) {
        foreach (TaskStep ts in steps)
        {
            if (ts.complete)
            {
                continue;
            }

            if (ts.type == type)
            {
                ts.complete = true;
                break;
            }
        }
    }

    public bool IsComplete()
    {
        foreach (TaskStep ts in steps)
        {
            if (!ts.complete) {
                return false;
            }
        }

        return true;
    }
}