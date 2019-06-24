
using UnityEngine;

public class InputManager : MonoBehaviour {

    public Camera Cam;

    public GameObject SelectParticle, ParticlesHolder;
	
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

    Vector3 Orig;

    GridLayout grid;
    Map_Generator MapGRef;
    UI_Manager UIM;
    private void Start()
    {
        grid = GameObject.FindGameObjectWithTag("Map").GetComponent<GridLayout>();
        MapGRef = grid.gameObject.GetComponent<Map_Generator>();
        UIM = this.GetComponent<UI_Manager>();
    }

    GameObject Temp;
    // Update is called once per frame
    void Update () {
        GetInput();

        if (LMBdown)
        {
            Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pz.z = 0;
            Vector3Int cellPos = grid.WorldToCell(pz);
            Vector3 cellPosFloat = cellPos + new Vector3(0.5f, 0.5f, 0);

            if (Temp != null)
            {
                Destroy(Temp);
            }

            Temp = Instantiate(SelectParticle, cellPosFloat, SelectParticle.transform.rotation, ParticlesHolder.transform);
            MapGRef.CurrentTile = MapGRef.DFindTile(cellPos.x, cellPos.y);
            UIM.ChangeOfSelection();

            //This is the debug part
          //  Debug.Log(MapGRef.CurrentTile.Walkable);
        }

        DragScreen();
   
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
}
