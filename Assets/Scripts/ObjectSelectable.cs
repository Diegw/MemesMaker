using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectSelectable : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public static Action OnSelectEvent;
    public static Action OnDeselectEvent;

    public void OnSelect(BaseEventData eventData)
    {
        OnSelectEvent?.Invoke();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnDeselectEvent?.Invoke();
    }
}