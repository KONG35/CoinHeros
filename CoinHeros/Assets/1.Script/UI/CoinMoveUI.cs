using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinMoveUI : MonoBehaviour
{
    [SerializeField]
    private CoinMoveItem[] coinItemArr;
    [SerializeField]
    private Image area;
    [SerializeField]
    private Vector2[] targetArr;
    private float areaWidth;
    private float areaHeight;
    private Vector2 areaMinVec;
    private Vector2 areaMaxVec;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitAreaLate());
    }

    IEnumerator InitAreaLate()
    {
        yield return null; // 1프레임 대기 (UI 레이아웃 확정)
        SetArea();
    }

    void SetArea()
    {
        areaWidth = area.rectTransform.rect.width;
        areaHeight = area.rectTransform.rect.height;
        areaMinVec = new Vector3(
            area.rectTransform.anchoredPosition.x - areaWidth / 2f,
            area.rectTransform.anchoredPosition.y - areaHeight / 2f
        );
    }

    public void CoinMove(int coinIndex, int moveIndex, float posX)
    {
        float ratio = 1f - Mathf.Clamp01((posX + 33f) / 25f);
        Vector2 pos = areaMinVec + new Vector2(areaWidth * ratio, Random.Range(0f, areaHeight));
        CoinMoveItem c = GameObject.Instantiate(coinItemArr[coinIndex], gameObject.transform);
        c.Move(pos, targetArr[moveIndex]);
    }
}
