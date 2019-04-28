using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskType {
    DepositMoney
}

public class Task : MonoBehaviour
{
    public const float StepCompletionTimeReduction = 10f; 
    
    public TaskType type;
    public List<TaskStep> steps;
    public int TotalSteps; // For debugging purposes
    public int StepsLeftToComplete; // For debugging purposes
    GameObject npc;
    public float TimeAlive;

    public static string GetTaskName(TaskType type)
    {
        return System.Enum.GetName(typeof(TaskType), type);
    }

    public Task(){
        steps = new List<TaskStep>();
    }

    void Start()
    {

    }
    
    void Update()
    {
        TimeAlive += Time.deltaTime;
        TotalSteps = steps.Count;
        StepsLeftToComplete = 0;
        foreach (TaskStep taskStep in steps)
        {
            if (!taskStep.complete)
            {
                StepsLeftToComplete++;
            }
        }
    }

    public enum CustomerMood
    {
        HAPPY,
        NUETRAL,
        ANGRY
    }
    public CustomerMood GetCustomerMood()
    {
        if (TimeAlive < 10)
        {
            return CustomerMood.HAPPY;
        } 
        
        if (TimeAlive < 20)
        {
            return CustomerMood.NUETRAL;
        }
        return CustomerMood.ANGRY;
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
                StepsLeftToComplete--;
                TimeAlive -= StepCompletionTimeReduction;
                if (TimeAlive <= 0)
                {
                    TimeAlive = 0;
                }
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