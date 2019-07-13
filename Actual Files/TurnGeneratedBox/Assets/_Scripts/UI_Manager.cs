
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
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
    }

    public TpRight Top_Right = new TpRight();

    Map_Generator MapLocal;
    InputManager InputM;


    private Dictionary<TileType, Sprite> TileTypeDic = new Dictionary<TileType, Sprite>();
    private Dictionary<MaterialTile, Sprite> MaterialDic=  new Dictionary<MaterialTile, Sprite>();

    //This is just for the tooltip
    private string yikes;

    [Header("UI Icons")]
    [Tooltip("Element 0 = Grass | Element 1 = Water")]
    public Sprite[] TileIcon = new Sprite[Enum.GetValues(typeof(TileType)).Length];
    [Tooltip("Element 0 = None | Element 1 = Tree | Element 2 = Mountain")]
    public Sprite[] MaterialIcon = new Sprite[Enum.GetValues(typeof(MaterialTile)).Length];



    [System.Serializable]
    public class AButtons
    {
        public GameObject HoldingBar;
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

        ChangeOfSelection();
    }
    void Update () {
		
	}


    public void ChangeOfSelection()
    {
        if (MapLocal.CurrentTile == null)
        {
            Bot_Left.Holder.gameObject.SetActive(false);
            UnitActionBarButtons.HoldingBar.SetActive(false);
            //Clean the spawned buttons
            for (int temp = 0; temp < UnitActionBarButtons.HoldingBar.transform.childCount; temp++)
            {
                Destroy(UnitActionBarButtons.HoldingBar.transform.GetChild(temp).gameObject);
            }
            MapLocal.ThirdLayer.ClearAllTiles();
        } else 
        {
            //Optimization real quick
            MapTile CurT = MapLocal.CurrentTile;

            //set all to false sot it puts only whats needed
            Bot_Left.Attack.gameObject.SetActive(false);
            Bot_Left.Life.gameObject.SetActive(false);
            Bot_Left.Roughness.gameObject.SetActive(false);
            Bot_Left.Holder.gameObject.SetActive(false);

            UnitActionBarButtons.HoldingBar.SetActive(false);
            //Clean the spawned buttons
            for (int temp=0; temp < UnitActionBarButtons.HoldingBar.transform.childCount; temp++)
            {
                Destroy(UnitActionBarButtons.HoldingBar.transform.GetChild(temp).gameObject);
            }
            //Clean the UI particles 
            MapLocal.ThirdLayer.ClearAllTiles();
                
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

                Bot_Left.Title.text = U.unitStats.Name;
                Bot_Left.Attack.gameObject.SetActive(true);
                Bot_Left.AttackP.text = U.unitStats.AttackPoints.ToString();
                Bot_Left.Life.gameObject.SetActive(true);
                Bot_Left.LifeP.text = U.unitStats.LifePoints.ToString();
                Bot_Left.Holder.gameObject.SetActive(true);

                //mostrar la barra de accion
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
                    Offset+=50;
                }


                UnitActionBarButtons.HoldingBar.GetComponent<RectTransform>().sizeDelta = new Vector2(TotalButtons * 50, 45);
                UnitActionBarButtons.HoldingBar.SetActive(true);
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


    public void ShowAreaAroundUnit(Unit U)
    {
        InputM.AllCurrentPaths = MapLocal.SpawnAreaParticle(U.GridPos, U.unitStats.ActionPoints);
        InputM.InMoveMode = true;

    }

}
