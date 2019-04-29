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
    

    public TaskStepType type;
    public bool complete;
    public bool npcStep = false;
    public Board.Board.Node node;
    public Icon icon = Icon.Empty;
    public string SFX;
    public bool lastStepForSuccess = false;
    
    public static TaskStep Create()
    {
        return new TaskStep();
    }

    public TaskStep Type(TaskStepType type)
    {
        this.type = type;
        return this;
    }

    public TaskStep NPC(bool isNpc = true)
    {
        npcStep = isNpc;
        return this;
    }

    public TaskStep SetIcon(Icon icon)
    {
        this.icon = icon;
        return this;
    }

    public TaskStep SFC(string sfx)
    {
        SFX = sfx;
        return this;
    }

    public TaskStep TriggersSuccess() {
        lastStepForSuccess = true;
        return this;
    }

    public void AddTo(Task task)
    {
        task.AddStep(this);
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

    public override string ToString() {
        return string.Format("[{0} at {2} Npc:{1} Icon:{3} Comp:{4}]", type, npcStep, node == null ? "(null_node)" : node.ToString(), icon, complete);
    }
}