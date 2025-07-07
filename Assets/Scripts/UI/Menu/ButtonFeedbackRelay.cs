using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonFeedbackRelay : MonoBehaviour, 
    ISelectHandler, 
    ISubmitHandler, 
    IPointerClickHandler,
    IPointerEnterHandler {
    public static event Action OnUIButtonPressed = delegate {};
    public static event Action OnUIButtonSelected = delegate {};

    public void OnSelect(BaseEventData eventData) {
        OnUIButtonSelected.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        OnUIButtonSelected.Invoke();
    }
    
    public void OnSubmit(BaseEventData eventData) {
        OnUIButtonPressed.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData) {
        OnUIButtonPressed.Invoke();
    }
}
