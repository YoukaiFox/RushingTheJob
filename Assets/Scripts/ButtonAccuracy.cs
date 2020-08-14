using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAccuracy : MiniGame
{
    #region Variables
    [Header("Gameplay values")]
    public float initialProgressValue = 0.2f;
    public float increaseValue = 0.1f;
    public float decreaseValue = 0.15f;
    public float actionSpeed = 0.0175f;
    public float nextPageDelay = 1f;
    [Header("References")]
    public Slider progressBar;
    public Slider actionBar;
    public AudioSource loopingSound;
    public AudioClip[] actionSounds;
    public AudioClip missSound;
    [Header("Debug")]
    public float minSuccessValue = 0.4f;
    public float maxSuccessValue = 0.6f;
    public bool isBarIncreasing;
    #endregion
    private void Update() 
    {
        MoveActionMeter();
        ReadInput();
    }
    private void MoveActionMeter()
    {
        if (GetIsMiniGameOver()) return;

        if (actionBar.value <= 0)
        {
            isBarIncreasing = true;
        }
        else if (actionBar.value >= 1)
        {
            isBarIncreasing = false;
        }

        if (isBarIncreasing)
            actionBar.value += actionSpeed;
        else
            actionBar.value -= actionSpeed;

        if (progressBar.value <= 0f)
        {
            LoseMiniGame();
        }
    }
    private void ReadInput()
    {
        if (GetIsMiniGameOver()) return;

        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            if (actionBar.value >= minSuccessValue && actionBar.value <= maxSuccessValue)
            {
                PlayAudioClip(actionSounds[Random.Range(0, actionSounds.Length)]);
                progressBar.value += increaseValue;
            }
            else
            {
                PlayAudioClip(missSound);
                ReactToMissAction();
                progressBar.value -= decreaseValue;
            }
        }
        if (progressBar.value >= 0.99f)
        {
            WinMiniGame();
        }
    } 
    protected override void Restart()
    {
        progressBar.value = initialProgressValue;
        actionBar.value = 0;
        loopingSound.loop = true;
        loopingSound.Play();
    }
    public override void DifficultyIncrease()
    {
        actionSpeed += 0.0075f;
        increaseValue -= 0.01f;
        decreaseValue += 0.05f;
    }
}
