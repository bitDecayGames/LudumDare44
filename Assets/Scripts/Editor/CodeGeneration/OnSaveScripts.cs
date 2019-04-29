using System.IO;
using UnityEditor;
using UnityEngine;

public class OnSaveScripts : UnityEditor.AssetModificationProcessor
{
    public static string[] OnWillSaveAssets(string[] paths)
    {
        GenerateScenes.CreateScenes();
        GenerateSFX.CreateSFX();
        GenerateSongs.CreateSongs();
        GenerateTags.CreateTags();

        // Get the name of the scene to save.
        string scenePath = string.Empty;
        string sceneName = string.Empty;

        foreach (string path in paths)
        {
            if (path.Contains(".unity"))
            {
                scenePath = Path.GetDirectoryName(path);
                sceneName = Path.GetFileNameWithoutExtension(path);
            }
        }

        if (sceneName.Length == 0)
        {
            return paths;
        }

        AddCurrentSceneToBuildIfNotPresent(scenePath, sceneName);

        return paths;
    }

    private static void AddCurrentSceneToBuildIfNotPresent(string scenePath, string sceneName)
    {
        var original = EditorBuildSettings.scenes;

        foreach (EditorBuildSettingsScene editorBuildSettingsScene in original)
        {
            if (editorBuildSettingsScene.path.Contains(sceneName))
            {
                return;
            }
        }
        
        var newSettings = new EditorBuildSettingsScene[original.Length + 1]; 
        System.Array.Copy(original, newSettings, original.Length); 
        var sceneToAdd = new EditorBuildSettingsScene(Path.Combine(scenePath, sceneName) + ".unity", true); 
        newSettings[newSettings.Length - 1] = sceneToAdd; 
        EditorBuildSettings.scenes = newSettings;
    }
}