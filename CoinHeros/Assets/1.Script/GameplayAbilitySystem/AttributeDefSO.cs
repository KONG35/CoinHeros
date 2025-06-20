using UnityEngine;
[CreateAssetMenu(menuName = "GAS/Attribute")]
public class AttributeDefSO : ScriptableObject
{
    public string Name;
    public float DefaultValue;
    public float MinValue;
    public float MaxValue;
    public StackPolicy stackingPolicy;
}