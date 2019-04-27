using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum TaskType {
    DepositMoney,

}

public class Task : MonoBehaviour
{
    TaskType type;
    List<TaskStep> tasks;
    GameObject npc;

    void Start()
    {
        tasks = new List<TaskStep>();
    }
    
    void Update()
    {
        
    }
}