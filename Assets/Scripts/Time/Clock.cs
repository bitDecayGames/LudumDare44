using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    private float _hourOfDay;
    private float _minuteOfDay;

    public TextMeshPro ClockText;
    
    private void Start()
    {
        _hourOfDay = 12;
        _minuteOfDay = 0;
    }

    public void Update()
    {
        _minuteOfDay += Time.deltaTime;

        if (_minuteOfDay >= 60)
        {
            MoveForwardAnHour();
        } 
        else if (_minuteOfDay < 0)
        {
            MoveBackwardAnHour();
        }

        ClockText.text = GetDisplayTime();
    }
    
    
    private void MoveForwardAnHour()
    {
        _minuteOfDay = 0;
        _hourOfDay++;
        if (_hourOfDay > 23)
        {
            _hourOfDay = 0;
        }
    }

    private void MoveBackwardAnHour()
    {
        _minuteOfDay = 59;
        _hourOfDay--;
        if (_hourOfDay < 0)
        {
            _hourOfDay = 23;
        }
    }

    public float GetDayTimeAsFloat()
    {
        return (_hourOfDay / 24f) + ((_minuteOfDay / 60) / 24);
    }
    
    public string GetDisplayTime()
    {
        return String.Format("{0:D2}{1}{2:D2} {3}",
            //To get the hours to properly display on a 12 hour scale, the math looks weird
            GetHourOfDayTwelveHourClock(),
            Mathf.FloorToInt(Time.time) % 2 != 0 ? ":" : " ",
            Mathf.FloorToInt(_minuteOfDay),
            _hourOfDay < 12 ? "AM" : "PM" );
    }

    private int GetHourOfDayTwelveHourClock()
    {
        var hourOfDayAsInt = Mathf.FloorToInt(_hourOfDay);
        
        if (hourOfDayAsInt == 0)
        {
            return 12;
        } 
        if (hourOfDayAsInt <= 12)
        {
            return Mathf.FloorToInt(_hourOfDay);
        }
        if (hourOfDayAsInt > 12)
        {
            return hourOfDayAsInt % 12;
        }
        
        throw new Exception(String.Format("Unable to determine hour of day for clock. Hour value: {0}", hourOfDayAsInt));
    }
}