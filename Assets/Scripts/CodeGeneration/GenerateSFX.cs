using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class GenerateSFX : MonoBehaviour
{
    private const string InputConfigPath = "Assets/Scripts/Configs/SFX.json";
    private const string OutputPath = "Assets/Scripts/Constants/SFX.cs";
    
    [MenuItem("Tools/Generate SFX")]
    public static void CreateSFX()
    {
        string classDefinition = string.Empty;

        classDefinition += "public class SFX\n";

        classDefinition += "{\n";


        SFXList sfxList = SFXList.LoadSFXList(InputConfigPath);
        
        foreach (string sound in sfxList.SFX)
        {
            classDefinition += string.Format("\tpublic const string {0} = \"{0}\";\n", sound);
        }

        classDefinition += "}\n";
        
        File.WriteAllText(OutputPath, classDefinition);
    }
}