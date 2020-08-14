using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogueManager : MonoBehaviour
{
    #region Variables
    public Dialogue[] dialoguesEng;
    public Dialogue[] dialoguesPtBr;
    public Dialogue[] dialogues;
    public Image dialogueWindow;
    public Image leftSprite;
    public Image leftBalloon;
    public TextMeshProUGUI leftBalloonTitle;
    public TextMeshProUGUI leftBalloonMessage;
    public Image rightSprite;
    public Image rightBalloon;
    public Image dummyImage;
    public TextMeshProUGUI rightBalloonTitle;
    public TextMeshProUGUI rightBalloonMessage;
    public int dialogueIndex;
    private int messageIndex;
    private bool canAdvanceDialog = true;
    public bool isOnConversation = false;
    public bool isOnNonSequentialDialogue = false;
    private string lastBalloonDirection;
    private Dialogue currentDialogue;
    public static Action OnDialogueStart;
    public static Action<bool> OnDialogueEnd;
    public static Action OnAllDialoguesEnd;
    public static DialogueManager Instance { get; private set; }
    #endregion
    private void Awake() 
    {
        if (Instance != null && Instance != this)
            Destroy (this.gameObject);
        else
            Instance = this;
    }
    private void Start() 
    {
        dialogueWindow.gameObject.SetActive(false);
        ToggleHUD(false);
        GameManager.OnDialogueCall += WarmUpConversation;
        Translate();
    }
    private void Update() 
    {
        if (Input.anyKeyDown)
            AdvanceDialogue();

        if (isOnConversation && Input.GetKeyDown(KeyCode.Escape))
            EndDialogue();
    }
    private void OnDisable() 
    {
        GameManager.OnDialogueCall -= StartDialogue;
    }
    private void WarmUpConversation()
    {
        Invoke(nameof(StartDialogue), 0.25f);
    }
    private void StartDialogue()
    {
        if (OnDialogueStart != null) OnDialogueStart();
        dialogueWindow.gameObject.SetActive(true);
        ToggleHUD(true);
        ResetDialogues();
        currentDialogue = dialogues[dialogueIndex];
        messageIndex = 0;
        canAdvanceDialog = true;
        isOnConversation = true;
        AdvanceDialogue();
    }
    public void AdvanceDialogue()
    {
        if (!canAdvanceDialog || (!isOnConversation && !isOnNonSequentialDialogue)) return;

        if (messageIndex >= currentDialogue.GetMessages().Length)
        {
            EndDialogue();
            return;
        }

        StartCoroutine(ButtonCooldown());
        WriteText(currentDialogue);
        messageIndex++;
    }
    private void EndDialogue()
    {
        dialogueWindow.gameObject.SetActive(false);
        ToggleHUD(false);

        if (isOnConversation)
        {
            isOnConversation = false;
            dialogueIndex++;

            if (dialogueIndex >= dialogues.Length && OnAllDialoguesEnd != null)
                OnAllDialoguesEnd();

            if (OnDialogueEnd != null) OnDialogueEnd(true);
        }
        else if (isOnNonSequentialDialogue)
        {
            isOnNonSequentialDialogue = false;
            if (OnDialogueEnd != null) OnDialogueEnd(false);
        }
    }
    public void StartNonSequentialDialogue(Dialogue _dialogue)
    {
        if (OnDialogueStart != null) OnDialogueStart();
        dialogueWindow.gameObject.SetActive(true);
        ToggleHUD(true);
        ResetDialogues();
        currentDialogue = _dialogue;
        messageIndex = 0;
        canAdvanceDialog = true;
        isOnNonSequentialDialogue = true;
        AdvanceDialogue();
    }
    private void WriteText(Dialogue _dialogue)
    {
        if (System.String.Equals(_dialogue.messages[messageIndex].GetLeftOrRight().ToLower(), "left"))
        {
            leftSprite.sprite = _dialogue.messages[messageIndex].GetPreview();
            leftSprite.color = Color.white;
            rightSprite.color = Color.gray;
            leftBalloon.gameObject.SetActive(true);
            rightBalloon.gameObject.SetActive(false);
            leftBalloonTitle.text = _dialogue.messages[messageIndex].GetMessageOwner();
            leftBalloonMessage.text = _dialogue.messages[messageIndex].GetMessageText();

            if (messageIndex == 0 || _dialogue.GetIsOneSidedConversation())
            {
                Color color = Color.white;
                color.a = 0;
                rightSprite.color = color;
            }

            lastBalloonDirection = "left";
        }
        else
        {
            rightSprite.sprite = _dialogue.messages[messageIndex].GetPreview();
            rightSprite.color = Color.white;
            leftSprite.color = Color.gray;
            rightBalloon.gameObject.SetActive(true);
            leftBalloon.gameObject.SetActive(false);
            rightBalloonTitle.text = _dialogue.messages[messageIndex].GetMessageOwner();
            rightBalloonMessage.text = _dialogue.messages[messageIndex].GetMessageText();

            if (messageIndex == 0 || _dialogue.GetIsOneSidedConversation())
            {
                Color color = Color.white;
                color.a = 0;
                leftSprite.color = color;
            }

            lastBalloonDirection = "right";
        }
    }
    private void ToggleHUD(bool toggle)
    {
        leftBalloon.gameObject.SetActive(toggle);
        rightBalloon.gameObject.SetActive(toggle);
        leftSprite.gameObject.SetActive(toggle);
        rightSprite.gameObject.SetActive(toggle);
    }
    private void ResetDialogues()
    {
        rightSprite.sprite = dummyImage.sprite;
        leftSprite.sprite = dummyImage.sprite;
    }
    private void Translate()
    {
        if (DataController.Instance && !DataController.Instance.GetIsEnglish())
        {
            dialogues = dialoguesPtBr;
            print("Português selecionado");
        }
        else
        {
            dialogues = dialoguesEng;
            print("English selected");
        }
    }
    private IEnumerator ButtonCooldown() 
    {
        canAdvanceDialog = false;
        yield return new WaitForSeconds(0.5f);
        canAdvanceDialog = true;
    }
    public bool GetIsOnConversation() {return isOnConversation;}
}