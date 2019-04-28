using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyScriptableObjects", menuName = "IconObjects", order = 2)]
public class IconObjects : ScriptableObject {
    public List<GameObject> icons = new List<GameObject>();
    
    private System.Random rnd = new System.Random();
    
    public GameObject PickOneAtRandom() {
        if (icons.Count == 0) throw new Exception("The icon object list cannot be null, you need to pick the correct ScriptableObject for this.");
        return icons[rnd.Next(icons.Count)];
    }

    public GameObject GetByName(string name) {
        if (icons.Count == 0) throw new Exception("The icon object list cannot be null, you need to pick the correct ScriptableObject for this.");
        if (name == null) throw new Exception("You dingus... you can't give me a null name");
        return icons.Find(i => i.name.ToLower() == name.ToLower());
    }
}