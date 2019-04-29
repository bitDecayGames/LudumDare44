using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class GenerateTags : MonoBehaviour
{
    private const string OutputPath = "Assets/Scripts/Constants/Tags.cs";
    
    [MenuItem("Tools/Generate Tags")]
    public static void CreateTags()
    {
        string classDefinition = string.Empty;

        classDefinition += "public class Tags\n";

        classDefinition += "{\n";


        List<string> tags = new List<string>(InternalEditorUtility.tags);
        foreach (string tag in tags)
        {
            classDefinition += string.Format("\tpublic const string {0} = \"{0}\";\n", tag);
        }

        classDefinition += "}\n";
        
        File.WriteAllText(OutputPath, classDefinition);
    }
}