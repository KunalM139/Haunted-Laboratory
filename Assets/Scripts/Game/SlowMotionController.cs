using System.Collections;
using UnityEngine;

public class SlowMotionController : MonoBehaviour
{
    public float slowMotionTimeScale = 0.25f;
    public float slowMotionDuration = 2f;

    public void TriggerSlowMotion()
    {
        StartCoroutine(SlowMotionRoutine());
    }

    private IEnumerator SlowMotionRoutine()
    {
        Time.timeScale = slowMotionTimeScale;
        
        if (AudioManager.Instance != null && AudioManager.Instance.alarmSound != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.alarmSound);

        yield return new WaitForSecondsRealtime(slowMotionDuration);
        
        Time.timeScale = 1f;
        GameManager.Instance.Victory();
    }
}
