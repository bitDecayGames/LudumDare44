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
        moveToSafeStep.type = TaskStepType.Safe;
        task.steps.Add(moveToSafeStep);

        TaskStep moveToSafeStep2 = new TaskStep();
        moveToSafeStep2.type = TaskStepType.Safe;
        task.steps.Add(moveToSafeStep2);
    }

    static void FillCashRegister(Task task) {
        task.type = TaskType.FillCashRegister;

        TaskStep moveToTeller = new TaskStep();
        moveToTeller.type = TaskStepType.EmptyCashRegister;
        task.steps.Add(moveToTeller);

        TaskStep moveToSafe = new TaskStep();
        moveToSafe.type = TaskStepType.Safe;
        task.steps.Add(moveToSafe);

        TaskStep moveToTeller2 = new TaskStep();
        moveToTeller2.type = TaskStepType.FullCashRegister;
        task.steps.Add(moveToTeller2);
    }

    static void OpenBankDoor(Task task) {
        task.type = TaskType.OpenBankDoor;

        TaskStep moveToBankDoor = new TaskStep();
        moveToBankDoor.type = TaskStepType.BankDoor;
        task.steps.Add(moveToBankDoor);
    }
}