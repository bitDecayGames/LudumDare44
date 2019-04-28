using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Icons;

public enum TaskStepType {
    Safe,
    VacuumTube,
    EmptyCashRegister,
    FullCashRegister,
    BankDoor,
    GetInLine,
    TalkToTeller
}

public class TaskStep
{

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

    public IconManager.Icon icon;
}