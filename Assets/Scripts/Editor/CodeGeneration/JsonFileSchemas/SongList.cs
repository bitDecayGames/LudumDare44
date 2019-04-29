using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SongList
{
    public List<string> Songs;
    
    public static SongList LoadSongsList(string path)
    {
        StreamReader reader = new StreamReader(path);
        var saveDataJson = reader.ReadToEnd();
        reader.Close();

        SongList songList = JsonUtility.FromJson<SongList>(saveDataJson);
        return songList;
    }
}