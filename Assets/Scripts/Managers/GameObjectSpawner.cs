using System;
using UnityEngine;

public class GameObjectSpawner : MonoBehaviour
{
    public GameObject GenericGameObject;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Instantiate(GenericGameObject);
        }
    }
}