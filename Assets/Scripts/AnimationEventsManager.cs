using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventsManager : MonoBehaviour
{
    public UnityEvent footstepEffect;
    public UnityEvent freeze;
    public UnityEvent unfreeze;
    public void PlayFootstep() {footstepEffect.Invoke();}
    public void FreezePlayer() {freeze.Invoke();}
    public void UnfreezePlayer() {unfreeze.Invoke();}
}
