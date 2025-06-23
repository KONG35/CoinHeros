using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool isFixed;
    private  float fixRotX;
    
    private Rigidbody rigid;
    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
        rigid.velocity = new Vector3(0, -35f, 0);
        fixRotX = transform.eulerAngles.x;
        Init(true);
    }
    public void Init(bool _isFixNeed)
    {
        if(_isFixNeed)
        {
            isFixed = true;
        }
        else
        {
            isFixed = false;

        }
    }
    // Start is called before the first frame update
    void Start()
    {


    }

    void LateUpdate()
    {
        if(isFixed)
        {
            Vector3 rot = transform.rotation.eulerAngles;
            rot.x = fixRotX;
            transform.rotation = Quaternion.Euler(rot);
        }
    }
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Basket")
        {
            isFixed = false;
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag=="Slider")
        {
            isFixed = false;
        }
    }
}
