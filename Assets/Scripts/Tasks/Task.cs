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
    public GameObject SomeNpc;
    public TaskManager taskManager;
    
    List<GameObject> Icons = new List<GameObject>();
    
    public TaskType type;
    public Queue<TaskStep> steps;
    List<TaskStep> completedSteps;
    GameObject npc;
    NpcController npcController;
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
        npcController.AssignStep(steps.Peek());
        CreateIconsForStep(steps.Peek());
    }
    
    void Update()
    {

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
        if (step.icon == Icon.Empty) {
            Debug.Log("Skipping icon for " + step);
            return;
        }

        Board.BoardManager boardManager = FindObjectOfType<Board.BoardManager>();
        string lowerName = TaskStep.GetStepName(step.type).ToLower();
        List<Board.Board.Occupier> locations = boardManager.board.stepLocations[lowerName];
        foreach (var loc in locations)
        {
            // TODO Move this offset somewhere else.
            Vector3 offset = new Vector3(-0.25f, 1f, 0f);
            GameObject iconObj = IconManager.GetLocalReference().CreateIcon(step.icon, loc.gameObject.transform, offset);
            Icons.Add(iconObj);
        }
    }

    void ClearIcons()
    {
        foreach (GameObject icon in Icons)
        {
            Destroy(icon);
        }
        Icons.Clear();
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