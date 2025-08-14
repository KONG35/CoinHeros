using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitInfoPanel : MonoBehaviour
{
    public List<BattleCharacterPanelItem> Items;


    public void SetItemList(CharacterBase[] Datas)
    {
        for(int i=0;i<Datas.Length;i++)
        {
            if(Datas[i]==null)
            {
                Items[i].gameObject.SetActive(false);
                continue;
            }
            Items[i].Init(Datas[i]);
            Items[i].gameObject.SetActive(true);
        }
    }
    public void SetItemList(CharacterBase Data,int index)
    {
        Items[index].Init(Data);
        Items[index].gameObject.SetActive(true);
    }

    public void UpdateItemList(CharacterBase[] Datas)
    {
        for(int i=0;i<Datas.Length;i++)
        {
            if(Datas[i]==null)
            {
                continue;
            }
            Items[i].Refresh(Datas[i]);
        }
    }
    public void UpdateItemList(CharacterBase Data,int index)
    {
        Items[index].Refresh(Data);
    }
    
}
