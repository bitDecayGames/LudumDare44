using System.Collections;
using System.Collections.Generic;
using Board;
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
        BoardManager board = FindObjectOfType<BoardManager>();
        foreach (var componentsInChild in map.GetComponentsInChildren<SuperObjectLayer>())
        {
            if ("Interactables" == componentsInChild.m_TiledName)
            {
                foreach (var superObject in componentsInChild.GetComponentsInChildren<SuperObject>())
                {
                    if ("Spawn" == superObject.m_TiledName)
                    {
                        var player = Instantiate(playerPrefab, map.transform);
                        player.boardPos.x = (int)superObject.m_X / TileConstants.TILE_SIZE; // 8 is tile size
                        player.boardPos.y = (int)superObject.m_Y / TileConstants.TILE_SIZE; // 8 is tile size

                        // TODO: Make sure this doesn't break the rest of the grid (i.e. Now everything else is off by (-2, -2)
                        player.boardPos.Add(new IsoVector2(-2, -2));
                        
                        var occupier = player.gameObject.AddComponent<Board.Board.Occupier>();
                        board.board.Set(occupier, player.boardPos.x, player.boardPos.y);
                    }
                }
            }
        }
    }

}
