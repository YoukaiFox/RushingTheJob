using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public abstract class MiniGame : MonoBehaviour
{
    #region Variables
    [Header("Global References")]
    public Image successImage;
    public Image failureImage;
    public Image staticElementsRoot;
    public AudioClip successSound;
    public AudioClip failureSound;
    public AudioSource[] auxiliarAudioSources;
    public AudioSource audioSource;
    private bool isMiniGameOver;
    private const float disableTime = 2f;
    private bool wonLastTry = false;
    public static System.Action OnWinning;
    public static System.Action OnLosing;
    public static System.Action OnMiniGameEnd;
    #endregion
    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnEnable() 
    {
        successImage.gameObject.SetActive(false);
        failureImage.gameObject.SetActive(false);
        isMiniGameOver = false;
        Restart();
    }
    protected void PlayAudioClip(AudioClip clip)
    {
        foreach (AudioSource source in auxiliarAudioSources)
        {
            if (!source.isPlaying)
            {
                source.clip = clip;
                source.Play();
                return;
            }
        }
    }
    protected virtual void WinMiniGame()
    {
        isMiniGameOver = true;
        successImage.gameObject.SetActive(true);
        audioSource.clip = successSound;
        audioSource.Play();
        wonLastTry = true;
        Invoke(nameof(Disable), disableTime);
    }
    protected virtual void LoseMiniGame()
    {
        isMiniGameOver = true;
        failureImage.gameObject.SetActive(true);
        audioSource.clip = failureSound;
        audioSource.Play();
        wonLastTry = false;
        Invoke(nameof(Disable), disableTime);
    }
    protected virtual void ReactToMissAction()
    {
        staticElementsRoot.transform.DOShakePosition(0.25f, 3f);
    }
    protected abstract void Restart();
    public abstract void DifficultyIncrease();
    public bool GetIsMiniGameOver() {return isMiniGameOver;}
    private void Disable()
    {
        gameObject.SetActive(false);

        if (wonLastTry)
        {
            if (OnWinning != null) OnWinning();
        }
        else
        {
            if (OnLosing != null) OnLosing();
        }
        OnMiniGameEnd();
    }
}
