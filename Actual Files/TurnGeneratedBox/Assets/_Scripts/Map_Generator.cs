using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine;
using System.IO;
using System;


public class Map_Generator : MonoBehaviour {

    //This will make it accesible by everyscript
    // public static Map_Generator MapStatic;
    string DataPath;


    public MapTile CurrentTile = null; //This is the current selected tile

    public bool MapAsPrefab = false;

    public Tilemap Map,FirstLayer, SecondLayer, ThirdLayer;
    public TileType DefaultTile;
    [System.Serializable]
    public class Grass_Tiles
    {
        public Tile[] Basic;

    }
    public Grass_Tiles grass_Tiles = new Grass_Tiles();

    [System.Serializable]
    public class Water_Tiles
    {
    public Tile[] Basic;
    public Tile[] Grass;
    public Tile[] Sand;
    public Tile[] Snow;

    /*
     0 - Un Lado
     1 - Dos Lados
     2 - Tres Lados
     3 - Todos los lados
     4 - Paralelos
     5 - Una esquina
     6 - Dos esquinas
     7 - Tres esquinas
     8 - Todas las esquinas
     9 - Lados opuestos
     */
    }
    public Water_Tiles water_Tiles = new Water_Tiles();

    [System.Serializable]
    public class Materials
    {
        public Tile[] Trees;
        public Tile[] Mountains;
    }
    public Materials materials = new Materials();

    [System.Serializable]
    public class Particles_Tiles
    {
        public Tile[] Area;

     /*
     0 - Un Lado
     1 - Dos Lados
     2 - Tres Lados
     3 - Todos los lados
     4 - Una esquinita   
     5 - Nungun lado
     6 - Path
      */


    }
    public Particles_Tiles particles_tiles = new Particles_Tiles();

    public Tile GridTileShowThing;

 
     public List<MapTile> AllMapTiles = new List<MapTile>(); //Need to find a way to access easily the list via the X and Y value



    [HideInInspector]
    public bool TurnModeOn = false;

    //Map settings
    //public int Size = 10;
    [Header("Seed Parameters")]
    public bool LoadSeed = false; //Prob not the best but whatever 
    public bool SaveSeed = true;
    [Tooltip("Element 0 = Water | Element 1 = Trees | Element 2 = Mountains")]
    public string[] Seed;
    [Header("Map Randomizer Parameters")]
    public int minW = 50;
    public int maxW = 100;
    public int minT = 0, maxT = 0;
    public int minM = 5, maxM = 10;

    private GameObject PlayerUnitsHolder;


    // This will insitalize a map with a certain type of tile
    private void Awake()
    {
        DataPath = Path.Combine(Application.persistentDataPath, "MapData.txt");
        AllMapTiles.Clear();
        if (!MapAsPrefab)
        {
            if (!StaticMapConf.NewMap)
            {
                if (!LoadSeed) //old slow way
                {
                    AllMapTiles = LoadMap();
                    if (AllMapTiles.Count > 0)
                    {

                        int S = (int)Mathf.Sqrt(AllMapTiles.Count); //Only works with squared maps
                        CreateGraph(S);
                        // PhysicalMap(AllMapTiles);
                    }
                } else
                {
                    MapGenerator(StaticMapConf.Size);
                    CreateGraph(StaticMapConf.Size);
                    LoadSeadedMap(Seed);
                }

            }
            else
            {//Esto es para crear un mapa nuevo
                Debug.Log("New map created");
                MapGenerator(StaticMapConf.Size);
                CreateGraph(StaticMapConf.Size);
                MapRandomizer(StaticMapConf.Size); //Algunas veces causa el (Array out of range)
            }
        } else
        {
            AllMapTiles = LoadMap();
            CreateGraph((int)Mathf.Sqrt(AllMapTiles.Count));
            Debug.Log("Map loaded as Prefab");
        }

    }
    void Start () {

        // List<MapTile> MyPath = new List<MapTile>();

        //  MyPath = Pathfinding() 

        PhysicalMap(AllMapTiles);
        PlayerUnitsHolder = GameObject.FindGameObjectWithTag("Player_UnitH");
       
    }

