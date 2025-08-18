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
    float spacingMultiplier = 0.97f; // 0.97 이상이면 오브젝트가 겹쳐서 배치됨

    static GridCoinPlacerWindow windowInstance;

    // 미리보기 위치 저장
    List<Vector3> previewPositions = new List<Vector3>();

    [MenuItem("Tools/지그재그 방식의 행열 Coin 배치")]
    public static void ShowWindow()
    {
        windowInstance = GetWindow<GridCoinPlacerWindow>("지그재그 방식의 행열 Coin 배치");
        SceneView.duringSceneGui -= windowInstance.OnSceneGUI;
        SceneView.duringSceneGui += windowInstance.OnSceneGUI;
    }

    void OnDisable()
    {
    }

    void OnGUI()
    {
        GUILayout.Label("지그재그 방식의 행열 구조로 코인을 배치하는 도구입니다", EditorStyles.helpBox);
        GUILayout.Label("행, 열 구조로 배치 (기본 오브젝트 + 추가 오브젝트)", EditorStyles.boldLabel);

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
            additionalPrefabCounts[i] = EditorGUILayout.IntField("개수", additionalPrefabCounts[i]);
            GUILayout.Space(5);
        }

        rows = EditorGUILayout.IntField("행 수", rows);
        columns = EditorGUILayout.IntField("열 수", columns);

        GUILayout.Space(5);
        layerCount = EditorGUILayout.IntField("층 수", layerCount);
        spacingMultiplier = EditorGUILayout.Slider("간격 배수", spacingMultiplier, 0.1f, 10f);
        parent = (Transform)EditorGUILayout.ObjectField("부모 오브젝트", parent, typeof(Transform), true);

        EditorGUILayout.Space();
        if (GUILayout.Button("배치 미리보기")) GeneratePreview();
        if (GUILayout.Button("배치하기")) PlaceHexGrid();
        if (GUILayout.Button("배치 그룹화")) PlacedOutGroup();
        if (GUILayout.Button("모두 삭제")) ClearAll();
    }

    void GeneratePreview()
    {
        previewPositions.Clear();

        if (defaultPrefab == null) return;

        float objectWidth = CoinMaker.GetObjectWidth(defaultPrefab);
        float sin60 = Mathf.Sin(60f * Mathf.Deg2Rad);
        float spacingX = objectWidth * spacingMultiplier;
        float spacingZ = objectWidth * sin60 * spacingMultiplier;
        float objectDepth = CoinMaker.GetObjectDepth(defaultPrefab);

        Vector3 origin = parent != null ? parent.position : Vector3.zero;

        for (int layer = 0; layer < layerCount; layer++)
        {
            float y = origin.y + layer * objectDepth;

            for (int c = 0; c < columns; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    float z = origin.z + c * spacingZ;
                    float x = origin.x - r * spacingX;
                    if (c % 2 == 1) x -= spacingX * 0.5f;

                    previewPositions.Add(new Vector3(x, y, z));
                }
            }
        }

        SceneView.RepaintAll();
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (previewPositions == null || previewPositions.Count == 0) return;

        Handles.color = Color.cyan;
            MeshFilter meshFilter = defaultPrefab.GetComponentInChildren<MeshFilter>();
        foreach (var pos in previewPositions)
        {
            //Handles.DrawWireCube(pos, Vector3.one * 0.9f);
            if (meshFilter != null)
            {
                Mesh mesh = meshFilter.sharedMesh;
                Material mat = new Material(Shader.Find("Hidden/PreviewWireShader")); // 와이어 프레임 렌더링용

                mat.SetPass(0);
                Graphics.DrawMeshNow(mesh, Matrix4x4.TRS(pos, Quaternion.identity, defaultPrefab.transform.lossyScale));
            }
        }
        
    }

    void PlaceHexGrid()
    {
        List<AdditionalPrefab> addPrefabs = new List<AdditionalPrefab>();
        for (int i = 0; i < additionalPrefabs.Count; i++)
        {
            addPrefabs.Add(new AdditionalPrefab(additionalPrefabs[i], additionalPrefabCounts[i]));
        }
        CoinMaker.PlaceGridObject(defaultPrefab, addPrefabs, parent, columns, rows, layerCount, spacingMultiplier);
        previewPositions.Clear();
    }

    void PlacedOutGroup()
    {
        if (parent.GetComponentInChildren<MeshFilter>() == false)
            return;

        GameObject group = new GameObject("PlacedGroup");
        group.transform.parent = parent;
        Transform[] objs = parent.GetComponentsInChildren<Transform>();
        int num = 0;
        foreach(var o in objs)
        {
            if(o.GetComponent<MeshFilter>())
            {
                GameObject go = new GameObject(num.ToString());
                go.transform.position = o.transform.position;
                go.transform.parent = group.transform;
                num++;
            }
        }

    }
    void ClearAll()
    {
        if (parent == null)
        {
            Debug.LogWarning("부모 오브젝트가 설정되지 않았습니다.");
            return;
        }
        for (int i = parent.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(parent.GetChild(i).gameObject);

        previewPositions.Clear();
    }

}
