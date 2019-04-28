using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyScriptableObjects", menuName = "NpcObjects", order = 1)]
public class NpcObjects : ScriptableObject {
    public List<GameObject> npcs = new List<GameObject>();

    private System.Random rnd = new System.Random();

    public GameObject PickOneAtRandom() {
        if (npcs.Count == 0) throw new Exception("The npc object list cannot be null, you need to pick the correct ScriptableObject for this.");
        return npcs[rnd.Next(npcs.Count)];
    }
}