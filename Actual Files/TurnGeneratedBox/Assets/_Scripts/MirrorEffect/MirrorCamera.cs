using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorCamera : MonoBehaviour
{
    private Camera TheCamera;
    public RectTransform MirrorWorldCanvas, Left,Right, Top, Bot, BotLeft, BotRight, TopLeft, TopRight;
    void Start()
    {
        TheCamera = this.GetComponent<Camera>();
        SetSize();
    }


    void SetSize()
    {
        float size = StaticMapConf.Size / 2;
        size += 0.5f; //Cheap fix idk 
        TheCamera.orthographicSize = size;

        MirrorWorldCanvas.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size * 2);
        MirrorWorldCanvas.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size * 2);
        
        //Left
        Left.offsetMin = new Vector2(-(size * 2) + 2,0);
        Left.offsetMax = new Vector2(-(size*2) + 2,0);
        //Right
        Right.offsetMin = new Vector2(size * 2 - 2, 0);
        Right.offsetMax = new Vector2(size * 2 - 2, 0);

        //Top
        Top.offsetMin = new Vector2(0,size * 2 - 2);
        Top.offsetMax = new Vector2(0, size * 2 - 2);

        //Bot
        Bot.offsetMin = new Vector2(0, -(size * 2) + 2);
        Bot.offsetMax = new Vector2(0, -(size * 2) + 2);

        //BotLeft
        BotLeft.offsetMin = new Vector2(-(size * 2) + 2, -(size * 2) + 2);
        BotLeft.offsetMax = new Vector2(-(size * 2) + 2, -(size * 2) + 2);

        //BotRight
        BotRight.offsetMin = new Vector2(size * 2 - 2, -(size * 2) + 2);
        BotRight.offsetMax = new Vector2(size * 2 - 2, -(size * 2) + 2);

        //TopLeft
        TopLeft.offsetMin = new Vector2(-(size * 2) + 2, size * 2 - 2);
        TopLeft.offsetMax = new Vector2(-(size * 2) + 2, size * 2 - 2);

        //TopRight
        TopRight.offsetMin = new Vector2(size * 2 - 2, size * 2 - 2);
        TopRight.offsetMax = new Vector2(size * 2 - 2, size * 2 - 2);

    }

}