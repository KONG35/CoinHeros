using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusBasketSpin : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Coin")
        {
            Coin c = collision.gameObject.GetComponent<Coin>();
            
        }
    }
}
