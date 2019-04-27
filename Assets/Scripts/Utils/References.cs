using System;
using UnityEngine;

public class References
{
    public static GameObject GetPlayerFromScene()
    {
        var playerGameObject = GameObject.FindGameObjectWithTag(Tags.Player);
        if (playerGameObject == null)
        {
            throw new Exception("Unable to find player in scene");
        }

        return playerGameObject;
    }
}