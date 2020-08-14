using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HitableKey : MonoBehaviour
{
    public Image image;
    public static System.Action OnSpawn;
    public static System.Action OnDeactivate;
    public ButtonHero.Direction direction;
    public Tween tween;
    public void SetSprite(Sprite sprite) {image.sprite = sprite;}
    public ButtonHero.Direction GetDirection() {return direction;}
    public void SetDirection(ButtonHero.Direction set) {direction = set;}
    private void OnDisable() {tween.Kill();}
    public void SetTween(Tween set) {tween = set;}
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}