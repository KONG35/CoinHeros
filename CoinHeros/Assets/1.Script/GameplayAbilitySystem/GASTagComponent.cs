using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GASTagComponent : MonoBehaviour
{
    public HashSet<string> Tags;
    const string StatePrefix = "State.";
    public void SetState(string state)
    {
        Tags.RemoveWhere(t => t.StartsWith(StatePrefix));
        Tags.Add(StatePrefix + state);
    }
    public bool HasAll(IEnumerable<string> must) => must.All(t => Tags.Contains(t));
    public bool HasAny(IEnumerable<string> any) => any.Any(t => Tags.Contains(t));
}
