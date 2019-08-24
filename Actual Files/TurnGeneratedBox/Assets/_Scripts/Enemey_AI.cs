using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemey_AI : MonoBehaviour {

    private Unit LocalUnit;
    public bool MovementActive;

    [Header("Explore Settings")]
    public int ExploreMaxMovementRange;
    public float WaitForNextMoveTime;

    [Header("Patrol Settings")]
    public int HexRadius;

    [Header("Flee Settings")]
    public float AlertTime;
    public int FleeRadius;
    // Use this for initialization
    void Start () {

        if(this.GetComponent<Unit>())
        {
            LocalUnit = this.GetComponent<Unit>();
        } else
        {
            Debug.Log("NullReference");
        }

        Mode_Flee(GameObject.FindGameObjectWithTag("Main_Pl").GetComponent<Unit>());
	}

    //The AI will be a set of instructions or states 
    private enum EnemyStates {Explore,Patrol, Chase, Flee };

    private EnemyStates CurrentState = EnemyStates.Explore;

    void Mode_Explore()
    {
        //Just walk around a set amount of points 
        //Select the points as soon as this state is called
        StartCoroutine(IndefRandomMovement());
        
    }

    IEnumerator IndefRandomMovement()
    {
        while (MovementActive) //The entire loop
        {
        while (LocalUnit.IsUnitMoving)
        {
            yield return null;
        }
            yield return new WaitForSeconds(WaitForNextMoveTime);
            int RX = (int)Random.Range(LocalUnit.GridPos.x - ExploreMaxMovementRange, LocalUnit.GridPos.x + ExploreMaxMovementRange);
            int RY = (int)Random.Range(LocalUnit.GridPos.y - ExploreMaxMovementRange, LocalUnit.GridPos.y + ExploreMaxMovementRange);

            if (LocalUnit.MapLocal.FindTile(RX,RY).Walkable)
            LocalUnit.MoveUnitTo(RX, RY);
        }
    }

    void Mode_Patrol()
    {
      //  List<MapTile> Path;

        //first find 6 points in the map to do the rounds 
        List<Vector2Int> Points = new List<Vector2Int>();
        int XPos = LocalUnit.GridPos.x;
        int YPos = LocalUnit.GridPos.y;
        // unnesesary this is too complex
        Points.Add(new Vector2Int( XPos + HexRadius, YPos));

        //This one is the hard one 
        Points.Add( new Vector2Int(XPos + (int) (HexRadius / 2), (int)(((Mathf.Sqrt(3) / 2) * HexRadius - 1) + YPos ))); //This is so cheap omg 
        Points.Add(new Vector2Int(XPos - (int)(HexRadius / 2), (int)(((Mathf.Sqrt(3) / 2) * HexRadius - 1) + YPos)));

        Points.Add(new Vector2Int(XPos - HexRadius, YPos));

        Points.Add(new Vector2Int(XPos - (int)(HexRadius / 2), YPos - (int)(((Mathf.Sqrt(3) / 2) * HexRadius))));
        Points.Add(new Vector2Int(XPos + (int)(HexRadius / 2), YPos - (int)(((Mathf.Sqrt(3) / 2) * HexRadius ))));
        

        /*Points[0] = new Vector2Int(XPos + HexRadius, YPos);
        Points[1] = new Vector2Int(XPos + HexRadius, YPos + HexRadius);
        Points[2] = new Vector2Int(XPos - HexRadius, YPos + HexRadius);
        Points[3] = new Vector2Int(XPos - HexRadius, YPos + HexRadius);*/


        for (int x = 0; x < Points.Count; x++) //check if they are walkable 
          {
            if(!LocalUnit.MapLocal.FindTile(Points[x].x, Points[x].y).Walkable)
            {
               Points.RemoveAt(x);
            }

          }
        List<MapTile>[] Paths = new List<MapTile>[6];

        List<MapTile> FullPath = new List<MapTile>();
        
        for (int x = 0; x < Points.Count; x++)
        {
            if (x != Points.Count - 1)
            {
                Paths[x] = LocalUnit.MapLocal.Pathfinding(Points[x], Points[x + 1]);
            } else
            {
                Paths[x] = LocalUnit.MapLocal.Pathfinding(Points[x], Points[0]);
            }
            if(Paths[x] != null)
            {
                FullPath.AddRange(Paths[x]);
            } else
            {
               // Paths[x].RemoveAll();
            }

        }

        StartCoroutine(PatrolZone(FullPath));

    }
    
    IEnumerator PatrolZone(List<MapTile> FullPath)
    {
        while(MovementActive)
        {
            while(LocalUnit.IsUnitMoving)
            {
                yield return null;
            }
            LocalUnit.MoveUnitToPremadePath(FullPath);

        }
   }


    void Mode_Chase(Unit Target)
    {
        //basically just follow to the last direction target was seen
        //but the tile closest to it on your direction
        StartCoroutine(KeepChasing(Target));

    }

    IEnumerator KeepChasing(Unit Target) //this shit doesnt work, it should update every time the player moves 
    {
       // yield return new WaitForSeconds(0.1f);

        List<MapTile> PathToTileClose = new List<MapTile>();
        Vector2 OldTPos = new Vector2();

        PathToTileClose = LocalUnit.MapLocal.Pathfinding(LocalUnit.GridPos, Target.GridPos);
        if (PathToTileClose.Count>1)
        PathToTileClose.RemoveAt(PathToTileClose.Count - 1); //this should work

        LocalUnit.MoveUnitToPremadePath(PathToTileClose);

        
        while (MovementActive)
        {

            yield return new WaitForSeconds(0.25f); //this is the refresh rate 

            if (LocalUnit.IsUnitMoving)
                yield return null;

            if (Target.GridPos != OldTPos)
            {
                if (Target.IsUnitMoving)
                    yield return null;

                //move to the new place
                OldTPos = Target.GridPos;
                PathToTileClose = LocalUnit.MapLocal.Pathfinding(LocalUnit.GridPos, Target.GridPos);
                if (PathToTileClose.Count > 1)
                    PathToTileClose.RemoveAt(PathToTileClose.Count - 1); //this should work
            }


            if (LocalUnit.GridPos == PathToTileClose[PathToTileClose.Count - 1].GetPos)
            {
                //Debug.Log("Got to target");
                yield return null;
            } else
            {
                LocalUnit.MoveUnitToPremadePath(PathToTileClose);
            }
                


            // if (OldTPos != Target.GridPos)
            //      PathToTileClose = LocalUnit.MapLocal.Pathfinding(LocalUnit.GridPos, Target.GridPos);
            /*
   

                        if (LocalUnit.GridPos == PathToTileClose[PathToTileClose.Count - 1].GetPos)
                            break;

                        if (OldTPos != Target.GridPos)
                        {
                            PathToTileClose = LocalUnit.MapLocal.Pathfinding(LocalUnit.GridPos, Target.GridPos);
                            PathToTileClose.RemoveAt(PathToTileClose.Count - 1); //this should work
                            LocalUnit.MoveUnitToPremadePath(PathToTileClose);
                            OldTPos = Target.GridPos;
                        }*/

        }
    }
    void Mode_Flee(Unit Target)
    {
        //it needs to just like run away?
        StartCoroutine(KeepFleeing());

    }

    IEnumerator KeepFleeing()
    {
        Unit Target = null;
        Vector2Int TargetEscapeTile = new Vector2Int();
        while(MovementActive)
        {
            yield return new WaitForSeconds(AlertTime); //refresh rate again (I should check if this actually helps with performance)
            Target = GetPlayerAroundHere(FleeRadius); //detect 

            if (Target == null)
            {
                yield return null;
            } else
            {
                //do the math so I can go to the opposite point and scape
                int HowFarToGo = (FleeRadius - LocalUnit.MapLocal.GetDistance(LocalUnit.GridPos, Target.GridPos)); //while its closer it should move further away (The minimum being 2)

                if (HowFarToGo <= 0)
                    HowFarToGo = 1;

                TargetEscapeTile =  ((LocalUnit.GridPos - Target.GridPos) * HowFarToGo) + Target.GridPos;

                MapTile Tile = LocalUnit.MapLocal.FindTile(TargetEscapeTile.x, TargetEscapeTile.y);

                if (Tile != null && Tile.Walkable)
                {
                    LocalUnit.MoveUnitTo(Tile.X, Tile.Y);
                } else if (Tile != null && !Tile.Walkable)
                {
                    //when this occurs I wanna try a full area of tiles near that one to get one route
                    List<MapTile> OtherOptions = LocalUnit.MapLocal.GetAreaAround(TargetEscapeTile, FleeRadius);

                    for (int x = 0; x < OtherOptions.Count; x++)
                    {
                        if (OtherOptions[x].Walkable)
                        {
                            LocalUnit.MoveUnitTo(OtherOptions[x].X, OtherOptions[x].Y);
                            break;
                        }
                    }

                }

            }

        }
    }

    Unit GetPlayerAroundHere(int ZoneSize)
    {
        List<MapTile> Area = LocalUnit.MapLocal.GetAreaAround(LocalUnit.GridPos, ZoneSize);

        for(int x= 0; x< Area.Count; x++)
        {
            if(Area[x].OcupiedByUnit == UnitIn.Player)
            {
                return Area[x].OcupyingUnit;
                
            }
        }
        return null;
    }
    

    //The Unit moves in its own 

}
