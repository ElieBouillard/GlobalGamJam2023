using UnityEngine;

public class FreezeFrameManager : Singleton<FreezeFrameManager>
{
    private System.Collections.IEnumerator _freezeFrameCoroutine;

    public static bool IsFroze => Exists() && Instance._freezeFrameCoroutine != null;

    public static void FreezeFrame(int framesDelay, float duration, float targetTimeScale = 0f, bool overrideCurrentFreeze = false)
    {
        if (!Exists() || duration == 0f)
            return;

        if (Instance._freezeFrameCoroutine != null)
        {
            if (!overrideCurrentFreeze)
                return;

            Instance.StopCoroutine(Instance._freezeFrameCoroutine);
        }

        Instance.StartCoroutine(Instance._freezeFrameCoroutine = FreezeFrameCoroutine(framesDelay, duration, targetTimeScale));
    }

    private static System.Collections.IEnumerator FreezeFrameCoroutine(int framesDelay, float dur, float targetTimeScale = 0f)
    {
        for (int i = 0; i < framesDelay; ++i)
            yield return new WaitForEndOfFrame();

        Time.timeScale = targetTimeScale;
        yield return new WaitForSecondsRealtime(dur);
        Time.timeScale = 1f;

        Instance._freezeFrameCoroutine = null;
    }
}