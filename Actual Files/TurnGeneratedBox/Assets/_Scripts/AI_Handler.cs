using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AI_State { Explore, Patrol, Flee, Chase };
public class AI_Handler : MonoBehaviour
{

    //This should eventually replace the Enemy_AI one

    [Header("General Settings")]
    public float RefreshRate = 0.5f;
    private AI_State UnitState = AI_State.Explore;
    private Unit LocalUnitScript;
    private bool ChangePath = false;

    [Header("State Defining")]
    public int LowHPDefine = 2;
    public int CloseToPlayerArea = 4;
    // Use this for initialization

    void Start()
    {
        if (this.GetComponent<Unit>())
        {
            LocalUnitScript = this.GetComponent<Unit>();
        }
        else
        {
            Debug.Log("Unit Script could not be found");
        }

    }

    /*
    List<MapTile> ExploreMode()
    {


    }

    AI_StateClass PatrolMode()
    {
        return new AI_StateClass(AI_State.Patrol);
    }

    List<MapTile> FleeMode()
    {

    }

    List<MapTile> ChaseMode()
    {

    }

    AI_State DetectState()
    {
        bool PlayerClose = false, LowHP = false;

        if (LocalUnitScript.unitStats.LifePoints <= LowHPDefine)
            LowHP = false;

        List<MapTile> Area = LocalUnitScript.MapLocal.GetAreaAround(LocalUnitScript.GridPos, CloseToPlayerArea); //I might wanna optimize this
        foreach (MapTile M in Area)
        {
            if(M.OcupiedByUnit == UnitIn.Player)
            {
                PlayerClose = true;
                break;
            }
        }

        if(!PlayerClose && !LowHP)
        {
            return AI_State.Explore;
        } else if (!PlayerClose && LowHP)
        {
            return AI_State.Patrol;
        } else if(PlayerClose && !LowHP)
        {
            return AI_State.Chase; //Might wanna pass the player's position right away
        } else
        {
            return AI_State.Flee; //Might wanna pass the player's position right away
        }
    }
    

    IEnumerator AI_Update()
    {
        List<MapTile> CurrentPath = new List<MapTile>();
        while (true)
        {
            yield return new WaitForSeconds(RefreshRate); //This should optimize it

            //I need to figure this out

            if (ChangePath)
            {
                switch (DetectState())
                {
                    case AI_State.Explore:
                        CurrentPath = ExploreMode();
                        break;

                    case AI_State.Chase:
                        CurrentPath = ChaseMode();
                        break;

                    case AI_State.Flee:
                        CurrentPath = FleeMode();
                        break;

                    case AI_State.Patrol:
                        CurrentPath = PatrolMode();
                        break;

                }
            } else
            {
                //if the path didnt change we gotta 
            }
       
        }
    }
}
*/

    public class AI_StateClass
    {
        public AI_State State;
        public bool Update = false;
        public List<MapTile> NextPath = new List<MapTile>();

        public AI_StateClass(AI_State state)
        {
            State = state;
        }


    } //I went with a class that can hold several variables so we can know when to update and stuff
}