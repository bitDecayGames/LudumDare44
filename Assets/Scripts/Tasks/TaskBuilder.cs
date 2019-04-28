using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO ADD ICONS EVERYWHERE!!! SHIT!!
public static class TaskBuilder
{
    public static void CreateRandomTask(Task task){
        DepositMoney(task);
    }

    static void DepositMoney(Task task){
        task.type = TaskType.DepositMoney;
        task.lineTask = true;

        TaskStep getInLIine = new TaskStep();
        getInLIine.type = TaskStepType.GetInLine;
        task.steps.Add(getInLIine);

        TaskStep depositMoney = new TaskStep();
        depositMoney.type = TaskStepType.TalkToTeller;
        depositMoney.icon = IconManager.Icon.CashRegister;
        task.steps.Add(depositMoney);
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

    static void EmptyCashRegister(Task task) {
        task.type = TaskType.EmptyCashRegister;

        TaskStep moveToTeller = new TaskStep();
        moveToTeller.type = TaskStepType.FullCashRegister;
        task.steps.Add(moveToTeller);

        TaskStep moveToSafe = new TaskStep();
        moveToSafe.type = TaskStepType.Safe;
        task.steps.Add(moveToSafe);
    }

    static void OpenBankDoor(Task task) {
        task.type = TaskType.OpenBankDoor;

        TaskStep moveToBankDoor = new TaskStep();
        moveToBankDoor.type = TaskStepType.BankDoor;
        task.steps.Add(moveToBankDoor);
    }
}