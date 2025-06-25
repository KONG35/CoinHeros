using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BonusRoulette : MonoBehaviour
{
    [SerializeField]
    private BonusInfo[] bonusInfos;

    [SerializeField]
    private BonusItem[] items;

    [SerializeField]
    private RectTransform arrowRectTr;

    [SerializeField]
    private TextMeshProUGUI remainCntTxt;

    [SerializeField]
    private RectTransform remainImgTr;

    [SerializeField]
    private int selectIdx;

    private RectTransform targetTr;
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
    private void SpinInit(CoinEnum _cEnum)
    {
        // (select idx+3) 부터 (add Idx+3) 까지 coin 등급에 맞는 아이템 set
        // ex. copper coin : Coin3 60% , Coin6 30% , Coin9  10%
        int addIdx = UnityEngine.Random.Range(30, 35);
        switch (_cEnum)
        {
            case CoinEnum.Copper:
            case CoinEnum.Silver:
                {
                    for(int i = selectIdx + 3; i< addIdx + 3; i++)
                    {
                        int n = (i) % items.Length;
                        float temp = UnityEngine.Random.Range(0f, 100f);
                        if (temp < 60f)
                        {
                            items[n].SetBonus(bonusInfos[0]);
                            
                        }
                        else if (temp < 90f)
                        {
                            items[n].SetBonus(bonusInfos[1]);
                        }
                        else
                        {
                            items[n].SetBonus(bonusInfos[2]);
                        }
                    }
                }
                break;
            case CoinEnum.Gold:
                break;
            case CoinEnum.Diamond:
                break;
            default:
                break;

        }
        selectIdx = (selectIdx + addIdx) % items.Length; 
        targetTr = items[selectIdx].recTr;
    }
    public IEnumerator SpinCor(CoinEnum _cEnum)
    {
        SpinInit(_cEnum);
        float period = UnityEngine.Random.Range(3f, 3.5f);

        remainCntTxt.text = "";

        yield return StartCoroutine(RemainCor());

        remainCntTxt.text = RouletteManager.Instance.remainBonusCnt.ToString();

        yield return StartCoroutine(SpinCor(period, Mathf.Abs(targetTr.position.x - arrowRectTr.position.x)));
    }
    private IEnumerator SpinCor(float duration, float len)
    {
        float elapsed = 0f;
        float margin = UnityEngine.Random.Range(-1f, 1f);
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
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

    }
    /// <summary>
    /// remain image 돌리는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator RemainCor()
    {
        float elapsed = 0f;
        float duration = 0.6f;
        float anglePerSecond = 720f / duration;

        while (elapsed < duration)
        {
            float deltaAngle = anglePerSecond * Time.deltaTime;
            remainImgTr.rotation *= Quaternion.Euler(0f, 0f, deltaAngle);  // 누적 회전
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    public void SetRemainCnt()
    {
        remainCntTxt.text = RouletteManager.Instance.remainBonusCnt.ToString();
    }
}
