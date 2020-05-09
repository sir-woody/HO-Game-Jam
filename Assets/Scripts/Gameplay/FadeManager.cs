using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Rendering;
using UnityEngine;

public class FadeManager : MonoBehaviour
{
    [SerializeField]
    private float fadeDuration = 0.2f;
    [SerializeField]
    private CanvasGroup fadeCanvasGroup = null;

    public void SetFade(float fade)
    {
        fadeCanvasGroup.alpha = fade;
    }
    public IEnumerator FadeOut()
    {
        float duration = 0;
        while (duration < fadeDuration)
        {
            duration = Mathf.Min(duration + Time.deltaTime, fadeDuration);
            fadeCanvasGroup.alpha = duration / fadeDuration;
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        float duration = 0;
        while (duration < fadeDuration)
        {
            duration = Mathf.Min(duration + Time.deltaTime, fadeDuration);
            fadeCanvasGroup.alpha = 1 - (duration / fadeDuration);
            yield return null;
        }
    }
}
