using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyBattleUnitItem : MonoBehaviour
{
    public CharacterData unitData;
    public RawImage UnitImage;

    public void SetData(CharacterData data)
    {
        unitData = data;
        if (unitData == null)
        {
            UnitImage.gameObject.SetActive(false);
            return;
        }
        UnitImage.texture = unitData.Image;
        UnitImage.gameObject.SetActive(true);
    }


}
