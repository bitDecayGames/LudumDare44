using UnityEngine;
using Utils;

public class NpcController : MonoBehaviour
{
    Task task;
    TaskStep taskStep;

    TaskStepType taskStepType;
    GameObject currentIcon;
    Movement.NpcMove npcMovement;
    TaskManager taskManager;
    private bool isInited = false;

    float MaxWaitTimeSeconds = 30;
    float totalWaitTime;

    bool waiting;
    bool angry;
    bool leaving;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        if(!isInited)
        {
            npcMovement = GetComponent<Movement.NpcMove>();
            npcMovement.Initialize();
            taskManager = FindObjectOfType<TaskManager>();
            totalWaitTime = MaxWaitTimeSeconds;
            isInited = true;
        }
    }

    public void Kill() {
        gameObject.AddComponent<KillAfterTime>().timeToKill = 3f;
        gameObject.AddComponent<SpriteRendererFadeOutOverTime>().timeToFadeOut = 2f;
    }

    void OnDestroy() {
        ClearIcon();
    }

    void Update()
    {
        totalWaitTime -= Time.deltaTime;
        CalculateMood();
    }

    void CalculateMood()
    {
        if (taskStepType == TaskStepType.LeaveBuilding)
        {
            if (!leaving)
            {
                ClearIcon();
                leaving = true;
            }
            return;
        }

        if (ShouldWait())
        {
            CreateIcon(Icon.Waiting);
            waiting = true;
        }
        else if (ShouldAnger())
        {
            CreateIcon(Icon.Angry);
            angry = true;
        }
        else if (ShouldLeave())
        {
            leaving = true;
            task.Fail();
        }
    }

    bool ShouldWait()
    {
        // 2/3
        return totalWaitTime <= 2 * MaxWaitTimeSeconds / 3 && !waiting;
    }

    bool ShouldAnger()
    {
        // 2/3
        return totalWaitTime <= MaxWaitTimeSeconds / 3 && !angry;
    }

    bool ShouldLeave()
    {
        return totalWaitTime <= 0 && !leaving;
    }

    void CreateIcon(Icon icon)
    {
        ClearIcon();
        currentIcon = IconManager.GetLocalReference().CreateIcon(icon, gameObject.transform.Find("Sprite").transform);
    }

    void ClearIcon()
    {   
        if (currentIcon != null) {
            Destroy(currentIcon);
            currentIcon = null;
        }
    }

    public void AssignTask(Task t)
    {
        task = t;
    }

    public void AssignStep(TaskStep step)
    {
        taskStep = step;
        taskStepType = taskStep.type;

        npcMovement.Move(taskStep, () => { taskManager.CompleteTaskStep(step.type, npcMovement.gameObject); });
    }
}