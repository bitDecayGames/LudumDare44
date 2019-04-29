using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskStepType {
    Safe,
    VacuumTube,
    BankDoor,
    GetInLine,
    CashRegister,
    CoinMachine,
    ATM,
    LeaveBuilding,
}

public class TaskStep
{

    public TaskStep(TaskStepType type, bool isNpc = false) {
        this.type = type;
        npcStep = isNpc;
    }

    public TaskStep(TaskStepType type, Icon icon, bool isNpc = false) {
        this.type = type;
        this.icon = icon;
        npcStep = isNpc;
    }
    
    public static string GetStepName(TaskStepType type)
    {
        return System.Enum.GetName(typeof(TaskStepType), type);
    }

    public static string GetStepNameReadable(TaskStepType type)
    {
        string typeName = System.Enum.GetName(typeof(TaskStepType), type);
        return System.Text.RegularExpressions.Regex.Replace(typeName, "(\\B[A-Z])", " $1");
    }

    public TaskStepType type;

    public bool complete;

    public bool npcStep = false;

    public Board.Board.Node node;

    public Icon icon = Icon.Empty;
}