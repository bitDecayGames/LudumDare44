using System;
using UnityEngine;

public enum Icon
{
    Angry,
    Elipsis,
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

    public GameObject CreateIcon(Icon icon, Transform parent)
    {
        string name = GetIconName(icon);
        return Instantiate(Icons.GetByName(name), parent.position + IconOffset, Quaternion.identity, parent);
    }
}