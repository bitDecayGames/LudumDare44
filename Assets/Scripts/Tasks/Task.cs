using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskType {
    DepositMoney
}

public class Task : MonoBehaviour
{
    public TaskType type;
    public List<TaskStep> steps;
    GameObject npc;

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
        steps = new List<TaskStep>();
    }

    void Start()
    {

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
            }
            break;
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