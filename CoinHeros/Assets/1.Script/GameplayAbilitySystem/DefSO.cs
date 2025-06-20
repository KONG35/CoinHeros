using System.Collections.Generic;
using UnityEngine;

public enum StackPolicy { Add, Multiply, Override }
[System.Serializable]
public struct AttributeCost
{
    public AttributeDefSO attribute;
    public float amount;
}
public abstract class DefinitionComponent<T> : MonoBehaviour
  where T : ScriptableObject
{
    [SerializeField] 
    protected List<T> definitions;
    protected virtual void Awake()
    {
        Initialize(definitions);
    }
    private void Initialize(List<T> defs)
    {
        OnInitialized(defs);
    }
    protected abstract void OnInitialized(List<T> defs);
}