using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GASCueComponent : MonoBehaviour
{
    [SerializeField] List<GameplayCueDefSO> cueDefinitions;
    Dictionary<string, GameplayCueDefSO> cueDict;
    void Awake() => cueDict = cueDefinitions.ToDictionary(c => c.cueTag);
    public void TriggerCue(string cueTag)
    {
        if (!cueDict.TryGetValue(cueTag, out var def)) return;
        var pos = def.attachToCaster ? transform.position : transform.position + def.worldOffset;
        Instantiate(def.cuePrefab, pos, Quaternion.identity);
    }
}
