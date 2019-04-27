using System.Collections;
using System.Collections.Generic;
using SuperTiled2Unity;
using UnityEngine;

public class Loader : MonoBehaviour
{

    public BoardPosition playerPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        SuperMap map = FindObjectOfType<SuperMap>();
        foreach (var componentsInChild in map.GetComponentsInChildren<SuperObjectLayer>())
        {
            if ("Interactables" == componentsInChild.m_TiledName)
            {
                foreach (var superObject in map.GetComponentsInChildren<SuperObject>())
                {
                    if ("Spawn" == superObject.m_TiledName)
                    {
                        var player = Instantiate(playerPrefab, map.transform);
//                        player.transform.position = superObject.transform.position;
                        player.boardPos.x = (int)superObject.m_X / 8; // 8 is tile size
                        player.boardPos.y = (int)superObject.m_Y / 8; // 8 is tile size
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
