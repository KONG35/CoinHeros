using UnityEngine;
using CoinHeros;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public bool isEquipped;
    public CharacterData equippedBy; // 어떤 캐릭터가 장착하고 있는지
} 