
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;

public class UI_Manager : MonoBehaviour {

    //BotRight
    [System.Serializable]
    public class BtRight
    {
        public GameObject Holder;
    }

    [Header("UI Parts")]
    public BtRight Bot_Right = new BtRight();

    //BotLeft
    [System.Serializable]
    public class BtLeft
    {
        public GameObject Holder;
        public TMP_Text Title, Attack, AttackP, Life, LifeP, Roughness, RoughnessP;
        public Image Icon;
    }

    public BtLeft Bot_Left =  new BtLeft();

    //TopLeft
    [System.Serializable]
    public class TpLeft
    {
        public GameObject Holder;
        public TMP_Text Turns, TurnsP, Gold, GoldP;
    }

    public TpLeft Top_Left = new TpLeft();

    //TopRight
    [System.Serializable]
    public class TpRight
    {
        public GameObject Holder;
        public TMP_Text Indicator;
    }

    public TpRight Top_Right = new TpRight();

    Map_Generator MapLocal;
    InputManager InputM;


    private Dictionary<TileType, Sprite> TileTypeDic = new Dictionary<TileType, Sprite>();
    private Dictionary<MaterialTile, Sprite> MaterialDic=  new Dictionary<MaterialTile, Sprite>();

    [Header("Start Settings")]
    public Unit StartSelectingUnit= null;

    //This is just for the tooltip
    private string yikes;

    [Header("UI Icons")]
    [Tooltip("Element 0 = Grass | Element 1 = Water")]
    public Sprite[] TileIcon = new Sprite[Enum.GetValues(typeof(TileType)).Length];
    [Tooltip("Element 0 = None | Element 1 = Tree | Element 2 = Mountain")]
    public Sprite[] MaterialIcon = new Sprite[Enum.GetValues(typeof(MaterialTile)).Length];

    [Header("Select Particles")]
    public GameObject SelectParticle;
    public GameObject PathParticle;
    public GameObject ParticlesHolder;
    private GameObject TempSelctParticle = null;
    private GameObject[] TempPathparticle = null;



    [System.Serializable]
    public class AButtons
    {
        public GameObject HoldingBar;
        public GameObject ActionPointLeftHolder;
        public TMP_Text ActionPointsValue;
        public GameObject MoveButton;
        public  GameObject AutoDestroyButton;
        public GameObject AttackButton;
    }
    [Header("Unit UI Buttons")]
    public AButtons UnitActionBarButtons;

    private void Start()
    {
        //Add the keys (I will have to automate this so it is easier to add stuff)
        TileTypeDic.Add(TileType.Grass, TileIcon[0]);
        TileTypeDic.Add(TileType.Water, TileIcon[1]);
        MaterialDic.Add(MaterialTile.None, MaterialIcon[0]);
        MaterialDic.Add(MaterialTile.Tree, MaterialIcon[1]);
        MaterialDic.Add(MaterialTile.Mountain, MaterialIcon[2]);

        MapLocal = GameObject.FindGameObjectWithTag("Map").GetComponent<Map_Generator>();
        InputM = this.GetComponent<InputManager>();

        if (MapLocal == null)
        {
            Debug.Log("Map reference could not be found");
        }

        if (StartSelectingUnit != null)
            StartCoroutine(SelectWithDelay());

        ChangeOfSelection();
    }

     public void CleanAllGUI()
    {
        //set all to false sot it puts only whats needed
        Bot_Left.Attack.gameObject.SetActive(false);
        Bot_Left.Life.gameObject.SetActive(false);
        Bot_Left.Roughness.gameObject.SetActive(false);
        Bot_Left.Holder.gameObject.SetActive(false);
        UnitActionBarButtons.ActionPointLeftHolder.SetActive(false);
        UnitActionBarButtons.HoldingBar.SetActive(false);
        //Clean the spawned buttons
        for (int temp = 0; temp < UnitActionBarButtons.HoldingBar.transform.childCount; temp++)
        {
            Destroy(UnitActionBarButtons.HoldingBar.transform.GetChild(temp).gameObject);
        }
        Top_Right.Indicator.text = "Free Mode";
        //Clean the UI particles 
        MapLocal.ThirdLayer.ClearAllTiles();
        //Clear the TempSelectParticle
        if (TempSelctParticle != null)
            Destroy(TempSelctParticle);


        //Clear the CurUnit
        //  InputM.CurUnit = null;
    }

