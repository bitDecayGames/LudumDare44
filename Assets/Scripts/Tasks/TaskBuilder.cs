using System;

public static class TaskBuilder
{
    private static int numRandomTasks = 3;

    public static void CreateRandomTask(Task task){
        var rand = new Random();
        int taskNumber = rand.Next(numRandomTasks);
//        int taskNumber = 0; // TODO: MW put this back
        // Debug.Log("Random task number: " + taskNumber);

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

        TaskStep.Create()
            .Type(TaskStepType.GetInLine)
            .NPC(true)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.CashRegister)
            .NPC(true)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.CashRegister)
            .SetIcon(Icon.Elipsis)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.Safe)
            .SetIcon(Icon.Money)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.CashRegister)
            .SetIcon(Icon.Elipsis)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.LeaveBuilding)
            .NPC(true)
            .AddTo(task);     
    }

    static void ChangeIntoCash(Task task) {
        task.type = TaskType.ChangeIntoCash;
        task.lineTask = true;

        TaskStep.Create()
            .Type(TaskStepType.GetInLine)
            .NPC(true)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.CashRegister)
            .NPC(true)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.CashRegister)
            .SetIcon(Icon.Elipsis)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.CoinMachine)
            .SetIcon(Icon.Coins)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.CashRegister)
            .SetIcon(Icon.Coins)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.LeaveBuilding)
            .NPC(true)
            .AddTo(task);        
    }

    static void ATMDeposit(Task task)
    {
        task.type = TaskType.ATMDeposit;

        TaskStep.Create()
            .Type(TaskStepType.Safe)
            .SetIcon(Icon.Check)
            .NPC(true)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.LeaveBuilding)
            .NPC(true)
            .AddTo(task);      
    }

    static void FillCashRegister(Task task) {
        task.type = TaskType.FillCashRegister;

        TaskStep.Create()
            .Type(TaskStepType.CashRegister)
            .SetIcon(Icon.Empty)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.Safe)
            .SetIcon(Icon.Money)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.CashRegister)
            .SetIcon(Icon.Money)
            .AddTo(task);
    }

    static void EmptyCashRegister(Task task) {
        task.type = TaskType.EmptyCashRegister;

        TaskStep.Create()
            .Type(TaskStepType.CashRegister)
            .SetIcon(Icon.FullRegister)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.Safe)
            .SetIcon(Icon.Money)
            .AddTo(task);
    }

    static void OpenBankDoor(Task task) {
        task.type = TaskType.OpenBankDoor;

        TaskStep.Create()
            .Type(TaskStepType.BankDoor)
            .AddTo(task);
    }
}