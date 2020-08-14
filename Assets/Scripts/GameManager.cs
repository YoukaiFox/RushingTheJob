using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Variables
    [Header("Gameplay values")]
    public float staminaGain = 0.5f;
    public float staminaWinCost = 0.1f;
    public float staminaDefeatCost = 0.25f;
    public int initialItemsQnty = 5;
    public float initialTime = 300f;
    private float lerpTime = 1f;
    [Header("References UI")]
    public ButtonMashing buttonMashing;
    public ButtonHero buttonHero;
    public ButtonAccuracy buttonAccuracy;
    public ButtonHoldAndRelease buttonHold;
    public Slider staminaBar;
    public Image arrow;
    public HorizontalLayoutGroup taskList;
    public TaskIcon iconCopyingMachine;
    public TaskIcon iconComputer;
    public TaskIcon iconStampingDesk;
    public TaskIcon iconSigningDesk;
    public GameObject[] tutorials;
    private int tutoIndex;
    public TextMeshProUGUI timerText;
    [Header("References Non-UI")]
    public AudioSource bgmPlayer;
    public AudioClip bgmRushStart;
    public SpriteRenderer clock;
    private Player player;
    private TaskIcon currentTask;
    public Transform coffeePosition;
    public Transform computerPosition;
    public Transform copyingMachinePosition;
    public Transform stampingDeskPosition;
    public Transform signingDeskPosition;
    public Transform bossPosition;
    public Collider2D coffee;
    public Collider2D boss;
    public Collider2D doorBlock;
    private System.Random random;
    private int lastRandomChoice;
    public int dialogueIndex;
    private bool isTimePaused;
    private bool hasCountdownStarted;
    private bool hasSkippedFreeze;
    private bool isManagingStamina;
    private float startingStaminaValue;
    private float endStaminaValue;
    private float currentLerpTime;
    private bool startingLerp;
    private List<int> numbersUsed;
    private Tween tween;
    private GameObject selectedMinigame;
    private Queue<TaskIcon> taskIcons;
    private List<GameObject> iconsObjects;
    public static System.Action OnDialogueCall;
    public static System.Action OnGameOver;
    public static GameManager Instance {get; private set;}
    #endregion
    private void Awake() 
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        random = new System.Random();
    }
    private void Start() 
    {
        taskIcons = new Queue<TaskIcon>();
        numbersUsed = new List<int>();
        iconsObjects = new List<GameObject>();
        player = FindObjectOfType<Player>();
        coffee.gameObject.layer = 0;
        timerText.gameObject.SetActive(false);
        clock.gameObject.SetActive(false);
        Invoke(nameof(StartConversation), 0.1f);
    }
    private void OnEnable() 
    {
        MiniGame.OnWinning += WinGame;
        MiniGame.OnLosing += LoseGame;
        MiniGame.OnMiniGameEnd += OnMiniGameEnd;
        DialogueManager.OnDialogueStart += OnConversationStart;
        DialogueManager.OnDialogueEnd += OnConversationEnd;
        Player.OnCoffeeEnd += OpenCoffeeDialogue;
    }
    private void OnDisable() 
    {
        MiniGame.OnWinning -= WinGame;
        MiniGame.OnLosing -= LoseGame;
        MiniGame.OnMiniGameEnd -= UnfreezePlayer;
        DialogueManager.OnDialogueStart -= OnConversationStart;
        DialogueManager.OnDialogueEnd -= OnConversationEnd;
        Player.OnCoffeeEnd -= OpenCoffeeDialogue;
    }
    private void Update() 
    {
        Timer();

        if (isManagingStamina)
            ManageStaminaSpeed();
    }
    private void Timer()
    {
        if (isTimePaused || !hasCountdownStarted) return;
        initialTime -= Time.deltaTime;
        timerText.text = Mathf.Round(initialTime).ToString();

        if (initialTime <= 0)
        {
            DataController.Instance.EndGame(false);
            bgmPlayer.DOFade(0f, 2f);
        }
    }
    private void WinGame()
    {
        player.IncreaseStamina(-staminaWinCost);
        currentTask.completedIcon.enabled = true;
        currentTask.iconSprite.color = Color.gray;

        startingStaminaValue = staminaBar.value;
        endStaminaValue = staminaBar.value - staminaWinCost;

        startingLerp = true;
        ManageStaminaSpeed();

        if (taskIcons.Count <= 0)
        {
            CompleteTaskList();
        }
        else
        {
            currentTask = taskIcons.Dequeue();
            SetArrowLocation(currentTask);
        }
    }
    private void LoseGame()
    {
        player.IncreaseStamina(-staminaDefeatCost);
        
        startingStaminaValue = staminaBar.value;
        endStaminaValue = staminaBar.value - staminaDefeatCost;

        startingLerp = true;
        ManageStaminaSpeed();

        if (staminaBar.value < 0.2f)
        {
            staminaBar.value = 0.2f;
        }
    }
    public void OpenMiniGame(string minigame)
    {
        if (!currentTask || !currentTask.CompareTag(minigame))
        {
            print("Agora não");
            return;
        }

        player.SetFrozen(true);

        if (String.Equals(minigame, "CopyMachine"))
        {
            selectedMinigame = (buttonAccuracy.gameObject);
        }
        else if (String.Equals(minigame, "Stamping"))
        {
            selectedMinigame = (buttonMashing.gameObject);
        }
        else if (String.Equals(minigame, "Signatures"))
        {
            selectedMinigame = (buttonHold.gameObject);
        }
        else if (String.Equals(minigame, "Typing"))
        {
            selectedMinigame = (buttonHero.gameObject);
        }
        else
        {
            print("ERRO!!!");
        }

        Invoke(nameof(OpenSelectedMinigame), 0.1f);
    }
    public void DrinkCoffee()
    {
        player.IncreaseStamina(staminaGain);

        startingStaminaValue = staminaBar.value;
        endStaminaValue = staminaBar.value + staminaGain;

        startingLerp = true;
        ManageStaminaSpeed();
    }
    public void OpenCoffeeDialogue()
    {
        if (dialogueIndex == 2)
            StartConversation();
    }
    public void GenerateSmartTaskList(int items)
    {
        for (int i = 0; i < initialItemsQnty; i++)
        {
            if (i == 4 || i == initialItemsQnty - 1)
            {
                bool number0used = false;
                bool number1used = false;
                bool number2used = false;
                bool number3used = false;

                foreach (int n in numbersUsed)
                {
                    if (n == 0)
                        number0used = true;
                    else if (n == 1)
                        number1used = true;
                    else if (n == 2)
                        number2used = true;
                    else if (n == 3)
                        number3used = true;
                }

                if (!number0used)
                    taskIcons.Enqueue(GenerateRandomTask(0));
                else if (!number1used)
                    taskIcons.Enqueue(GenerateRandomTask(1));
                else if (!number2used)
                    taskIcons.Enqueue(GenerateRandomTask(2));
                else if (!number3used)
                    taskIcons.Enqueue(GenerateRandomTask(3));
                else
                    taskIcons.Enqueue(GenerateRandomTask(random.Next(0, 4)));
            }
            else
            {
                int rng = random.Next(0, 4);

                while (rng == lastRandomChoice)
                {
                    rng = random.Next(0, 4);
                }

                numbersUsed.Add(rng);
                taskIcons.Enqueue(GenerateRandomTask(rng));
                lastRandomChoice = rng;
            }
        }

        foreach (TaskIcon t in taskIcons)
        {
            iconsObjects.Add(t.gameObject);
        }

        currentTask = taskIcons.Dequeue();
        SetArrowLocation(currentTask);
    }
    private TaskIcon GenerateRandomTask(int taskNumber)
    {
        switch (taskNumber)
        {
            case 0:
                return Instantiate(iconCopyingMachine, taskList.transform, false);
            case 1:
                return Instantiate(iconSigningDesk, taskList.transform, false);
            case 2:
                return Instantiate(iconStampingDesk, taskList.transform, false);
            case 3:
                return Instantiate(iconComputer, taskList.transform, false);
            default:
                return Instantiate(iconComputer, taskList.transform, false);
        }
    }
    private void SetArrowLocation(Vector2 position)
    {
        arrow.transform.position = position;
        tween.Kill();
        tween = arrow.transform.DOLocalMoveY(arrow.transform.localPosition.y - 5f, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }
    private void SetArrowLocation(TaskIcon icon)
    {
        if (icon.CompareTag("CopyMachine"))
        {
            SetArrowLocation(copyingMachinePosition.position);
        }
        else if (icon.CompareTag("Stamping"))
        {
            SetArrowLocation(stampingDeskPosition.position);
        }
        else if (icon.CompareTag("Signatures"))
        {
            SetArrowLocation(signingDeskPosition.position);
        }
        else if (icon.CompareTag("Typing"))
        {
            SetArrowLocation(computerPosition.position);
        }
    }
    private void CompleteTaskList()
    {
        SetArrowLocation(bossPosition.position);
        boss.gameObject.layer = 9;
    }
    private void OnConversationEnd(bool sequentialDialogue)
    {
        if (sequentialDialogue)
        {
            dialogueIndex++;

            if (dialogueIndex == 1)
            {
                SetArrowLocation(bossPosition.position);
                TriggerTutorial();
            }
            else if (dialogueIndex == 2)
            {
                SetArrowLocation(coffeePosition.position);
                coffee.gameObject.layer = 9;
                boss.gameObject.layer = 0;
                TriggerTutorial();
            }
            else if (dialogueIndex == 3)
            {
                GenerateSmartTaskList(initialItemsQnty);
                hasCountdownStarted = true;
                timerText.gameObject.SetActive(true);
                clock.gameObject.SetActive(true);
                bgmPlayer.clip = bgmRushStart;
                bgmPlayer.Play();
                bgmPlayer.DOFade(0.65f, 3f);
            }
            else if (dialogueIndex == 4)
            {
                ClearDoneTasks();
                IncreaseDifficulty();
                GenerateSmartTaskList(initialItemsQnty);
                boss.gameObject.layer = 0;
            }
            else if (dialogueIndex == 5)
            {
                ClearDoneTasks();
                IncreaseDifficulty();
                GenerateSmartTaskList(initialItemsQnty);
                boss.gameObject.layer = 0;
            }
            else if (dialogueIndex == 6)
            {
                boss.gameObject.layer = 0;
                SetArrowLocation(doorBlock.transform.position);
                doorBlock.enabled = false;
            }
        }

        player.SetFrozen(false);
        isTimePaused = false;
    }
    public void OnConversationStart()
    {
        isTimePaused = true;
        player.SetFrozen(true);

        foreach (GameObject window in tutorials)
        {
            window.SetActive(false);
        }

        if (dialogueIndex == 2)
        {
            bgmPlayer.DOFade(0f, 3f);
        }
    }
    public void StartConversation()
    {
        OnDialogueCall();
    }
    private void OpenSelectedMinigame()
    {
        selectedMinigame.gameObject.SetActive(true);
    }
    private void ClearDoneTasks()
    {
        foreach (GameObject g in iconsObjects)
        {
            Destroy(g);
        }
    }
    public void ExitDoor()
    {
        DataController.Instance.EndGame(true);
    }
    private void OnMiniGameEnd()
    {
        if (player.GetStamina() >= 0.2f)
        {
            UnfreezePlayer();
        }
        else
        {
            if (hasSkippedFreeze)
            {
                UnfreezePlayer();
            }

            if (!hasSkippedFreeze && player.GetReachedLowStamina())
            {
                hasSkippedFreeze = true;
            }
        }
    }
    private void UnfreezePlayer()
    {
        player.SetFrozen(false);
    }
    private void IncreaseDifficulty()
    {
        initialItemsQnty++;
        buttonMashing.DifficultyIncrease();
        buttonHero.DifficultyIncrease();
        buttonAccuracy.DifficultyIncrease();
        buttonHold.DifficultyIncrease();
    }
    private void ManageStaminaSpeed() {
        if (startingLerp) 
        {
            currentLerpTime = 0f;
            startingLerp = false;
            isManagingStamina = true;

            if (endStaminaValue > 1f)
                endStaminaValue = 1f;

            if (endStaminaValue < 0.2f)
                endStaminaValue = 0.2f;
        }

        if (currentLerpTime >= lerpTime)
            currentLerpTime = lerpTime;
        else
            currentLerpTime += Time.deltaTime;

        staminaBar.value = Mathf.Lerp(startingStaminaValue, endStaminaValue, currentLerpTime / lerpTime);

        if (staminaBar.value == endStaminaValue)
        {    
            isManagingStamina = false;
        }
    }
    public void TriggerTutorial()
    {
        tutorials[tutoIndex++].SetActive(true);
    }
}
