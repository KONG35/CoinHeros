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
        CharNameList.Add("아렌");
        CharNameList.Add("세린");
        CharNameList.Add("다크리안");
        CharNameList.Add("벨로나");
        CharNameList.Add("타렌");
        CharNameList.Add("에실리아");
        CharNameList.Add("카인");
        CharNameList.Add("미르셀");
        CharNameList.Add("도린");
        CharNameList.Add("라일라");
        CharNameList.Add("자린");
        CharNameList.Add("일런");
        CharNameList.Add("탈린");
        CharNameList.Add("매리스");
        CharNameList.Add("카델");
        CharNameList.Add("네리아"); 
        CharNameList.Add("루벤");
        CharNameList.Add("벨테인");
        CharNameList.Add("하센");
        CharNameList.Add("스톰하트");
        CharNameList.Add("아이언핏");
        CharNameList.Add("문상품");
        CharNameList.Add("그림포지");
        CharNameList.Add("은하");
        CharNameList.Add("블러드벨트");
        CharNameList.Add("나이트쉐이드");
        CharNameList.Add("페일윈드");
        CharNameList.Add("실버문");
        CharNameList.Add("프레이야");
        CharNameList.Add("애쉬윈드");
        CharNameList.Add("루미에르");
        CharNameList.Add("스칼로스");
    }
}
