using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO ADD ICONS EVERYWHERE!!! SHIT!!

public static class TaskBuilder
{
    private static int numRandomTasks = 3;

    public static void CreateRandomTask(Task task){
        var rand = new System.Random();
        int taskNumber = rand.Next(numRandomTasks);
        Debug.Log("Random task number: " + taskNumber);

        switch (taskNumber)
        {
            case 0:
                DepositMoney(task);
                break;
            case 1:
                ChangeIntoCash(task);
                break;
            case 2:
                ATMDeposit(task);
                break;
            default:
                break;
        }

    }

    static void DepositMoney(Task task){
        task.type = TaskType.DepositMoney;
        task.lineTask = true;

        TaskStep getInLine = new TaskStep();
        getInLine.type = TaskStepType.GetInLine;
        getInLine.npcStep = true;
        task.AddStep(getInLine);

        TaskStep talkToCustomer = new TaskStep();
        talkToCustomer.type = TaskStepType.CashRegister;
        talkToCustomer.icon = Icon.Elipsis;
        task.AddStep(talkToCustomer);
        
        TaskStep depositMoney = new TaskStep();
        depositMoney.type = TaskStepType.Safe;
        depositMoney.icon = Icon.Angry;
        task.AddStep(depositMoney);

        TaskStep talkToCustomer2 = new TaskStep();
        talkToCustomer2.type = TaskStepType.CashRegister;
        talkToCustomer2.icon = Icon.Elipsis;
        task.AddStep(talkToCustomer2);
    }

    static void ChangeIntoCash(Task task) {
        task.type = TaskType.ChangeIntoCash;
        task.lineTask = true;

        TaskStep getInLine = new TaskStep();
        getInLine.type = TaskStepType.GetInLine;
        getInLine.npcStep = true;
        task.AddStep(getInLine);

        TaskStep talkToCustomer = new TaskStep();
        talkToCustomer.type = TaskStepType.CashRegister;
        talkToCustomer.icon = Icon.Elipsis;
        task.AddStep(talkToCustomer);
        
        TaskStep changeChange = new TaskStep();
        changeChange.type = TaskStepType.CoinMachine;
        changeChange.icon = Icon.Angry;
        task.AddStep(changeChange);

        TaskStep talkToCustomer2 = new TaskStep();
        talkToCustomer2.type = TaskStepType.CashRegister;
        talkToCustomer2.icon = Icon.Elipsis;
        task.AddStep(talkToCustomer2);

    }

    static void ATMDeposit(Task task)
    {
        task.type = TaskType.ATMDeposit;

        TaskStep depositATM = new TaskStep();
        depositATM.type = TaskStepType.ATM;
        depositATM.icon = Icon.Angry;
        depositATM.npcStep = true;
        task.AddStep(depositATM);
    }

    static void FillCashRegister(Task task) {
        task.type = TaskType.FillCashRegister;

        TaskStep moveToTeller = new TaskStep();
        moveToTeller.type = TaskStepType.CashRegister;
        task.AddStep(moveToTeller);

        TaskStep moveToSafe = new TaskStep();
        moveToSafe.type = TaskStepType.Safe;
        task.AddStep(moveToSafe);

        TaskStep moveToTeller2 = new TaskStep();
        moveToTeller2.type = TaskStepType.CashRegister;
        task.AddStep(moveToTeller2);
    }

    static void EmptyCashRegister(Task task) {
        task.type = TaskType.EmptyCashRegister;

        TaskStep moveToTeller = new TaskStep();
        moveToTeller.type = TaskStepType.CashRegister;
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