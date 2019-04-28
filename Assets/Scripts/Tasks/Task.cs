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
    Queue<TaskStep> steps;
    List<TaskStep> completedSteps;
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
        steps = new Queue<TaskStep>();
        completedSteps = new List<TaskStep>();
    }

    void Start()
    {
        npc = Instantiate(SomeNpc);
        npcController = npc.GetComponent<NpcController>();
        npcController.task = this;
        CreateIconsForStep(steps.Peek());
    }
    
    void Update()
    {
        TimeAlive += Time.deltaTime;
    }

    void OnDestroy()
    {
        Destroy(npc);
    }

    public void AddStep(TaskStep step)
    {
        steps.Enqueue(step);
    }

    void CreateIconsForStep(TaskStep step)
    {
        Icons.Add(IconManager.GetLocalReference().CreateIcon(step.icon, npc.transform));
    }

    void ClearIcons()
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
        TaskStep currentStep = steps.Peek();
        if (currentStep.type != type || currentStep.npcStep != npcStep)
        {
            Debug.LogWarning("Step " + type + " cannot be completed");
            return;
        }

        steps.Dequeue();
        currentStep.complete = true;
        completedSteps.Add(currentStep);
        Debug.Log(type + " Complete");

        TimeAlive -= StepCompletionTimeReduction;
        if (TimeAlive <= 0)
        {
            TimeAlive = 0;
        }

        ClearIcons();
        if (!IsComplete())
        {
            TaskStep nextStep = steps.Peek();
            npcController.AssignStep(nextStep);
            CreateIconsForStep(nextStep);
        }
    }

    public bool IsComplete()
    {
        return steps.Count <= 0;
    }
}