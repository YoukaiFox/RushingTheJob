using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoCloseWindow : MonoBehaviour
{
    private List<Graphic> graphics; 
    private void Awake() 
    {
        graphics = new List<Graphic>();

        for (int i = 0; i < transform.childCount; i++)
        {
            graphics.Add(transform.GetChild(i).GetComponent<Graphic>());
        }
    }
    private void OnEnable() 
    {
        StartCoroutine(FadeOut());
    }
    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(2.5f);

        int i = 0;

        while (i < 20)
        {
            foreach (Graphic graphic in graphics)
            {
                Color color = graphic.color;
                color.a -= 0.05f;
                graphic.color = color;
            }

            yield return new WaitForSeconds(0.05f);
        }

        gameObject.SetActive(false);
    }
}
