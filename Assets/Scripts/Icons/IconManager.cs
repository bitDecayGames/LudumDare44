using System;
using UnityEngine;

public enum Icon
{
    Empty,
    Angry,
    Check,
    Coins,
    Elipsis,
    EmptyRegister,
    FullRegister,
    Happy,
    Money,
    Open,
    Shut,
    VacuumIn,
    VacuumOut,
    Waiting,
}

public class IconManager : MonoBehaviour
{
    private readonly Vector3 IconOffset = Vector3.up * 1.3f + Vector3.right * 0.9f;

    public IconObjects Icons;

    public static IconManager GetLocalReference()
    {
        return FindObjectOfType<IconManager>();
    }

    public static string GetIconName(Icon icon)
    {
        return System.Enum.GetName(typeof(Icon), icon);
    }

    public GameObject CreateIcon(Icon icon, Transform parent, Vector3 offset = new Vector3())
    {
        string name = GetIconName(icon);
        return Instantiate(Icons.GetByName(name), parent.position + IconOffset + offset, Quaternion.identity, parent);
    }
}