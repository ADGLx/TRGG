using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AI_State { Explore, Patrol, Flee, Chase };
public class AI_Handler : MonoBehaviour
{
    private bool UpdateMovementNow = true;
    //private bool UpdateNow = true;
    //This should eventually replace the Enemy_AI one

    [Header("General Settings")]
    public float RefreshRate = 0.5f;
    public AI_State UnitState = AI_State.Explore;
    private Unit LocalUnitScript;
    private Vector2Int LastPos = new Vector2Int();

    [Header("State Defining")]
    public int LowHPDefine = 2;
    public int DetectRadius = 4;
    // Use this for initialization

    [Header("Explote Settings")]
    public int ExploreMaxMovementRange = 4;

    [Header("Patrol Settings")]
    public int HexRadius = 4;

        //Specific values used to evaluate the AI state
    private Vector2Int PlayerLastSeen = new Vector2Int();

    void Start()
    {
        if (this.GetComponent<Unit>())
        {
            LocalUnitScript = this.GetComponent<Unit>();
            LastPos = LocalUnitScript.GridPos;
        }
        else
        {
            Debug.Log("Unit Script could not be found");
        }

     //   StartCoroutine(AI_Update()); cant have both of them running it probably breaks something 
        StartCoroutine(MoveGenerator());
    }


    AI_State DetectState() //Might wanna change the way im returning the value of the player's location
    {
        bool LowHP = false;

        Tuple<bool, Vector2Int> PlayerInfo;
        //Item1= PlayerClose, Item2=PlayerPos

        if (LocalUnitScript.unitStats.LifePoints <= LowHPDefine)
            LowHP = false;

        List<MapTile> Area = LocalUnitScript.MapLocal.GetAreaAround(LocalUnitScript.GridPos, DetectRadius); //I might wanna optimize this

        PlayerInfo = GetPlayerClose(Area); //this access only the bool

        if(!PlayerInfo.Item1 && !LowHP)
        {
            return AI_State.Explore;
        } else if (!PlayerInfo.Item1 && LowHP)
        {
            return AI_State.Patrol;
        } else if(PlayerInfo.Item1 && !LowHP)
        {
            PlayerLastSeen = PlayerInfo.Item2;
            return AI_State.Chase; //Might wanna pass the player's position right away (done)
        } else
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
        while (true)
        {
           // yield return new WaitForSeconds(RefreshRate); //prevents it from updating toooooo much (might not need it)


            Debug.Log("Its updating");
            if (ShouldChangePath(UnitState))
            {
                UnitState = DetectState();
            } else
            {
                //just let it continue their path to wherever
            }

            //So first it is gonna check if it needs to change paths or not
            // The shoudl chage path should update everytime the AI changes tiles
          
            while (LastPos == LocalUnitScript.GridPos) //prevents it from updating too often
                yield return null; //this should wait until its true


        }
    }

    IEnumerator MoveGenerator () //This will update too but only when the Movement is done
    {
        while (true)
        {
            yield return new WaitForSeconds(RefreshRate); //Might need this just in case



            UpdateMovementNow = false; //It is updating so we dont need it to update once this happens
            switch (UnitState)
            {
                
               case AI_State.Explore:
                    Debug.Log("Why");
                    //This is too random, it need a bit more of a general direction to walk to (might wanna create a way to store all the data of the visited places
                    int RX = UnityEngine.Random.Range(LocalUnitScript.GridPos.x - ExploreMaxMovementRange, LocalUnitScript.GridPos.x + ExploreMaxMovementRange);
                    int RY = UnityEngine.Random.Range(LocalUnitScript.GridPos.y - ExploreMaxMovementRange, LocalUnitScript.GridPos.y + ExploreMaxMovementRange);

                    if (LocalUnitScript.MapLocal.FindTile(RX, RY).Walkable)
                        StartCoroutine(LocalUnitScript.MoveUnitTo(RX, RY));
                    else
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
                        if (!LocalUnitScript.MapLocal.FindTile(P.x, P.y).Walkable) //check if u can walk to all of em
                            Debug.LogWarning(P + " is not walkable");
                    }

                for(int x =0; x < Points.Count; x++)
                    {
                        if(LocalUnitScript.MapLocal.FindTile(Points[x].x, Points[x].y).Walkable)
                        {
                            StartCoroutine(LocalUnitScript.MoveUnitTo(Points[x].x, Points[x].y));

                            while (LocalUnitScript.GridPos != Points[x] && !UpdateMovementNow)
                                yield return null;

                        } 

                        if (x == Points.Count - 1) //this should keep it going forever
                            x = 0;
                    }

                   // StartCoroutine(LocalUnitScript.MoveUnitToPremadePath(FullPath));
                   // Debug.Log("Lengh: " + FullPath.Count);
                    break; //Patrol (there might be something wrong with the way it picks the Points

               case AI_State.Flee:



                    break; //Flee
              
                case AI_State.Chase:
                    break; //Chase
               
                default:
                    break;
            }

            while (!UpdateMovementNow) 
                yield return null;


        }
    }

    bool ShouldChangePath(AI_State CurrentState) 
    {
        //The last seen should update before this?????


        switch(CurrentState)
        {
            case AI_State.Explore:
                //It changes if the player is close???? Or it never changes idk
                return false;

            case AI_State.Patrol:
                //It also never changes?????????
                return false;

            case AI_State.Chase:
                //This will change state if the unit that im chasing is not currently close by
                //If the unit that im chasing is not in the last seen spot, channge directions
                if (InRange(LocalUnitScript.GridPos, PlayerLastSeen) && LocalUnitScript.MapLocal.FindTile(PlayerLastSeen.x, PlayerLastSeen.y).OcupiedByUnit == UnitIn.Player) //if the player has not moved
                    return false;
               else
                    return true;
                
            case AI_State.Flee:
                //It should stop fleeing it the player is no longer close

                if (!InRange(LocalUnitScript.GridPos, PlayerLastSeen)) //Might need a revision
                    return true;
                else
                    return false;
           
            default:
                Debug.Log("Implement change here for new behaviour");
                return false;
        }

    }

    Tuple<bool, Vector2Int> GetPlayerClose(List<MapTile> Area) //Idk how efficient are the tuples 
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

        return Tuple.Create(PlayerClose, Vector2Int.zero); //if it could not be found it returns false with zero for the vector
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