using UnityEngine;
using System.Collections;

public class PopupController : MonoBehaviour
{
       
    public float fadeDuration = 0.5f;     
    public float autoHideDelay = 0f;       

    private CanvasGroup canvasGroup;
    private Coroutine currentRoutine;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

       
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

   
    
    public void ShowPopup()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(Fade(0f, 1f, () =>
        {
            if (autoHideDelay > 0f)
                Invoke(nameof(HidePopup), autoHideDelay);
        }));
    }

    public void HidePopup()
    {
        if (currentRoutine != null)
             StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(Fade(1f, 0f, () =>
        {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        }));
    }


    
    private IEnumerator Fade(float from, float to, System.Action onComplete = null)
    {
        float elapsed = 0f;
        canvasGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = to;
        onComplete?.Invoke();
        currentRoutine = null;
    }
}
