using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class GridCoinPlacerWindow : EditorWindow
{
    GameObject defaultPrefab;
    List<GameObject> additionalPrefabs = new List<GameObject>();
    List<int> additionalPrefabCounts = new List<int>();

    int rows = 5;
    int columns = 5;
    int layerCount = 2;
    Transform parent;
    float spacingMultiplier = 1.0f;

    //지그재그 방식의 육각 타일 그리드 배치 도구
    [MenuItem("Tools/지그재그 방식의 행열 Coin 배치")]
    public static void ShowWindow()
    {
        GetWindow<GridCoinPlacerWindow>("지그재그 방식의 행열 Coin 배치");
    }

    void OnGUI()
    {
        GUILayout.Label("지그재그 방식의 육각형 그리드 배치 도구", EditorStyles.helpBox);
        GUILayout.Label("행, 열 형태 헥사곤 패킹 (랜덤 프리팹 + 개수 제어)", EditorStyles.boldLabel);

        defaultPrefab = (GameObject)EditorGUILayout.ObjectField("기본 프리팹", defaultPrefab, typeof(GameObject), false);

        GUILayout.Space(5);
        int newCount = EditorGUILayout.IntField("추가 프리팹 개수", additionalPrefabs.Count);
        while (newCount > additionalPrefabs.Count)
        {
            additionalPrefabs.Add(null);
            additionalPrefabCounts.Add(0);
        }
        while (newCount < additionalPrefabs.Count)
        {
            additionalPrefabs.RemoveAt(additionalPrefabs.Count - 1);
            additionalPrefabCounts.RemoveAt(additionalPrefabCounts.Count - 1);
        }

        GUILayout.Space(10);
        for (int i = 0; i < additionalPrefabs.Count; i++)
        {
            additionalPrefabs[i] = (GameObject)EditorGUILayout.ObjectField($"추가 프리팹 {i + 1}", additionalPrefabs[i], typeof(GameObject), false);
            additionalPrefabCounts[i] = EditorGUILayout.IntField("생성 수", additionalPrefabCounts[i]);
            GUILayout.Space(5);
        }

        rows = EditorGUILayout.IntField("행 수", rows);
        columns = EditorGUILayout.IntField("열 수", columns);
        
        GUILayout.Space(5);
        layerCount = EditorGUILayout.IntField("층 수", layerCount);
        spacingMultiplier = EditorGUILayout.Slider("간격 배율", spacingMultiplier, 1f, 2f);
        parent = (Transform)EditorGUILayout.ObjectField("부모 오브젝트", parent, typeof(Transform), true);

        EditorGUILayout.Space();
        if (GUILayout.Button("배치하기")) PlaceHexGrid();
        if (GUILayout.Button("모두 삭제")) ClearAll();
    }

    void PlaceHexGrid()
    {
        List<AdditionalPrefab> addPrefabs = new List<AdditionalPrefab>();
        for(int i=0;i<additionalPrefabs.Count;i++)
        {
            addPrefabs.Add(new AdditionalPrefab(additionalPrefabs[i], additionalPrefabCounts[i]));
        }
        CoinMaker.PlaceGridObject(defaultPrefab, addPrefabs, parent, columns, rows, layerCount, spacingMultiplier);
    }

    void ClearAll()
    {
        if (parent == null)
        {
            Debug.LogWarning("부모 오브젝트를 지정하세요.");
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
}
