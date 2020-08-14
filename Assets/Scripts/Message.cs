using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Message
{
    public string owner;
    [TextArea(3, 5)]
    public string message;
    public string leftOrRightBalloon;
    public Sprite ownerPreview;
    public string GetMessageOwner() {return owner;}
    public string GetMessageText() {return message;}
    public string GetLeftOrRight() {return leftOrRightBalloon;}
    public Sprite GetPreview() {return ownerPreview;}
}