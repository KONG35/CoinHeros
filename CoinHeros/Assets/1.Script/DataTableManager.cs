using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTableManager : Singleton<DataTableManager>
{
    public int MaxLv = 200;
    public int StartExp = 100;
    public List<long> ExpTable;
    public List<int> RankMaxLvTable;

    public List<float> STRGradeEfficiency;
    public List<float> MAGGradeEfficiency;
    public List<float> CONGradeEfficiency;
    public List<float> AGIGradeEfficiency;
    public List<float> SPRGradeEfficiency;
    public List<float> LUKGradeEfficiency;

    public List<string> CharNameList;

    public List<CharacterData> characterPrefabList;
    public List<MonsterData> MonsterPrefabList;


    public int MaxStage = 1000;


    public float minMonsterState = 400f;
    public float maxMonsterState = 2000f;

    public void Start()
    {
    }

    public long GetTotalPrevExp(int CurLv)
    {
        long total = 0;
        for(int i=0;i<CurLv-1;i++)
        {
            total += ExpTable[i];
        }
        return total;
    }
    [Button]
    public void NameInit()
    {
        CharNameList = new List<string>();
        CharNameList.Add("�Ʒ�");
        CharNameList.Add("����");
        CharNameList.Add("��ũ����");
        CharNameList.Add("���γ�");
        CharNameList.Add("Ÿ��");
        CharNameList.Add("���Ǹ���");
        CharNameList.Add("ī��");
        CharNameList.Add("�̸���");
        CharNameList.Add("����");
        CharNameList.Add("���϶�");
        CharNameList.Add("�ڸ�");
        CharNameList.Add("�Ϸ�");
        CharNameList.Add("Ż��");
        CharNameList.Add("�Ÿ���");
        CharNameList.Add("ī��");
        CharNameList.Add("�׸���"); 
        CharNameList.Add("�纥");
        CharNameList.Add("������");
        CharNameList.Add("�ϼ�");
        CharNameList.Add("������Ʈ");
        CharNameList.Add("���̾���");
        CharNameList.Add("����ǰ");
        CharNameList.Add("�׸�����");
        CharNameList.Add("����");
        CharNameList.Add("�����座Ʈ");
        CharNameList.Add("����Ʈ���̵�");
        CharNameList.Add("��������");
        CharNameList.Add("�ǹ���");
        CharNameList.Add("�����̾�");
        CharNameList.Add("�ֽ�����");
        CharNameList.Add("��̿���");
        CharNameList.Add("��Į�ν�");
    }
}
