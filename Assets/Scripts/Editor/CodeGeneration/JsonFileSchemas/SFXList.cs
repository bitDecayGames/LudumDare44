using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SFXList
{
    public List<string> SFX;
    
    public static SFXList LoadSFXList(string path)
    {
        StreamReader reader = new StreamReader(path);
        var saveDataJson = reader.ReadToEnd();
        reader.Close();

        SFXList sfxList = JsonUtility.FromJson<SFXList>(saveDataJson);
        return sfxList;
    }
}