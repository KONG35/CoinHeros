using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyObjectClick : MonoBehaviour
{
    Camera cam;

    public List<Collider> Obj;
    public List<Outline> ObjOutLine;

    public bool ignore = false;
    LobbyUI lobby;

    public void Awake()
    {
        cam = Camera.main;
        lobby = FindObjectOfType<LobbyUI>();
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
        if (ignore)
            return;

        foreach (var o in ObjOutLine)
        {
            o.enabled = false;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("MapObject")))
        {
            int index = Obj.FindIndex(x => x == hit.collider);
            if (0<=index)
            {
                ObjOutLine[index].enabled = true;

                if (Input.GetMouseButtonUp(0))
                    lobby.SetUIStep((LobbyUI.eUIStep)index);

            }
            Debug.Log(index);
        }

    }
}
