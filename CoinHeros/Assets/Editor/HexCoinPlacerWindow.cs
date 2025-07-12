using UnityEditor;
using UnityEngine;

public class HexCoinPlacerWindow : EditorWindow
{
    GameObject prefab;
    int coinsPerLayer = 10;
    int layerCount = 3;
    Transform parent;
    float spacingMultiplier = 1.0f;

    // 중앙에서 시작하여 육각형 링 형태로 확장되는 계층적 배치 도구
    [MenuItem("Tools/육각 구조 링 형 Coin 배치")]
    public static void ShowWindow()
    {
        GetWindow<HexCoinPlacerWindow>("육각 구조 링 형 Coin 배치");
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
        if (GUILayout.Button("배치하기")) CoinMaker.PlaceHexObjects(prefab, parent, layerCount, coinsPerLayer, spacingMultiplier);
        if (GUILayout.Button("모두 삭제")) ClearAll();
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
