using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
public class CoinRemainItem : MonoBehaviour
{
    [SerializeField]
    private Image image;
    private RectTransform rect;
    private Vector2 originPos;
    private Vector2 movePos = new Vector2(100f, 200f);
    [SerializeField]
    private float time = 0.25f;
    
    private void Awake()
    {
        rect = gameObject.GetComponent<RectTransform>();
        originPos = rect.anchoredPosition;
    }
    public void SetSprite(Sprite _sp)
    {
        image.sprite = _sp;
        rect.anchoredPosition = originPos;
    }
    public void Pop()
    {
        StartCoroutine(PopCor());
    }
    IEnumerator PopCor()
    {
        Vector2 targetPos = originPos + movePos;
        Vector2 controlPoint = originPos + new Vector2(movePos.x * 0.5f, movePos.y + 50f); // 제어점 (중간 높이)
        
        float elapsed = 0f;
        while(elapsed < time)
        {
            float t = elapsed / time;
            Vector2 vec = BezierCurve(originPos, controlPoint, targetPos, t);
            rect.anchoredPosition = vec;
            elapsed += Time.fixedDeltaTime;
            yield return null;
        }
        
        rect.anchoredPosition = targetPos;
    }
    
    // 2차 베지어 곡선
    private Vector2 BezierCurve(Vector2 start, Vector2 control, Vector2 end, float t)
    {
        float u = 1f - t;
        return u * u * start + 2f * u * t * control + t * t * end;
    }
    [Button]
    private void EditorImageSet()
    {
        image = gameObject.GetComponent<Image>();
    }
}
