using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CreditsManager : MonoBehaviour
{
    public GameObject creditsText;
    private Transform creditsTextTransform;
    private Tween tween;
    private void Start() 
    {
        creditsTextTransform = creditsText.transform;
        tween = creditsTextTransform.DOLocalMoveY(865f, 65f).SetEase(Ease.Linear);
    }
    private void OnDisable() 
    {
        tween.Kill();
    }
    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
