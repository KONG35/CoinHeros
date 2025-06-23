using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusItem : MonoBehaviour
{
    [SerializeField]
    private Image iconImg;
    private int index;
    public RectTransform recTr { get; private set; }
    private void Awake()
    {
        recTr = gameObject.GetComponent<RectTransform>();
    }
    public void SetItem(int num)
    {
        index = num;

    }
    public int Selected()
    {
        return index;
    }
}
