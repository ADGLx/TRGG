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

        Vector2Int DebugPos = new Vector2Int(3, 0);
        List<MapTile> MyPath = new List<MapTile>();
        MyPath = MapLocal.Pathfinding(GridPos, new Vector2Int(DebugPos.x, DebugPos.y));
        PlaceTarget(DebugPos.x, DebugPos.y);
        Debug.Log("Distance "+MapLocal.GetDistance(DebugPos, GridPos));
        
        if (MyPath !=null)
        Debug.Log(MyPath.Count);

    }

    void GetPos()
    {
        float PosX, PosY;

        PosX = this.transform.position.x;
        PosY = this.transform.position.y;

        GridPos = new Vector2Int(Convert.ToInt32(PosX), Convert.ToInt32(PosY - 1));

        MapLocal.OcupyTileUnit(this, GridPos.x, GridPos.y);
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
}
