using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Opening : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private AudioClip fillingSound;
    [SerializeField] private AudioClip shishiOdoshiSound;
    [SerializeField] private AudioSource soundPlayer;
    private List<Graphic> graphics;
    private void Awake() 
    {
        graphics = new List<Graphic>();

        for (int i = 0; i < transform.childCount; i++)
        {
            graphics.Add(transform.GetChild(i).GetComponent<Graphic>());
        }

        RemoveAlphas();
    }
    private void Start() 
    {
        StartCoroutine(FadeIn());
    }
    private IEnumerator FadeIn()
    {
        soundPlayer.PlayOneShot(fillingSound);
        int i = 0;

        while (i < 50)
        {
            foreach (Graphic graphic in graphics)
            {
                Color color = graphic.color;
                color.a += 0.02f;
                graphic.color = color;
            }
            
            i++;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitUntil(() => soundPlayer.isPlaying);
        soundPlayer.PlayOneShot(shishiOdoshiSound);
        yield return new WaitForSeconds(0.05f);
        RemoveAlphas();

        yield return new WaitForSeconds(2f);

        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);

        gameObject.SetActive(false);
    }
    private void RemoveAlphas()
    {
        foreach (Graphic graphic in graphics)
        {
            Color color = graphic.color;
            color.a = 0f;
            graphic.color = color;
        }
    }
}
