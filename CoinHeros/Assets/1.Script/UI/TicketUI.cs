using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TicketUI : MonoBehaviour
{
    [SerializeField] private Image[] signalImgArr;
    [SerializeField] private TextMeshProUGUI ticketCountTxt;

    Animation ticketAnim;
    private int animCount;
    private int ticketCount;

    private int offset; // 현재 활성화된 signal index
    private int count;  // 활성화된 신호 개수
    object lockObj = new object(); // null 대신 new object()로 초기화
    private void Awake()
    {
        foreach (var s in signalImgArr)
        {
            s.color = Color.black;
        }
        SetCount(0);
        offset = 0;
        animCount = 0;
        ticketCount = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Loop());
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
                signalImgArr[offset].color = Color.white;
                offset = (offset + 1) % signalImgArr.Length;
                PlusCount(-1);
                if(offset==0)
                {
                    foreach (var s in signalImgArr)
                    {
                        s.color = Color.black;
                    }
                    ++animCount;
                    //yield return ticketAnim.Play();
                    ++ticketCount;
                    ticketCountTxt.text = string.Format("{0:D4}", ticketCount);
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
}
