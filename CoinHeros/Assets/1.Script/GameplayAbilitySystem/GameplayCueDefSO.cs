using UnityEngine;

[CreateAssetMenu(menuName = "GAS/CueDefinition")]
public class GameplayCueDefSO : ScriptableObject
{
    public string cueTag;
    public GameObject cuePrefab;
    public bool attachToCaster;
    public Vector3 worldOffset;
}