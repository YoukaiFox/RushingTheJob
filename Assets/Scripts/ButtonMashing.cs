using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMashing : MiniGame
{
    #region Variables
    [Header("Gameplay values")]
    public float initialProgressValue = 0.2f;
    public float increaseValue = 0.05f;
    public float decreaseValue = 0.001f;
    public float restInterval = 0.05f;
    [Header("References")]
    public Slider progressBar;
    public Image buttonIcon;
    public AudioClip[] actionSounds;
    [Header("Debug")]
    public float colorChangeDuration = 0.1f;
    public Color buttonPressColor = Color.red;
    private Color initialButtonColor;
    private bool hasColorChanged;
    private bool isResting;
    #endregion
    private void Update() 
    {
        DecreaseValue();
        ReadInput();    
    }
    private void DecreaseValue()
    {
        if (GetIsMiniGameOver() || isResting) return;

        progressBar.value -= decreaseValue;

        if (progressBar.value <= 0)
        {
            LoseMiniGame();
        }
    }
    private void ReadInput()
    {
        if (GetIsMiniGameOver() || isResting) return;

        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            progressBar.value += increaseValue;
            PlayAudioClip(actionSounds[Random.Range(0, actionSounds.Length)]);
            ChangeButtonColor();
            RestAction();
        }
        if (progressBar.value >= 0.99f)
        {
            WinMiniGame();
        }
    }
    private void ChangeButtonColor()
    {
        if (GetIsMiniGameOver() || isResting) return;

        if (!hasColorChanged)
        {
            hasColorChanged = true;
            buttonIcon.color = buttonPressColor;
            Invoke(nameof(ResetColor), colorChangeDuration);
        }
    }
    private void ResetColor()
    {
        hasColorChanged = false;
        buttonIcon.color = initialButtonColor;
    }
    protected override void Restart()
    {
        hasColorChanged = false;
        initialButtonColor = Color.white;
        progressBar.value = initialProgressValue;
    }
    public override void DifficultyIncrease()
    {
        increaseValue -= 0.01f;
        decreaseValue += 0.0003f;
    }
    private void RestAction()
    {
        if (!isResting)
        {
            isResting = true;
            Invoke(nameof(RestAction), restInterval);
        }
        else
        {
            isResting = false;
        }
    }
}
