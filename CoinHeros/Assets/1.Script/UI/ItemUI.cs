using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CoinHeros;

public class ItemUI : MonoBehaviour
{
    public Image BackGround;
    public RawImage Icon;
    public TextMeshProUGUI Count;

    public void SetItemData(ItemData Data)
    {
        Icon.texture =  Data.icon.texture;
    }
}
