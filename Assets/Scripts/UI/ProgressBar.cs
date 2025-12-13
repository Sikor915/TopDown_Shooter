using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] Image progressImage;
    [SerializeField] float defaultFillSpeed = 1f;
    [SerializeField] UnityEvent<float> onProgress;
    [SerializeField] UnityEvent onComplete;

    Coroutine fillCoroutine;

    public void SetProgress(float targetFillAmount)
    {
        SetProgress(targetFillAmount, defaultFillSpeed);
    }

    public void SetProgress(float targetFillAmount, float fillSpeed, bool shouldReset = true)
    {
        if (targetFillAmount < 0f || targetFillAmount > 1f)
        {
            Debug.LogError("Target fill amount must be between 0 and 1.");
            targetFillAmount = Mathf.Clamp01(targetFillAmount);
        }
        if (shouldReset) progressImage.fillAmount = 0f;
        if (targetFillAmount != progressImage.fillAmount)
        {
            if (fillCoroutine != null)
            {
                StopCoroutine(fillCoroutine);
            }
            fillCoroutine = StartCoroutine(FillProgress(targetFillAmount, fillSpeed));
        }
    }

    IEnumerator FillProgress(float targetFillAmount, float fillSpeed)
    {
        float time = 0;
        float initialFillAmount = progressImage.fillAmount;

        while (time < 1f)
        {
            progressImage.fillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, time);
            time += Time.deltaTime * fillSpeed;

            onProgress?.Invoke(progressImage.fillAmount);
            yield return null;
        }

        progressImage.fillAmount = targetFillAmount;
        onProgress?.Invoke(progressImage.fillAmount);
        onComplete?.Invoke();
    }
}
