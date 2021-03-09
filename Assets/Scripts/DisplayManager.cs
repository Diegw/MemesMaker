using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    [TabGroup("REFERENCES")][SerializeField] private TextMeshProUGUI _vsDisplay = null;

    private void OnEnable()
    {
        Stage.OnVsDisplayHide += Hide;
    }
    
    private void OnDisable()
    {
        Stage.OnVsDisplayHide -= Hide;
    }

    private void Hide()
    {
        _vsDisplay.enabled = false;
    }

    private void OnDestroy()
    {
        Stage.OnVsDisplayHide -= Hide;
    }
}