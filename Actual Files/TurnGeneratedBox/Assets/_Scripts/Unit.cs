using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;

public class Unit : MonoBehaviour {

    [System.Serializable]
    public class UnitStats
    {
        public string Name;
        public int AttackPoints;
        public int LifePoints;
        public int ActionPoints;
    }
    public UnitStats unitStats;

    public Vector2Int GridPos = new Vector2Int();
    public UnitIn Type = UnitIn.Player;
    // public Tile A; no clue what this was for 
    public float AnimSpeed = 1f;
    public GameObject Target;
    private GameObject TempParticles;

    [System.Serializable]
    public class PossibleAction
    {
        public bool Move;
        public bool AutoDestroy;
        public bool Attack;
    }

    public PossibleAction Actions;

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
        SetPos(GridPos.x, GridPos.y);

        //  MoveUnitTo(-5, -3); this is making it so when the pathfinding happens the thing is set as not occupied
        //MoveUnitTo(4, -4);

       // MapLocal.SpawnAreaParticle(GridPos, 2);

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
      //  StartCoroutine(PathMoveAnim(MyPath));
        StartCoroutine(MoveToTileAnim(MyPath));

        //This should be in another function to be more organized tho

        //  PlaceTarget(DebugPos.x, DebugPos.y); Add this somewhere else
    }


    IEnumerator MoveToTileAnim(List<MapTile> thPath)
    {
        float step = AnimSpeed * Time.deltaTime;

        foreach (MapTile T in thPath)
        {
            
            while (this.transform.position != MapLocal.SetTilePosToWorld(T.X, T.Y)) //this is dangerous
            {
                this.transform.position = Vector2.MoveTowards(this.transform.position, MapLocal.SetTilePosToWorld(T.X, T.Y), step);
                yield return null;
            }
            MapLocal.UnocupyTileUnit(GridPos.x, GridPos.y);
            SetPos(T.X, T.Y);
            unitStats.ActionPoints--;
        }



    }
}
