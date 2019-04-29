using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang.Runtime;
using UnityEngine;

public enum TaskType {
    DepositMoney,
    FillCashRegister,
    EmptyCashRegister,
    OpenBankDoor,
    ChangeIntoCash,
    VacuumTubeDeposit,
    ATMDeposit
}

public class Task : MonoBehaviour
{
    public GameObject SomeNpc;
    public TaskManager taskManager;
    
    List<GameObject> Icons = new List<GameObject>();
    
    public TaskType type;
    public Queue<TaskStep> steps;
    List<TaskStep> completedSteps;
    NpcController npcController;
    public int lineNumber;
    public bool lineTask = false;
    public int numSteps = 0;
    bool failed;

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
        if (HaveNpcSteps())
        {
            npcController = SomeNpc.GetComponent<NpcController>();
            if (npcController == null)
            {
                throw new RuntimeException("NPC did not have NPC Controller on prefab");
            }
            npcController.Init();
            npcController.AssignTask(this);
            if(steps.Peek().npcStep)
            {
                npcController.AssignStep(steps.Peek());
            }
        }
        CreateIconsForStep(steps.Peek());
        taskManager = GetComponentInParent<TaskManager>();
    }
    
    void Update()
    {
        numSteps = steps.Count;

    }

    void OnDestroy()
    {
        SomeNpc.GetComponent<NpcController>().Kill();
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
        if (!boardManager.board.stepLocations.ContainsKey(lowerName)) throw new Exception("Couldn't find task step type in the board: " + lowerName);
        List<Board.Board.Occupier> locations = boardManager.board.stepLocations[lowerName];
        foreach (var loc in locations)
        {
            // TODO Move this offset somewhere else.
            Vector3 offset = new Vector3(-0.25f, 1f, 0f);
            GameObject iconObj;
            // If the game object has an underlying sprite, follow that instead
            if (loc.gameObject.transform.Find("Sprite") != null)
            {
                 iconObj = IconManager.GetLocalReference().CreateIcon(step.icon, loc.gameObject.transform.Find("Sprite").transform, offset);
            }
            else
            {
                iconObj = IconManager.GetLocalReference().CreateIcon(step.icon, loc.gameObject.transform, offset);    
            }
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

    public void Fail()
    {
        steps.Clear();
        ClearIcons();
        failed = true;
        TaskStep leaveStep = new TaskStep(TaskStepType.LeaveBuilding, Icon.Angry, true);
        AddStep(leaveStep);
        npcController.AssignStep(leaveStep);
    }

    public void CompleteStep(TaskStepType type, GameObject completer) {
        if (steps.Count <= 0)
        {
            Debug.Log("BEHAVIOR UNKNOWN: Tried to complete a task step with no steps remaining");
            return;
        }
        TaskStep currentStep = steps.Peek();
        if (currentStep.type != type)
        {
            Debug.LogWarning("Step " + type + " cannot be completed");
            return;
        }

        if (completer != null && SomeNpc != completer)
        {
            Debug.Log("Wrong NPC tried to complete a task");
            return;
        }

        steps.Dequeue();
        currentStep.complete = true;
        completedSteps.Add(currentStep);
        Debug.Log(type + " Complete");

        if (completer == null)
        {
            var player = FindObjectOfType<PlayerTaskController>();
            if (taskManager != null && taskManager.Feedback != null) taskManager.Feedback.Positive("you got there mfer!!", player.transform);
        }

        ClearIcons();
        if (!IsComplete() && !IsFailed())
        {
            TaskStep nextStep = steps.Peek();
            if(nextStep.npcStep)
            {
                npcController.AssignStep(nextStep);
            }
            CreateIconsForStep(nextStep);
        }
    }

    public bool IsComplete()
    {
        return steps.Count <= 0 && !failed;
    }

    public bool IsFailed()
    {
        return failed;
    }

    private bool HaveNpcSteps(){
        bool haveNpcSteps = false;
        foreach (TaskStep step in steps)
        {
            if(step.npcStep)
            {
                haveNpcSteps = true;
                break;
            }
        }
        return haveNpcSteps;
    }
}