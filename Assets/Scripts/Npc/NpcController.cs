using UnityEngine;

public class NpcController : MonoBehaviour
{
    public Task task;
    public TaskStep taskStep;

    public TaskStepType taskStepType;

    private void Update()
    {
        
    }

    public void AssignStep(TaskStep step)
    {
        taskStep = step;
        taskStepType = taskStep.type;

        switch (step.type)
        {
            case TaskStepType.Safe:
                //GetComponent<NpcMovementController>().MoveToPlace(task.type, taskManager.CompleteTaskStep(taskStep.type)) 
                break;
            
            case TaskStepType.VacuumTube:
                break;
        }
    }

    public void CompleteCurrentStep()
    {
        FindObjectOfType<TaskManager>().CompleteTaskStep(taskStep.type, true);
    }
}