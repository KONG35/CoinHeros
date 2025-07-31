using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Codice.Client.BaseCommands.Import.Commit;

[CustomEditor(typeof(TinyCharacterController)), CanEditMultipleObjects]
public class TinyCharacterControllerEditor : Editor
{
    private SerializedProperty  Body;
    private SerializedProperty  Cloak;
    private SerializedProperty  BackPack;

    private SerializedProperty  Head_Glass;
    private SerializedProperty  Head_Ears;
    private SerializedProperty  Head_Crown;
    private SerializedProperty  Head_Mask;
    private SerializedProperty  Head_Mustache;
    private SerializedProperty  Head_Eye;
    private SerializedProperty  Head_Mouth;
    private SerializedProperty  Head_Hair;
    private SerializedProperty  Head_Head;
    private SerializedProperty  Head_Armor;
    private SerializedProperty  Head_Hat;
    private SerializedProperty  Head_EyeBrow;

    private SerializedProperty  Left_Bow;
    private SerializedProperty  Left_Sword;
    private SerializedProperty  Left_Shield;
    private SerializedProperty  Right_Arrow;
    private SerializedProperty  Right_Sword;
    private SerializedProperty  Right_TwoHandSword;
    private SerializedProperty  Right_Wand;
    private SerializedProperty  Right_Spear;



