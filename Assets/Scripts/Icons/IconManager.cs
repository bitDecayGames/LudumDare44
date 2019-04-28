using System;
using UnityEngine;

public class IconManager : MonoBehaviour
{
    private readonly Vector3 IconOffset = Vector3.up * 1.3f + Vector3.right * 0.9f;

    public IconObjects Icons;

    public static IconManager GetLocalReference()
    {
        return FindObjectOfType<IconManager>();
    }
    
    public enum Icon
    {
        CashRegister
    }

    public GameObject CreateIcon(Icon icon, Transform parent)
    {
        switch (icon)
        {
           case Icon.CashRegister:
               return Instantiate(Icons.GetByName("Elipsis"), parent.position + IconOffset, Quaternion.identity, parent);
        }
        
        throw new Exception("Unable to determine icon");
    }
}