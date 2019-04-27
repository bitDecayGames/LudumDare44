using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum TaskStepType {
    DepositMoney,
    MoveToSafe
}

public class TaskStep : MonoBehaviour
{
    TaskStepType type;

    void Start()
    {
        type = TaskStepType.DepositMoney;
    }
    
    void Update()
    {
        
    }
}