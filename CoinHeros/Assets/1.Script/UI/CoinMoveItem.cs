using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinMoveItem : MonoBehaviour
{
    private Vector3 targetPos;
    private RectTransform rect;
    public void Move(Vector2 s, Vector2 t)
    {
        rect = gameObject.GetComponent<RectTransform>();
        rect.anchoredPosition = s;
        targetPos = t;
        //targetPos = new Vector3(t.x, t.y, 0f);
        gameObject.SetActive(true);
        StartCoroutine(MoveCor());
    }
    IEnumerator MoveCor()
    {
        float elapsed = 0f;
        float duration = 0.5f;
        while(elapsed<duration)
        {
            float t = elapsed / duration;
            rect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            
            yield return new WaitForFixedUpdate();
            elapsed += Time.fixedDeltaTime;
        }
        yield return null;

        elapsed = 0f;
        duration = 2f;

        Vector2 startPos = rect.anchoredPosition;
        
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            
            yield return new WaitForFixedUpdate();
            elapsed += Time.fixedDeltaTime;
        }
        rect.anchoredPosition = targetPos;
        yield return null;
    }
}
