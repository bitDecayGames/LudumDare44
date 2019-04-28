using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskType {
    DepositMoney,
    FillCashRegister,
    EmptyCashRegister,
    OpenBankDoor
}

public class Task : MonoBehaviour
{
    public const float StepCompletionTimeReduction = 10f;

    public GameObject SomeNpc;
    public TaskManager taskManager;
    
    List<GameObject> Icons = new List<GameObject>();
    
    public TaskType type;
    public List<TaskStep> steps;
    GameObject npc;
    NpcController npcController;
    public float TimeAlive;
    public int lineNumber;
    public bool lineTask = false;

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
        npc = Instantiate(SomeNpc);
        npcController = npc.GetComponent<NpcController>();
        npcController.task = this;
        CreateIconsForStep(steps[0]);
    }
    
    void Update()
    {
        TimeAlive += Time.deltaTime;
    }

    void OnDestroy()
    {
        Destroy(npc);
    }

    public void CreateIconsForStep(TaskStep taskStep)
    {
        switch (taskStep.icon)
        {
            case IconManager.Icon.CashRegister:
                Icons.Add(IconManager.GetLocalReference().CreateIcon(IconManager.Icon.CashRegister, npc.transform));
                break;
            
            default:
                break;
        }
    }

    public void ClearIcons()
    {
        foreach (GameObject icon in Icons)
        {
            Destroy(icon);
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
        if (IsComplete())
        {
            return CustomerMood.HAPPY;
        }
        
        if (TimeAlive < 20)
        {
            return CustomerMood.NUETRAL;
        }
        return CustomerMood.ANGRY;
    }

    public void CompleteStep(TaskStepType type, bool npcStep) {
        for (int i = 0; i < steps.Count; i++)
        {
            if (steps[i].complete)
            {
                continue;
            }

            if (steps[i].type == type && steps[i].npcStep == npcStep)
            {
                steps[i].complete = true;
                TimeAlive -= StepCompletionTimeReduction;
                if (TimeAlive <= 0)
                {
                    TimeAlive = 0;
                }

                ClearIcons();
                if (StepsLeft() > 0)
                {
                    npcController.AssignStep(steps[i + 1]);
                    CreateIconsForStep(steps[i + 1]);
                }
            }
            break;
        }
    }

    public uint StepsLeft()
    {
        uint stepsLeft = 0;
        foreach (TaskStep ts in steps)
        {
            if (!ts.complete) {
                stepsLeft++;
            }
        }
        return stepsLeft;
    }

    public uint StepsComplete()
    {
        uint stepsComplete = 0;
        foreach (TaskStep ts in steps)
        {
            if (ts.complete) {
                stepsComplete++;
            }
        }
        return stepsComplete;

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