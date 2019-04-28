using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO ADD ICONS EVERYWHERE!!! SHIT!!
public static class TaskBuilder
{
    public static void CreateRandomTask(Task task){
        // EmptyCashRegister(task);
        // DepositMoney(task);
        TestTask(task);
    }

    static void TestTask(Task task)
    {
        task.type = TaskType.TestTask;

        TaskStep moveToSafe = new TaskStep();
        moveToSafe.type = TaskStepType.Safe;
        task.AddStep(moveToSafe);
    }

    static void DepositMoney(Task task){
        task.type = TaskType.DepositMoney;
        task.lineTask = true;

        TaskStep getInLine = new TaskStep();
        getInLine.type = TaskStepType.GetInLine;
        getInLine.npcStep = true;
        task.AddStep(getInLine);

        TaskStep depositMoney = new TaskStep();
        depositMoney.type = TaskStepType.TalkToTeller;
        task.AddStep(depositMoney);
    }

    static void FillCashRegister(Task task) {
        task.type = TaskType.FillCashRegister;

        TaskStep moveToTeller = new TaskStep();
        moveToTeller.type = TaskStepType.EmptyCashRegister;
        task.AddStep(moveToTeller);

        TaskStep moveToSafe = new TaskStep();
        moveToSafe.type = TaskStepType.Safe;
        task.AddStep(moveToSafe);

        TaskStep moveToTeller2 = new TaskStep();
        moveToTeller2.type = TaskStepType.FullCashRegister;
        task.AddStep(moveToTeller2);
    }

    static void EmptyCashRegister(Task task) {
        task.type = TaskType.EmptyCashRegister;

        TaskStep moveToTeller = new TaskStep();
        moveToTeller.type = TaskStepType.FullCashRegister;
        moveToTeller.icon = Icon.Elipsis;
        task.AddStep(moveToTeller);

        TaskStep moveToSafe = new TaskStep();
        moveToSafe.type = TaskStepType.Safe;
        moveToSafe.icon = Icon.Angry;
        task.AddStep(moveToSafe);
    }

    static void OpenBankDoor(Task task) {
        task.type = TaskType.OpenBankDoor;

        TaskStep moveToBankDoor = new TaskStep();
        moveToBankDoor.type = TaskStepType.BankDoor;
        task.AddStep(moveToBankDoor);
    }
}