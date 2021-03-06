﻿
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public Camera Cam;

    //public GameObject SelectParticle, ParticlesHolder; the particles should not be handled by the input manager script 

    public class InputKeys
    {
        public float Sensivility = 0.5f;
        public float ScrollSensivility = 2f;
        public string SPACE = "space";
        public string ENTER = "return";
        public string ESCAPE = "escape";
        public string HORIZONTAL = "Horizontal";
        public string VERTICAL = "Vertical";
    }
    public InputKeys Key = new InputKeys();

    [HideInInspector]
    public bool RMBup, RMBdown, RMB, LMB, LMBup, LMBdown, Escp, spc;

    [HideInInspector]
    public float horizontal, vertical;


    [HideInInspector]
    public Vector3 MousePos;

    private float Scroll, LastScroll;

    [Header("Camera Movement")]
    public float CameraSpeed;
    public float EdgeTolerance;
    public float MinZoom;
    public float MaxZoom;
    private int CamX, CamY;



    [Header("Debug UI")]
    [Tooltip("Made to check the nodes")]
    public GameObject DebugText;
    public GameObject DebugHolder;
    public float MapEdgeDebug;

    Vector3 Orig;

    [HideInInspector]
    public bool InMoveMode = false;
    [HideInInspector]
    public bool InAttackMode = false;
    [HideInInspector]
    public IDictionary<MapTile, List<MapTile>> AllCurrentPaths;
    // [HideInInspector]
    public Unit CurUnit;
    // 

    public MapTile CurHoveredTile = null;
    public List<MapTile> OldCurPath = null;

    GridLayout grid;
    Map_Generator MapGRef;
    UI_Manager UIM;
    private void Start()
    {
        grid = GameObject.FindGameObjectWithTag("Map").GetComponent<GridLayout>();
        MapGRef = grid.gameObject.GetComponent<Map_Generator>();
        UIM = this.GetComponent<UI_Manager>();
        Scroll = LastScroll;

        MapEdgeDebug = ((StaticMapConf.Size) / 2);
    }

    //GameObject Temp;
    // I should clean this fixed update later
    void Update() { //it has to be update in order to register
        GetInput();
        CameraMovement();
        CameraZoom();
        //select
        if (LMBdown && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) //this prevents me from actually clicking on a UI element
        {
          
            MapTile Temp = GetMapTileOnMousePos();
           
            if(Temp != null)
            {
                MapGRef.CurrentTile = Temp;
                UIM.ChangeOfSelection();

            } else
            {
                //Making the selection the opposite one
                Vector3Int TempV = MapGRef.GetOppositeTileOnBoarder(Temp.X, Temp.Y);
                Vector2Int TargetPos = new Vector2Int(TempV.x, TempV.y);
                MapTile TargetTile = MapGRef.FindTile(TargetPos.x, TargetPos.y);

                if (TargetTile != null)
                {
                    MapGRef.CurrentTile = TargetTile;
                    UIM.ChangeOfSelection();
                }



            }


            /*
            if (InMoveMode)// this is for the ON turn thing only
            {
                if (AllCurrentPaths.ContainsKey(MapGRef.CurrentTile))
                    StartCoroutine(CurUnit.MoveUnitTo(cellPos.x, cellPos.y));

                InMoveMode = false;
            }*/


            //   ShowAllDebugUI();
        }
        //movement
        if (!(MapGRef.TurnModeOn || CurUnit == null || !RMBdown || UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()))
        {
            Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pz.z = 0;
            Vector3Int cellPos = grid.WorldToCell(pz);


            if (MapGRef.FindTile(cellPos.x, cellPos.y) != null && MapGRef.FindTile(cellPos.x, cellPos.y).Walkable)
            {
                StartCoroutine(CurUnit.MoveUnitTo(cellPos.x, cellPos.y));

                MapGRef.CurrentTile = MapGRef.FindTile(cellPos.x, cellPos.y);
                UIM.ChangeOfSelection();
                //  UIM.ClearPath();
            } else
            {
               
                Vector3Int TempV = MapGRef.GetOppositeTileOnBoarder(cellPos.x, cellPos.y);
                Vector2Int TargetPos = new Vector2Int(TempV.x, TempV.y);
                MapTile TargetTile = MapGRef.FindTile(TargetPos.x, TargetPos.y);

                if (TargetTile != null)
                {
                    StartCoroutine(CurUnit.MoveUnitTo(TargetPos.x, TargetPos.y));

                    MapGRef.CurrentTile = TargetTile;
                    UIM.ChangeOfSelection();
                }
            }
        }
/*
        //This is just for the area thing (I gotta migrate all this to the UI_Manager)
        if (InMoveMode)
        {
            Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pz.z = 0;
            Vector3Int cellPos = grid.WorldToCell(pz);
            //  Vector3 cellPosFloat = cellPos + new Vector3(0.5f, 0.5f, 0);

            MapTile NewCurHoveredTile = MapGRef.FindTile(cellPos.x, cellPos.y);

            //So once this happens it is the old one 
            if (NewCurHoveredTile != CurHoveredTile && AllCurrentPaths.ContainsKey(NewCurHoveredTile))
            {
                // MapGRef.ThirdLayer.ClearAllTiles();
                //This makes it so it everything is cleared 
                List<MapTile> CurHovPath = AllCurrentPaths[NewCurHoveredTile];
                if (CurHovPath != null && CurHovPath != OldCurPath)
                {
                    //This refreshes the old path to the orginal
                    if (OldCurPath != null)
                        foreach (MapTile M in OldCurPath)
                        {
                            if (M != null)
                                MapGRef.ThirdLayer.SetTile(new Vector3Int(M.X, M.Y, 0), MapGRef.particles_tiles.Area[5]);
                        }

                    for (int x = 0; x < CurHovPath.Count; x++)
                    {
                        if (CurHovPath[x] != null)
                        {
                            if (x != CurHovPath.Count - 1)
                            {

                                MapGRef.ThirdLayer.SetTile(new Vector3Int(CurHovPath[x].X, CurHovPath[x].Y, 0), MapGRef.particles_tiles.Area[6]);
                            } else
                            {
                                MapGRef.ThirdLayer.SetTile(new Vector3Int(CurHovPath[x].X, CurHovPath[x].Y, 0), MapGRef.particles_tiles.Area[7]);
                            }

                        }

                    }

                    OldCurPath = CurHovPath;

                } else



                    //  MapGRef.ThirdLayer.SetTile(new Vector3Int(NewCurHoveredTile.X, NewCurHoveredTile.Y, 0), MapGRef.particles_tiles.Area[6]);
                    CurHoveredTile = NewCurHoveredTile;
            }


        }*/

        //DragScreen();

        if (Escp)
            UIM.ShowEscapeMenu();

    }

    private void FixedUpdate()
    {
        LoopCamera();
    }

    void DragScreen()
    {
        if (RMBdown)
        {
            Orig = MousePos;
            return;
        }

        if (!RMB)
            return;

        Vector3 pos = Cam.ScreenToViewportPoint(MousePos - Orig);
        Vector3 move = new Vector3(pos.x * Key.Sensivility, pos.y * Key.Sensivility, 0);
        transform.Translate(-move, Space.World);

    }

    private void CameraMovement()
    {
        //Also lets create a tolerance 
        //Check if the mouse is on the edge
        //Debug.Log(MousePos.x + ";" + MousePos.y);

        if (MousePos.x >= Screen.width - EdgeTolerance || horizontal > 0)
        {
            CamX = 1;
        } else if (MousePos.x <= EdgeTolerance || horizontal < 0)
        {
            CamX = -1;
        } else
        {
            CamX = 0;
        }

        if (MousePos.y >= Screen.height - EdgeTolerance || vertical > 0)
        {
            CamY = 1;
        } else if (MousePos.y <=  EdgeTolerance || vertical < 0)
        {
            CamY = -1;
        } else
        {
            CamY = 0;
        }

        //I might wanna smooth the movement later
        this.transform.position = Vector3.MoveTowards(this.transform.position, this.transform.position + new Vector3(CamX, CamY, 0), CameraSpeed * Time.deltaTime);

        if (spc && CurUnit != null)
            this.transform.position = new Vector3(CurUnit.transform.position.x, CurUnit.transform.position.y, this.transform.position.z);

    }

    private void CameraZoom()
    {
        if (Scroll != LastScroll)
        {
            Cam.orthographicSize = Mathf.Clamp(-Scroll + Cam.orthographicSize, MinZoom, MaxZoom);
            LastScroll = Scroll;
        }
    }

    void GetInput()
    {
        MousePos = Input.mousePosition;

        RMB = Input.GetMouseButton(1);
        RMBup = Input.GetMouseButtonUp(1);
        RMBdown = Input.GetMouseButtonDown(1);

        LMB = Input.GetMouseButton(0);
        LMBup = Input.GetMouseButtonUp(0);
        LMBdown = Input.GetMouseButtonDown(0);

        Scroll = Input.mouseScrollDelta.y; //for some reason the value x is ignored

        Escp = Input.GetKeyDown(Key.ESCAPE);
        spc = Input.GetKeyDown(Key.SPACE);

        horizontal = Input.GetAxis(Key.HORIZONTAL);
        vertical = Input.GetAxis(Key.VERTICAL);

    }

    void ShowAllDebugUI()
    {
        //This is the debug part
        //If it already exist we gotta delete it 

        for (int x = 0; x < DebugHolder.transform.childCount; x++)
        {
            Destroy(DebugHolder.transform.GetChild(x).gameObject);
        }

        foreach (MapTile N in MapGRef.AllMapTiles)
        {
            SpawnDebugUIOnTile(N);
        }
    }

    void SpawnDebugUIOnTile(MapTile tile)
    {
        GameObject Temp2;
        Vector3 cellDebugPos = new Vector3(tile.X + 0.5f,tile.Y + 0.5f, -2);
        Temp2 = Instantiate(DebugText, cellDebugPos, DebugText.transform.rotation, DebugHolder.transform);
        Temp2.transform.GetChild(0).GetComponent<TextMesh>().text = "(" + tile.X + "," + tile.Y + ")";
        Temp2.transform.GetChild(1).GetComponent<TextMesh>().text = "F: " + tile.FCost;
        Temp2.transform.GetChild(2).GetComponent<TextMesh>().text = "G: " + tile.GCost;
        Temp2.transform.GetChild(3).GetComponent<TextMesh>().text = "H: " + tile.HCost;
        Temp2.transform.GetChild(4).GetComponent<TextMesh>().text = "W: " + tile.Walkable.ToString();
    }

    private void LoopCamera()
    {


        if (Cam.transform.position.x > MapEdgeDebug - 1)
            Cam.transform.position = new Vector3(-MapEdgeDebug, Cam.transform.position.y, Cam.transform.position.z);
        else if (Cam.transform.position.x < -MapEdgeDebug)
           Cam.transform.position = new Vector3(MapEdgeDebug - 1.00f, Cam.transform.position.y, Cam.transform.position.z);
       
        else if (Cam.transform.position.y > MapEdgeDebug - 1)
            Cam.transform.position = new Vector3(Cam.transform.position.x, -MapEdgeDebug, Cam.transform.position.z);
        else if (Cam.transform.position.y < -MapEdgeDebug)
            Cam.transform.position = new Vector3(Cam.transform.position.x, MapEdgeDebug - 1.00f, Cam.transform.position.z);
           
    }

    public MapTile GetMapTileOnMousePos()
    {
        Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pz.z = 0;
        Vector3Int cellPos = grid.WorldToCell(pz);
        MapTile Temp = MapGRef.FindTile(cellPos.x, cellPos.y);

        return Temp;
    }

}