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
                if(LocalMap.GetDistance(Player.GridPos, U.GridPos) <= U.unitStats.VisionRange)
                {
                    Debug.Log("Attack");
                }
            }
            yield return new WaitForSeconds(RefreshRate);
        }
    }
}
