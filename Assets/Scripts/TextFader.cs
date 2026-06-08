using System.Collections;
using UnityEngine;
using TMPro; // Required for TextMeshPro

public class TextFader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI myText;
    [SerializeField] private float fadeDuration = 2f;

    void Start()
    {
        // Start the fade-in effect on game start
        StartCoroutine(FadeInText());
    }

    public IEnumerator FadeInText()
    {
        float currentTime = 0f;
        Color originalColor = myText.color;

        // Ensure the text starts fully transparent
        myText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            // Smoothly calculate alpha between 0 (transparent) and 1 (opaque)
            float newAlpha = Mathf.Lerp(0f, 1f, currentTime / fadeDuration);
            myText.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            yield return null;
        }
    }
} 