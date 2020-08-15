using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AI_State { Explore, Patrol, Flee, Chase , Still, TurnMode};
public class AI_Handler : MonoBehaviour
{
    [HideInInspector]
    public bool UpdateMovementNow = true;
    //private bool UpdateNow = true;
    //This should eventually replace the Enemy_AI one

    [Header("General Settings")]
    public float RefreshRate = 0.5f;
    public AI_State UnitState = AI_State.Explore;
    private Unit LocalUnitScript;
    private Vector2Int CurTargetTile = Vector2Int.zero;
   // private Vector2Int LastPos = new Vector2Int();

    [Header("State Defining")]
    public int LowHPDefine = 2;
    public int DetectRadius = 4;
    // Use this for initialization

    [Header("Explote Settings")]
    public int ExploreMaxMovementRange = 4;

    [Header("Patrol Settings")]
    public int HexRadius = 4;

    [Header("Flee Settings")]
    public float AlertTime = 1f;

    //Specific values used to evaluate the AI state
    private Vector2Int PlayerLastSeen = new Vector2Int();
    private Vector2Int LasPosLocal = new Vector2Int();
    private bool PlayerClose = false;

    void Start()
    {
        if (this.GetComponent<Unit>())
        {
            LocalUnitScript = this.GetComponent<Unit>();
            LasPosLocal = LocalUnitScript.GridPos;
        }
        else
        {
            Debug.Log("Unit Script could not be found");
        }



            StartCoroutine(DetectPlayer());
            StartCoroutine(AI_Update()); //cant have both of them running it probably breaks something 
            StartCoroutine(MoveGenerator()); //I dont remember what this is for


    }

    private void FixedUpdate()
    {
        DebugColorChange();

     //    Debug.Log(PlayerClose);


    }

    private void DebugColorChange()
    {
        switch(UnitState)
        {
            case AI_State.Explore:

                this.GetComponent<SpriteRenderer>().color = Color.green;
                break;

            case AI_State.Chase:
                this.GetComponent<SpriteRenderer>().color = Color.red;
                break;
           
            case AI_State.Flee:
                this.GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            
            case AI_State.Patrol:
                this.GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            
            case AI_State.Still:
                this.GetComponent<SpriteRenderer>().color = Color.white;
                break;
            case AI_State.TurnMode:
                this.GetComponent<SpriteRenderer>().color = Color.black;
                break;
        }
    }

    AI_State DetectState() //Might wanna change the way im returning the value of the player's location
    {
        bool LowHP = false;

        Tuple<bool, Vector2Int> PlayerInfo;
        //Item1= PlayerClose, Item2=PlayerPos

        if (LocalUnitScript.unitStats.LifePoints <= LowHPDefine)
            LowHP = false;

        List<MapTile> Area = LocalUnitScript.MapLocal.GetAreaAround(LocalUnitScript.GridPos, DetectRadius); //I might wanna optimize this

        PlayerInfo = GetPlayerClose(Area);

        if (LocalUnitScript.MapLocal.TurnModeOn)
        {
            return AI_State.TurnMode;
        }
        else if (!PlayerInfo.Item1 && !LowHP)
        {
            return AI_State.Explore;
        } else if (!PlayerInfo.Item1 && LowHP)
        {
            return AI_State.Patrol;
        } else if(PlayerInfo.Item1 && !LowHP)
        {
            PlayerLastSeen = PlayerInfo.Item2;
        //    Debug.Log
            return AI_State.Chase; //Might wanna pass the player's position right away (done)
        }      
        else
        {
            PlayerLastSeen = PlayerInfo.Item2;
            return AI_State.Flee; //Might wanna pass the player's position right away (done)
        }
    }

    IEnumerator AI_Update()
    {
        //  List<MapTile> CurrentPath = new List<MapTile>();

        //  UnitState = DetectState(); //the first detect state
        // StartCoroutine(MoveGenerator());
        //Have it update everytime the unit moves to a new tile 
        yield return new WaitForSeconds(0.5f);
        while (true)
        {

            
            yield return new WaitForSeconds(RefreshRate); //prevents it from updating too much

       //   Debug.Log("Its updating");
            
            if (ShouldChangePath(UnitState))
            {
               UnitState = DetectState();
                //UnitState = AI_State.Chase;
               UpdateMovementNow = true;
               // Debug.Log("Changing state to ");
            } else
            {
                //just let it continue their path to wherever
            }
            //So first it is gonna check if it needs to change paths or not
            // The shoudl chage path should update everytime the AI changes tiles
        }
    }

