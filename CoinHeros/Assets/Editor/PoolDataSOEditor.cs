using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(PoolDataSO))]
public class PoolDataSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var data = target as PoolDataSO;

        if (data.prefab != null)
        {
            var comp = data.prefab.GetComponent<IPoolable>();
            if (comp != null)
            {
                //data.componentTypeName = comp.GetType().AssemblyQualifiedName;
                EditorGUILayout.HelpBox($"자동 인식된 타입: {comp.GetType().Name}", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("IPoolable을 구현한 컴포넌트가 없음", MessageType.Error);
            }
        }
    }
}

#endif