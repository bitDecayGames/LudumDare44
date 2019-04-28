using System;
using UnityEngine;

public class IconManager : MonoBehaviour
{
    private readonly Vector3 IconOffset = Vector3.up * 1.3f + Vector3.right * 0.9f;
    public GameObject CashRegisterIcon;

    public static IconManager GetLocalReference()
    {
        return FindObjectOfType<IconManager>();
    }
    
    public enum Icon
    {
        CashRegister
    }

    public GameObject CreateIcon(Icon icon, Vector3 location)
    {
        switch (icon)
        {
           case Icon.CashRegister:
               return Instantiate(CashRegisterIcon, location + IconOffset, Quaternion.identity);
        }
        
        throw new Exception("Unable to determine icon");
    }
}