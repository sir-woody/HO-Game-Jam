using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FadeManager : Singleton<FadeManager>
{
    [SerializeField]
    private float fadeDuration = 0.2f;
    [SerializeField]
    private CanvasGroup fadeCanvasGroup = null;

    public float FadeDuration
    {
        get
        {
            return this.fadeDuration;
        }

        set
        {
            this.fadeDuration = value;
        }
    }

    public void SetFade(float fade)
    {
        fadeCanvasGroup.alpha = fade;
    }
    public IEnumerator FadeOut(float duration = -1)
    {
        if (duration == 0)
        {
            fadeCanvasGroup.alpha = 1;
            yield break;
        }
        float time = 0;
        if (duration < 0)
        {
            duration = FadeDuration;
        }
        while (time < duration)
        {
            time = Mathf.Min(time + Time.deltaTime, duration);
            fadeCanvasGroup.alpha = time / duration;
            yield return null;
        }
    }

    public IEnumerator FadeIn(float duration = -1)
    {
        if (duration == 0)
        {
            fadeCanvasGroup.alpha = 0;
            yield break;
        }
        float time = 0;
        if (duration < 0)
        {
            duration = FadeDuration;
        }
        while (time < duration)
        {
            time = Mathf.Min(time + Time.deltaTime, duration);
            fadeCanvasGroup.alpha = 1 - (time / duration);
            yield return null;
        }
    }
}
