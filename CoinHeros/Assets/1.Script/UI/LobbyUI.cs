using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public List<Transform> UIObject;
    public List<RectTransform> ObjectUIPanel;

    Camera cam;
    LobbyObjectClick objectUI;

    public TextMeshProUGUI TextMoney;

    public LobbyBattleUI BattleUI;
    public LobbyUnitListUI UnitListUI;
    public CharacterUI UnitUI;
    public LobbyUnitShopUI UnitShopUI;

    public Button BackBtn;

    public eUIStep curStep;
    public List<UIPanelStep> Step;

    public void Awake()
    {
        cam = Camera.main; PosInit();
        objectUI = FindObjectOfType<LobbyObjectClick>();
        BackBtn.onClick.AddListener(btnBack);

        btnBack();
        UnitShopUI.Init();
        UnitListUI.Init();
    }
    public void OnEnable()
    {
        TextMoney.text = UserData.Instance.Gold.ToString();
    }

    public void PosInit()
    {
        for(int i=0;i<UIObject.Count;i++)
        {
            float Y = UIObject[i].GetComponent<MeshFilter>().mesh.bounds.size.y;
            Vector3 pos = UIObject[i].position;
            pos.y += Y*0.75f;
            ObjectUIPanel[i].position = cam.WorldToScreenPoint(pos);
        } 
    }

    public void btnBack()
    {
        BackBtn.gameObject.SetActive(false);
        BattleUI.gameObject.SetActive(false);
        UnitListUI.gameObject.SetActive(false);
        UnitUI.gameObject.SetActive(false);
        UnitShopUI.gameObject.SetActive(false);
        objectUI.enabled = true;
    }
    [System.Serializable]
    public struct UIPanelStep
    {
        public string Name;
        public List<GameObject> panels;
        public List<Transform> pos;
    }

    public void SetUIStep(eUIStep step)
    {
        curStep = step;
        if (Step.Count <= (int)step)
            return;
        var items = Step[(int)step];

        for(int i=0;i<items.panels.Count;i++)
        {
            items.panels[i].gameObject.SetActive(true);
            items.panels[i].transform.position = items.pos[i].position;
        }
        BackBtn.gameObject.SetActive(true);
        objectUI.enabled = false;
    }

    public enum eUIStep
    {
        UnitState,
        UnitShop,
        Battle,
        ItemShop,
        Quest,
        Attribute,
        Forge,

        Count
    }
}
