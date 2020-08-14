using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHoldAndRelease : MiniGame
{
    #region Variables
    [Header("Gameplay values")]
    public float initialProgressValue = 0.2f;
    public float progressIncreaseValue = 0.15f;
    public float progressDecreaseValue = 0.175f;
    public float holdIncreaseValue = 0.005f;
    public float actualReleaseMinPosition = 0.5f;
    public float actualReleaseMaxPosition = 0.6f;
    [Header("References")]
    public Slider progressBar;
    public Slider actionBar;
    public Image actionBarHandle;
    public Image releaseMarker;
    public AudioClip writeSound;
    public AudioClip hitSound;
    public AudioClip missSound;
    [Header("Debug")]
    public float releaseInterval = 0.125f;
    public float minReleasePos = 0.5f;
    public float maxReleasePos = 0.8f;
    public bool isGameActive;
    private bool hasPressed;
    #endregion
    private void Update() 
    {
        ReadInput();
    }
    private void ReadInput()
    {
        if (!isGameActive || GetIsMiniGameOver()) return;

        if (Input.GetKey(KeyCode.Space))
        {
            actionBar.value += holdIncreaseValue;

            if (!audioSource.isPlaying)
            {
                audioSource.clip = writeSound;
                audioSource.Play();
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (actionBar.value >= actualReleaseMinPosition && actionBar.value <= actualReleaseMaxPosition)
            {
                progressBar.value += progressIncreaseValue;
                PlayAudioClip(hitSound);
                StartCoroutine(Proceed());
            }
            else
            {
                progressBar.value -= progressDecreaseValue;
                PlayAudioClip(missSound);
                StartCoroutine(Proceed());
            }

            audioSource.Stop();
        }

        if (progressBar.value >= 0.99f)
        {
            isGameActive = false;
            WinMiniGame();
        }
        else if (progressBar.value <= 0)
        {
            isGameActive = false;
            LoseMiniGame();
        }
    }
    private IEnumerator Proceed()
    {
        isGameActive = false;
        yield return new WaitForSeconds(releaseInterval);
        isGameActive = true;
        ChangeReleasePosition();
    }
    private void ChangeReleasePosition()
    {
        actualReleaseMinPosition = Random.Range(minReleasePos, maxReleasePos);
        actualReleaseMaxPosition = actualReleaseMinPosition + releaseInterval;
        actionBar.value = (actualReleaseMaxPosition + actualReleaseMinPosition) / 2;
        releaseMarker.transform.position = actionBarHandle.transform.position;
        actionBar.value = 0;
    }
    protected override void Restart()
    {
        progressBar.value = initialProgressValue;
        ChangeReleasePosition();
        isGameActive = true;

        if (Input.GetKeyUp(KeyCode.Space))
            hasPressed = true;
    }
    public override void DifficultyIncrease()
    {
        progressIncreaseValue -= 0.01f;
        holdIncreaseValue += 0.0075f;
        progressDecreaseValue += 0.01f;
    }
}
