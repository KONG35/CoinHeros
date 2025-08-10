using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CoinRemainItem : MonoBehaviour
{
    [SerializeField]
    private Image image;
    
    public void SetSprite(Sprite _sp)
    {
        image.sprite = _sp;
    }
    public void Pop()
    {

    }
    IEnumerator PopCor()
    {
        return null;
        
    }
}
