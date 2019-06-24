using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapTile
{
    public TileType Type;
    public int X, Y, Roughness;
    public MapTile[] Neighbours = new MapTile[4];
    // 0 es arriba
    // 1 es derecha
    // 2 es abajo
    // 3 es izquierda
    public MapTile[] FarNeighbours = new MapTile[4];
    public MaterialTile OcupedByMat = MaterialTile.None;
    public UnitIn OcupiedByUnit = UnitIn.None;

    public bool Walkable
    {
        get
        {
            if ( OcupiedByUnit == UnitIn.None && OcupedByMat == MaterialTile.None && Type != TileType.Water)
            {
                return true;
            } else
            {
                return false;
            }
        }
    }

    //Stuff for the pathfinding
        public int GCost, HCost;
        public MapTile parent; //Not totally sure how this one works
    public int FCost
    {
        get
        {
            return GCost + HCost;
        }
    }

    public MapTile(int newX, int newY, int newRoughness, TileType NewType) //Simple constructor to give it values quick
    {
        Neighbours = new MapTile[4];
        FarNeighbours = new MapTile[4];
        X = newX;
        Y = newY;
        Roughness = newRoughness;
        Type = NewType;
        OcupedByMat = MaterialTile.None;
      //  Debug.Log("me llaman");
    }

    public Vector2Int GetPos
    {
        get
        {
            return new Vector2Int(X, Y);
        }

    }

}

public enum TileType { Grass, Water };
public enum MaterialTile { None, Tree, Mountain};
public enum UnitIn { None, Player, AI};

