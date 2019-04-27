using System.Collections;
using System.Collections.Generic;
using SuperTiled2Unity;
using UnityEngine;
using Utils;

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
                        player.boardPos.x = (int)superObject.m_X / TileConstants.TILE_SIZE; // 8 is tile size
                        player.boardPos.y = (int)superObject.m_Y / TileConstants.TILE_SIZE; // 8 is tile size
                        
                        // TODO: Make sure this doesn't break the rest of the grid (i.e. Now everything else is off by (-2, -2)
                        player.boardPos.Add(new IsoVector2(-2, -2));
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
