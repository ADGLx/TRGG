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
    public bool TurnModeOn = false, PlayerTurn = false;

    //Map settings
    //public int Size = 10;
    [Header("Seed Parameters")]
    public bool LoadSeed = false; //Prob not the best but whatever 
  //  public bool SaveSeed = true;
   // [Tooltip("Element 0 = Water | Element 1 = Trees | Element 2 = Mountains")]
    private string Seed;
    [Header("Map Randomizer Parameters")]
    public int minW = 50;
    public int maxW = 100;
    public int minT = 0, maxT = 0;
    public int minM = 5, maxM = 10;

    [System.Serializable]
    public class AIEnemies
    {
        public GameObject[] AllEnemies; //Might wanna have a variable for each enemy
    }
    public AIEnemies aienemies = new AIEnemies();
    [Header("Enemy Spawner Randomizer")]
    public int MaxEnemyAmount = 10;
    public int MinEnemyAmount = 6;
    public int AmountOfZones = 8;


    private GameObject PlayerUnitsHolder;

    //This contains all the info on the mirror tiles
   // public Dictionary<Vector2Int, MapTile> AllMirrorTiles = new Dictionary<Vector2Int, MapTile>();



    // This will insitalize a map with a certain type of tile
    private void Awake()
    {
      //  DataPath = Path.Combine(Application.persistentDataPath, "MapData.txt");
        AllMapTiles.Clear();

            if (!StaticMapConf.NewMap)
            {
                if (!LoadSeed) // this is like if I wanna load it from the editor but normally I wouldnt 
                {
                    MapGenerator(StaticMapConf.Size);
                    CreateGraph(StaticMapConf.Size);
                    MapRandomizer(StaticMapConf.Size);
                   
                    /*
                    AllMapTiles = LoadMap();
                    if (AllMapTiles.Count > 0)
                    {

                        int S = (int)Mathf.Sqrt(AllMapTiles.Count); //Only works with squared maps
                        CreateGraph(S);
                        // PhysicalMap(AllMapTiles);
                    }

                    */
                } else
                {
                    //no se va a romper si el tama;o cambia?
                    MapGenerator(StaticMapConf.Size);
                    CreateGraph(StaticMapConf.Size);
                    LoadSeadedMap();
                }

            }
            else
            {//Esto es para crear un mapa nuevo
                Debug.Log("New map created");
                MapGenerator(StaticMapConf.Size);
                CreateGraph(StaticMapConf.Size);
                MapRandomizer(StaticMapConf.Size); //Algunas veces causa el (Array out of range)
                
            }

    }
    void Start () {

        // List<MapTile> MyPath = new List<MapTile>();

        //  MyPath = Pathfinding() 
        CreateMapBounds(); //This changes the stuff
        PhysicalMap(AllMapTiles);
        PlayerUnitsHolder = GameObject.FindGameObjectWithTag("Player_UnitH");
        SpawnEnemies();
       // StartCoroutine(CreateMirrorTiles(18));
        //StartCoroutine(UpdateThingsOverMirrorTiles());
      //  SaveTheMap(true);
        // Debug.Log(GetDistance(new Vector2Int(27, -27), new Vector2Int(27, 27)));


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
            ChangeTile(TileType.Water, RTile.X, RTile.Y, 1);
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
                        ChangeTile(TileType.Water, N.X, N.Y, 1);
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
                OcupyTileMat(MaterialTile.Wall, RTile.X, RTile.Y);

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
                        OcupyTileMat(MaterialTile.Wall, T.X, T.Y);
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
                                    OcupyTileMat(MaterialTile.Wall, N.Neighbours[E].X, N.Neighbours[E].Y);
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
                OcupyTileMat(MaterialTile.Tree, RTile.X, RTile.Y);

                int StartCounter = UnityEngine.Random.Range(minT, maxT);


                //  foreach (MapTile MP in )
                for (int t = 0; t < StartCounter; t++)
                {
                    //creates the star
                    foreach (MapTile N in RTile.Neighbours)
                    {
                        if (N != null && N.Type != TileType.Water && N.OcupedByMat == MaterialTile.None)
                        {
                            OcupyTileMat(MaterialTile.Tree, N.X, N.Y);
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
                    case TileType.Ground:
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
                            if (MapData[x].Neighbours[nn]!= null && MapData[x].Neighbours[nn].Type == TileType.Ground)
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
                                    if (MapData[x].FarNeighbours[fn] != null && MapData[x].FarNeighbours[fn].Type == TileType.Ground)
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

                    case MaterialTile.Wall:
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

            //create the Grid show thing (I think we dont need this 
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

    void ChangeTile(TileType T, int X, int Y, int Roughness)
    {
        MapTile Tile = FindTile(X, Y);

        if (Tile != null)
        {
            Tile.Type = T;

        }
 
    }

    void OcupyTileMat (MaterialTile M, int X, int Y)
    {
        MapTile Tile = FindTile(X, Y);
        if (Tile != null)
        {
            Tile.OcupedByMat = M;

        }
            
    }

    public void OcupyTileUnit (Unit U, int X, int Y)
    {
        MapTile t = FindTile(X, Y);


        if (t != null)
        {
            //Imma add in here the teleport thing eventhough it might be messy?
            if (t.IsBound)
            {
                StartCoroutine(TeleportToBound(t, U));
            } else
            {
                t.OcupiedByUnit = U.Type;
                t.OcupingUnitScript = U;
            }



         //   U.gameObject.transform.position = SetTilePosToWorld(X,Y);
            //This seems to work tho
           // Debug.Log("Ay");
        }

    }

    //this is just to keep them properly instialized
    public void InstializeUnit(Unit U, int X, int Y)
    {
        if (FindTile(X, Y) != null && (FindTile(X, Y).Walkable))
        {
            FindTile(X, Y).OcupiedByUnit = U.Type;
            FindTile(X, Y).OcupingUnitScript = U;
            U.gameObject.transform.position = SetTilePosToWorld(X,Y);

        } else
        {
            U.transform.gameObject.SetActive(false);
            Debug.Log("Could not Intialize: " + U);
        }
    }

    public void UnocupyTileUnit (int X, int Y)
    {
        if (FindTile(X,Y) != null)
        {
            FindTile(X, Y).OcupiedByUnit = UnitIn.None;
            FindTile(X, Y).OcupingUnitScript = null;
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


      //  if (Size % 2 != 0) //not sure if it works
       //    StartMax++;

        
       // StartMin++;
        for (int y = StartMin ; y <= StartMax ; y++)
            {
            for (int x = StartMin ; x <= StartMax ; x++)
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
                 //   else { Debug.Log("Invalid Neighbour on top"); }

                    if (x != StartMax) //Cheapest way to fix it, but it works
                    {
                        FindTile(x, y).Neighbours[1] = FindTile(x + 1, y); //Right
                    }
                  //  else { Debug.Log("Invalid Neighbour on right"); }


                    if (y != StartMin - 1) //Ta mal
                    {
                        FindTile(x, y).Neighbours[2] = FindTile(x, y - 1); //Bottom
                    }
                //    else { Debug.Log("Invalid Neighbour on bot"); }


                    if (x != StartMin - 1)
                    {
                        FindTile(x, y).Neighbours[3] = FindTile(x - 1, y); //Left
                    }
                //    else { Debug.Log("Invalid Neighbour on left"); }





                    //far neighbours
                    if (!(y == StartMax || x == StartMin - 1))
                    {
                        FindTile(x, y).FarNeighbours[0] = FindTile(x - 1, y + 1); //TopLeft
                    }
                //    else { Debug.Log("Invalid FarNeighbour on topleft"); }


                    if (!(y == StartMax || x == StartMax))
                    {
                        FindTile(x, y).FarNeighbours[1] = FindTile(x + 1, y + 1); //topright
                    }
                //    else { Debug.Log("Invalid FarNeighbour on topright"); }


                    if (!(y == StartMin - 1 || x == StartMax))
                    {
                        FindTile(x, y).FarNeighbours[2] = FindTile(x + 1, y - 1); //Botright
                    }
               //     else { Debug.Log("Invalid FarNeighbour on botright"); } 


                    if (!(y == StartMin - 1 || x == StartMin - 1))
                    {
                        FindTile(x, y).FarNeighbours[3] = FindTile(x - 1, y - 1); //Botleft
                    }
                 //   else { Debug.Log("Invalid FarNeighbour on botleft"); } 


                    //This is the new stuff about the circular world (the preivous stuff is super messy)
                    if(x==StartMax && (y != StartMin || y != StartMax)) 
                    {
                        FindTile(x, y).Neighbours[1] = FindTile(StartMin + 1, y); //derecha

                        FindTile(x, y).Neighbours[0] = null;
                        FindTile(x, y).Neighbours[2] = null;
                        //Making sure it is not neighbour with the other teleport tiles
                        // Debug.Log(FindTile(x, y).Neighbours[1].GetPos);
                    } else if (x == StartMin && (y != StartMin || y != StartMax))
                    {
                        FindTile(x, y).Neighbours[3] = FindTile(StartMax - 1, y); //Izquierda

                        FindTile(x, y).Neighbours[0] = null;
                        FindTile(x, y).Neighbours[2] = null;
                        //  Debug.Log(FindTile(x, y).Neighbours[3].GetPos);
                    } 

                    if(y == StartMax && (x != StartMin || x != StartMax))
                    {
                        FindTile(x, y).Neighbours[0] = FindTile(x, StartMin + 1); //Arriba 

                        FindTile(x, y).Neighbours[1] = null;
                        FindTile(x, y).Neighbours[3] = null;
                    }
                    else if (y == StartMin && (x != StartMin || x != StartMax))
                    {
                        FindTile(x, y).Neighbours[2] = FindTile(x, StartMax - 1); //Abajo
                        FindTile(x, y).Neighbours[1] = null;
                        FindTile(x, y).Neighbours[3] = null;
                    }


                }



            }
            }

      //  Debug.Log(prueba);
    }
    
    //public MapTile DFindTile(int X, int Y) //This works but seems slower than the other one
    //{
    //    int Size = (int)Mathf.Sqrt(AllMapTiles.Count);
    //    int Index = (X + Size/2)+ ((Y + Size/2) * Size); //Only works with squared maps
    //                                                     //  Debug.Log(Size/2);
    //    if (AllMapTiles[Index] != null)
    //    {
    //        return AllMapTiles[Index];
    //    }
    //    else
    //    {
    //        Debug.Log("Tile does not exist for the index :" + Index);
    //        return null;
    //    }

    //}
    

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
        //Implementing the looping thing (it seems to work now)
        int dx, dy;

            dx = Math.Abs(A.x - B.x);
            dy = Math.Abs(A.y - B.y);

        if (dx > StaticMapConf.Size / 2)
        {
            int DistanceToXPos, DistanceToXNeg;

            //I can just get the distance from both points to the end X and add them
            if (A.x > B.x)
            {
                DistanceToXNeg = Math.Abs(B.x - (-StaticMapConf.Size / 2));
                DistanceToXPos = Mathf.Abs(A.x - (StaticMapConf.Size / 2));
            } else
            {
                DistanceToXNeg = Math.Abs(A.x - (-StaticMapConf.Size / 2));
                DistanceToXPos = Mathf.Abs(B.x - (StaticMapConf.Size / 2));
            }

            dx = DistanceToXPos + DistanceToXNeg + 1; //idk why the one
            
        }

        if (dy > StaticMapConf.Size / 2)
        {
            int DistanceToYPos, DistanceToYNeg;

            //I can just get the distance from both points to the end X and add them
            if (A.y > B.y)
            {
                DistanceToYNeg = Math.Abs(B.y - (-StaticMapConf.Size / 2));
                DistanceToYPos = Mathf.Abs(A.y - (StaticMapConf.Size / 2));
            }
            else
            {
                DistanceToYNeg = Math.Abs(A.y - (-StaticMapConf.Size / 2));
                DistanceToYPos = Mathf.Abs(B.y - (StaticMapConf.Size / 2));
            }

            dy = DistanceToYPos + DistanceToYNeg + 1; //idk why the one

        }


        /*
        if (dy > StaticMapConf.Size / 2)
            dy = Mathf.Max(A.y, B.y) + Mathf.Min(A.y, B.y) + 1;
        */

        //return dx + dy;

        //this helps the program move kinda like diagonally 
        int min = Math.Min(dx, dy);
        int max = Math.Max(dx, dy);

        int diagonalSteps = min;
        int straightSteps = max - min;

        return (int)(Math.Sqrt(2) * diagonalSteps + straightSteps);

        //try to tell the pathfinding that if its on the limit, the distance is one 
    }

     public List<MapTile> GetWalkableAreaAround(Vector2Int Origin, int size)
    {
        List<MapTile> list = new List<MapTile>();
        for(int x = -size; x<= size; x++)
        {
            for(int y = -size; y <= size; y++)
            {
                MapTile T = FindTile(Origin.x+x, Origin.y+y);

                if (T != null && T.Walkable == true && Pathfinding(Origin,T.GetPos).Count<= size) //If I am already doing the pathfinding I might just pass it?
                {
                    list.Add(T);
                }
            }
        }


        return list;
    }

    public List<MapTile> GetAreaAround(Vector2Int Origin, int size)
    {
        List<MapTile> Area = new List<MapTile>();
        for(int x = Origin.x - size; x < Origin.x + size + 1; x++)
        {
            for(int y = Origin.y - size; y < Origin.y + size + 1; y++)
            {
                if (FindTile(x,y) != null)
                {
                    Area.Add(FindTile(x, y));
                } else if (x >= (StaticMapConf.Size / 2) || y >= (StaticMapConf.Size / 2) || x <= (StaticMapConf.Size / 2) || y <= (StaticMapConf.Size / 2))
                {

                    Vector3Int Bro = GetOppositeTileOnBoarder(x, y);
                    Area.Add(FindTile(Bro.x, Bro.y));
                   // Area.Add(AllMirrorTiles[new Vector2Int(x, y)]);
                }
                else
                {
                    Debug.Log(x + ";" + y);
                }

            }
        }

        return Area;
    }
    /*
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
    }*/

    /*
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
    }*/ //Old ways that make no sense

    public void SaveTheMap()
    {

            for (int x = 0; x < AllMapTiles.Count; x++) //This might be a bit heavy but it doesnt matter
            {
                MapTile Temp = AllMapTiles[x];

                string TileString = (int)Temp.Type + "" + (int)Temp.OcupedByMat + "|"; //First what it is then what is has on it

                Seed += TileString;
            }




        string path = Application.dataPath + "/MapLog.txt";
        File.WriteAllText(path, Seed);



    }
    public void LoadSeadedMap()
    {
     
        Seed = File.ReadAllText(Application.dataPath + "/MapLog.txt");
        //This is gonna be easier now its gonna start from the first spot 
        for (int x = 0; x < (AllMapTiles.Count * 3); x++)
        {
            MapTile Temp = AllMapTiles[x/3];
            int TT = (int)char.GetNumericValue(Seed[x]);
            TileType TempType = (TileType)TT;
            x++; //This stuff is so we can keep track of where our thing is reading the seed

            int TM = (int)char.GetNumericValue(Seed[x]);
            MaterialTile TempMat = (MaterialTile)TM;
            x++;

            // ChangeTile(TempType, Temp.X, Temp.Y, Temp.Roughness);
            //Changing the Type directly
            if(AllMapTiles[x / 3] != null)
            {
                AllMapTiles[x / 3].Type = TempType;
                AllMapTiles[x / 3].OcupedByMat = TempMat;
            } else
            {
                Debug.LogWarning("Tile not found");
            }

        
        //   Debug.Log("(" + Temp.X + ";" + Temp.Y + ")" + TempType + "," + TempMat);
        }


        //This can be expanded

    }


    //I wanna create the teleport Tiles at the edge of the map
    //This is gonna confuse the Pathfinding
    private void CreateMapBounds()
    {
        int StartMin = -(StaticMapConf.Size / 2);
        int StartMax = StaticMapConf.Size / 2;


        for (int Y = StartMin; Y <= StartMax; Y++)
        {
            MapTile Bound = FindTile(StartMin, Y);
            ChangeTile(DefaultTile, StartMin, Y,0);
            OcupyTileMat(MaterialTile.None, StartMin, Y);
            Bound.IsBound = true;

           // if (Y == StartMin || Y == StartMax)
             //   FindTile(StartMin, Y) = false; Fix later
        }

        for (int X = StartMin; X <= StartMax; X++)
        {
            ChangeTile(DefaultTile, X, StartMin, 0);
            OcupyTileMat(MaterialTile.None, X, StartMin);
            FindTile(X, StartMin).IsBound = true;
        }

        for (int W = StartMin; W <= StartMax; W++)
        {
            ChangeTile(DefaultTile, W, StartMax, 0);
            OcupyTileMat(MaterialTile.None, W, StartMax);
            FindTile(W, StartMax).IsBound = true;
        }

        for (int Z = StartMin; Z <= StartMax; Z++)
        {
            ChangeTile(DefaultTile, StartMax, Z, 0);
            OcupyTileMat(MaterialTile.None, StartMax, Z);
            FindTile(StartMax, Z).IsBound = true;
        }

    }



    private IEnumerator TeleportToBound(MapTile Tile, Unit U)
    {
        int TargetX = Tile.X, TargetY = Tile.Y;

        if (Tile.X == StaticMapConf.Size / 2)
            TargetX = -Tile.X + 1;
        else if (Tile.X == -(StaticMapConf.Size / 2))
            TargetX = -Tile.X - 1;

        if (Tile.Y == StaticMapConf.Size / 2)
            TargetY = -Tile.Y + 1;
        else if (Tile.Y == -(StaticMapConf.Size / 2))
            TargetY = -Tile.Y - 1;

        MapTile t = FindTile(TargetX, TargetY);
        if (t != null)
        {
            while (U.transform.position != SetTilePosToWorld(Tile.X, Tile.Y))
                yield return null;

            //this is a lazy way to fix things but it works
            yield return new WaitForSeconds(0.01f);

            U.TeleportUnitTo(t.X, t.Y);
            //I need to the thing to wait for the thing to stop moving to then do this
        } else
        {
            Debug.LogWarning("Could not find tile");
        }


    }

    /*
    private IEnumerator CreateMirrorTiles(int Size)
    {
        
        //Only keep the part where it identifies all the stuff and saves it to the dictionary
        int StartMin = -(StaticMapConf.Size / 2);
        int StartMax = StaticMapConf.Size / 2;

        Vector3Int OriginPos;
        Vector3Int TargetPos;
        TileBase ActualTile;
        TileBase OcupiedBy;

        //start drawing from the bottonleft and keep going
        for (int Y = StartMin - Size; Y <= StartMax + Size; Y++)
        {
            for (int X = StartMin - Size; X <= StartMax + Size; X++)
            {
                if ((X <= StartMin || X >= StartMax || Y <= StartMin || Y >= StartMax))
                {
                    //now we need to decide which side is the thing on and what should it imitate
                    //Map.SetTile(new Vector3Int(X, Y, 0), water_Tiles.Basic[0]);

                    OriginPos = new Vector3Int(X, Y, 0);
                    TargetPos = GetOppositeTileOnBoarder(X, Y);

                    ActualTile = Map.GetTile(TargetPos);


                    //The tile info
                    Map.SetTile(OriginPos, ActualTile);
                    Map.SetTransformMatrix(OriginPos, Map.GetTransformMatrix(TargetPos));

                    //What is is occupied with
                    OcupiedBy = SecondLayer.GetTile(TargetPos);
                    SecondLayer.SetTile(OriginPos, OcupiedBy);

                    if (FindTile(TargetPos.x, TargetPos.y) != null)
                    AllMirrorTiles.Add(new Vector2Int(X, Y), FindTile(TargetPos.x, TargetPos.y));
                }
            }
        }


        //In here all that updates per sec

        while (true)
        {
           
            foreach (Vector2Int Origin in AllMirrorTiles.Keys)
            {
                OriginPos = new Vector3Int(Origin.x, Origin.y, 0);
                TargetPos = new Vector3Int(AllMirrorTiles[Origin].X, AllMirrorTiles[Origin].Y, 0);

                //Only updates what it is occupied with 
                OcupiedBy = SecondLayer.GetTile(TargetPos);

                if (SecondLayer.GetTile(TargetPos) != OcupiedBy)
                SecondLayer.SetTile(OriginPos, OcupiedBy);
            }



            //This is gonna permanentelly update I think, after I can make it so it only updates when the camera is close
            yield return new WaitForSecondsRealtime(1f);
        }



        //So first its basically gonna generate the stuff that is at the begining starting by the changing the teleport tiles

    }


    private IEnumerator UpdateThingsOverMirrorTiles()
    {
        Unit Player = GameObject.FindWithTag("Main_Pl").GetComponent<Unit>();

        if (Player == null)
            Debug.LogError("Player wasnt found");


        foreach(Vector2Int x in AllMirrorTiles.Keys)
        {
            Debug.Log(x);
        }



        while (true)
        {
            //start by mirroring the player 
            if (AllMirrorTiles.ContainsKey(Player.GridPos))
            {
                Debug.Log("ay");
            }




           yield return new WaitForSecondsRealtime(UpdateTime);
        }
    }
    */

    public Vector3Int GetOppositeTileOnBoarder (int X, int Y)
    {
        int StartMin = -(StaticMapConf.Size / 2);
        int StartMax = StaticMapConf.Size / 2;
        bool Bot = false, Right = false, Top = false, Left = false;
        Vector3Int TargetPos = new Vector3Int();

        if (X >= StartMax)
        {
            Right = true;
        }
        else if (X <= StartMin)
        {
            Left = true;
        }

        if (Y >= StartMax)
        {
            Top = true;
        }
        else if (Y <= StartMin)
        {
            Bot = true;
        }



        if (Bot && !Right && !Left)
        {
            TargetPos = new Vector3Int(X, StartMax - (StartMin - Y + 1), 0); //find a way to tell where the size thing is 

        }
        else if (Right && !Bot && !Top)//derecha
        {
            TargetPos = new Vector3Int(StartMin + (X - StartMax + 1), Y, 0); //find a way to tell where the size thing is 
        }
        else if (Top && !Right && !Left)//Arriba
        {
            TargetPos = new Vector3Int(X, StartMin + (Y - StartMax + 1), 0);
        }
        else if (Left && !Bot && !Top)//Izquierda
        {
            TargetPos = new Vector3Int(StartMax - (StartMin - X + 1), Y, 0);
        }
        else if (Bot && Left)//uttom left corner
        {
            TargetPos = new Vector3Int(StartMax - (StartMin - X + 1), StartMax - (StartMin - Y + 1), 0);
        }
        else if (Bot && Right)
        {
            TargetPos = new Vector3Int(StartMin + (X - StartMax + 1), StartMax - (StartMin - Y + 1), 0);
        }
        else if (Top && Right)
        {
            TargetPos = new Vector3Int(StartMin + (X - StartMax + 1), StartMin + (Y - StartMax + 1), 0);
        }
        else if (Top && Left)
        {
            TargetPos = new Vector3Int(StartMax - (StartMin - X + 1), StartMin + (Y - StartMax + 1), 0);
        }
        else
        {
            TargetPos = new Vector3Int(100, 0, 0);
            Debug.Log("this should not happen");
        }

        return TargetPos;
    }

    private void SpawnEnemies()
    {
        //First divide the map in 8 Zones 
        MapTile[,] Zones = new MapTile[AmountOfZones, AllMapTiles.Count/AmountOfZones];

        for(int x= 0; x<AmountOfZones; x++)
        {
            for(int y = 0; y < AllMapTiles.Count/ AmountOfZones; y++)
            {
                Zones[x, y] = AllMapTiles[x + y];
            }
        }

        int AmountOfEnemies = UnityEngine.Random.Range(MinEnemyAmount, MaxEnemyAmount);
        Tuple<int, bool>[] SpawnLimitation = new Tuple<int, bool>[AmountOfZones];

        for(int w = 0; w <AmountOfZones; w++)
        {
            if (UnityEngine.Random.value >= 0.5)
            {
                // return true;
                SpawnLimitation[w] = new Tuple<int, bool>(w, true); 
            }
            //return false;
            SpawnLimitation[w] = new Tuple<int, bool>(w, false);
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
