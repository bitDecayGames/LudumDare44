using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float DurationSeconds = 60;
    public bool complete;
    float timerSeconds;

    void Start()
    {
        Reset();
    }

    void Update()
    {
        if (complete)
        {
            return;
        }

        timerSeconds -= Time.deltaTime;
        if (timerSeconds <= 0)
        {
            MarkComplete();
        }
    }

    void MarkComplete()
    {
        Debug.Log("Timer Complete");
        complete = true;
    }

    public void Reset()
    {
        timerSeconds = DurationSeconds;
        complete = false;
    }
}