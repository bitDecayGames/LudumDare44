using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GenerateSongs : MonoBehaviour
{
    private const string InputConfigPath = "Assets/Scripts/Configs/Songs.json";
    private const string OutputPath = "Assets/Scripts/Constants/Songs.cs";
    
    [MenuItem("Tools/Generate Songs")]
    public static void CreateSongs()
    {
        string classDefinition = string.Empty;
        
        classDefinition += "public class Songs\n";

        classDefinition += "{\n";

        classDefinition += "\tpublic enum SongName\n";
        classDefinition += "\t{\n";
        
        SongList songList = SongList.LoadSongsList(InputConfigPath);
        
        foreach (string songName in songList.Songs)
        {
            byte[] sceneAsByteArray = System.Text.Encoding.UTF8.GetBytes(songName.ToCharArray());
            byte songEnumByteValue = 0;
            foreach (byte b in sceneAsByteArray)
            {
                songEnumByteValue += b;
            }
            classDefinition += string.Format("\t\t{0} = {1},\n", songName, songEnumByteValue);
        }
        
        classDefinition += "\t}\n";
        classDefinition += "}\n";
        
        File.WriteAllText(OutputPath, classDefinition);
    }
}