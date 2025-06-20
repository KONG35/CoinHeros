using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTableManager : Singleton<DataTableManager>
{
    public int MaxLv = 200;
    public int StartExp = 100;
    public float AddLvExpMultiply = 1.2f;
    public List<int> ExpTable;
    public List<int> RankMaxLvTable;

    public List<CharacterData> characterPrefabList;

    public void Start()
    {
        InitExpTable();
        InitRankMaxLvTable();
    }


    public void InitExpTable()
    {
        ExpTable = new List<int>();
        int Exp = StartExp;
        for (int i=0;i< MaxLv;i++)
        {
            ExpTable.Add(Exp);
            Exp = (int)(Exp * AddLvExpMultiply);
        }
    }
    public void InitRankMaxLvTable()
    {
        RankMaxLvTable = new List<int>();

        RankMaxLvTable.Add(20);
        RankMaxLvTable.Add(30);
        RankMaxLvTable.Add(40);
        RankMaxLvTable.Add(50);
        RankMaxLvTable.Add(60);
        RankMaxLvTable.Add(70);
        RankMaxLvTable.Add(80);
        RankMaxLvTable.Add(90);
        RankMaxLvTable.Add(100);
    }

    public int GetTotalPrevExp(int CurLv)
    {
        int total = 0;
        for(int i=0;i<CurLv-1;i++)
        {
            total += ExpTable[i];
        }
        return total;
    }
}
