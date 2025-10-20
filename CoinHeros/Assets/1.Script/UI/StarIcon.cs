using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StarIcon : MonoBehaviour
{
    [SerializeField] Image starImg;
    [SerializeField] Image moveImg;

    private Vector3 originPos = Vector3.zero;
    private Vector3 targetPos = Vector3.zero;
    private Color originMoveImgColor;
    public void Set(Vector3 _target)
    {
        originPos = moveImg.rectTransform.position;
        targetPos = _target;
        starImg.color = Color.black;
        originMoveImgColor = moveImg.color;
        moveImg.gameObject.SetActive(false);
    }
    public void SetWhite()
    {
        starImg.color = Color.white;
    }
    public void Move()
    {
        starImg.color = Color.black;
        
        StopAllCoroutines();
        StartCoroutine(MoveCor());
    }
    IEnumerator MoveCor()
    {
        float duration = 0.4f;
        float elapsed = 0f;
        moveImg.gameObject.SetActive(true);

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);

            moveImg.rectTransform.position = Vector3.Lerp(originPos, targetPos, t);
            // 알파값은 점점 사라지게
            float alpha = 1f - t;
            moveImg.color = new Color(originMoveImgColor.r, originMoveImgColor.g, originMoveImgColor.b, alpha);
            yield return null;
            elapsed += Time.deltaTime;
        }
        moveImg.gameObject.SetActive(false);
        moveImg.rectTransform.position = originPos;
    }
}