    private TinyCharacterController Data;
    private void OnEnable()
    {
        Data = target as TinyCharacterController;

        Body = serializedObject.FindProperty("Body");
        Cloak = serializedObject.FindProperty("Cloak");
        BackPack = serializedObject.FindProperty("BackPack");

        Head_Glass = serializedObject.FindProperty("Head_Glass");
        Head_Ears = serializedObject.FindProperty("Head_Ears");
        Head_Crown = serializedObject.FindProperty("Head_Crown");
        Head_Mask = serializedObject.FindProperty("Head_Mask");
        Head_Mustache = serializedObject.FindProperty("Head_Mustache");
        Head_Eye = serializedObject.FindProperty("Head_Eye");
        Head_Mouth = serializedObject.FindProperty("Head_Mouth");
        Head_Hair = serializedObject.FindProperty("Head_Hair");
        Head_Head = serializedObject.FindProperty("Head_Head");
        Head_Armor = serializedObject.FindProperty("Head_Armor");
        Head_Hat = serializedObject.FindProperty("Head_Hat");
        Head_EyeBrow = serializedObject.FindProperty("Head_EyeBrow");

        Left_Bow = serializedObject.FindProperty("Left_Bow");
        Left_Sword = serializedObject.FindProperty("Left_Sword");
        Left_Shield = serializedObject.FindProperty("Left_Shield");
        Right_Arrow = serializedObject.FindProperty("Right_Arrow");
        Right_Sword = serializedObject.FindProperty("Right_Sword");
        Right_TwoHandSword = serializedObject.FindProperty("Right_TwoHandSword");
        Right_Wand = serializedObject.FindProperty("Right_Wand");
        Right_Spear = serializedObject.FindProperty("Right_Spear");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        DrawPart("Head", Data.Head_Head, ref Data.Select_Head);
        DrawPart("Body", Data.Body, ref Data.Select_Body);

        DrawPart("HeadArmor", Data.Head_Armor, ref Data.Select_Armor);
        if (Data.Select_Armor == null)
        {
            DrawPart("Hat", Data.Head_Hat, ref Data.Select_Hat);
            if (Data.Select_Hat == null)
                DrawPart("Hair", Data.Head_Hair, ref Data.Select_Hair);
            if (Data.Select_Hair == null)
                DrawPart("EyeBrow", Data.Head_EyeBrow, ref Data.Select_EyeBrow);

            DrawPart("Eye", Data.Head_Eye, ref Data.Select_Eye);
            DrawPart("Mouth", Data.Head_Mouth, ref Data.Select_Mouth);
            DrawPart("Mustache", Data.Head_Mustache, ref Data.Select_Mustache);
            DrawPart("Ears", Data.Head_Ears, ref Data.Select_Ears);
            DrawPart("Crown", Data.Head_Crown, ref Data.Select_Crown);
            DrawPart("Mask", Data.Head_Mask, ref Data.Select_Mask);
            DrawPart("Glass", Data.Head_Glass, ref Data.Select_Glass);
        }
        DrawPart("Cloak", Data.Cloak, ref Data.Select_Cloak);
        DrawPart("BackPack", Data.BackPack, ref Data.Select_BackPack);







        //Hand
        Data.eRw = (TinyCharacterController.eRWeapon)EditorGUILayout.EnumPopup("Right Hand Type", Data.eRw);

        List<GameObject> RHandList = Data.eRw switch
        {
            TinyCharacterController.eRWeapon.arrow => Data.Right_Arrow,
            TinyCharacterController.eRWeapon.spear => Data.Right_Spear,
            TinyCharacterController.eRWeapon.sword => Data.Right_Sword,
            TinyCharacterController.eRWeapon.twohandsword => Data.Right_TwoHandSword,
            TinyCharacterController.eRWeapon.wand => Data.Right_Wand,
            TinyCharacterController.eRWeapon.None => null,
            _ => null
        };

        if (RHandList != null && RHandList.Count > 0)
        {
            int selectedIndex = Mathf.Max(0, RHandList.IndexOf(Data.SelectRw));

            string[] options = RHandList.Select(obj => obj != null ? obj.name : "Null").ToArray();

            selectedIndex = EditorGUILayout.Popup("Right_Hand", selectedIndex, options);

            Data.SelectRw = RHandList[selectedIndex];

            
        }
        else
        {
            EditorGUILayout.LabelField("No Weapon ");
            Data.SelectRw = null;
        }
        if (GUILayout.Button("Apply RightHand Weapon"))
        {
            foreach (var obj in Data.Right_Arrow)
            {
                if (obj != null)
                    obj.SetActive(obj == Data.SelectRw);
                if (obj == Data.SelectRw)
                {
                    obj.transform.parent.gameObject.SetActive(true);
                }
            }
            foreach (var obj in Data.Right_Spear)
            {
                if (obj != null)
                    obj.SetActive(obj == Data.SelectRw);
            }
            foreach (var obj in Data.Right_Sword)
            {
                if (obj != null)
                    obj.SetActive(obj == Data.SelectRw);
            }
            foreach (var obj in Data.Right_TwoHandSword)
            {
                if (obj != null)
                    obj.SetActive(obj == Data.SelectRw);
            }
            foreach (var obj in Data.Right_Wand)
            {
                if (obj != null)
                    obj.SetActive(obj == Data.SelectRw);
            }
        }

        Data.eLw = (TinyCharacterController.eLWeapon)EditorGUILayout.EnumPopup("Left Hand Type", Data.eLw);
        List<GameObject> LHandList = Data.eLw switch
        {
            TinyCharacterController.eLWeapon.bow => Data.Left_Bow,
            TinyCharacterController.eLWeapon.shield => Data.Left_Shield,
            TinyCharacterController.eLWeapon.sword => Data.Left_Sword,
            TinyCharacterController.eLWeapon.None => null,
            _ => null
        };
        if (LHandList != null && LHandList.Count > 0)
        {
            int selectedIndex = Mathf.Max(0, LHandList.IndexOf(Data.SelectLw));

            string[] options = LHandList.Select(obj => obj != null ? obj.name : "Null").ToArray();

            selectedIndex = EditorGUILayout.Popup("Left_Hand", selectedIndex, options);

            Data.SelectLw = LHandList[selectedIndex];

            
        }
        else
        {
            EditorGUILayout.LabelField("No Weapon ");
            Data.SelectLw = null;
        }
        if (GUILayout.Button("Apply LeftHand Weapon"))
        {
            foreach (var obj in Data.Left_Bow)
            {
                if (obj != null)
                {
                    obj.SetActive(obj == Data.SelectLw);
                    if (obj == Data.SelectLw)
                    {
                        obj.transform.parent.gameObject.SetActive(true);
                    }
                }
            }
            foreach (var obj in Data.Left_Shield)
            {
                if (obj != null)
                    obj.SetActive(obj == Data.SelectLw);
            }
            foreach (var obj in Data.Left_Sword)
            {
                if (obj != null)
                    obj.SetActive(obj == Data.SelectLw);
            }
        }

        Data.SetCharacterSelectObject();

        if (GUI.changed)
            EditorUtility.SetDirty(Data);
    }


    private void DrawPart(string label, List<GameObject> list, ref GameObject selected)
    {
        int selectedIndex = Mathf.Max(0, list.IndexOf(selected));
        if (selected == null)
            selectedIndex = list.Count;

        string[] options = list
            .Select(obj => obj != null ? obj.name : "Null")
            .Concat(new[] { "None" })
            .ToArray();

        selectedIndex = EditorGUILayout.Popup(label, selectedIndex, options);

        if (list.Count <= selectedIndex)
            selected = null;
        else
            selected = list[selectedIndex];

    }

}
