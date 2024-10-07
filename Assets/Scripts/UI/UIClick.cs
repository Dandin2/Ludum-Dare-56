using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action clickAction;
    public Action hoverAction;
    public Action unHoverAction;

    private AudioSource audioSource;

    public void Start()
    {
        audioSource= GetComponent<AudioSource>();
        if(audioSource == null)
        {
            audioSource = Camera.main.GetComponent<AudioSource>();
        }
    }

    public void SetClickAction(Action onClick)
    {
        clickAction = onClick;
    }

    public void SetHoverAction(Action onHover)
    {
        hoverAction = onHover;
    }

    public void SetUnhoverAction(Action onUnhover)
    {
        unHoverAction = onUnhover;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(audioSource!= null)
        {
            audioSource.Play();
        }
        clickAction?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverAction?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        unHoverAction?.Invoke();
    }
}
