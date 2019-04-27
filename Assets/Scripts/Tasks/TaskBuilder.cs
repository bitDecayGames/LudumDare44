using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TaskBuilder
{
    public static void CreateRandomTask(Task task){
        DepositMoney(task);
    }
    static void DepositMoney(Task task){
        task.type = TaskType.DepositMoney;

        TaskStep moveToSafeStep = new TaskStep();
        moveToSafeStep.type = TaskStepType.MoveToSafe;
        task.steps.Add(moveToSafeStep);
    }


}