    IEnumerator MoveGenerator () //This will update too but only when the Movement is done
    {
        while (true)
        {
           //Might need this just in case

            UpdateMovementNow = false; //It is updating so we dont need it to update once this happens
                                       //  Debug.Log("Updating movement");          


            switch (UnitState)
            {
                
               case AI_State.Explore:

                 //   Debug.Log("w");
                    //This is too random, it need a bit more of a general direction to walk to (might wanna create a way to store all the data of the visited places
                    int RX = UnityEngine.Random.Range(LocalUnitScript.GridPos.x - ExploreMaxMovementRange, LocalUnitScript.GridPos.x + ExploreMaxMovementRange);
                    int RY = UnityEngine.Random.Range(LocalUnitScript.GridPos.y - ExploreMaxMovementRange, LocalUnitScript.GridPos.y + ExploreMaxMovementRange);

                    if (LocalUnitScript.MapLocal.FindTile(RX, RY).Walkable)
                    {
                        StartCoroutine(LocalUnitScript.MoveUnitTo(RX, RY));
                        CurTargetTile = new Vector2Int(RX,RY);

                        while (LocalUnitScript.GridPos != new Vector2Int(RX, RY) && !UpdateMovementNow) //this helps this constant
                            yield return null;
                       
                    }

                    UpdateMovementNow = true;
                    break; //Explore (I want to make this way more complex later)

                case AI_State.Patrol:

                   // Debug.Log("This is running");
                    //first find 6 points in the map to do the rounds 
                    List<Vector2Int> Points = new List<Vector2Int>();
                    int XPos = LocalUnitScript.GridPos.x;
                    int YPos = LocalUnitScript.GridPos.y;
                    // unnesesary this is too complex
                    Points.Add(new Vector2Int(XPos + HexRadius, YPos));

                    //This one is the hard one 
                    Points.Add(new Vector2Int(XPos + (int)(HexRadius / 2), (int)(((Mathf.Sqrt(3) / 2) * HexRadius - 1) + YPos))); //This is so cheap omg 
                    Points.Add(new Vector2Int(XPos - (int)(HexRadius / 2), (int)(((Mathf.Sqrt(3) / 2) * HexRadius - 1) + YPos)));

                    Points.Add(new Vector2Int(XPos - HexRadius, YPos));

                    Points.Add(new Vector2Int(XPos - (int)(HexRadius / 2), YPos - (int)(((Mathf.Sqrt(3) / 2) * HexRadius))));
                    Points.Add(new Vector2Int(XPos + (int)(HexRadius / 2), YPos - (int)(((Mathf.Sqrt(3) / 2) * HexRadius))));

                    foreach (Vector2Int P in Points)
                    {
                  //      Debug.Log("Point pos: " + P.x + ";" + P.y);
                        if (!LocalUnitScript.MapLocal.FindTile(P.x, P.y).Walkable) //check if u can walk to all of em
                            Debug.LogWarning(P + " is not walkable");
                    }

                for(int x =0; x < Points.Count; x++)
                    {
                        if (x == Points.Count - 1) //this should keep it going forever
                            x = 0;

                        if (LocalUnitScript.MapLocal.FindTile(Points[x].x, Points[x].y).Walkable)
                        {
                           
                            StartCoroutine(LocalUnitScript.MoveUnitTo(Points[x].x, Points[x].y));
                            CurTargetTile = Points[x];

                            while (LocalUnitScript.GridPos != Points[x] && !UpdateMovementNow) //this stops the infinte loops from going bananas
                                yield return null;

                        } else
                        {
                            Debug.Log("Not walkable " + Points[x]);
                        }

                    }

                   // StartCoroutine(LocalUnitScript.MoveUnitToPremadePath(FullPath));
                   // Debug.Log("Lengh: " + FullPath.Count);
                    break; //Patrol 

               case AI_State.Flee:

                    //If fleeing all I have to do is run away from the player's last seen locaiton
                    yield return new WaitForSeconds(AlertTime);

                    int HowFarToGo = DetectRadius - LocalUnitScript.MapLocal.GetDistance(LocalUnitScript.GridPos, PlayerLastSeen);

                    if(HowFarToGo <= 0)
                    {
                        HowFarToGo = 1;
                    }

                    Vector2Int TargetEscapeTile = ((LocalUnitScript.GridPos - PlayerLastSeen) * HowFarToGo) + PlayerLastSeen;

                    MapTile Tile = LocalUnitScript.MapLocal.FindTile(TargetEscapeTile.x, TargetEscapeTile.y);

                    if (Tile != null && Tile.Walkable)
                    {
                       // Debug.Log(PlayerLastSeen);
                        StartCoroutine(LocalUnitScript.MoveUnitTo(Tile.X, Tile.Y));
                        CurTargetTile = Tile.GetPos;
                    }
                    else if (Tile != null && !Tile.Walkable)
                    {
                        //when this occurs I wanna try a full area of tiles near that one to get one route
                        List<MapTile> OtherOptions = LocalUnitScript.MapLocal.GetAreaAround(TargetEscapeTile, DetectRadius);

                        for (int x = 0; x < OtherOptions.Count; x++)
                        {
                            if (OtherOptions[x].Walkable)
                            {
                              //  Debug.Log(PlayerLastSeen);
                                StartCoroutine(LocalUnitScript.MoveUnitTo(OtherOptions[x].X, OtherOptions[x].Y));
                                CurTargetTile = OtherOptions[x].GetPos;
                                break;
                            }

                            if(x == OtherOptions.Count - 1)
                                Debug.Log("No Valid Target Tile has been found");
                        }

                    }

                    break; //Flee (Im guessing it works? It needs the PlayerLastSeen variable so we need more testing)
              
                case AI_State.Chase:

                    //Just get the last know position and go to a close hex 
                    MapTile TargetTile = LocalUnitScript.MapLocal.FindTile(PlayerLastSeen.x, PlayerLastSeen.y);
                    if (TargetTile.Walkable)
                    {
                        StartCoroutine(LocalUnitScript.MoveUnitTo(TargetTile.X, TargetTile.Y));
                        CurTargetTile = TargetTile.GetPos;
                    }
                    else
                    {
                        //If it is not walkable imma just look for the closest hex for me to get there 
                        List<MapTile> PathToUnit = LocalUnitScript.MapLocal.Pathfinding(LocalUnitScript.GridPos, PlayerLastSeen);
                        for(int x = PathToUnit.Count - 1; x>= 0; x--)
                        {
                            if (PathToUnit[x].Walkable)
                            {
                                StartCoroutine(LocalUnitScript.MoveUnitTo(PathToUnit[x].X, PathToUnit[x].Y));
                                CurTargetTile = TargetTile.GetPos;
                                break;
                            }

                        }

                    }

                    break; //Chase

   
                case AI_State.Still:
                    //just stand still basically
                    LocalUnitScript.StopMoving = true;
                   
                    break; //Still

                case AI_State.TurnMode:
                    // Come up with a way to move to the player or maybe flee? Those should be the only moves? Not sure tho
                    break;

               default:
                    Debug.LogWarning("UnitState is not recognized");
                    break; //default (just in case I forget to add another state)
            }

            while (!UpdateMovementNow) 
                yield return null;


        }
    }

