using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StarImageEffect : MonoBehaviour
{
    [SerializeField] private Image glowImg; // Glow 전용 이미지
    [SerializeField] private float duration = 0.5f; // 전체 지속 시간
    [SerializeField] private float scaleAmount = 0.1f; // 커졌다 작아지는 크기
    
    private Image threestarImg;
    private Coroutine cor;
    private void Awake()
    {
        cor = null;
        threestarImg = gameObject.GetComponent<Image>();
    }
    public void Show()
    {
        StopCoroutine(cor);
        cor = StartCoroutine(GlowEffectCor());
    }

    private IEnumerator GlowEffectCor()
    {
        glowImg.gameObject.SetActive(true);
        Color baseColor = glowImg.color;
        Vector3 baseScale = Vector3.one;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 커졌다 작아지는 효과
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * scaleAmount;
            glowImg.transform.localScale = baseScale * scale;
            threestarImg.transform.localScale = Vector3.one * scale;

            // 알파값은 점점 사라지게
            float alpha = 1f - t;
            glowImg.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

            yield return null;
        }

        glowImg.gameObject.SetActive(false);
        glowImg.color = baseColor; // 색상 복구
        glowImg.transform.localScale = Vector3.one; // 스케일 복구
        threestarImg.transform.localScale = Vector3.one; // 스케일 복구
    }
}
