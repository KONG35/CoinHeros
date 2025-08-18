using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleUIManager : Singleton<BattleUIManager>
{
    [Header("UI 패널들")]
    public BattleUnitInfoPanel UnitPanel;
    public BattleUnitInfoPanel MonsterPanel;
    public BattleRewardUI RewardPopupPanel;
    
    protected override void Awake()
    {
        base.isDone = false;
        base.Awake();
    }
    
    /// <summary>
    /// 특정 타입의 객체를 찾아서 반환
    /// </summary>
    public T FindObject<T>() where T : MonoBehaviour
    {
        T foundObject = FindObjectOfType<T>();
        if (foundObject != null)
        {
            Debug.Log($"BattleUIManager: {typeof(T).Name} 찾음");
        }
        else
        {
            Debug.LogWarning($"BattleUIManager: {typeof(T).Name}를 찾을 수 없음");
        }
        return foundObject;
    }
    
    /// <summary>
    /// 모든 특정 타입의 객체를 찾아서 반환
    /// </summary>
    public T[] FindAllObjects<T>() where T : MonoBehaviour
    {
        T[] foundObjects = FindObjectsOfType<T>();
        Debug.Log($"BattleUIManager: {typeof(T).Name} {foundObjects.Length}개 찾음");
        return foundObjects;
    }
    
    public void RewardAction()
    {
        RewardPopupPanel.gameObject.SetActive(true);
    }
}
