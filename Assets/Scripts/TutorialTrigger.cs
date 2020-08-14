using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.layer == 10)
        {
            GameManager.Instance.TriggerTutorial();
            Destroy(gameObject);
        }
    }
}
