using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public class AdditionalPrefab
{
    public GameObject prefab;
    public int counts;
    public AdditionalPrefab(GameObject go, int c)
    {
        prefab = go;
        counts = c;
    }
}
public static class CoinMaker
{
    // grid 구조, 
    public static void PlaceGridObject(GameObject defaultPrefab, List<AdditionalPrefab> additionalPrefabs, Transform parent, float columns, float rows, int layerCount, float spacingMultiplier = 1f)
    {
        if (defaultPrefab == null)
        {
            Debug.LogError("기본 프리팹이 지정되지 않았습니다.");
            return;
        }
        float objectWidth = GetObjectWidth(defaultPrefab);

        float sin60 = Mathf.Sin(60f * Mathf.Deg2Rad);
        float spacingX = objectWidth * spacingMultiplier;
        float spacingZ = objectWidth * sin60 * spacingMultiplier;

        float objectDepth = GetObjectDepthZ(defaultPrefab);

        Vector3 origin = parent != null ? parent.position : Vector3.zero;

        for (int layer=0;layer<layerCount;layer++)
        {
            // 층 그룹
            GameObject layerGroup = new GameObject($"Layer_{layer}");
            if (parent != null) layerGroup.transform.SetParent(parent);
            // Y축 위치
            layerGroup.transform.position = origin + Vector3.up * (layer * objectDepth);
            // 셀 좌표 생성
            List<Vector3> cellPositions = new List<Vector3>();

            for (int c = 0; c < columns; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    float z = origin.z + c * spacingZ;
                    float x = origin.x - r * spacingX;
                    float y = layerGroup.transform.position.y;
                    if (c % 2 == 1)
                        x -= spacingX * 0.5f;
                    cellPositions.Add(new Vector3(x, y, z));
                }
            }

            // 배치할 프리팹 리스트 준비
            List<GameObject> prefabPool = new List<GameObject>();
            for (int i = 0; i < additionalPrefabs.Count; i++)
            {
                GameObject pf = additionalPrefabs[i].prefab;
                int count = additionalPrefabs[i].counts;
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
#if UNITY_EDITOR
                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToUse);
                obj.transform.localPosition = cellPositions[i];
                obj.transform.localRotation = Quaternion.identity;

                if (parent != null) obj.transform.SetParent(layerGroup.transform);
                Undo.RegisterCreatedObjectUndo(obj, "Place Coin Object");
#else
                    GameObject obj = GameObject.Instantiate(prefabToUse);
                    obj.transform.localPosition = cellPositions[i];
                    obj.transform.localRotation = Quaternion.identity;

                    if (parent != null) obj.transform.SetParent(layerGroup.transform);
                    Undo.RegisterCreatedObjectUndo(obj, "Place Coin Object");
#endif
            }
        }
        
        
    }
    // 육각 구조
    public static void PlaceHexObjects(GameObject defaultPrefab, Transform parent, int layerCount, int coinsPerLayer, float spacingMultiplier = 1f)
    {
        if (defaultPrefab == null)
        {
            Debug.LogError("프리팹이 지정되지 않았습니다.");
            return;
        }

        // 오브젝트 크기 측정
        float objectWidth = GetObjectWidth(defaultPrefab);
        float objectDepth = GetObjectDepthZ(defaultPrefab);

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
#if UNITY_EDITOR
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(defaultPrefab);
                    obj.transform.SetParent(layerGroup.transform);
                    obj.transform.localPosition = localPos;
                    obj.transform.localRotation = Quaternion.identity;

                    Undo.RegisterCreatedObjectUndo(obj, "Place Coin Object");
#else
                    GameObject obj = GameObject.Instantiate(defaultPrefab);
                    obj.transform.SetParent(layerGroup.transform);
                    obj.transform.localPosition = localPos;
                    obj.transform.localRotation = Quaternion.identity;

                    Undo.RegisterCreatedObjectUndo(obj, "Place Coin Object");
#endif
                    placed++;
                }
                ring++;
            }
        }
    }

    static float GetObjectWidth(GameObject go)
    {
        Bounds b = new Bounds(go.transform.position, Vector3.zero);
        foreach (var r in go.GetComponentsInChildren<Renderer>())
            b.Encapsulate(r.bounds);
        return Mathf.Max(b.size.x, b.size.z);
    }
    static float GetObjectDepthZ(GameObject go)
    {
        Bounds b = new Bounds(go.transform.position, Vector3.zero);
        foreach (var r in go.GetComponentsInChildren<Renderer>())
            b.Encapsulate(r.bounds);
        return b.size.z;
    }
}
