using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VisionManager : MonoBehaviour
{
    //This should update only with movement I think???

    public Unit Player;
    public Tilemap tileMap;
    public float UpdateRate;

    public TileBase UndiscoveredTile;
    public TileBase UnvisibleTile;

    private Map_Generator LocalMap;
    private List<MapTile> TilesCloseToPlayer = new List<MapTile>();


    void Start()
    {
        LocalMap = this.GetComponent<Map_Generator>();

        // HideAllTiles();

        StartCoroutine(UpdateAllTiles());
        StartCoroutine(GivePlayerVision());

        // GivePlayerVision();
    }

    // Update is called once per frame
    void Update()
    {
       // WhenThePlayerMoves();
    }

    private void HideAllTiles()
    {
        foreach(MapTile T in LocalMap.AllMapTiles)
        {
            Vector3Int Pos = new Vector3Int(T.X, T.Y, 0);
            tileMap.SetTile(Pos, UndiscoveredTile);

        }
    }

    IEnumerator GivePlayerVision()
    {

        while (true)
        {
            yield return new WaitUntil(() => Player.IsUnitMoving == false);

                foreach (MapTile T1 in TilesCloseToPlayer)
                {
                    T1.Visible = false;
                }
                TilesCloseToPlayer.Clear();

                List<MapTile> Tiles = LocalMap.GetAreaAround(Player.GridPos, Player.unitStats.VisionRange);

                foreach (MapTile T in Tiles)
                {

                    TilesCloseToPlayer.Add(T);
                    T.Visible = true;
                    T.Discovered = true;

                   // Debug.Log(T.GetPos);
                }
            //Clear the previous vision first 
            yield return new WaitUntil(() => Player.IsUnitMoving == true);

          
        }
    }


    IEnumerator UpdateAllTiles()
    {
        
        while (true)
        {
            //Actual Tiles
            foreach (MapTile T in LocalMap.AllMapTiles)
            {
                Vector3Int Pos = new Vector3Int(T.X, T.Y, 0);

                if (!T.Discovered)
                {
                    if(tileMap.GetTile(Pos) == null)
                    {
                        tileMap.SetTile(Pos, UndiscoveredTile);
                    }
                } else if (!T.Visible)
                {
                        tileMap.SetTile(Pos, UnvisibleTile);
                 
                } else if(T.Visible)
                {
                    tileMap.SetTile(new Vector3Int(T.X, T.Y, 0), null);
                }

                if (T.IsBound && T.Discovered && T.Visible)
                    tileMap.SetTile(new Vector3Int(T.X, T.Y, 0), null);

            }
         

            
            //Mirror TIles
            foreach (Vector2Int Origin in LocalMap.AllMirrorTiles.Keys)
            {

                MapTile TargetTile = LocalMap.AllMirrorTiles[Origin];

              //  Debug.Log(Origin);

                if (TargetTile != null)
                {

                    if (TargetTile.Visible)
                    {
                        tileMap.SetTile(new Vector3Int(Origin.x, Origin.y, 0), null);
                    }
                    else if (TargetTile.Discovered)
                    {
                        tileMap.SetTile(new Vector3Int(Origin.x, Origin.y, 0), UnvisibleTile);
                    }
                    else
                    {
                        tileMap.SetTile(new Vector3Int(Origin.x, Origin.y, 0), UndiscoveredTile);
                    }

                    
                }
                else
                {
                   // Debug.Log("Null Tile");
                }
            }

            

            yield return new WaitForSeconds(UpdateRate);
        }
      
    }

    /*
    private void WhenThePlayerMoves()
    {
        if(Player.IsUnitMoving && !IsTrigger)
        {
          //  Debug.Log("Moving"); I can probably implement it here to update every time the position changes
            IsTrigger = true;
        }

        if (IsTrigger && !Player.IsUnitMoving)
        {
            IsTrigger = false;

            GivePlayerVision(); 
        }

    }*/

}
