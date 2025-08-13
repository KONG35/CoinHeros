using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BonusRoulette : MonoBehaviour
{
    [SerializeField]
    private CoinLaunchMachine coinMachine;

    [SerializeField]
    private BonusUIDataSO[] bonusUIDataSO;

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
    private BonusManager bonusManager;
    private RouletteManager rouletteManager;
    private void Start()
    {
        bonusManager = BonusManager.Instance;
        rouletteManager = RouletteManager.Instance;
        head = 0;
        selectIdx = 2;
        intervalVec = items[1].recTr.position - items[0].recTr.position;
        for(int i=0;i<items.Length;i++)
        {
            items[i].SetIndex(i);
        }
    }
    public IEnumerator SpinCor(CoinEnum _cEnum)
    {
        SpinInit(_cEnum);
        float period = UnityEngine.Random.Range(3f, 3.5f);

        remainCntTxt.text = "";

        yield return StartCoroutine(RemainCor());

        remainCntTxt.text = rouletteManager.remainBonusCnt.ToString();

        yield return StartCoroutine(SpinCor(period, Mathf.Abs(targetTr.position.x - arrowRectTr.position.x)));

        ShowBonus(items[selectIdx].bonus);
    }
    private void SpinInit(CoinEnum _cEnum)
    {
        // ex. copper coin : Coin3 50% , Coin6 30% , Coin9  10%, earthQuake 10%
        int addIdx = 32;
        List<BonusUIDataSO> tempList = bonusUIDataSO.Where(x => x.appearCoinEnum <= _cEnum).ToList();

        // basicPercent /totalPercent 확률
        float totalPercent = 0f;
        foreach(var t in tempList)
        {
            totalPercent += t.basicPercent;
        }

        for (int i = selectIdx + 3; i < addIdx + 3; i++)
        {
            int n = (i) % items.Length;
            float percent = UnityEngine.Random.Range(0f, totalPercent);
            float tempPercent = 0f;
            for(int j=0;j<tempList.Count;j++)
            {
                tempPercent += tempList[j].basicPercent;
                if (percent < tempPercent)
                {
                    items[n].SetBonus(tempList[j]);
                    break;
                }
            }
        }
        
        selectIdx = (selectIdx + addIdx) % items.Length; 
        targetTr = items[selectIdx].recTr;
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

            // Ŀ�� ����: ó�� ������ �� �̸����� ���� ���� �� ������ ����
            float ease = 1f - Mathf.Pow(1f - t, 2f); // Ease-out cubic

            float targetMoved = len * ease;
            float delta = targetMoved - prevMoved;
            prevMoved = targetMoved;

            if (targetTr.position.x - arrowRectTr.position.x <= margin)
            {
                break;
            }

            // ������������� ��ӵ�
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
    /// remain image ������ �ڷ�ƾ
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
            remainImgTr.rotation *= Quaternion.Euler(0f, 0f, deltaAngle);  // ���� ȸ��
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    public void SetRemainCnt()
    {
        remainCntTxt.text = RouletteManager.Instance.remainBonusCnt.ToString();
    }
    [Button]
    private void ShowBonus(BonusEnum _bonus)
    {
        switch(_bonus)
        {
            case BonusEnum.Coin3:
            case BonusEnum.Coin6:
            case BonusEnum.Coin9:
                {
                    coinMachine.ShowBonusCoin(_bonus, CoinEnum.Gold);
                }
                break;
            default:
                {
                    bonusManager.Show(_bonus);
                }
                break;
        }
    }
}
