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
    public void Pop(int n)
    {
        itemGroup[n].Pop();
    }
    public IEnumerator SetItemGroupCor(List<CoinEnum> cEnumList)
    {
        for(int i=0;i<cEnumList.Count;i++)
        {
            int n = (int)cEnumList[i];
            itemGroup[i].SetSprite(coinSp[n]);
            yield return new WaitForSeconds(0.2f);
        }
        yield return null;
    }
}
