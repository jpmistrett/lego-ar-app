using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimedDisappear : MonoBehaviour
{
    
    public enum fadeType {fadeIn, fadeOut};

    public fadeType fadeTypeOption;
    public bool fireOnAwake = true;
    public float initialPause;
    public float seconds;
    public Image image;
    public SVGImage svgImage;
    public TextMeshProUGUI text;

    void Awake()
    {
        if (fireOnAwake)
        {
            if (fadeTypeOption == fadeType.fadeOut)
            {
                DeactivateMe();
            }
        }
    }

    public void DeactivateMe ()
    {
        StartCoroutine(FadeTo(0.0f, seconds));
    }

    IEnumerator RemoveAfterSeconds (float seconds){
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }

    IEnumerator FadeTo(float aValue, float aTime)
    {
        yield return new WaitForSeconds(initialPause);
        
        StartCoroutine(RemoveAfterSeconds(seconds));

        if (image != null)
        {
            float alpha = image.color.a;
            float r = image.color.r;
            float g = image.color.g;
            float b = image.color.b;

            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                Color newColor = new Color(r, g, b, Mathf.Lerp(alpha, aValue, t));
                
                image.color = newColor;

                    if (text != null)
                {
                    text.color = newColor;
                }
                yield return null;
            }
        }

        if (svgImage != null)
        {
            float alpha = svgImage.color.a;
            float r = svgImage.color.r;
            float g = svgImage.color.g;
            float b = svgImage.color.b;
            
            float textAlpha = text.color.a;
            float textR = text.color.r;
            float textG = text.color.g;
            float textB = text.color.b;
            
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                Color newColor = new Color(r, g, b, Mathf.Lerp(alpha, aValue, t));
                Color newTextColor = new Color(textR, textG, textB, Mathf.Lerp(textAlpha, aValue, t));
                
                svgImage.color = newColor;

                if (text != null)
                {
                    text.color = newTextColor;
                }

                yield return null;
            }
        }
    }
}
