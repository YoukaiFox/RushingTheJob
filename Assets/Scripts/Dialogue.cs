using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Dialogue : ScriptableObject
{
    public bool isOneSidedConversation;
    [TextArea(3, 5)]
    public string description;
    public Message[] messages;
    public Message[] GetMessages() {return messages;}
    public bool GetIsOneSidedConversation() {return isOneSidedConversation;}
}