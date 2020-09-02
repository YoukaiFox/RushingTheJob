using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataController : MonoBehaviour
{
    private bool isInEnglish = true;
    private bool gotGoodEnding;
    public static DataController Instance { get; private set; }
    private void Awake() 
    {
        if (Instance != null && Instance != this)
            Destroy (this.gameObject);
        else
            Instance = this;
    }
    private void Start() 
    {
        DontDestroyOnLoad(gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    public void SetIsEnglish(bool set)
    {
        isInEnglish = set;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    public void EndGame(bool goodEnding)
    {
        gotGoodEnding = goodEnding;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Ending");
    }
    public bool GetIsEnglish() {return isInEnglish;}
    public bool GetGotGoodEnding() {return gotGoodEnding;}
}