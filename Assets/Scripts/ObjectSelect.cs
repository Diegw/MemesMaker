using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MEC;

public class ObjectSelect : MonoBehaviour
{
    public static ObjectSelect Instance => _instance;
    public GameObject ObjectSelected => _objectSelected;

    private static ObjectSelect _instance = null;
    [SerializeField] private GameObject _objectSelected = null;

    public void Construct()
    {
        if(_instance != null)
        {
            Destroy(_instance);
        }
        _instance = this;
        DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
        ObjectSelectable.OnSelectEvent += SetObjectSelected;
    }

    private void OnDisable()
    {
        ObjectSelectable.OnSelectEvent -= SetObjectSelected;
    }

    private void SetObjectSelected()
    {
        _objectSelected = EventSystem.current.currentSelectedGameObject;
    }

    public void SelectObject(GameObject gameObject)
    {
        Timing.RunCoroutine( SelectObject_Coroutine(gameObject) );
    }

    private IEnumerator<float> SelectObject_Coroutine(GameObject gameObject)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return Timing.WaitForOneFrame;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}