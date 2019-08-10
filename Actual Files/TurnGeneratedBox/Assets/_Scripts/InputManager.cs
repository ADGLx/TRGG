
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public Camera Cam;

    //public GameObject SelectParticle, ParticlesHolder; the particles should not be handled by the input manager script 
	
    public class InputKeys
    {
        public float Sensivility = 0.5f;
        public float ScrollSensivility = 2f;
        public string ENTER = "return";
        public string ESCAPE = "escape";
    }
    InputKeys Key = new InputKeys();

    [HideInInspector]
    public bool RMBup, RMBdown, RMB, LMB, LMBup, LMBdown;


    [HideInInspector]
    public Vector3 MousePos;

    [Header("Camera Movement")]
    public float CameraSpeed;
    public float EdgeTolerance;
    private int CamX, CamY;


    [Header("Debug UI")]
    [Tooltip("Made to check the nodes")]
    public GameObject DebugText;
    public GameObject DebugHolder;

    Vector3 Orig;

    [HideInInspector]
    public bool InMoveMode = false;
    [HideInInspector]
    public IDictionary<MapTile, List<MapTile>> AllCurrentPaths;
   // [HideInInspector]
    public Unit CurUnit;
   // 

    private MapTile CurHoveredTile = null;
    private List<MapTile> OldCurPath = null;

    GridLayout grid;
    Map_Generator MapGRef;
    UI_Manager UIM;
    private void Start()
    {
        grid = GameObject.FindGameObjectWithTag("Map").GetComponent<GridLayout>();
        MapGRef = grid.gameObject.GetComponent<Map_Generator>();
        UIM = this.GetComponent<UI_Manager>();
    }

    //GameObject Temp;
    // Update is called once per frame
    void FixedUpdate () {
        GetInput();
        CameraMovement();
        //Depende del tipo de movimiento 
        //Cuando clickee
        if (LMBdown && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) //this prevents me from actually clicking on a UI element
        {
                //Gotta add something to prevent from finding it when ontop of UI
                Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pz.z = 0;
                Vector3Int cellPos = grid.WorldToCell(pz);
         /*       Vector3 cellPosFloat = cellPos + new Vector3(0.5f, 0.5f, 0);

                if (Temp != null)
                {
                    Destroy(Temp);
                } */

               // Temp = Instantiate(SelectParticle, cellPosFloat, SelectParticle.transform.rotation, ParticlesHolder.transform);
                MapGRef.CurrentTile = MapGRef.FindTile(cellPos.x, cellPos.y);
                UIM.ChangeOfSelection();



                if (InMoveMode)
                {
                    if (AllCurrentPaths.ContainsKey(MapGRef.CurrentTile))
                        CurUnit.MoveUnitTo(cellPos.x, cellPos.y);

                    InMoveMode = false;
                }
 
           

         //   ShowAllDebugUI();
        }

        if (!MapGRef.TurnModeOn && CurUnit != null && RMBdown && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pz.z = 0;
            Vector3Int cellPos = grid.WorldToCell(pz);

            if ( MapGRef.FindTile(cellPos.x, cellPos.y) != null)
            {
                CurUnit.MoveUnitTo(cellPos.x, cellPos.y);
            }
        }
        
        //This is just for the area thing 
        if(InMoveMode)
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
                    foreach(MapTile M in OldCurPath)
                    {
                        if (M != null)
                        MapGRef.ThirdLayer.SetTile(new Vector3Int(M.X, M.Y, 0), MapGRef.particles_tiles.Area[5]);
                    }

                    for (int x = 0; x < CurHovPath.Count; x++)
                    {
                        if (CurHovPath[x] != null)
                        {
                            if (x != CurHovPath.Count -1)
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


        }

        //DragScreen();
   
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
        
         if (MousePos.x >= Screen.width - EdgeTolerance)
        {
            CamX = 1;
        } else if(MousePos.x <= EdgeTolerance)
        {
            CamX = -1;
        } else
        {
            CamX = 0;
        }
        
        if(MousePos.y >= Screen.height - EdgeTolerance)
        {
            CamY = 1;
        } else if (MousePos.y <=  EdgeTolerance)
        {
            CamY = -1;
        } else
        {
            CamY = 0;
        }

        //I might wanna smooth the movement later
        this.transform.position = Vector3.MoveTowards(this.transform.position, this.transform.position + new Vector3(CamX, CamY, 0), CameraSpeed);

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

}
