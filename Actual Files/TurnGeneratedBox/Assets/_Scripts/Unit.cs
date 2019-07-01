using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Unit : MonoBehaviour {

    public Vector2Int GridPos = new Vector2Int();
    public UnitIn Type = UnitIn.Player;
    public Tile A;
    public GameObject Target;
    private GameObject TempParticles;

    Map_Generator MapLocal;
    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("Map").GetComponent<Map_Generator>() != null)
        {
            MapLocal = GameObject.FindGameObjectWithTag("Map").GetComponent<Map_Generator>();
        } else
        {
            Debug.Log("Map_Generator could not be found");
        }

    }
    void Start()
    {
         GetPos();
        SetPos(GridPos.x, GridPos.y);

        MoveUnitTo(-5, -3);

        /*List<MapTile> MyPath = new List<MapTile>();
        MyPath = MapLocal.Pathfinding(GridPos, new Vector2Int(DebugPos.x, DebugPos.y));
        */

      //  Debug.Log("Distance "+MapLocal.GetDistance(DebugPos, GridPos));
        
       // if (MyPath !=null)
     //   Debug.Log(MyPath.Count);

    }

    void GetPos()
    {
        float PosX, PosY;

        PosX = this.transform.position.x;
        PosY = this.transform.position.y;

        GridPos = new Vector2Int(Convert.ToInt32(PosX), Convert.ToInt32(PosY - 1));
    }

    public void SetPos(int X, int Y)
    {
        GridPos = new Vector2Int(X,Y);
        MapLocal.OcupyTileUnit(this, GridPos.x, GridPos.y);
    }

    void PlaceTarget(int X, int Y)
    {
        if (TempParticles != null)
        {
            Destroy(TempParticles);
            
        }

        Vector3 cellPos = MapLocal.SetTilePosToWorld(X, Y);

        TempParticles = Instantiate(Target, cellPos, Target.transform.rotation, this.transform);
    }

    public void MoveUnitTo(int TargetX, int TargetY)
    {
        List<MapTile> MyPath = new List<MapTile>();
        Vector2Int DebugPos = new Vector2Int(TargetX, TargetY);
        MyPath = MapLocal.Pathfinding(GridPos, new Vector2Int(DebugPos.x, DebugPos.y));

        //This should be in another function to be more organized tho
        foreach(MapTile T in MyPath)
        {
            Debug.Log(T.X + " / " + T.Y);
        }
        PlaceTarget(DebugPos.x, DebugPos.y);
    }
}
