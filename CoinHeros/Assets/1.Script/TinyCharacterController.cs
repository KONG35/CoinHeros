using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyCharacterController : MonoBehaviour
{

    public List<GameObject> Body;
    public GameObject Select_Body;
    public List<GameObject> Cloak;
    public GameObject Select_Cloak;
    public List<GameObject> BackPack;
    public GameObject Select_BackPack;

    public List<GameObject> Head_Glass;
    public GameObject Select_Glass;
    public List<GameObject> Head_Ears;
    public GameObject Select_Ears;
    public List<GameObject> Head_Crown;
    public GameObject Select_Crown;
    public List<GameObject> Head_Mask;
    public GameObject Select_Mask;
    public List<GameObject> Head_Mustache;
    public GameObject Select_Mustache;
    public List<GameObject> Head_Eye;
    public GameObject Select_Eye;
    public List<GameObject> Head_Mouth;
    public GameObject Select_Mouth;
    public List<GameObject> Head_Hair;
    public GameObject Select_Hair;
    public List<GameObject> Head_Head;
    public GameObject Select_Head;
    public List<GameObject> Head_Armor;
    public GameObject Select_Armor;
    public List<GameObject> Head_Hat;
    public GameObject Select_Hat;
    public List<GameObject> Head_EyeBrow;
    public GameObject Select_EyeBrow;


    public List<GameObject> Left_Bow;
    public List<GameObject> Left_Sword;
    public List<GameObject> Left_Shield;
    public List<GameObject> Right_Arrow;
    public List<GameObject> Right_Sword;
    public List<GameObject> Right_TwoHandSword;
    public List<GameObject> Right_Wand;
    public List<GameObject> Right_Spear;

    public Animator Bow;
    public Animator Arrow;

    public void Awake()
    {
        
    }


    [Button]
    public void GetItem()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        Transform Left = transform;
        Transform Right = transform;
        Transform Head = transform;
        Body = new List<GameObject>();
        Cloak = new List<GameObject>();
        BackPack = new List<GameObject>();

        Head_Glass = new List<GameObject>();
        Head_Ears = new List<GameObject>();
        Head_Crown = new List<GameObject>();
        Head_Mask = new List<GameObject>();
        Head_Mustache = new List<GameObject>();
        Head_Eye = new List<GameObject>();
        Head_Mouth = new List<GameObject>();
        Head_Hair = new List<GameObject>();
        Head_Head = new List<GameObject>();
        Head_Armor = new List<GameObject>();
        Head_Hat = new List<GameObject>();
        Head_EyeBrow = new List<GameObject>();


        Left_Bow = new List<GameObject>();
        Left_Sword = new List<GameObject>();
        Left_Shield = new List<GameObject>();
        Right_Arrow = new List<GameObject>();
        Right_Sword = new List<GameObject>();
        Right_Wand = new List<GameObject>();
        Right_Spear = new List<GameObject>();
        Right_TwoHandSword = new List<GameObject>();
        //Body
        foreach (Transform child in allChildren)
        {
            if (child == transform)
                continue;

            if (child.name.Contains("Body"))
            {
                Body.Add(child.gameObject);
            }
            else if (child.name.Contains("Cloak0"))
            {
                Cloak.Add(child.gameObject);
            }
            else if (child.name.Contains("BackPack"))
            {
                BackPack.Add(child.gameObject);
            }
            else if (child.name == "head")
            {
                Head = child;
            }
            else if (child.name == "weapon_r")
            {
                Right = child;
            }
            else if (child.name == "weapon_l")
            {
                Left = child;
            }
        }

        //Right
        Transform[] Rightchild = Right.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in Rightchild)
        {
            if (child.name.Contains("Arrow0"))
            {
                Right_Arrow.Add(child.gameObject);
            }
            else if (child.name.Contains("Sword")&& child.name.Contains("OHS"))
            {
                Right_Sword.Add(child.gameObject);
            }
            else if (child.name.Contains("Sword") & child.name.Contains("THS"))
            {
                Right_TwoHandSword.Add(child.gameObject);
            }
            else if (child.name.Contains("Wand"))
            {
                Right_Wand.Add(child.gameObject);
            }
            else if (child.name.Contains("Spear"))
            {
                Right_Spear.Add(child.gameObject);
            }
        }
        //Left
        Transform[] Leftchild = Left.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in Leftchild)
        {
            if (child.name.Contains("Bow0"))
            {
                Left_Bow.Add(child.gameObject);
            }
            else if (child.name.Contains("Sword"))
            {
                Left_Sword.Add(child.gameObject);
            }
            else if (child.name.Contains("Shield"))
            {
                Left_Shield.Add(child.gameObject);
            }
        }
        //Head
        Transform[] Headchild = Head.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in Headchild)
        {
            if (child.name.Contains("Glass"))
            {
                Head_Glass.Add(child.gameObject);
            }
            else if (child.name.Contains("Ears"))
            {
                Head_Ears.Add(child.gameObject);
            }
            else if (child.name.Contains("Crown"))
            {
                Head_Crown.Add(child.gameObject);
            }
            else if (child.name.Contains("Mask"))
            {
                Head_Mask.Add(child.gameObject);
            }
            else if (child.name.Contains("Mustache"))
            {
                Head_Mustache.Add(child.gameObject);
            }
            else if (child.name.Contains("Eyebrow"))
            {
                Head_EyeBrow.Add(child.gameObject);
            }
            else if (child.name.Contains("Eye"))
            {
                Head_Eye.Add(child.gameObject);
            }
            else if (child.name.Contains("Mouth"))
            {
                Head_Mouth.Add(child.gameObject);
            }
            else if (child.name.Contains("Hair"))
            {
                Head_Hair.Add(child.gameObject);
            }
            else if (child.name.Contains("Armor"))
            {
                Head_Armor.Add(child.gameObject);
            }
            else if (child.name.Contains("Head"))
            {
                Head_Head.Add(child.gameObject);
            }
            else if (child.name.Contains("Hat"))
            {
                Head_Hat.Add(child.gameObject);
            }
        }

    }


    public void SetCharacterSelectObject()
    {
        ListDisable(Body);
        ListDisable(BackPack);
        ListDisable(Cloak);

        ListDisable(Head_Hair);
        ListDisable(Head_Armor);

        ListDisable(Head_Eye);
        ListDisable(Head_Mouth);
        ListDisable(Head_Mustache);
        ListDisable(Head_Hat);
        ListDisable(Head_Ears);
        ListDisable(Head_Crown);
        ListDisable(Head_Mask);
        ListDisable(Head_Glass);
        ListDisable(Head_EyeBrow);
        ListDisable(Head_Head);


        ListDisable(Left_Bow);
        ListDisable(Left_Sword);
        ListDisable(Left_Shield);
        ListDisable(Right_Arrow);
        ListDisable(Right_Sword);
        ListDisable(Right_TwoHandSword);
        ListDisable(Right_Wand);
        ListDisable(Right_Spear);


        if (Select_Body)
            Select_Body.SetActive(true);

        if (Select_BackPack)
            Select_BackPack.SetActive(true);
        if (Select_Cloak)
            Select_Cloak.SetActive(true);


        if (Select_Armor)
            Select_Armor.SetActive(true);
        else
        {
            if (Select_Head)
                Select_Head.SetActive(true);

            if (Select_Hat)
            {
                Select_Hat.SetActive(true); 
                if (Select_EyeBrow)
                    Select_EyeBrow.SetActive(true);
            }
            else
            {
                if (Select_Hair)
                {
                    Select_Hair.SetActive(true);
                    if (Select_Crown)
                        Select_Crown.SetActive(true);
                }
                else if (Select_EyeBrow)
                    Select_EyeBrow.SetActive(true);
            }
            if (Select_Eye)
                Select_Eye.SetActive(true);
            if (Select_Glass)
                Select_Glass.SetActive(true);
            if (Select_Mouth)
                Select_Mouth.SetActive(true);
            if (Select_Mustache)
                Select_Mustache.SetActive(true);
            if (Select_Mask)
                Select_Mask.SetActive(true);
            if (Select_Ears)
                Select_Ears.SetActive(true);
        }



        if (eRw == eRWeapon.None)
        {
            if (eLw == eLWeapon.sword)
            {
                eLw = eLWeapon.None;
                SelectLw = null;
            }
        }
        if (eLw == eLWeapon.bow)
        {
            eRw = eRWeapon.arrow;
            int index = Left_Bow.IndexOf(SelectLw);
            SelectRw = Right_Arrow[index];
            
        }else
        {
            if (eRw == eRWeapon.arrow)
            {
                eRw = eRWeapon.None; 
                SelectRw = null;
            }
        }
        if(eRw == eRWeapon.twohandsword || eRw == eRWeapon.spear)
        {
            SelectLw = null;
            eLw = eLWeapon.None;
        }
        if (SelectRw)
            SelectRw.SetActive(true);

        if (SelectLw)
            SelectLw.SetActive(true);

        SetAnim();
    }

    private void SetAnim()
    {
        var Data = GetComponent<CharacterData>();

        if (Data == null)
            return;

        CharacterData.eJobAnim Job = CharacterData.eJobAnim.None;
        if (eRw == eRWeapon.arrow && eLw == eLWeapon.bow)
            Job = CharacterData.eJobAnim.Archer;
        else if (eRw == eRWeapon.sword && eLw == eLWeapon.sword)
            Job = CharacterData.eJobAnim.DoubleSword;
        else if (eRw == eRWeapon.sword && eLw == eLWeapon.shield)
            Job = CharacterData.eJobAnim.SwordAndShield;
        else if (eRw == eRWeapon.wand )
            Job = CharacterData.eJobAnim.Magic;
        else if (eRw == eRWeapon.twohandsword)
            Job = CharacterData.eJobAnim.TwoHandSword;
        else if (eRw == eRWeapon.sword && eLw != eLWeapon.sword)
            Job = CharacterData.eJobAnim.SingleSword;
        else if (eRw == eRWeapon.spear)
            Job = CharacterData.eJobAnim.Spear;
        else
            Job = CharacterData.eJobAnim.None;

        if(Data._anim)
            Data._anim.runtimeAnimatorController = Data.jobAnims[(int)Job];
    }

    private void ListDisable(List<GameObject> list)
    {
        foreach (var obj in list)
        {
            obj.SetActive(false);
        }
    }

    [Button]
    public void RandPreset()
    {

    }

    public eRWeapon eRw;
    public GameObject SelectRw;
    [Button]
    public void SetRightHand()
    {

    }
    public eLWeapon eLw;
    public GameObject SelectLw;
    [Button]
    public void SetLeftHand()
    {

    }

    public enum eRWeapon
    {
        arrow,
        sword,
        twohandsword,
        wand,
        spear,
        None
    }
    public enum eLWeapon
    {
        bow,
        sword,
        shield,
        None
    }
}
