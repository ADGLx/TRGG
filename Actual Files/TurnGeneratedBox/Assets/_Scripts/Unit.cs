﻿using System;
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
        public int MaxLifePoints;
        public int ActionPoints;
        public int MaxActionPoints;
        public int VisionRange;
        public int AttackRange;
    }
    public UnitStats unitStats;

    public Vector2Int GridPos = new Vector2Int();
    public UnitIn Type = UnitIn.Player;
    // public Tile A; no clue what this was for 
    public float AnimSpeed = 1f;
   // public GameObject Target;
    private GameObject TempParticles;
    [HideInInspector]
    public bool IsUnitMoving = false;
    [HideInInspector]
    public bool StopMoving = false;
    private bool IsThisMainP = false; //This is a cheap way to fix it, I gotta clean this later
    [HideInInspector]
    public bool IsAttacking = false;

    public bool IsUnitInMoveAnim = false;

  
    [System.Serializable]
    public class PossibleAction
    {
        public bool Move;
        public bool AutoDestroy;
        public bool Attack;
    }

    public PossibleAction Actions;

    [HideInInspector]
    public Map_Generator MapLocal;
    private UI_Manager UI_MLocal;
    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("Map").GetComponent<Map_Generator>() != null)
        {
            MapLocal = GameObject.FindGameObjectWithTag("Map").GetComponent<Map_Generator>();
        } else
        {
            Debug.Log("Map_Generator could not be found");
        }
        //Gotta call the UI when it is moving 
        if (GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UI_Manager>() != null)
        {
            UI_MLocal = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UI_Manager>();
        } else
        {
            Debug.Log("UI_Manager could not be found");
        }


        if(this.gameObject.tag == "Main_Pl")
        {
            IsThisMainP = true;
        } else
        {
            IsThisMainP = false;
        }
    }
    void Start()
    {
        MapLocal.InstializeUnit(this, GridPos.x, GridPos.y);
        //LastPos = GridPos;
        //  MoveUnitTo(-5, -3); this is making it so when the pathfinding happens the thing is set as not occupied
        //MoveUnitTo(4, -4);

        // MapLocal.SpawnAreaParticle(GridPos, 2);
        if(MapLocal.GetComponent<VisionManager>().enabled)
        StartCoroutine(HideIfNotvisible());

    }


    public void SetPos(int X, int Y)
    {
        GridPos = new Vector2Int(X,Y);
        MapLocal.OcupyTileUnit(this, GridPos.x, GridPos.y);

    }

    public void TeleportUnitTo(int X, int Y)
    {
        //StopMoving = true;
        this.transform.position = (MapLocal.SetTilePosToWorld(X, Y));
        GridPos = new Vector2Int(X, Y);
        MapLocal.OcupyTileUnit(this, GridPos.x, GridPos.y);
    }


   public IEnumerator MoveUnitTo(int TargetX, int TargetY) //this one will be the one thats called in general and will allow to stop
    {
        if(IsUnitMoving)
        {
            StopMoving = true;

            while(IsUnitMoving) //this can cause an infinite loops but whatever
            {
               // Debug.Log("Waiting");
                yield return null;
            }
        }

        
        List<MapTile> Path = new List<MapTile>();


        if (MapLocal.FindTile(TargetX,TargetY).Walkable)
        {
            Path = MapLocal.Pathfinding(GridPos, new Vector2Int(TargetX, TargetY));

            if (Path != null)
            {
                IsUnitMoving = true;
                if (IsThisMainP)
                    UI_MLocal.ShowPath(Path); //Show the UI

                //Start the walk thing
                for (int x = 0;x< Path.Count; x++)
                {
                    //In here I start the movement for the next tile
                    //Yielding until its completed 
                    yield return StartCoroutine(MoveNextTile(Path[x].X, Path[x].Y));

                    if (StopMoving)
                    {
                        StopMoving = false;
                        break;
                    }

                    if (IsThisMainP)
                        UI_MLocal.ClearSpecPath(x);

                }

                IsUnitMoving = false;

            } else
            {
                Debug.Log("Path is null");
                //This is a way to fix it for the AI
                if (this.GetComponent<AI_Handler>()) //this is kinda cheap
                    this.GetComponent<AI_Handler>().UpdateMovementNow = true;
            }
        }
        
        
        
        
        /*
        List<MapTile> MyPath = new List<MapTile>();
        Vector2Int DebugPos = new Vector2Int(TargetX, TargetY);
        if (MapLocal.FindTile(TargetX,TargetY).Walkable == true)
        {
            // if (!IsUnitMoving)
            // {
            MyPath = MapLocal.Pathfinding(GridPos, new Vector2Int(DebugPos.x, DebugPos.y));
                //  StartCoroutine(PathMoveAnim(MyPath));
                if (MyPath != null)
                {
                    StartCoroutine(MoveToTileAnim(MyPath));
                }
                else
                {
                    Debug.Log("Path couldnt be found");
                }
           // } 

        } else
        {
            Debug.Log("Target Tile isnt walkable");
        }




        //This should be in another function to be more organized tho

        //  PlaceTarget(DebugPos.x, DebugPos.y); Add this somewhere else
        */
    }

    IEnumerator MoveNextTile(int X, int Y)
    {
        MapLocal.UnocupyTileUnit(GridPos.x, GridPos.y);//This helped prevent a bit of stuff 
        SetPos(X, Y);
        while (this.transform.position != MapLocal.SetTilePosToWorld(X, Y))
        {
            IsUnitInMoveAnim = true;
            this.transform.position = Vector2.MoveTowards(this.transform.position, MapLocal.SetTilePosToWorld(X, Y), AnimSpeed * Time.deltaTime);
            yield return null;
        }

        //   LastPos = GridPos;
        IsUnitInMoveAnim = false;

        if (MapLocal.TurnModeOn)
            unitStats.ActionPoints--;
    }

    //Obsoleting this one for now
    public IEnumerator MoveUnitToPremadePath(List<MapTile> Path) 
    {
        if (Path != null)
        {
           while (IsUnitMoving)
            {
                yield return null;
            }
                if (Path[0].GetPos != GridPos)
                {
                    //  Debug.Log("Starting point doesnt match");
                    StartCoroutine(MoveNextTile(Path[0].GetPos.x, Path[0].GetPos.y)); 
                }
          //      StartCoroutine(MoveToTileAnim(Path));
            

        } else
        {
            Debug.Log("Loaded Path is null");
        }

    }

    IEnumerator HideIfNotvisible()
    {
        while(true) //this kinda works
        {
            yield return new WaitUntil(() => IsUnitMoving == true);

            if (!MapLocal.FindTile(GridPos.x, GridPos.y).Visible)
            {
                this.GetComponent<SpriteRenderer>().enabled = false;
            } else
            {
                this.GetComponent<SpriteRenderer>().enabled = true;
            }

            
        }


       
    }

   public void TakeDamage (int SourceDamage)
    {
        unitStats.LifePoints -= SourceDamage;
    }

    public void DealDamageTo(int X, int Y, UnitIn TypeOfUnit)
    {
       // Debug.Log("Dealing Damage"); In here I might wanna implement the weapon system or sum

        if (TypeOfUnit == UnitIn.AI)
        {
            //Find the AI creature
            GameObject[] AllUnits = GameObject.FindGameObjectsWithTag("Enemy_AI");
            Unit Enemy = null;
            foreach (GameObject T in AllUnits)
            {
                if(T.GetComponent<Unit>() != null && T.GetComponent<Unit>().GridPos.x == X && T.GetComponent<Unit>().GridPos.y == Y)
                {
                    Enemy = T.GetComponent<Unit>();
                    break;
                }

            }

            if(Enemy == null)
            {
                Debug.LogError("Enemy not found");
                return;
            }

            Enemy.TakeDamage(unitStats.AttackPoints);
            
        }

    }

    /*
    //Fix this to make it work with all the units (Not only the main one)
    IEnumerator MoveToTileAnim(List<MapTile> thPath)
    {
        if (IsUnitMoving) //the command is ignored if the thing is moving
        {
            StopMoving = true;
            yield break;
        }


        float step = AnimSpeed * Time.deltaTime;

        if (IsThisMainP)
            UI_MLocal.CleanAllGUI();

        IsUnitMoving = true;
        for (int m = 0; m < thPath.Count; m++) //change the foreach with a for 
        {
            if (StopMoving)//this stops it from moving now we gotta find a way to make it move to the next one 
            {
                IsUnitMoving = false;
                StopMoving = false;
                yield break;
            }

            MapLocal.UnocupyTileUnit(GridPos.x, GridPos.y);//This helped prevent a bit of stuff 

            while (this.transform.position != MapLocal.SetTilePosToWorld(thPath[m].X, thPath[m].Y)) //this is dangerous
            {
                IsUnitMovingBTiles = true;
                this.transform.position = Vector2.MoveTowards(this.transform.position, MapLocal.SetTilePosToWorld(thPath[m].X, thPath[m].Y), step);
                yield return null;
            }
            SetPos(thPath[m].X, thPath[m].Y);
            IsUnitMovingBTiles = false;


            if (MapLocal.TurnModeOn)
            unitStats.ActionPoints--;

            if (m + 1 < thPath.Count && !thPath[m + 1].Walkable)
                break;//this will stop the loop from happening one more time if the next thing is occupied 
        }
        

        if (IsThisMainP)
        {
            MapLocal.CurrentTile = MapLocal.FindTile(GridPos.x, GridPos.y);
            UI_MLocal.ChangeOfSelection();
        }


        IsUnitMoving = false;

    }
    */
}
