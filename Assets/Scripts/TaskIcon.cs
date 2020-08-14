using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TaskIcon : MonoBehaviour
{
    public Image iconSprite;
    public Image completedIcon;
    private void OnEnable() 
    {
        completedIcon.enabled = false;
    }
}
