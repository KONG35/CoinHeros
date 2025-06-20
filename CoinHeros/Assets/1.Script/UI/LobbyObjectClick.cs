using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyObjectClick : MonoBehaviour
{
    Camera cam;

    public List<Collider> Obj;
    public List<Outline> ObjOutLine;

    public void Awake()
    {
        cam = Camera.main;
    }
    public void Start()
    {
        ObjOutLine = new List<Outline>();
        foreach(var o in Obj)
        {
            ObjOutLine.Add(o.GetComponent<Outline>());
        }
    }

    public void LateUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        foreach (var o in ObjOutLine)
        {
            o.enabled = false;
        }

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("MapObject")))
        {
            int index = Obj.FindIndex(x => x == hit.collider);
            if (0<=index)
            {
                ObjOutLine[index].enabled = true;
            }
            Debug.Log(index);
        }

    }
}
