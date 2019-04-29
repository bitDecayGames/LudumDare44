using System;
using System.Collections;
using System.Collections.Generic;

public static class TaskBuilder
{

    public static void CreateTask (Task task)
    {
        switch (task.type)
        {
            case TaskType.DepositMoney:
                DepositMoney(task);
                break;
            case TaskType.FillCashRegister:
                FillCashRegister(task);
                break;
            case TaskType.EmptyCashRegister:
                EmptyCashRegister(task);
                break;
            case TaskType.OpenBankDoor:
                OpenBankDoor(task);
            break;
           case TaskType.ChangeIntoCash:
               ChangeIntoCash(task);
               break;
           case TaskType.ATMDeposit:
               ATMDeposit(task);
               break;
            case TaskType.OpenAccount:
                OpenAccount(task);
            break;
            case TaskType.CheckCashing:
                CheckCashing(task);
            break;
            case TaskType.VacuumTubeDeposit:
                VacuumTubeDeposit(task);
            break;
            case TaskType.VacuumTubeCoinChange:
                VacuumTubeCoinChange(task);
            break;
        }
    }

    public static Dictionary<TaskType, List<String>> GetAllTaskAndStepDetails()
    {
        Dictionary<TaskType, List<String>> dict = new Dictionary<TaskType, List<String>>();
        dict.Add(TaskType.DepositMoney, DepositMoney());
        // dict.Add(TaskType.FillCashRegister, FillCashRegister());
        // dict.Add(TaskType.EmptyCashRegister, EmptyCashRegister());
        dict.Add(TaskType.OpenBankDoor, OpenBankDoor());
        dict.Add(TaskType.ChangeIntoCash, ChangeIntoCash());
        dict.Add(TaskType.ATMDeposit, ATMDeposit());
        dict.Add(TaskType.OpenAccount, OpenAccount());
        dict.Add(TaskType.CheckCashing, CheckCashing());
        dict.Add(TaskType.VacuumTubeDeposit, VacuumTubeDeposit());
        dict.Add(TaskType.VacuumTubeCoinChange, VacuumTubeCoinChange());

        return dict;
    }
    
    public static void GetStartingTask(Task task, string level)
    {
        task.type = TaskType.OpenBankDoor;

        TaskStep.Create()
            .Type(TaskStepType.Safe)
            .SetIcon(Icon.Money)
            .AddTo(task);
        
        TaskStep.Create()
            .Type(TaskStepType.CashRegister)
            .SetIcon(Icon.Money)
            .AddTo(task);
    }

