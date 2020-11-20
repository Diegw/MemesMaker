using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MEC;

public class ObjectSelect : MonoBehaviour
{
    public static ObjectSelect Instance => _instance;
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

    public void SelectObject(GameObject gameObject)
    {
        Timing.RunCoroutine( SelectObject_Coroutine(gameObject) );
    }

    public IEnumerator<float> SelectObject_Coroutine(GameObject gameObject)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return Timing.WaitForOneFrame;
        EventSystem.current.SetSelectedGameObject(gameObject);
        _objectSelected = gameObject;
    }
}