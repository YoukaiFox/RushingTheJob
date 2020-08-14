using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    public Image currentImage;
    public Image nextImage;
    public Image blackScreen;
    public Sprite[] endingSprites;
    public Sprite[] chosenEndingSprites;
    public AudioSource bgmPlayer;
    public AudioClip goodEndingBGM;
    public AudioClip badEndingBGM;
    private int index;
    private bool canAdvance;
    private Color transparentWhite = new Color(1, 1, 1, 0);
    private void Start() 
    {
        chosenEndingSprites = new Sprite[3];
        chosenEndingSprites[0] = endingSprites[0];

        if (DataController.Instance && DataController.Instance.GetGotGoodEnding())
        {
            chosenEndingSprites[1] = endingSprites[1];
            chosenEndingSprites[2] = endingSprites[3];
            bgmPlayer.PlayOneShot(goodEndingBGM);
        }
        else
        {
            chosenEndingSprites[1] = endingSprites[2];
            chosenEndingSprites[2] = endingSprites[4];
            bgmPlayer.PlayOneShot(badEndingBGM);
        }

        currentImage.sprite = chosenEndingSprites[0];
        nextImage.gameObject.SetActive(false);
        StartCoroutine(FadeFromBlack());
    }
    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Space) && canAdvance) 
        {
            AdvanceScene();
        }
    }
    public void AdvanceScene()
    {
        canAdvance = false;
        index++;

        if (index < chosenEndingSprites.Length)
        {
            StartCoroutine(CrossFade());
        }
        else
        {
            StartCoroutine(FadeToBlack());
        }
    }
    private IEnumerator FadeFromBlack()
    {
        Color color = Color.black;
        blackScreen.color = color;
        blackScreen.gameObject.SetActive(true);
        int i = 0;

        while (i < 20)
        {
            color = blackScreen.color;
            color.a -= 0.05f;
            blackScreen.color = color;
            i++;
            yield return new WaitForSeconds(0.025f);
        }

        canAdvance = true;
    }
    private IEnumerator FadeToBlack()
    {
        Color color = Color.black;
        color.a = 0;
        blackScreen.color = color;
        blackScreen.gameObject.SetActive(true);
        int i = 0;

        while (i < 20)
        {
            color = blackScreen.color;
            color.a += 0.05f;
            blackScreen.color = color;
            i++;
            yield return new WaitForSeconds(0.1f);
        }

        blackScreen.color = Color.black;

        UnityEngine.SceneManagement.SceneManager.LoadScene("ThankYouScene");
    }
    private IEnumerator CrossFade()
    {
        nextImage.sprite = chosenEndingSprites[index];
        nextImage.color = transparentWhite;
        nextImage.gameObject.SetActive(true);

        int i = 0;

        while (i < 20)
        {
            Color color = currentImage.color;
            color.a -= 0.05f;
            currentImage.color = color;
            color = nextImage.color;
            color.a += 0.05f;
            nextImage.color = color;
            i++;
            yield return new WaitForSeconds(0.1f);
        }

        currentImage.sprite = nextImage.sprite;
        currentImage.color = Color.white;
        nextImage.gameObject.SetActive(false);
        canAdvance = true;
    }
}
