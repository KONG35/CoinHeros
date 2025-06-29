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
    Transform parent;
    float spacingMultiplier = 1.0f;

    [MenuItem("Tools/행열 Coin 배치 (헥스)")]
    public static void ShowWindow()
    {
        GetWindow<GridCoinPlacerWindow>("행열 Coin 배치 (헥스)");
    }

    void OnGUI()
    {
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
        spacingMultiplier = EditorGUILayout.Slider("간격 배율", spacingMultiplier, 1f, 2f);
        parent = (Transform)EditorGUILayout.ObjectField("부모 오브젝트", parent, typeof(Transform), true);

        EditorGUILayout.Space();
        if (GUILayout.Button("배치하기")) PlaceHexGrid();
        if (GUILayout.Button("모두 삭제")) ClearAll();
    }

    void PlaceHexGrid()
    {
        if (defaultPrefab == null)
        {
            Debug.LogError("기본 프리팹이 지정되지 않았습니다.");
            return;
        }

        GameObject preview = (GameObject)PrefabUtility.InstantiatePrefab(defaultPrefab);
        float objectWidth = GetObjectWidth(preview);
        DestroyImmediate(preview);

        float sin60 = Mathf.Sin(60f * Mathf.Deg2Rad);
        float spacingX = objectWidth * spacingMultiplier;
        float spacingZ = objectWidth * sin60 * spacingMultiplier;

        Vector3 origin = parent != null ? parent.position : Vector3.zero;

        // 셀 좌표 생성
        List<Vector3> cellPositions = new List<Vector3>();
        for (int c = 0; c < columns; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                float z = origin.z + c * spacingZ;
                float x = origin.x - r * spacingX;
                if (c % 2 == 1)
                    x -= spacingX * 0.5f;
                cellPositions.Add(new Vector3(x, origin.y, z));
            }
        }

        // 배치할 프리팹 리스트 준비
        List<GameObject> prefabPool = new List<GameObject>();
        for (int i = 0; i < additionalPrefabs.Count; i++)
        {
            GameObject pf = additionalPrefabs[i];
            int count = additionalPrefabCounts[i];
            if (pf != null && count > 0)
            {
                for (int j = 0; j < count; j++)
                    prefabPool.Add(pf);
            }
        }
        while (prefabPool.Count < cellPositions.Count)
        {
            prefabPool.Add(defaultPrefab);
        }

        // 셔플하여 배치
        System.Random rand = new System.Random();
        for (int i = prefabPool.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            var temp = prefabPool[i];
            prefabPool[i] = prefabPool[j];
            prefabPool[j] = temp;
        }

        // 생성
        for (int i = 0; i < cellPositions.Count; i++)
        {
            GameObject prefabToUse = prefabPool[i];
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToUse);
            obj.transform.position = cellPositions[i];
            obj.transform.rotation = Quaternion.identity;
            if (parent != null) obj.transform.SetParent(parent);

            Undo.RegisterCreatedObjectUndo(obj, "Place Coin Object");
        }
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
