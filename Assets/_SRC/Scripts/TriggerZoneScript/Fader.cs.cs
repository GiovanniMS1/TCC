using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class Fader : MonoBehaviour
{
    public Tilemap tilemap;
    public float fadeDuration = 0.5f;

    public void Fade(bool fadeDirection)
    {
        float targetAlpha = fadeDirection ? 1f : 0f;
        StartCoroutine(FadeCoroutine(targetAlpha));
    }

    private IEnumerator FadeCoroutine(float targetAlpha)
    {
        // Pega a cor inicial do Tilemap
        Color startColor = tilemap.color;
        float startAlpha = startColor.a;
        
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            
            tilemap.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            
            yield return null;
        }
        
        // Assegura que o valor final seja exatamente o targetAlpha
        tilemap.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }
}