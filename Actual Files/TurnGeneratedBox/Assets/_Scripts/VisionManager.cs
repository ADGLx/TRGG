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

    private bool IsTrigger = false;
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


            //Clear the previous vision first 
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
                // tileMap.SetTile(new Vector3Int(T.X, T.Y,0), null);
              }

            yield return new WaitUntil(() => Player.IsUnitMoving == true);
        }
    }


    IEnumerator UpdateAllTiles()
    {
        
        while (true)
        {

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



            }

            //UpdateMirrorTIles();
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

    private void UpdateMirrorTIles()
    {
        int Size = StaticMapConf.Size;

        int StartMin = -(Size / 2);
        int StartMax = Size / 2;


        Vector3Int OriginPos = new Vector3Int();
        Vector3Int TargetPos = new Vector3Int();


        //start drawing from the bottonleft and keep going
        for (int Y = StartMin - Size; Y <= StartMax + Size; Y++)
        {
            for (int X = StartMin - Size; X <= StartMax + Size; X++)
            {
                if ((X <= StartMin || X >= StartMax || Y <= StartMin || Y >= StartMax))
                {
                    OriginPos = new Vector3Int(X, Y, 0);

                    TargetPos = LocalMap.GetOppositeTileOnBoarder(X, Y);

                    MapTile TargetTile = LocalMap.FindTile(TargetPos.x, TargetPos.y);

                    if(TargetTile != null)
                    {
                        if (TargetTile.Visible)
                        {
                            tileMap.SetTile(OriginPos, null);
                        }
                        else if (TargetTile.Discovered)
                        {
                            tileMap.SetTile(OriginPos, UnvisibleTile);
                        }
                        else //Not discovered or visible
                        {
                            tileMap.SetTile(OriginPos, UndiscoveredTile);
                        }
                    } else
                    {
                        Debug.Log("Null Tile");
                    }


                }
            }
        }
    } //Fix mirror tiles
}
