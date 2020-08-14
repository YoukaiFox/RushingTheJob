using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonHero : MiniGame
{
   #region Variables
    [Header("Gameplay values")]
    public float initialProgressValue = 0.2f;
    public float successIncreaseValue = 0.05f;
    public float errorDecreaseValue = 0.1f;
    public float keySummonInterval = 6f;
    public float timeToKeyReachDestination = 12f;
    [Header("References")]
    public Slider progressBar;
    public Transform keysStartingPosition;
    public Transform keysEndPosition;
    public Transform keysCheckPosition;
    public AudioClip[] hitSounds;
    public AudioClip missSound;
    public Sprite arrowUp;
    public Sprite arrowRight;
    public Sprite arrowDown;
    public Sprite arrowLeft;
    public HitableKey keyPrefab;
    public ParticleSystem particlesSuccess;
    public ParticleSystem particlesError;
    [Header("Debug")]
    public float overlapRadius = 0.5f;
    public int keysOnScreen = 0;
    public int maxKeysOnScreen = 5;
    public int keyIndex;
    public List<HitableKey> keyRenderers;
    #endregion
    public enum Direction {Up, Right, Down, Left}
    private void Start() 
    {
        InstantiateKeys();
        HitableKey.OnSpawn += IncreaseOnScreenKeys;
        HitableKey.OnDeactivate += DecreaseOnScreenKeys;
    }
    private void OnDestroy() 
    {
        HitableKey.OnSpawn -= IncreaseOnScreenKeys;
        HitableKey.OnDeactivate -= DecreaseOnScreenKeys;
    }
    private void Update() 
    {
        ReadInput();
    }
    private void ReadInput()
    {
        if (GetIsMiniGameOver()) return;

        if (Input.anyKeyDown)
        {
            Direction directionPressed;
            Collider2D collider = Physics2D.OverlapCircle(keysCheckPosition.position, overlapRadius);
            
            if (collider)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    directionPressed = Direction.Up;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    directionPressed = Direction.Right;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    directionPressed = Direction.Down;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    directionPressed = Direction.Left;
                }
                else
                {
                    PlayAudioClip(missSound);
                    particlesError.Play();
                    ComputeInput(false);
                    return;
                }

                HitableKey key = collider.gameObject.GetComponent<HitableKey>();

                if (key && key.GetDirection() == directionPressed)
                {
                    DeactivateKey(key, true);
                    return;
                }
                else
                {
                    DeactivateKey(key, false);
                }
            }

            PlayAudioClip(missSound);
            particlesError.Play();
            ComputeInput(false); 
        }

        if (progressBar.value <= 0)
        {
            DisableAllKeys();
            LoseMiniGame();
            return;
        }
    }
    private void ComputeInput(bool success)
    {
        if (success)
        {
            progressBar.value += successIncreaseValue;
        }
        else
        {
            progressBar.value -= errorDecreaseValue;
            ReactToMissAction();
        }

        if (progressBar.value >= 0.99f)
        {
            DisableAllKeys();
            WinMiniGame();
            return;
        }
    }
    private void InstantiateKeys()
    {
        keyRenderers = new List<HitableKey>();

        for (int i = 0; i < maxKeysOnScreen; i++)
        {
            HitableKey key = Instantiate(keyPrefab, transform, false);
            key.gameObject.SetActive(false);
            keyRenderers.Add(key);
        }
    }
    private HitableKey GetAvailableKey()
    {
        foreach (HitableKey key in keyRenderers)
        {
            if (!key.gameObject.activeInHierarchy)
            {
                return key;
            }
        }

        return null;
    }
    protected override void Restart()
    {
        keyIndex = 0;
        keysOnScreen = 0;
        progressBar.value = initialProgressValue;
        Invoke(nameof(SummonNextKey), keySummonInterval);
    }
    public override void DifficultyIncrease()
    {
        successIncreaseValue -= 0.05f;
        errorDecreaseValue += 0.125f;
        keySummonInterval -= 0.5f;
        timeToKeyReachDestination -= 1f;
    }
    private void SummonNextKey()
    {
        if (GetIsMiniGameOver()) return;
        HitableKey key = GetAvailableKey();
        if (!key) return;

        var directionsArray = System.Enum.GetValues(typeof(Direction));
        Direction dir = (Direction) Random.Range(0, directionsArray.Length);
        key.SetDirection(dir);
        SetKeySprite(key, dir);
        key.transform.position = keysStartingPosition.position;
        Tween tween = key.transform.DOMoveX(keysEndPosition.position.x, timeToKeyReachDestination).SetEase(
            Ease.Linear).OnComplete(() => DeactivateKey(key, false));
        key.SetTween(tween);
        key.gameObject.SetActive(true);
        keyIndex++;

        Invoke(nameof(SummonNextKey), keySummonInterval);
    }
    private void SetKeySprite(HitableKey key, Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                key.SetSprite(arrowUp);
                break;
            case Direction.Right:
                key.SetSprite(arrowRight);
                break;
            case Direction.Down:
                key.SetSprite(arrowDown);
                break;
            case Direction.Left:
                key.SetSprite(arrowLeft);
                break;
            default:
                break;
        }
    }
    private void DeactivateKey(HitableKey key, bool success)
    {
        if (!key) return;
        ComputeInput(success);
        key.Deactivate();

        if (success)
        {
            PlayAudioClip(hitSounds[Random.Range(0, hitSounds.Length)]);
            particlesSuccess.Play();
        }
        else
        {
            PlayAudioClip(missSound);
            particlesError.Play();
        }
    }
    private void IncreaseOnScreenKeys()
    {
        keysOnScreen++;
    }
    private void DecreaseOnScreenKeys()
    {
        keysOnScreen--;
    }
    private void DisableAllKeys()
    {
        foreach (HitableKey key in keyRenderers)
        {
            key.gameObject.SetActive(false);
        }
    }
}