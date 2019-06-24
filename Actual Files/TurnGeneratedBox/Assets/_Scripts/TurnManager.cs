using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnOwner { Player, AI};
public class TurnManager : MonoBehaviour {

    public TurnOwner ActiveTurn;

    public void PassTurn()
    {
        if (ActiveTurn == TurnOwner.Player)
        {
            ActiveTurn = TurnOwner.AI;
        } else
        {
            ActiveTurn = TurnOwner.Player;
        }
    }
}
