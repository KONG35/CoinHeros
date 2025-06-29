using UnityEditor;
using UnityEngine;

public class HexCoinPlacerWindow : EditorWindow
{
    GameObject prefab;
    int coinsPerLayer = 10;
    int layerCount = 3;
    Transform parent;
    float spacingMultiplier = 1.0f;

    [MenuItem("Tools/육각 구조 Coin 배치")]
    public static void ShowWindow()
    {
        GetWindow<HexCoinPlacerWindow>("육각 구조 Coin 배치");
    }

    void OnGUI()
    {
        GUILayout.Label("육각 구조 Coin 배치", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        coinsPerLayer = EditorGUILayout.IntField("한 층당 배치 개수", coinsPerLayer);
        layerCount = EditorGUILayout.IntField("층 수", layerCount);
        spacingMultiplier = EditorGUILayout.Slider("Spacing Multiplier", spacingMultiplier, 1f, 2f);
        parent = (Transform)EditorGUILayout.ObjectField("부모 오브젝트", parent, typeof(Transform), true);

        EditorGUILayout.Space();
        if (GUILayout.Button("배치하기")) PlaceObjects();
        if (GUILayout.Button("모두 삭제")) ClearAll();
    }

    void PlaceObjects()
    {
        if (prefab == null)
        {
            Debug.LogError("프리팹이 지정되지 않았습니다.");
            return;
        }

        // 오브젝트 크기 측정
        GameObject preview = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        float objectWidth = GetObjectWidth(preview);
        float objectDepth = GetObjectDepthZ(preview);
        DestroyImmediate(preview);

        Vector3 baseCenter = parent != null ? parent.position : Vector3.zero;

        for (int layer = 0; layer < layerCount; layer++)
        {
            // 층 그룹
            GameObject layerGroup = new GameObject($"Layer_{layer}");
            if (parent != null) layerGroup.transform.SetParent(parent);
            // Y축 위치
            layerGroup.transform.position = baseCenter + Vector3.up * (layer * objectDepth);
            // 10°씩 회전 오프셋
            layerGroup.transform.rotation = Quaternion.Euler(0, layer * 10f, 0);

            int placed = 0;
            int ring = 0;
            while (placed < coinsPerLayer)
            {
                int countInRing = (ring == 0) ? 1 : 6 * ring;
                float radius = objectWidth * spacingMultiplier * ring;
                float angleStep = 360f / countInRing;

                for (int i = 0; i < countInRing && placed < coinsPerLayer; i++)
                {
                    float angleDeg = angleStep * i;
                    float rad = Mathf.Deg2Rad * angleDeg;

                    Vector3 localPos = new Vector3(
                        Mathf.Cos(rad) * radius,
                        0f,
                        Mathf.Sin(rad) * radius
                    );

                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    obj.transform.SetParent(layerGroup.transform);
                    obj.transform.localPosition = localPos;
                    obj.transform.localRotation = Quaternion.identity;

                    Undo.RegisterCreatedObjectUndo(obj, "Place Coin Object");
                    placed++;
                }
                ring++;
            }
        }
    }

    void ClearAll()
    {
        if (parent == null)
        {
            Debug.LogWarning("부모 오브젝트가 지정되지 않았습니다.");
            return;
        }
        for (int i = parent.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(parent.GetChild(i).gameObject);
    }

    float GetObjectWidth(GameObject go)
    {
        Bounds b = new Bounds(go.transform.position, Vector3.zero);
        foreach (var r in go.GetComponentsInChildren<Renderer>())
            b.Encapsulate(r.bounds);
        return Mathf.Max(b.size.x, b.size.z);
    }

    float GetObjectDepthZ(GameObject go)
    {
        Bounds b = new Bounds(go.transform.position, Vector3.zero);
        foreach (var r in go.GetComponentsInChildren<Renderer>())
            b.Encapsulate(r.bounds);
        return b.size.z;
    }
}
