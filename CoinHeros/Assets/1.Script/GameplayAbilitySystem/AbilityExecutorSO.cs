using UnityEngine;

public abstract class AbilityExecutorSO : ScriptableObject
{
    public abstract void Execute(AbilityContext context);
}
public class AbilityContext
{
    public GameObject Caster;
    public int AbilityLevel;
    public AbilityDefSO Definition;
    public GASAttributeSetComponent Attributes;
    public GASTagComponent Tags;
    //public ObjectPool<ProjectlieObject> Pool;
}