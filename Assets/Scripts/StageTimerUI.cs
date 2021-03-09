using UnityEngine;
using TMPro;

public class StageTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerDisplay = null;

    private void OnEnable()
    {
        Match.OnStageTimeUpdated += UpdateTimerDisplay;
        Tournament.OnUpdateBattleTimer += UpdateTimerDisplay;
    }

    private void OnDisable()
    {
        Match.OnStageTimeUpdated -= UpdateTimerDisplay;
        Tournament.OnUpdateBattleTimer -= UpdateTimerDisplay;
    }

    private void UpdateTimerDisplay(float currentTime)
    {
        if(_timerDisplay == null)
        {
            Debug.LogError("TimerDisplay is null");
            return;
        }
        _timerDisplay.text = currentTime.ToString();
    }
}