    void MapGenerator(int size)
    {
        AllMapTiles = new List<MapTile>();
 

        int StartMin = -(size / 2);
        int StartMax = size / 2;
      //  Debug.Log("Star min: " + StartMin);
        if (size % 2 != 0) //not sure if it works
            StartMax++;

        for (int y = StartMin; y < StartMax; y++)
        {
            for (int x = StartMin; x < StartMax; x++)
            {
                CreateTile(DefaultTile, x, y);
            }

        }
    }

    void MapRandomizer(int Size)
    {
      //  int StartMin = -(Size / 2);
       // int StartMax = Size / 2;
        //Debug.Log(LakeAmount);
       // if (Size % 2 != 0) //not sure if it works
        //    StartMax++;

        //Water Part 
        int LakeAmount = UnityEngine.Random.Range(1, Size / 5);
       // Seed += LakeAmount + ".";
        List<MapTile> PossibleTiles = new List<MapTile>();
        for (int R = 0; R < LakeAmount; R++)
        {
            //Change this a bit to make the code shorter
            /*
            int RX = UnityEngine.Random.Range(StartMin, StartMax - 1);
            int RY = UnityEngine.Random.Range(StartMin, StartMax - 1);
            Seed += RX;
            Seed += RY;
            */
            int RIndex = UnityEngine.Random.Range(0, AllMapTiles.Count - 1);
            MapTile RTile = AllMapTiles[RIndex];
            ChangeTile(TileType.Water, RTile.X, RTile.Y, 1, SaveSeed);
           // Seed[0] += RIndex+ "|";

            //crear estrellas una cerca de la otra
            int StartCounter = UnityEngine.Random.Range(minW, maxW);
          //  Seed += StartCounter + ":";

          //  foreach (MapTile MP in )
            for (int t = 0; t < StartCounter; t++)
            {
                //creates the star
                foreach (MapTile N in RTile.Neighbours)
                {
                    if (N != null && N.Type != TileType.Water)
                    {
                        ChangeTile(TileType.Water, N.X, N.Y, 1, true);
                       // Seed[0] += AllMapTiles.IndexOf(N) + "|"; 
                        PossibleTiles.Add(N);
                    }

                }

                int TheTile = UnityEngine.Random.Range(0, PossibleTiles.Count);
                if (PossibleTiles[TheTile]!= null)
                {
                    RTile = PossibleTiles[TheTile];
                    PossibleTiles.RemoveAt(TheTile);
                } else
                    {
                        Debug.Log("Hay Un null");
                    }


                if (PossibleTiles.Count <= 0)
                {
                    Debug.Log("See acabaron");
                    break;
                }
            }

        }
    // Cheap way to fix
        Seed[0] = Seed[0].Remove(Seed[0].Length - 1);
        //Debug.Log(Seed[0]);


        //Mountains Part
        int MountainsAmount = UnityEngine.Random.Range(4, Size / 4);
        for (int R = 0; R < MountainsAmount; R++)
        {
            /*
            int RX = UnityEngine.Random.Range(StartMin, StartMax - 1);
            int RY = UnityEngine.Random.Range(StartMin, StartMax - 1);
            */
            int RIndex = UnityEngine.Random.Range(0, AllMapTiles.Count - 1);
            MapTile RTile = AllMapTiles[RIndex];


            if (RTile != null && RTile.Type != TileType.Water && RTile.OcupedByMat  == MaterialTile.None)
            {
                OcupyTileMat(MaterialTile.Mountain, RTile.X, RTile.Y, SaveSeed);

                int RLengh = UnityEngine.Random.Range(minM, maxM);
                int RDir = UnityEngine.Random.Range(0, 3);
                List<MapTile> TilesInM = new List<MapTile>();

               // int Temp = 0;
                for (int M = 0; M <=RLengh; M++)
                {
                    MapTile T = null;

                  //  Debug.Log(RDir);
    
                    if (RTile.Neighbours[RDir] != null)
                    {
                        T = RTile.Neighbours[RDir];
                        RTile = T;
                    }


                    TilesInM.Add(T);
                  //  Debug.Log(TilesInM.Count);

                    if (T != null && T.Type != TileType.Water && T.OcupedByMat == MaterialTile.None)
                    {
                        OcupyTileMat(MaterialTile.Mountain, T.X, T.Y, SaveSeed);
                    }  else if (T != null && T.Type == TileType.Water) 
                    {
                        if (TilesInM[TilesInM.Count - 2] != null)
                        {
                     //       Debug.Log("uhh");
                            for (int E=0; E<= 3; E++)
                            {
                                MapTile N = TilesInM[TilesInM.Count - 2];
                                //this never happens for some reason 
                                if (N != null && N.Type != TileType.Water && N.Neighbours[E] != null && N.Neighbours[E].Type!= TileType.Water && N.Neighbours[E].OcupedByMat == MaterialTile.None)
                                {
                                    RDir = E;
                                    // RX = N.X;
                                    // RY = N.Y;
                                    RTile = N;
                                    OcupyTileMat(MaterialTile.Mountain, N.Neighbours[E].X, N.Neighbours[E].Y, SaveSeed);
                                 //   Debug.Log(E);
                                    break;
                                }

                            }
                        }

                    }

                    //Randomize a bit the dir
                    int RR = UnityEngine.Random.Range(0, 100);
                    int Chance = 75;
                    if (RR > Chance)
                    {
                       
                        //change directions
                        if (RDir == 0 || RDir == 2)
                        {
                            if (RR > Chance + (Chance / 2))
                            {
                                RDir = 1;
                            } else
                            {
                                RDir = 3;
                            }
                        }
                        else
                        {
                            if (RR > Chance + (Chance / 2))
                            {
                                RDir = 0;
                            }
                            else
                            {
                                RDir = 2;
                            }
                        }
                    }
                }
            }
        }

        Seed[1] = Seed[1].Remove(Seed[1].Length - 1);
        //Trees Part
        PossibleTiles.Clear();
        int ForestAmount = UnityEngine.Random.Range( Size/4, Size / 2);
        for (int R = 0; R < ForestAmount; R++)
        {
            /*
             int RX = UnityEngine.Random.Range(StartMin, StartMax - 1);
             int RY = UnityEngine.Random.Range(StartMin, StartMax - 1);
             */
             int RIndex = UnityEngine.Random.Range(0, AllMapTiles.Count-1);
            MapTile RTile = AllMapTiles[RIndex];

            if (RTile != null && RTile.OcupedByMat == MaterialTile.None && RTile.Type != TileType.Water)
            {
                OcupyTileMat(MaterialTile.Tree, RTile.X, RTile.Y, SaveSeed);

                int StartCounter = UnityEngine.Random.Range(minT, maxT);


                //  foreach (MapTile MP in )
                for (int t = 0; t < StartCounter; t++)
                {
                    //creates the star
                    foreach (MapTile N in RTile.Neighbours)
                    {
                        if (N != null && N.Type != TileType.Water && N.OcupedByMat == MaterialTile.None)
                        {
                            OcupyTileMat(MaterialTile.Tree, N.X, N.Y, SaveSeed);
                            PossibleTiles.Add(N);
                        }

                    }

                    int TheTile = UnityEngine.Random.Range(0, PossibleTiles.Count);
                    if (PossibleTiles[TheTile] != null)
                    {
                      //  RX = PossibleTiles[TheTile].X;
                       // RY = PossibleTiles[TheTile].Y;
                        RTile = PossibleTiles[TheTile];
                        PossibleTiles.RemoveAt(TheTile);
                    }
                    else
                    {
                        Debug.Log("Hay Un null");
                    }


                    if (PossibleTiles.Count <= 0)
                    {
                        Debug.Log("See acabaron");
                        break;
                    }
                }

            } 

            //crear estrellas una cerca de la otra
          
        }
        Seed[2] = Seed[2].Remove(Seed[2].Length - 1);



    }
    
