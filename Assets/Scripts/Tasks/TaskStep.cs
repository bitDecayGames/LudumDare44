using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskStepType {
    MoveToSafe,
}

public class TaskStep
{
    public TaskStepType type;

    public bool complete;
}