using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    private static GameObjectManager instance;
    public static GameObjectManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject();
                instance = gameObject.AddComponent<GameObjectManager>();
                gameObject.name = "Game Object Manager";
            }

            return instance;
        }
    }
	
    void Awake ()
    {
        if (instance == null)
        {
            instance = this;
        }
		
        DontDestroyOnLoad(gameObject);
    }
    
    
    private List<GameObject> _registeredGameObjects = new List<GameObject>();

    public void RegisterGameObject(GameObject gameObject)
    {
        _registeredGameObjects.Add(gameObject);
    }

    public bool DeregisterGameObject(GameObject gameObject)
    {
        return _registeredGameObjects.Remove(gameObject);
    }
    
    public GameObject GetGameObject(int index)
    {
        if (_registeredGameObjects.Count < index)
        {
            throw new Exception("Game object registry out of bounds exception");
        }

        return _registeredGameObjects[index];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            Debug.Log(String.Format("Registered game object count: {0}", _registeredGameObjects.Count));
        }
    }
}