using UnityEngine;

public enum NPCMood
{
    HAPPY,
    NEUTRAL,
    ANGRY
}

public class NpcController : MonoBehaviour
{
    TaskStep taskStep;

    TaskStepType taskStepType;
    GameObject currentIcon;
    Movement.NpcMove npcMovement;
    TaskManager taskManager;
    private bool isInited = false;

    // float TimeAlive;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        if(!isInited)
        {
            CreateIcon();
            npcMovement = GetComponent<Movement.NpcMove>();
            npcMovement.Initialize();
            taskManager = FindObjectOfType<TaskManager>();
            isInited = true;
        }
    }

    void OnDestroy()
    {
        ClearIcon();
    }

    void Update()
    {
        // TimeAlive += Time.deltaTime;
        // // TODO Move
        // TimeAlive -= StepCompletionTimeReduction;
        // if (TimeAlive <= 0)
        // {
        //     TimeAlive = 0;
        // }

    }

    void CreateIcon()
    {
        currentIcon = IconManager.GetLocalReference().CreateIcon(Icon.Elipsis, gameObject.transform);
    }

    void ClearIcon()
    {
        Destroy(currentIcon);
    }

    // public NPCMood GetCustomerMood()
    // {
    //     if (task.IsComplete())
    //     {
    //         return CustomerMood.HAPPY;
    //     }
        
    //     if (TimeAlive < 20)
    //     {
    //         return CustomerMood.NEUTRAL;
    //     }

    //     return CustomerMood.ANGRY;
    // }

    public void AssignStep(TaskStep step)
    {
        taskStep = step;
        taskStepType = taskStep.type;

        npcMovement.Move(taskStep, () => { taskManager.CompleteTaskStep(step.type, npcMovement.gameObject); });
    }
}