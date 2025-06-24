using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BonusRoulette : MonoBehaviour
{
    [SerializeField]
    private BonusItem[] items;

    [SerializeField]
    private RectTransform arrowRectTr;

    private RectTransform targetTr;

    [SerializeField]
    private int selectIdx;

    private Vector3 intervalVec;
    private int head;
    private int tail => (head + items.Length-1) % items.Length;
    private void Start()
    {
        head = 0;
        selectIdx = 2;
        intervalVec = items[1].recTr.position - items[0].recTr.position;
        for(int i=0;i<items.Length;i++)
        {
            items[i].SetIndex(i);
        }
    }
    [Button]
    //public void Spin()
    //{
    //    int addIdx = Random.Range(30, 35);
    //    selectIdx = (selectIdx + addIdx) % items.Length;
    //    targetTr = items[selectIdx].recTr;
    //    float period = Random.Range(3f, 3.5f);

    //    StartCoroutine(SpinCor(period, Mathf.Abs(targetTr.position.x - arrowRectTr.position.x)));
    //}
    public IEnumerator SpinCor()
    {
        int addIdx = Random.Range(30, 35);
        selectIdx = (selectIdx + addIdx) % items.Length;
        targetTr = items[selectIdx].recTr;
        float period = Random.Range(3f, 3.5f);

        yield return StartCoroutine(SpinCor(period, Mathf.Abs(targetTr.position.x - arrowRectTr.position.x)));
    }
    private IEnumerator SpinCor(float duration, float len)
    {
        float elapsed = 0f;
        float margin = Random.Range(-1f, 1f);
        len += margin;
        float prevMoved = 0f;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);

            // 커브 적용: 처음 빠르고 → 미리부터 점점 감속 → 끝에서 정지
            float ease = 1f - Mathf.Pow(1f - t, 2f); // Ease-out cubic

            float targetMoved = len * ease;
            float delta = targetMoved - prevMoved;
            prevMoved = targetMoved;

            if (targetTr.position.x - arrowRectTr.position.x <= margin)
            {
                break;
            }

            // 가까워질때까지 등속도
            for (int i = 0; i < items.Length; i++)
            {
                items[i].recTr.position -= new Vector3(delta, 0f, 0f);
            }

            if (items[head].recTr.position.x <= -200f)
            {
                items[head].recTr.position = items[tail].recTr.position + intervalVec;
                head = (head + 1) % items.Length;
            }
            yield return new WaitForFixedUpdate();
            elapsed += Time.fixedDeltaTime;
        }

    }
}
