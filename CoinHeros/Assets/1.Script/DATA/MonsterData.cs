using NaughtyAttributes;
using System.Collections.Generic;
using UnityEditor;

public class MonsterData : CharacterBase
{
    public MonsterAttackType AttackType;
    private void Awake()
    {
    }
    protected override void Start()
    {
        base.Start();
    }

    [Button]
    public void AddGASBaseAttribute()
    {
        string[] guids = AssetDatabase.FindAssets("t:AttributeDefSO");
        if (_state == null)
            _state = GetComponent<GASAttributeSetComponent>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            AttributeDefSO so = AssetDatabase.LoadAssetAtPath<AttributeDefSO>(path);

            if (so != null)
            {
                _state.AddDefinitionAttribute(so);
            }
        }
    }


    public enum MonsterAttackType{
        Melee,
        Magic,
        Count
    }
}
