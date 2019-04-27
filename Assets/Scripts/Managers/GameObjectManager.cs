using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
    
    [SerializeField]
    private List<GameObject> RegisteredGameObjects = new List<GameObject>();

    public void RegisterGameObject(GameObject gameObject)
    {
        RegisteredGameObjects.Add(gameObject);
    }

    public bool DeregisterGameObject(GameObject gameObject)
    {
        return RegisteredGameObjects.Remove(gameObject);
    }
    
    public GameObject GetGameObject(int index)
    {
        if (RegisteredGameObjects.Count < index)
        {
            throw new Exception("Game object registry out of bounds exception");
        }

        return RegisteredGameObjects[index];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            Debug.Log(String.Format("Registered game object count: {0}", RegisteredGameObjects.Count));
        }
    }
}