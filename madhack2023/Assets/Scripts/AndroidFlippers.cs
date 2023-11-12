using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AndroidFlippers : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    bool isLeft = false;

    [SerializeField]
    Sprite downSprite;
    [SerializeField]
    Sprite upSprite;

    [SerializeField]
    UnityEngine.UI.Image spriteSwap;

    public void OnPointerDown(PointerEventData eventData)
    {
        spriteSwap.sprite = downSprite;
        FlipperInput.OnUIDown?.Invoke(isLeft ? "left" : "right");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        spriteSwap.sprite = upSprite;
        FlipperInput.OnUIUp?.Invoke(isLeft ? "left" : "right");
    }
}
