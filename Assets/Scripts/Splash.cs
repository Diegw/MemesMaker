using UnityEngine;
using UnityEngine.UI;

public class Splash : MonoBehaviour
{
    private void OnEnable()
    {
        InputReceptor.OnAnyEvent += Continue;
    }

    private void OnDisable()
    {
        InputReceptor.OnAnyEvent -= Continue;
    }

    private void Continue()
    {
        ScenesManager.Instance.LoadScene("Menu");
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}