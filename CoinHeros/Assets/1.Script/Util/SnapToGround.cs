using JetBrains.Annotations;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToGround : MonoBehaviour
{
    [Button]
    public void Snep()
    {
        var obj = Physics.RaycastAll(transform.position + Vector3.up*100f,Vector3.down,1000f);
        float y = float.MinValue;
        foreach(var o in obj)
        {
            if (y < o.point.y)
                y = o.point.y;
        }
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
