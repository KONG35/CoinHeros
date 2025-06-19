using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Outline_AvgNor : MonoBehaviour
{
    Material outlineMat;
    GameObject OutLineObject;
    private void Awake()
    {
        if (outlineMat==null)
        {
            outlineMat = new Material(Shader.Find("Custom/OutLine"));
            outlineMat.hideFlags = HideFlags.HideAndDontSave;
        }
        Transform ot = transform.Find("OutLineObj");
        if (ot)
        {
            OutLineObject = ot.gameObject;
            if(GetComponent<MeshFilter>())
            {
                OutLineObject.GetComponent<MeshRenderer>().material = outlineMat;
            }
            if(GetComponent<SkinnedMeshRenderer>())
            {
                OutLineObject.GetComponent<SkinnedMeshRenderer>().material = outlineMat;
            }
        }
        if(OutLineObject==null)
        {
            OutLineObject = new GameObject("OutLineObj");
            OutLineObject.transform.parent = transform;
            if(GetComponent<MeshFilter>())
            {
                OutLineObject.AddComponent<MeshFilter>();
                var r = OutLineObject.AddComponent<MeshRenderer>();
                r = GetComponent<MeshRenderer>();
                Mesh m = (Mesh)Instantiate(GetComponent<MeshFilter>().sharedMesh);
                MeshNormalAverage(ref m);
                OutLineObject.GetComponent<MeshFilter>().sharedMesh = m;
                OutLineObject.GetComponent<MeshRenderer>().material = outlineMat;
            }
            if(GetComponent<SkinnedMeshRenderer>())
            {
                var smr = OutLineObject.AddComponent<SkinnedMeshRenderer>();
                smr = GetComponent<SkinnedMeshRenderer>();
                Mesh m = (Mesh)Instantiate(GetComponent<SkinnedMeshRenderer>().sharedMesh);
                MeshNormalAverage(ref m);
                OutLineObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = m;
                OutLineObject.GetComponent<SkinnedMeshRenderer>().material = outlineMat;
            }
            OutLineObject.transform.localPosition = Vector3.zero;

            OutLineObject.transform.localRotation = Quaternion.identity;

            OutLineObject.transform.localScale = Vector3.one;
        }
    }

    void MeshNormalAverage(ref Mesh mesh)
    {
        Dictionary<Vector3, List<int>> map = new Dictionary<Vector3, List<int>>();
        
        for(int i = 0; i<mesh.vertexCount;i++)
        {
            if (!map.ContainsKey(mesh.vertices[i]))
            {
                map.Add(mesh.vertices[i], new List<int>());
            }
            map[mesh.vertices[i]].Add(i);
        }
        Vector3[] normals = mesh.normals;
        Vector3 nor;

        foreach(var p  in map)
        {
            nor = Vector3.zero;
            foreach (var n in p.Value)
            {
                nor += mesh.normals[n];
            }
            nor /= p.Value.Count;
            foreach (var n in p.Value)
            {
                normals[n] = nor;
            }
        }
        mesh.normals = normals;
    }
}


