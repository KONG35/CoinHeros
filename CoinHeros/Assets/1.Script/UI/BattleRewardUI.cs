using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleRewardUI : MonoBehaviour
{
    public TextMeshProUGUI TitleText;

    public TextMeshProUGUI RewardGoldText;
    public TextMeshProUGUI RewardExpText;
    public TextMeshProUGUI MaxFloor;

    public List<ItemUI> bootyList;
    public ItemUI ItemPrefab;

    public Button ExitBtn;


    public void Start()
    {
        ExitBtn.onClick.AddListener(OnclickExit);
    }

    public void OnEnable()
    {
        
    }

    public void OnclickExit()
    {
        SceneManager.Instance.LoadLobby();
    }
}
