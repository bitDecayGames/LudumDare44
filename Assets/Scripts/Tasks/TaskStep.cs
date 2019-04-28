using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskStepType {
    MoveToSafe,
    MoveToVacuumTube
}

public class TaskStep
{

    public static string GetStepName(TaskStepType type)
    {
        return System.Enum.GetName(typeof(TaskStepType), type);
    }

    public TaskStepType type;

    public bool complete;
}