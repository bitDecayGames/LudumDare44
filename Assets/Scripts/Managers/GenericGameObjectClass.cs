using System;
using UnityEngine;

public class GenericGameObjectClass : MonoBehaviour
{
    private float _timeAlive;
    private void Awake()
    {
        GameObjectManager.Instance.RegisterGameObject(gameObject);
    }

    private void Update()
    {
        _timeAlive += Time.deltaTime;

        if (_timeAlive >= 10)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Self destructing. Deregistering self");
        
        bool success = GameObjectManager.Instance.DeregisterGameObject(gameObject);
        if (!success)
        {
            throw new Exception("Unable to deregister self from game object manager");
        }
    }
}