    static void DepositMoney(Task task){
        task.type = TaskType.DepositMoney;
        task.lineTask = true;
        
        TaskStep.Create()
            .Type(TaskStepType.WaitZone)
            .NPC()
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.GetInLine)
            .NPC()
            .Meta(TaskStepType.CashRegister)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.CashRegister)
            .NPC()
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
            .TriggersSuccess()
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.LeaveBuilding)
            .NPC()
            .AddTo(task);     
    }

    static List<String> DepositMoney()
    {
        List<String> stepList = new List<String>();
        stepList.Add(TaskStepType.CashRegister.ToString().ToLower());
        stepList.Add(TaskStepType.Safe.ToString().ToLower());
        stepList.Add(TaskStepType.LeaveBuilding.ToString().ToLower());
        return stepList;
    }

    static void ChangeIntoCash(Task task) {
        task.type = TaskType.ChangeIntoCash;
        task.lineTask = true;
        
        TaskStep.Create()
            .Type(TaskStepType.WaitZone)
            .NPC()
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.GetInLine)
            .NPC()
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.CashRegister)
            .NPC()
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
            .TriggersSuccess()
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.LeaveBuilding)
            .NPC()
            .AddTo(task);        
    }
    
    static List<String> ChangeIntoCash()
    {
        List<String> stepList = new List<String>();
        stepList.Add(TaskStepType.CashRegister.ToString().ToLower());
        stepList.Add(TaskStepType.CoinMachine.ToString().ToLower());
        stepList.Add(TaskStepType.LeaveBuilding.ToString().ToLower());
        return stepList;
    }

    static void ATMDeposit(Task task)
    {
        task.type = TaskType.ATMDeposit;
        task.lineTask = true;

        TaskStep.Create()
            .Type(TaskStepType.GetInLine)
            .NPC()
            .Meta(TaskStepType.ATM)
            .AddTo(task);
        
        TaskStep.Create()
            .Type(TaskStepType.ATM)
            .NPC()
            .AddTo(task);

        // MW this will make the check icon appear for a second I think
        TaskStep.Create()
            .Type(TaskStepType.ATM)
            .SetIcon(Icon.Check)
            .NPC()
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.LeaveBuilding)
            .NPC()
            .AddTo(task);      
    }
    
    static List<String> ATMDeposit()
    {
        List<String> stepList = new List<String>();
        stepList.Add(TaskStepType.ATM.ToString().ToLower());
        stepList.Add(TaskStepType.LeaveBuilding.ToString().ToLower());
        return stepList;
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

    static List<String> FillCashRegister()
    {
        List<String> stepList = new List<String>();
        stepList.Add(TaskStepType.CashRegister.ToString().ToLower());
        stepList.Add(TaskStepType.Safe.ToString().ToLower());
        return stepList;
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
 
    static List<String> EmptyCashRegister()
    {
        List<String> stepList = new List<String>();
        stepList.Add(TaskStepType.CashRegister.ToString().ToLower());
        stepList.Add(TaskStepType.Safe.ToString().ToLower());
        return stepList;
    }

    static void OpenBankDoor(Task task) {
        task.type = TaskType.OpenBankDoor;

        TaskStep.Create()
            .Type(TaskStepType.BankDoor)
            .AddTo(task);
    }

    static List<String> OpenBankDoor()
    {
        List<String> stepList = new List<String>();
        stepList.Add(TaskStepType.BankDoor.ToString().ToLower());
        return stepList;
    }

    static void OpenAccount(Task task)
    {
        task.type = TaskType.OpenAccount;
        
        TaskStep.Create()
            .Type(TaskStepType.AccountComputer)
            .NPC()
            .AddTo(task);
        
        TaskStep.Create()
            .Type(TaskStepType.AccountComputer)
            // Replace with account open icon
            .SetIcon(Icon.Open)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.LeaveBuilding)
            .NPC()
            .AddTo(task);      
    }
    
    static List<String> OpenAccount()
    {
        List<String> stepList = new List<String>();
        stepList.Add(TaskStepType.AccountComputer.ToString().ToLower());
        stepList.Add(TaskStepType.LeaveBuilding.ToString().ToLower());
        return stepList;
    }

    static void CheckCashing(Task task)
    {
        task.type = TaskType.CheckCashing;
        task.lineTask = true;

        TaskStep.Create()
            .Type(TaskStepType.GetInLine)
            .NPC()
            .Meta(TaskStepType.CashRegister)
            .AddTo(task);
        
        TaskStep.Create()
            .Type(TaskStepType.CashRegister)
            .NPC()
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.CashRegister)
            .SetIcon(Icon.Check)
            .AddTo(task);      

        TaskStep.Create()
            .Type(TaskStepType.LeaveBuilding)
            .NPC()
            .AddTo(task);      
    }
    
    static List<String> CheckCashing()
    {
        List<String> stepList = new List<String>();
        stepList.Add(TaskStepType.CashRegister.ToString().ToLower());
        stepList.Add(TaskStepType.LeaveBuilding.ToString().ToLower());
        return stepList;
    }

    static void VacuumTubeDeposit(Task task)
    {
        task.type = TaskType.VacuumTubeDeposit;

        // TODO Make car somehow????
        TaskStep.Create()
            .Type(TaskStepType.VacuumTube)
            .SetIcon(Icon.VacuumIn)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.Safe)
            .SetIcon(Icon.Money)
            .AddTo(task);      

        TaskStep.Create()
            .Type(TaskStepType.VacuumTube)
            .SetIcon(Icon.VacuumOut)
            .AddTo(task);      
    }
    
    static List<String> VacuumTubeDeposit()
    {
        List<String> stepList = new List<String>();
        stepList.Add(TaskStepType.Safe.ToString().ToLower());
        stepList.Add(TaskStepType.VacuumTube.ToString().ToLower());
        return stepList;
    }

    static void VacuumTubeCoinChange(Task task)
    {
        task.type = TaskType.VacuumTubeCoinChange;

        // TODO Make car somehow????
        TaskStep.Create()
            .Type(TaskStepType.VacuumTube)
            .SetIcon(Icon.VacuumIn)
            .AddTo(task);

        TaskStep.Create()
            .Type(TaskStepType.CoinMachine)
            .SetIcon(Icon.Coins)
            .AddTo(task);      

        TaskStep.Create()
            .Type(TaskStepType.VacuumTube)
            .SetIcon(Icon.VacuumOut)
            .AddTo(task);      
    }
    
    static List<String> VacuumTubeCoinChange()
    {
        List<String> stepList = new List<String>();
        stepList.Add(TaskStepType.CoinMachine.ToString().ToLower());
        stepList.Add(TaskStepType.VacuumTube.ToString().ToLower());
        return stepList;
    }
}