    IEnumerator DetectPlayer()
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            yield return new WaitForSeconds(RefreshRate);

            //might wanna make this an actual coroutine
            Tuple<bool, Vector2Int> PlInfo = GetPlayerClose(LocalUnitScript.MapLocal.GetAreaAround(LocalUnitScript.GridPos, DetectRadius));
            PlayerClose = PlInfo.Item1;

            if (PlInfo.Item1)
                PlayerLastSeen = PlInfo.Item2; //Debug.Log("Player close");
        }
    }

    bool ShouldChangePath(AI_State CurrentState) 
    {
        //The last seen should update before this?????
        //I will update the player last seen in here, just to make sure it is always updated
        /*  Tuple<bool, Vector2Int> PlInfo = GetPlayerClose(LocalUnitScript.MapLocal.GetAreaAround(LocalUnitScript.GridPos, DetectRadius));

          if (PlInfo.Item1)
              PlayerLastSeen = PlInfo.Item2; //Debug.Log("Player close");
          */

        switch (CurrentState)
        {
            case AI_State.Explore:
                //it should change if the player is close
                if (PlayerClose)
                {
                    return true;
                } else
                return false;

            case AI_State.Patrol:
                //It also never changes?????????
                //it should change if the player is close
                if (PlayerClose)
                {
                    return true;
                } else
                return false;

            case AI_State.Chase:
                //it will always change because the player moves and when it reaches its destiny it will be stoped anyway 
                return true; 


            case AI_State.Flee:
                //It should stop fleeing it the player is no longer closeS
                if (!InRange(LocalUnitScript.GridPos, PlayerLastSeen)) //Might need a revision
                    return true;
                else
                    return false;
          
            case AI_State.Still:
                // it doesnt change for now
                return false;

            case AI_State.TurnMode:
                if (!LocalUnitScript.MapLocal.TurnModeOn)
                {
                    return true;
                }
                else
                {
                    return false;
                }


            default:
                Debug.Log("Implement change here for new behaviour");
                return false;
        }

    }

    Tuple<bool, Vector2Int> GetPlayerClose(List<MapTile> Area) 
    {
        bool PlayerClose = false;
        Vector2Int PlayerPos = Vector2Int.zero;

        foreach (MapTile M in Area)
        {
            if (M.OcupiedByUnit == UnitIn.Player)
            {
                PlayerClose = true;
                PlayerPos = M.GetPos;
                return Tuple.Create(PlayerClose, PlayerPos);
            }
        }

        return Tuple.Create(PlayerClose, PlayerLastSeen); //if it could not be found it returns false with the last seen pos
    }

    bool InRange(Vector2Int PosA, Vector2Int PosB)
    {
        if(LocalUnitScript.MapLocal.GetDistance(PosA,PosB) <= DetectRadius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    //public class AI_StateClass
    //{
    //    public AI_State State;
    //    public bool Update = false;
    //    public List<MapTile> NextPath = new List<MapTile>();

    //    public AI_StateClass(AI_State state)
    //    {
    //        State = state;
    //    }


    //} //I went with a class that can hold several variables so we can know when to update and stuff




}