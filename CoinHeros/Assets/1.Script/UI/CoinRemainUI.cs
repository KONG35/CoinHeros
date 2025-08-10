using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CoinRemainUI: MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI numTxt;
    [SerializeField]
    private CoinRemainItem[] itemGroup;
    private CoinSpawnManager coinSpawnManager;
    // Start is called before the first frame update
    void Start()
    {
        coinSpawnManager = CoinSpawnManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        numTxt.text = coinSpawnManager.remainCoinCount.ToString() + "/" +coinSpawnManager.maxCoinCount.ToString();
    }
}
