using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public List<Transform> UIObject;
    public List<RectTransform> ObjectUIPanel;

    Camera cam;

    public LobbyBattleUI BattleUI;
    public LobbyUnitListUI UnitListUI;
    public CharacterUI UnitUI;


    public void Awake()
    {
        cam = Camera.main; PosInit();
    }


    public void PosInit()
    {
        for(int i=0;i<UIObject.Count;i++)
        {
            float Y = UIObject[i].GetComponent<MeshFilter>().mesh.bounds.size.y;
            Vector3 pos = UIObject[i].position;
            pos.y += Y*0.75f;
            ObjectUIPanel[i].position = cam.WorldToScreenPoint(pos);
        } 
    }

}
