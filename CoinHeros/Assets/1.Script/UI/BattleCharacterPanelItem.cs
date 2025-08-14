using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleCharacterPanelItem : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI HP;
    public TextMeshProUGUI Coin;
    public RawImage Icon;
    public Image Hpimage;
    public Image Mpimage;

    

    public void Init(CharacterBase character)
    {
        var GasData = GASAttributeData.Instance;
        Name.text = character._name;
        var curhp = character.GetState(GasData.HP);
        var maxhp = character.GetState(GasData.MaxHP);
        HP.text = curhp.ToString("0")+ "/" + maxhp.ToString("0") ;
        Coin.text = character.GetState(GasData.ActionCoin).ToString("0")+ "/" + character.GetState(GasData.MaxActionCoin).ToString("0") ;
        Icon.texture = character.Image;
        Hpimage.fillAmount = curhp/maxhp;
        Mpimage.fillAmount = character.GetState(GasData.MP)/character.GetState(GasData.MaxMP);

        var isMonster = character as MonsterData;
        if(isMonster!=null)
            Coin.transform.parent.gameObject.SetActive(false);
        else
            Coin.transform.parent.gameObject.SetActive(true);
    }

    public void Refresh(CharacterBase character)
    {
        var GasData = GASAttributeData.Instance;
        var curhp = character.GetState(GasData.HP);
        var maxhp = character.GetState(GasData.MaxHP);
        Coin.text = character.GetState(GasData.ActionCoin).ToString("0")+ "/" + character.GetState(GasData.MaxActionCoin).ToString("0") ;
        HP.text = curhp.ToString("0")+ "/" + maxhp.ToString("0") ;
        Hpimage.fillAmount = curhp/maxhp;
        Mpimage.fillAmount = character.GetState(GasData.MP)/character.GetState(GasData.MaxMP);
    }
}
