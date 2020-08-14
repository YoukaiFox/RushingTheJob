using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThankYouManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Application.Quit();
    }
}
