using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    float RefreshRate= 0.25f;
    public Unit Player;
    public GameObject EnemyHolder;
    private List<Unit> AllEnemies = new List<Unit>();
    Map_Generator LocalMap;
    public UI_Manager UIM;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Unit U in EnemyHolder.transform.GetComponentsInChildren<Unit>()) //create a way to detect when a new enemy is detected and add to it
        {
            AllEnemies.Add(U);
        }
        LocalMap = this.GetComponent<Map_Generator>();
        StartCoroutine(Refresh());
    }

IEnumerator Refresh()
    {
        while (true)
        {
           foreach(Unit U in AllEnemies)
            {
            if((LocalMap.GetDistance(Player.GridPos, U.GridPos) <= U.unitStats.VisionRange) && U.IsAttacking == false && Player.IsAttacking == false)
                {
                    Debug.Log("Attack");
                    Player.IsAttacking = true;
                    U.IsAttacking = true;
                    LocalMap.TurnModeOn = true;
                    LocalMap.PlayerTurn = true;
                    Player.StopMoving = true;
                    UIM.ClearPath();
                    LocalMap.CurrentTile = LocalMap.FindTile(Player.GridPos.x, Player.GridPos.y);
                    UIM.ChangeOfSelection();


                } //It stars the attack phase
           
             if (U.IsAttacking && Player.IsAttacking)  //it waits until the player moves 
                {
              
                if(Player.unitStats.ActionPoints == 0)
                    {
                        //Instruct the Enemy to move in a turn (Or generate the movement or sum)
                    }
                
                }
            
            
            
            }

            yield return new WaitForSeconds(RefreshRate);
        }
    }
}
