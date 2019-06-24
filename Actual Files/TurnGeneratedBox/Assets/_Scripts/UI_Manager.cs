
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

    Map_Generator Map;

    private Dictionary<TileType, Sprite> TileTypeDic = new Dictionary<TileType, Sprite>();
    private Dictionary<MaterialTile, Sprite> MaterialDic=  new Dictionary<MaterialTile, Sprite>();

    //This is just for the tooltip
    private string yikes;

    [Header("UI Icons")]
    [Tooltip("Element 0 = Grass | Element 1 = Water")]
    public Sprite[] TileIcon = new Sprite[Enum.GetValues(typeof(TileType)).Length];
    [Tooltip("Element 0 = None | Element 1 = Tree | Element 2 = Mountain")]
    public Sprite[] MaterialIcon = new Sprite[Enum.GetValues(typeof(MaterialTile)).Length];
    
    private void Start()
    {
        //Add the keys (I will have to automate this so it is easier to add stuff)
        TileTypeDic.Add(TileType.Grass, TileIcon[0]);
        TileTypeDic.Add(TileType.Water, TileIcon[1]);
        MaterialDic.Add(MaterialTile.None, MaterialIcon[0]);
        MaterialDic.Add(MaterialTile.Tree, MaterialIcon[1]);
        MaterialDic.Add(MaterialTile.Mountain, MaterialIcon[2]);

        Map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map_Generator>();

        if (Map == null)
        {
            Debug.Log("Map reference could not be found");
        }

        ChangeOfSelection();
    }
    void Update () {
		
	}


    public void ChangeOfSelection()
    {
        if (Map.CurrentTile == null)
        {
            Bot_Left.Holder.gameObject.SetActive(false);
        } else 
        {
            //Optimization real quick
            MapTile CurT = Map.CurrentTile;
            
            //Tipo de recurso
            if (CurT.OcupedByMat != MaterialTile.None)
            {
               Bot_Left.Title.text = CurT.OcupedByMat.ToString();
                Bot_Left.Attack.gameObject.SetActive(false);
                Bot_Left.Life.gameObject.SetActive(false);
                Bot_Left.Roughness.gameObject.SetActive(false);
                Bot_Left.Icon.sprite = MaterialDic[CurT.OcupedByMat];
                Bot_Left.Holder.gameObject.SetActive(true);

            }
            //Tipo de Unidad
            else if (CurT.OcupiedByUnit != UnitIn.None)
            {
                Bot_Left.Title.text = "Not Set";
                Bot_Left.Attack.gameObject.SetActive(true);
                Bot_Left.AttackP.text = "Not Set";
                Bot_Left.Life.gameObject.SetActive(true);
                Bot_Left.LifeP.text = "Not Set";
                Bot_Left.Roughness.gameObject.SetActive(false);
                Bot_Left.Holder.gameObject.SetActive(true);
            }
            // Tipo de tile
            else if (CurT.OcupedByMat == MaterialTile.None && CurT.OcupiedByUnit == UnitIn.None)
            {
                Bot_Left.Title.text = CurT.Type.ToString();
                Bot_Left.Attack.gameObject.SetActive(false);
                Bot_Left.Life.gameObject.SetActive(false);
                //   Bot_Left.Icon.gameObject.SetActive(false); //temporary
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

}
