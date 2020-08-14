using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public AudioClip buttonClick;
    public AudioSource sndSource;
    public void OpenCredits()
    {
        sndSource.PlayOneShot(buttonClick);
        StartCoroutine(OpenCreditsRoutine());
    }
    public void StartGame()
    {
        sndSource.PlayOneShot(buttonClick);
        StartCoroutine(StartGameRoutine());
    }
    private IEnumerator OpenCreditsRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
    }
    private IEnumerator StartGameRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