    //esto puede ser optimizado con el SetTileRotation
    //Esta verga no furula
    void PhysicalMap(List<MapTile> MapData)
    {
        //Change this so it goes organized by the order of the map because the order is probably not being respected
        for (int x =0; x < MapData.Count; x++)
        {
            if (MapData[x] != null)
            {
                Tile ActualTile = null;
                Tile OcupyTile = null;
                bool North = false, East = false, South = false, West = false;
                bool NorthWest = false, NorthEast = false, SouthEast = false, SouthWest = false;
                int GrassCloseN = 0, GrassFarN = 0;
                //The floor
                switch (MapData[x].Type)
                {
                    case TileType.Grass:
                        ActualTile = grass_Tiles.Basic[0];
                        break;
                    case TileType.Water:
                        // Debug.Log("agua");
                        //select the one that is actually acurate
                        North = false; East = false; South = false; West = false;
                        NorthWest = false; NorthEast = false; SouthEast = false; SouthWest = false;

                        for (int nn=0; nn< MapData[x].Neighbours.Length; nn++)
                        //foreach (MapTile N in AllMapTiles[x].Neighbours)
                        {
                            if (MapData[x].Neighbours[nn]!= null && MapData[x].Neighbours[nn].Type == TileType.Grass)
                            {
                                GrassCloseN++;

                                if (nn == 0) //North
                                {
                                    North = true;
                                }

                                if (nn == 2) //South
                                {
                                    South = true;
                                }

                                if (nn == 1) //East
                                {
                                    East = true;
                                }

                                if (nn == 3) //West
                                {
                                    West = true;
                                }
                            }
                                
                        }

                       switch(GrassCloseN)
                        {
                            case 4:
                                ActualTile = water_Tiles.Grass[3];
                                break;

                            case 3:
                                ActualTile = water_Tiles.Grass[2];
                                break;
                            case 2:
                                if ((North && South) || (East && West))
                                {
                                    ActualTile = water_Tiles.Grass[4];
                                } else
                                {
                                    ActualTile = water_Tiles.Grass[1];
                                }

                                break;
                            case 1:
                               // Debug.Log("should");
                                ActualTile = water_Tiles.Grass[0];
                                break;

                            default:
                                //Algo esta mal aqui
                                for (int fn=0; fn < MapData[x].FarNeighbours.Length; fn++)
                                //foreach (MapTile FN in AllMapTiles[x].FarNeighbours)
                                {
                                    if (MapData[x].FarNeighbours[fn] != null && MapData[x].FarNeighbours[fn].Type == TileType.Grass)
                                    {
                                        GrassFarN++;

                                        if (fn == 0) // NorthWest
                                        {
                                            NorthWest = true;
                                        }
                                        if (fn == 1) // NorthEast
                                        {
                                            NorthEast = true;
                                        }
                                        if (fn == 2) // SouthEast
                                        {
                                            SouthEast = true;
                                        }
                                        if (fn == 3) // SouthWest
                                        {
                                            SouthWest = true;
                                        }
                                    }

                                }
                                break;
                        }

                        if (GrassCloseN == 0)
                        {
                            switch (GrassFarN)
                            {
                                case 4:
                                    ActualTile = water_Tiles.Grass[8];
                                    break;
                                case 3:
                                    ActualTile = water_Tiles.Grass[7];
                                    break;
                                case 2:
                                    if ((NorthWest && SouthEast) || (NorthEast && SouthWest))
                                    {
                                        ActualTile = water_Tiles.Grass[9];
                                    } else
                                    {
                                        ActualTile = water_Tiles.Grass[6];
                                    }


                                    break;
                                case 1:
                                    ActualTile = water_Tiles.Grass[5];
                                    break;
                                default:
                                    ActualTile = water_Tiles.Basic[0]; //basic water always
                                    break;
                            }
                        }

                       
                       // ActualTile = water_Tiles.Basic[0];
                        break;

                }
                //Whats on Layer 2
                switch (MapData[x].OcupedByMat)
                {
                    case MaterialTile.None:
                        OcupyTile = null;
                        break;

                    case MaterialTile.Tree:
                        int R = UnityEngine.Random.Range(0, materials.Trees.Length);
                        OcupyTile = materials.Trees[R];
                        break;

                    case MaterialTile.Mountain:
                        OcupyTile = materials.Mountains[0];
                        break;


                }

                // I changed the X and Y and it worked, not sure why tho
                Map.SetTile(new Vector3Int(MapData[x].X, MapData[x].Y, 0), ActualTile);
                SetTileRotation(MapData[x].X, MapData[x].Y,GrassCloseN,GrassFarN,North,East,South,West,NorthWest,NorthEast,SouthEast,SouthWest);

                if (OcupyTile != null)
                {
                    SecondLayer.SetTile(new Vector3Int(AllMapTiles[x].X, AllMapTiles[x].Y, 0), OcupyTile);
                }


                GrassCloseN = 0;
                GrassFarN = 0;
            } else
            {
                Debug.Log("Ay mama");
                MapData.RemoveAt(x);
            }

            //create the Grid show thing
            FirstLayer.SetTile(new Vector3Int(MapData[x].X, MapData[x].Y,0), GridTileShowThing);


          //  Debug.Log("X:" + MapData[0].X + " |Y:" + MapData[0].Y + " / Type:" + MapData[0].Type);

        }
    }


