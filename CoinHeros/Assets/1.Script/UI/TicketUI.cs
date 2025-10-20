using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TicketUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ticketCountTxt;

    [SerializeField] private StarIcon[] starIconArr;
    [SerializeField] private RectTransform targetTr;

    [SerializeField] private StarImageEffect effect;
    private int ticketCount;

    private int curCount; // 현재 활성화된 signal index
    private int count;  // 활성화된 신호 개수
    private Vector2[] originPosArr;

    object lockObj = new object(); // null 대신 new object()로 초기화
    private void Awake()
    {
        SetCount(0);
        curCount = 0;
        ticketCount = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Loop());
        foreach (var s in starIconArr)
        {
            s.Set(targetTr.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Loop()
    {
        while(true)
        {
            if(HasCount()) // lock을 사용하는 메서드로 변경
            {
                starIconArr[curCount].SetWhite();
                curCount = (curCount + 1) % starIconArr.Length;
                PlusCount(-1);
                if(curCount==0)
                {
                    foreach(var s in starIconArr)
                    {
                        s.Move();
                    }
                    effect.Show();
                    ++ticketCount;
                    ticketCountTxt.text = string.Format("{0:D4}", ticketCount);
                    yield return new WaitForSeconds(0.1f);
                }
            }
            yield return null;
        }
    }
    public void PlusCount(int n)
    {
        lock (lockObj)
        {
            count += n;
        }
    }

    // count 값을 안전하게 읽는 메서드
    public int GetCount()
    {
        lock (lockObj)
        {
            return count;
        }
    }

    // count 값을 안전하게 설정하는 메서드
    public void SetCount(int value)
    {
        lock (lockObj)
        {
            count = value;
        }
    }

    // count 값이 0보다 큰지 안전하게 확인하는 메서드
    public bool HasCount()
    {
        lock (lockObj)
        {
            return count > 0;
        }
    }
    [Button]
    private void EditStarMove()
    {
        effect.Show();
        
        foreach (var s in starIconArr)
        {
            s.Move();
        }
    }
}
