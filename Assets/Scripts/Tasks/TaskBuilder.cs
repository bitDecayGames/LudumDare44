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
//        int taskNumber = 0; // TODO: MW put this back
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
            // case 3:
            //     FillCashRegister(task);
            //     break;
            // case 4:
            //     EmptyCashRegister(task);
            //     break;
            default:
                break;
        }

    }

    static void DepositMoney(Task task){
        task.type = TaskType.DepositMoney;
        task.lineTask = true;

        task.AddStep(new TaskStep(TaskStepType.GetInLine, true));
        task.AddStep(new TaskStep(TaskStepType.CashRegister, true));
        task.AddStep(new TaskStep(TaskStepType.CashRegister, Icon.Elipsis));
        task.AddStep(new TaskStep(TaskStepType.Safe, Icon.Angry));
        task.AddStep(new TaskStep(TaskStepType.CashRegister, Icon.Elipsis));
        task.AddStep(new TaskStep(TaskStepType.LeaveBuilding, true));        
    }

    static void ChangeIntoCash(Task task) {
        task.type = TaskType.ChangeIntoCash;
        task.lineTask = true;

        task.AddStep(new TaskStep(TaskStepType.GetInLine, true));
        task.AddStep(new TaskStep(TaskStepType.CashRegister, true));
        task.AddStep(new TaskStep(TaskStepType.CashRegister, Icon.Elipsis));
        task.AddStep(new TaskStep(TaskStepType.CoinMachine, Icon.Coins));
        task.AddStep(new TaskStep(TaskStepType.CashRegister, Icon.Coins));
        task.AddStep(new TaskStep(TaskStepType.LeaveBuilding, true));        
    }

    static void ATMDeposit(Task task)
    {
        task.type = TaskType.ATMDeposit;

        task.AddStep(new TaskStep(TaskStepType.Safe, Icon.Angry, true));
        task.AddStep(new TaskStep(TaskStepType.LeaveBuilding, true));        
    }

    static void FillCashRegister(Task task) {
        task.type = TaskType.FillCashRegister;

        task.AddStep(new TaskStep(TaskStepType.CashRegister, Icon.Angry));
        task.AddStep(new TaskStep(TaskStepType.Safe, Icon.Angry));
        task.AddStep(new TaskStep(TaskStepType.CashRegister, Icon.Angry));
    }

    static void EmptyCashRegister(Task task) {
        task.type = TaskType.EmptyCashRegister;

        task.AddStep(new TaskStep(TaskStepType.CashRegister, Icon.Elipsis));
        task.AddStep(new TaskStep(TaskStepType.Safe, Icon.Angry));
    }

    static void OpenBankDoor(Task task) {
        task.type = TaskType.OpenBankDoor;

        task.AddStep(new TaskStep(TaskStepType.BankDoor));
    }
}