    public void ChangeOfSelection()
    {
        CleanAllGUI();



        if (MapLocal.CurrentTile != null)
        {
            //Optimization real quick
            MapTile CurT = MapLocal.CurrentTile;

            //in here the select thing should be spawned
            Vector3 cellPosFloat = CurT.GetPos + new Vector2(0.5f, 0.5f);
            TempSelctParticle = Instantiate(SelectParticle, cellPosFloat, SelectParticle.transform.rotation, ParticlesHolder.transform);

            //Tipo de recurso
            if (CurT.OcupedByMat != MaterialTile.None)
            {
               Bot_Left.Title.text = CurT.OcupedByMat.ToString();
                Bot_Left.Icon.sprite = MaterialDic[CurT.OcupedByMat];
                Bot_Left.Holder.gameObject.SetActive(true);

            }
            //Tipo de Unidad
            else if (CurT.OcupiedByUnit != UnitIn.None)
            {
                Unit U = CurT.OcupyingUnit;

                //Select the unit on top of it 
                if (CurT.OcupiedByUnit == UnitIn.Player)
                {
                   InputM.CurUnit = CurT.OcupyingUnit;
                }
                else
                {
                    InputM.CurUnit = null;
                }

                Bot_Left.Title.text = U.unitStats.Name;
                Bot_Left.Attack.gameObject.SetActive(true);
                Bot_Left.AttackP.text = U.unitStats.AttackPoints.ToString();
                Bot_Left.Life.gameObject.SetActive(true);
                Bot_Left.LifeP.text = U.unitStats.LifePoints.ToString();
                Bot_Left.Holder.gameObject.SetActive(true);

                //mostrar la barra de accion solo si esta en modo de turnos
                if(MapLocal.TurnModeOn)
                {
                    Top_Right.Indicator.text = "Turn Mode";
                    int TotalButtons = 0;
                    if (U.Actions.Move)
                    {
                        TotalButtons++;
                        GameObject ButtonTemp = Instantiate(UnitActionBarButtons.MoveButton, UnitActionBarButtons.HoldingBar.transform);
                        ButtonTemp.GetComponent<Button>().onClick.AddListener(delegate { ShowAreaAroundUnit(U); });

                    }
                    if (U.Actions.Attack)
                    {
                        TotalButtons++;
                        Instantiate(UnitActionBarButtons.AttackButton, UnitActionBarButtons.HoldingBar.transform);
                    }
                    if (U.Actions.AutoDestroy)
                    {
                        TotalButtons++;
                        Instantiate(UnitActionBarButtons.AutoDestroyButton, UnitActionBarButtons.HoldingBar.transform);
                    }

                    int Offset = (TotalButtons - 1) * (-25);



                    for (int temp = 0; temp < UnitActionBarButtons.HoldingBar.transform.childCount; temp++)
                    {
                        //place them where they are supposed to
                        UnitActionBarButtons.HoldingBar.transform.GetChild(temp).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(Offset, 0);
                        Offset += 50;
                    }


                    UnitActionBarButtons.HoldingBar.GetComponent<RectTransform>().sizeDelta = new Vector2(TotalButtons * 50, 45);

                    //set the Action Points left
                    UnitActionBarButtons.ActionPointsValue.text = U.unitStats.ActionPoints.ToString();

                    UnitActionBarButtons.ActionPointLeftHolder.SetActive(true);
                    UnitActionBarButtons.HoldingBar.SetActive(true);
                }
                
            }
            // Tipo de tile
            else if (CurT.OcupedByMat == MaterialTile.None && CurT.OcupiedByUnit == UnitIn.None)
            {
                Bot_Left.Title.text = CurT.Type.ToString();
                Bot_Left.Icon.sprite = TileTypeDic[CurT.Type];
                Bot_Left.Roughness.gameObject.SetActive(true);
                Bot_Left.RoughnessP.text = CurT.Roughness.ToString();
                Bot_Left.Holder.gameObject.SetActive(true);
            } else
            {
                Debug.Log("Something went wrong");
            }

           // Bot_Left.Title.text = "X: " + CurT.X + " | Y: " + CurT.Y;
        }
    }

    //this should change later on, to optimize the selection
    IEnumerator SelectWithDelay()
    {
        yield return new WaitForSeconds(.1f);
        MapLocal.CurrentTile = MapLocal.FindTile(StartSelectingUnit.GridPos.x, StartSelectingUnit.GridPos.y);
        ChangeOfSelection();
    }

    public void ShowAreaAroundUnit(Unit U)
    {
        InputM.AllCurrentPaths = MapLocal.SpawnAreaParticle(U.GridPos, U.unitStats.ActionPoints);
        InputM.InMoveMode = true;
        InputM.CurUnit = U;

    }

    public void ShowPath(List<MapTile> P)
    {
        ClearPath();

        TempPathparticle = new GameObject[P.Count];
        for(int x = 0; x< P.Count; x++)
        {
            Vector3 cellPosFloat = P[x].GetPos + new Vector2(0.5f, 0.5f);
            TempPathparticle[x] = Instantiate(PathParticle, cellPosFloat, PathParticle.transform.rotation, ParticlesHolder.transform);
        }
    }

    private void ClearPath()
    {
        if (TempPathparticle != null)
        {
            foreach (GameObject a in TempPathparticle)
            {
                Destroy(a);
            }
        }
    }

    public void ClearSpecPath (int X)
    {
        if (TempPathparticle[X] != null)
        {
            Destroy(TempPathparticle[X]);
        }
    }
}