    void SetTileRotation (int X, int Y,int AmountN, int AmountFN, bool N, bool E, bool S, bool W, bool NW, bool NE, bool SE, bool SW)
    {
        float R = 0;
        //Identifiying all
        if (AmountN != 0)
        {
            switch(AmountN)
            {
                case 4:
                    //Nothing happens so rotate equal 0
                    break;
                case 3:
                    if (N && E && S && !W)
                    {
                        R = -90 ;
                    } else if (!N && E && S && W)
                    {
                        R = 180 ;
                    } else if (N && !E && S && W)
                    {
                        R = 90 ;
                    }

                    break;
                case 2:
                    if (N && E && !S && !W)
                    {
                        R = -90 ;
                    } else if (!N && E && S && !W)
                    {
                        R = 180 ;
                    } else if (!N && !E && S && W)
                    {
                        R = 90 ;
                    } else if (N && !E && S && !W)
                    {
                        R = -90 ;
                    }
                    break;

                case 1:
                    if (!N && E && !S && !W)
                    {
                        R = -90 ;
                    }
                    else if (!N && !E && S && !W)
                    {
                        R = 180 ;
                    }
                    else if (!N && !E && !S && W)
                    {
                        R = 90 ;
                    }
                    break;
            }

        } else
        {
            switch (AmountFN)
            {
                case 4:
                    break;
                case 3: //no lo he probao
                    if (!NW && NE && SE && SW)
                    {
                        R = -90 ;
                    } else if (NW && !NE && SE && SW)
                    {
                        R = 180 ;
                    } else if (NW && NE && !SE && SW)
                    {
                        R = 90 ;
                    }
                    break;
                case 2:
                    if (!NW && NE && SE && !SW)
                    {
                        R = -90 ;
                    } else if (!NW && !NE && SE && SW)
                    {
                        R = 180 ;
                    } else if (NW && !NE && !SE && SW)
                    {
                        R = 90;
                    } else if (!NW && NE && !SE && SW)
                    {
                        R = -90 ;
                    }
                    break;
                case 1:
                    if (!NW && !NE && SE && !SW)
                    {
                        R = -90;
                    } else if (!NW && !NE && !SE && SW)
                    {
                        R = 180 ;
                    } else if (NW && !NE && !SE && !SW)
                    {
                        R = 90;
                    }
                    break;
            }
        }




        var m = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, R), Vector3.one);
        if (FindTile(X,Y)!=null)
        {
            Map.SetTransformMatrix(new Vector3Int(X,Y,0), m);
        }

        
    }
	

    void CreateTile(TileType T, int X, int Y)
    {
        MapTile Plox = new MapTile(X, Y,1, T);
        AllMapTiles.Add(Plox);

    }

    void ChangeTile(TileType T, int X, int Y, int Roughness, bool Saving)
    {
        MapTile Tile = FindTile(X, Y);

        if (Tile != null)
        {
            Tile.Type = T;

            if (Saving)
            {
                switch (T)
                {
                    case TileType.Water:
                        Seed[0] += AllMapTiles.IndexOf(Tile) + "|";
                        break;
                }
            }

        }
 
    }

    void OcupyTileMat (MaterialTile M, int X, int Y, bool Saving)
    {
        MapTile Tile = FindTile(X, Y);
        if (Tile != null)
        {
            Tile.OcupedByMat = M;

            if(Saving)
            {
                switch (M)
                {
                    case MaterialTile.Mountain:
                        Seed[1] += AllMapTiles.IndexOf(Tile) + "|";
                        break;

                    case MaterialTile.Tree:
                        Seed[2] += AllMapTiles.IndexOf(Tile) + "|"; ;
                        break;
                }
            }

        }
            
    }

    public void OcupyTileUnit (Unit U, int X, int Y)
    {
        if (FindTile(X, Y) != null)
        {
            FindTile(X, Y).OcupiedByUnit = U.Type;
            FindTile(X, Y).OcupyingUnit = U;
            U.gameObject.transform.position = SetTilePosToWorld(X,Y);
            //This seems to work tho
           // Debug.Log("Ay");
        }

    }

    public void UnocupyTileUnit (int X, int Y)
    {
        if (FindTile(X,Y) != null)
        {
            FindTile(X, Y).OcupiedByUnit = UnitIn.None;
            FindTile(X, Y).OcupyingUnit = null;
        }
    }

    public Vector3 SetTilePosToWorld(int X, int Y)
    {
        return (Map.CellToWorld(new Vector3Int(X, Y, 0))) + new Vector3(.5f, .5f, 0);
    }
    
    void CreateGraph(int Size)
    {
        //Crea la lista e nuevo
        
        //Right not it will only take all the neighbours and add them
        int StartMax = Size / 2;
        int StartMin = -(Size / 2);


        if (Size % 2 != 0) //not sure if it works
           StartMax++;

        
       // StartMin++;
        for (int y = StartMin ; y < StartMax ; y++)
            {
            for (int x = StartMin ; x < StartMax ; x++)
                {
                //porsia
                if (FindTile(x,y)!= null)
                {
                    FindTile(x, y).Neighbours = new MapTile[4];
                    FindTile(x, y).FarNeighbours = new MapTile[4];

                    //Close neighbours
                    if (y != StartMax)
                    {
                        FindTile(x, y).Neighbours[0] = FindTile(x, y + 1); //Top
                    }
                    else { Debug.Log("Invalid Neighbour on top"); }

                    if (x != StartMax) //Cheapest way to fix it, but it works
                    {
                        FindTile(x, y).Neighbours[1] = FindTile(x + 1, y); //Right
                    }
                    else { Debug.Log("Invalid Neighbour on right"); }


                    if (y != StartMin - 1) //Ta mal
                    {
                        FindTile(x, y).Neighbours[2] = FindTile(x, y - 1); //Bottom
                    }
                    else { Debug.Log("Invalid Neighbour on bot"); }


                    if (x != StartMin - 1)
                    {
                        FindTile(x, y).Neighbours[3] = FindTile(x - 1, y); //Left
                    }
                    else { Debug.Log("Invalid Neighbour on left"); }





                    //far neighbours
                    if (!(y == StartMax || x == StartMin - 1))
                    {
                        FindTile(x, y).FarNeighbours[0] = FindTile(x - 1, y + 1); //TopLeft
                    }
                    else { Debug.Log("Invalid FarNeighbour on topleft"); }


                    if (!(y == StartMax || x == StartMax))
                    {
                        FindTile(x, y).FarNeighbours[1] = FindTile(x + 1, y + 1); //topright
                    }
                    else { Debug.Log("Invalid FarNeighbour on topright"); }


                    if (!(y == StartMin - 1 || x == StartMax))
                    {
                        FindTile(x, y).FarNeighbours[2] = FindTile(x + 1, y - 1); //Botright
                    }
                    else { Debug.Log("Invalid FarNeighbour on botright"); } //Ta Mal


                    if (!(y == StartMin - 1 || x == StartMin - 1))
                    {
                        FindTile(x, y).FarNeighbours[3] = FindTile(x - 1, y - 1); //Botleft
                    }
                    else { Debug.Log("Invalid FarNeighbour on botleft"); } //Ta mal

                }



            }
            }

      //  Debug.Log(prueba);
    }
    
    public MapTile DFindTile(int X, int Y) //This works but seems slower than the other one
    {
        int Size = (int)Mathf.Sqrt(AllMapTiles.Count);
        int Index = (X + Size/2)+ ((Y + Size/2) * Size); //Only works with squared maps
                                                         //  Debug.Log(Size/2);
        if (AllMapTiles[Index] != null)
        {
            return AllMapTiles[Index];
        }
        else
        {
            Debug.Log("Tile does not exist for the index :" + Index);
            return null;
        }

    }
    

    public MapTile FindTile(int x, int y) //This super good now
    {
        // Debug.Log("x");
        return AllMapTiles.Find(whatever => whatever.X == x &&  whatever.Y == y);
        //Seems to work wow


        /*
        for (int i = 0; i < AllMapTiles.Count; i++)
        {
            if (AllMapTiles[i].X == x && AllMapTiles[i].Y == y)
            {
                return AllMapTiles[i];
            }
        }
        */
       // Debug.Log("Tile ("+x+"/"+y+") could not be found");
        //return null;
    }


    //this is not needed (Might delete later)
    private Unit FindUnit (int x, int y)
    {
        Unit u = null;
        foreach (Unit child in PlayerUnitsHolder.transform.GetComponentsInChildren<Unit>())
        {
            if (child.GridPos.x == x && child.GridPos.y == y)
            {
                u = child;
                break;
            }

        }

        if (u == null)
            Debug.Log("Unit couldnt be found");

        return u;

    }
    
    public List <MapTile> Pathfinding(Vector2Int Origin, Vector2Int Target)
    {
        MapTile OriginT = FindTile(Origin.x, Origin.y);
        MapTile TargetT = FindTile(Target.x, Target.y);

        //Temporarily unocupy the start (Cheap way to fix it) Might not even be necessary but I cant be bothered to change it
        UnitIn TempU = OriginT.OcupiedByUnit;
        OriginT.OcupiedByUnit = UnitIn.None;

        Heap<MapTile> OpenTiles = new Heap<MapTile>(AllMapTiles.Count); //not sure how it wooorks
        HashSet<MapTile> ClosedTiles= new HashSet<MapTile>();
        OpenTiles.Add(OriginT);

        for (int b = 0; b < 3; b++)
        {
            while (OpenTiles.Count > 0)
            {
                MapTile current = OpenTiles.RemoveFirst();

                ClosedTiles.Add(current);

                if (current == TargetT)
                {
                    List<MapTile> Path = new List<MapTile>();
                    MapTile cur = TargetT;
                    while (cur != OriginT)
                    {
                        Path.Add(cur);
                        cur = cur.parent;
                    }

                    Path.Reverse();
                   OriginT.OcupiedByUnit = TempU; //cheap way 
                    return Path;

                }

                foreach (MapTile Neighbour in current.Neighbours)
                {

                    if (Neighbour == null || !current.Walkable || ClosedTiles.Contains(Neighbour))
                    {
                        continue;
                    }

                    int MoveCostToN = current.GCost + GetDistance(current.GetPos, Neighbour.GetPos); //This probably does not work

                    if (MoveCostToN < Neighbour.GCost || !OpenTiles.Contains(Neighbour))
                    {
                 
                        Neighbour.GCost = MoveCostToN;
                        Neighbour.HCost = GetDistance(Neighbour.GetPos, TargetT.GetPos); //Same thing with this
                        Neighbour.parent = current;

                        if (!OpenTiles.Contains(Neighbour))
                            OpenTiles.Add(Neighbour);
                    }

                }
            }

        }


        OriginT.OcupiedByUnit = TempU; 
        return null;
 
    }

    public int GetDistance(Vector2Int A, Vector2Int B) //This seems to work
    {
        int dx = Math.Abs(A.x - B.x);
        int dy = Math.Abs(A.y - B.y);

        int min = Math.Min(dx, dy);
        int max = Math.Max(dx, dy);

        int diagonalSteps = min;
        int straightSteps = max - min;

        return (int)(Math.Sqrt(2) * diagonalSteps + straightSteps);
    }

     List<MapTile> GetWalkableAreaAround(Vector2Int Origin, int size)
    {
        List<MapTile> list = new List<MapTile>();
        for(int x = -size; x<= size; x++)
        {
            for(int y = -size; y <= size; y++)
            {
                MapTile T = FindTile(Origin.x+x, Origin.y+y);

                if (T != null && T.Walkable == true)
                {
                    list.Add(T);
                }
            }
        }


        return list;
    }

    // I wanna save all the pathfindings so they dont have to be done again
    public IDictionary<MapTile, List<MapTile>> SpawnAreaParticle(Vector2Int Origin, int size)
    {
        IDictionary<MapTile, List<MapTile>> AllPaths = new Dictionary<MapTile, List<MapTile>>();
        List <MapTile> WalkableTiles = GetWalkableAreaAround(Origin, size);
        //First clear all the third layer
        ThirdLayer.ClearAllTiles();
        foreach(MapTile N in WalkableTiles)
        {
            List<MapTile> CurPath = Pathfinding(Origin, new Vector2Int(N.X, N.Y));
            AllPaths.Add(N, CurPath);

            //This looks super slow
            if (CurPath != null)
            {
                ThirdLayer.SetTile(new Vector3Int(N.X, N.Y, 0), particles_tiles.Area[5]);
            }

        }

        return AllPaths;
    }


    public void SaveMap()
    {
        //Muuuy lento
          string jsonString = "";
          for (int x = 0; x < AllMapTiles.Count; x++)
          {
              jsonString += JsonUtility.ToJson(AllMapTiles[x]);
              jsonString += "\n";
          }

          jsonString = jsonString.Remove(jsonString.Length - 1, 1);
          using (StreamWriter streamWriter = File.CreateText(DataPath))
          {
              streamWriter.Write(jsonString);
          }

    }
    //seems to work but it is extremely slow
    public List<MapTile> LoadMap()
    {
        List<MapTile> LoadedMap = new List<MapTile>();
        using (StreamReader streamReader = File.OpenText(DataPath))
        {

            string AllLines = streamReader.ReadToEnd();
            AllLines.Split('\n');
            for (int x = 0; x < AllLines.Split('\n').Length; x++)
            {
                LoadedMap.Add(JsonUtility.FromJson<MapTile>(AllLines.Split('\n')[x]));
            }

            //LoadedMap.Add(JsonUtility.FromJson<MapTile>(jsonString));
        }
        return LoadedMap;
    }

    public void LoadSeadedMap(string[] seed)
    {
        //Lakes
        string[] ListIndexWater = System.Text.RegularExpressions.Regex.Split(seed[0], @"\D+");
        for (int i = 0; i < ListIndexWater.Length; i++)
        {
            int Index;
            
            if (Int32.TryParse(ListIndexWater[i], out Index))
            {
                ChangeTile(TileType.Water, AllMapTiles[Index].X, AllMapTiles[Index].Y, 1, false);
            } else
            {
                Debug.Log("Could not indentify the Index: "+Index);
            }
        }

        //Mountain
        string[] ListIndexMountain = System.Text.RegularExpressions.Regex.Split(seed[1], @"\D+");
        for (int i = 0; i < ListIndexMountain.Length; i++)
        {
            int Index;

            if (Int32.TryParse(ListIndexMountain[i], out Index))
            {
                OcupyTileMat(MaterialTile.Mountain, AllMapTiles[Index].X, AllMapTiles[Index].Y, false);
            }
            else
            {
                Debug.Log("Could not indentify the Index: " + Index);
            }

        }
       
        //Tree
        string[] ListIndexTree = System.Text.RegularExpressions.Regex.Split(seed[2], @"\D+");
        for (int i = 0; i < ListIndexTree.Length; i++)
        {
            int Index;

            if (Int32.TryParse(ListIndexTree[i], out Index))
            {
                OcupyTileMat(MaterialTile.Tree, AllMapTiles[Index].X, AllMapTiles[Index].Y, false);
            }
            else
            {
                Debug.Log("Could not indentify the Index: " + Index);
            }

        }



    }



    //This way does not work because the data saved is too complex

        /*
    public void SaveMapSerialized()
    {
        SaveSerialized save = new SaveSerialized()
        {
            SavedSerializedMap = AllMapTiles
        };

        var binaryformatter = new BinaryFormatter();

        using (var fileStream = File.Create(SerializedDataPath))
        {
            binaryformatter.Serialize(fileStream, save);
        }

        Debug.Log("Data Serialized Saved");
    }

    public void LoadMapSerialized()
    {
        if (File.Exists(SerializedDataPath))
        {
            SaveSerialized save;

            var binaryformatter = new BinaryFormatter();

            using (var fileStream = File.Open(SerializedDataPath, FileMode.Open))
            {
                save = (SaveSerialized)binaryformatter.Deserialize(fileStream);
            }

            Debug.Log("Map Serialized Saved");
            NewShit = save.SavedSerializedMap;
        } else
        {
            Debug.Log("Serialized Saved Map not foud");
       
        }
    }
    */

}
