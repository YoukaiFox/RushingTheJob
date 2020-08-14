using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElementHighlight : MonoBehaviour
{
    [SerializeField] private bool activateOutline;
    [SerializeField] private bool enlargeElement;
    [SerializeField] private float enlargementModifier;
    [SerializeField] private AudioClip highlightSound;
    private AudioSource sndEffects;
    private Vector2 initialScale;
    private Outline outline;
    private Image image;
    private void Start() 
    {
        outline = GetComponent<Outline>();
        image = GetComponent<Image>();
        sndEffects = GetComponent<AudioSource>();
        if (outline) outline.enabled = false;
        initialScale = transform.localScale;
    }
    private void OnMouseEnter() 
    {
        if (activateOutline && outline)
            outline.enabled = true;

        if (enlargeElement && image)
            transform.localScale *= enlargementModifier;

        if (highlightSound && sndEffects)
            sndEffects.PlayOneShot(highlightSound);
    }
    private void OnMouseExit() 
    {
        if (activateOutline && outline)
            outline.enabled = false;

        if (enlargeElement && image)
            transform.localScale = initialScale;
    }
    
}
