using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float stamina = 1;
    [SerializeField] private float movingSpeed = 2;
    [SerializeField] private float castRadius = 0.75f;
    public string copyMachineTagName = "CopyMachine";
    public string stampingTagName = "Stamping";
    public string signaturesTagName = "Signatures";
    public string typingTagName = "Typing";
    public string coffeeTagName = "CoffeeMachine";
    public string bossTagName = "Boss";
    public ExtraAttributes.Direction direction;
    [SerializeField] private Dialogue lowStaminaDialogue;
    [Header("Audio")]
    [SerializeField] private AudioSource soundEffects;
    [SerializeField] private AudioClip[] footsteps;
    [SerializeField] private AudioClip coffeeDrinking;
    private Vector2 input;
    private Rigidbody2D rb2d;
    private Animator animator;
    private Transform ownTransform;
    private bool isIdle;
    private bool isFrozen;
    private bool reachedLowStamina;
    private int interactiveDetectLayer;
    private float originalSpeed;
    public static System.Action OnCoffeeEnd;
    private void Awake() 
    {
        ownTransform = transform;
        interactiveDetectLayer = 1 << LayerMask.NameToLayer("Interactive");
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        originalSpeed = movingSpeed;
        animator.speed = stamina;
        movingSpeed = originalSpeed * stamina;
    }
    private void Update() 
    {
        input = GetDirectionalInput();
        ChangeDirection();
        Animate();
        Act();
    }
    private void FixedUpdate() 
    {
        Move(movingSpeed * Time.deltaTime * 100 * input.normalized);
    }
    private void Move(Vector2 move) 
    {
        if (isFrozen) return;
        rb2d.velocity = move;
    }
    private void Act()
    {
        if (!Input.GetKeyDown(KeyCode.Space) || isFrozen) return;

        Collider2D collider2D = Physics2D.OverlapCircle(ownTransform.position, castRadius, interactiveDetectLayer);

        if (collider2D)
        {
            animator.Play("idle_up");
            
            if (collider2D.gameObject.CompareTag(copyMachineTagName))
            {
                GameManager.Instance.OpenMiniGame(copyMachineTagName);
            }
            else if (collider2D.gameObject.CompareTag(stampingTagName))
            {
                GameManager.Instance.OpenMiniGame(stampingTagName);
            }
            else if (collider2D.gameObject.CompareTag(signaturesTagName))
            {
                GameManager.Instance.OpenMiniGame(signaturesTagName);
            }
            else if (collider2D.gameObject.CompareTag(typingTagName))
            {
                GameManager.Instance.OpenMiniGame(typingTagName);
            }
            else if (collider2D.gameObject.CompareTag(coffeeTagName) && stamina < 1f)
            {
                GameManager.Instance.DrinkCoffee();
                isFrozen = true;
                direction = ExtraAttributes.Direction.Down;
                animator.Play("drink_coffee");
                soundEffects.PlayOneShot(coffeeDrinking);
                rb2d.velocity = Vector2.zero;
            }
            else if (collider2D.gameObject.CompareTag(bossTagName))
            {
                GameManager.Instance.StartConversation();
            }
            else
            {
                print("ERRO!!!");
            }
        }
    }
    private void Animate()
    {
        if (isFrozen) return;
        isIdle = input == Vector2.zero;

        if (isIdle)
        {
            switch (direction)
            {
                case ExtraAttributes.Direction.Up:
                    animator.Play("idle_up");
                    break;
                case ExtraAttributes.Direction.Right:
                    animator.Play("idle_right");
                    break;
                case ExtraAttributes.Direction.Down:
                    animator.Play("idle_down");
                    break;
                case ExtraAttributes.Direction.Left:
                    animator.Play("idle_left");
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (direction)
            {
                case ExtraAttributes.Direction.Up:
                    animator.Play("walk_up");
                    break;
                case ExtraAttributes.Direction.Right:
                    animator.Play("walk_right");
                    break;
                case ExtraAttributes.Direction.Down:
                    animator.Play("walk_down");
                    break;
                case ExtraAttributes.Direction.Left:
                    animator.Play("walk_left");
                    break;
                default:
                    break;
            }
        }
    }
    private void ChangeDirection()
    {
        if (isFrozen) return;
        if (input.x > 0)
        {
            direction = ExtraAttributes.Direction.Right;
        }
        else if (input.x < 0)
        {
            direction = ExtraAttributes.Direction.Left;
        }
        else
        {
            if (input.y > 0)
            {
                direction = ExtraAttributes.Direction.Up;
            }
            else if (input.y < 0)
            {
                direction = ExtraAttributes.Direction.Down;
            }
        }
    }
    public void PlayFootstep()
    {
        soundEffects.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
    }
    public Vector2 GetDirectionalInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    public void IncreaseStamina(float increase) 
    {
        if (stamina > 1f)
            stamina = 1;
        else if (stamina < 0.2f)
            stamina = 0.2f;

        stamina += increase;
        animator.speed = stamina;
        movingSpeed = originalSpeed * stamina;

        if (stamina >= 1f)
        {
            animator.speed = 1f;
            movingSpeed = originalSpeed * 1f;
            stamina = 1;
        }
        else if (stamina <= 0.2f)
        {
            animator.speed = 0.2f;
            movingSpeed = originalSpeed * 0.2f;
            stamina = 0.2f;

            if (!reachedLowStamina)
            {
                reachedLowStamina = true;
                DialogueManager.Instance.StartNonSequentialDialogue(lowStaminaDialogue);
            }
        }
    }
    public void SetFrozen(bool set)
    {
        isFrozen = set;

        if (set)
            rb2d.velocity = Vector2.zero;
    }
    public void SetFrozen(bool set, float time)
    {
        StartCoroutine(FreezeRoutine(set, time));
    }
    private IEnumerator FreezeRoutine(bool set, float time)
    {
        yield return new WaitForSeconds(time);
        SetFrozen(set);
    }
    public void Unfreeze()
    {
        isFrozen = false;
    }
    public void FinishCoffee()
    {
        if (!DialogueManager.Instance.GetIsOnConversation())
        {
            isFrozen = false;
            OnCoffeeEnd();
        }
    }
    public ExtraAttributes.Direction GetDirection() {return direction;}
    public float GetStamina() {return stamina;}
    public bool GetReachedLowStamina() {return reachedLowStamina;}
}
