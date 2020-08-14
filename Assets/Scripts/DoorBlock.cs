using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBlock : MonoBehaviour
{
    public Dialogue doorBlockedDialogue;

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.layer == 10)
        {
            DialogueManager.Instance.StartNonSequentialDialogue(doorBlockedDialogue);
        }
    }
}
