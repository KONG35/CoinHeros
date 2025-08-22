using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;
public class CoinRemainUI: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numTxt;
    [SerializeField] private Sprite[] coinSp;
    [SerializeField] private CoinRemainItem[] itemGroup;
    private CoinSpawnManager coinSpawnManager;
    public void Init()
    {
        coinSpawnManager = CoinSpawnManager.Instance;
        for(int i=0;i<itemGroup.Length;i++)
        {
            itemGroup[i].Init();

            if(i<coinSpawnManager.maxCoinCount)
                itemGroup[i].gameObject.SetActive(true);
            else
                itemGroup[i].gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(coinSpawnManager==null)
            return;
        numTxt.text = coinSpawnManager.remainCoinCount.ToString() + "/" +coinSpawnManager.maxCoinCount.ToString();
    }
    public IEnumerator PopCor(int n)
    {
        yield return StartCoroutine(itemGroup[n].PopCor());
    }
    public IEnumerator SetItemGroupCor(List<CoinEnum> cEnumList, bool isAnim = true)
    {
        for(int i=0;i<cEnumList.Count;i++)
        {
            int n = (int)cEnumList[i];
            itemGroup[i].SetSprite(coinSp[n]);
            if(isAnim)
                yield return new WaitForSeconds(0.2f);
        }
        yield return null;
    }
}
