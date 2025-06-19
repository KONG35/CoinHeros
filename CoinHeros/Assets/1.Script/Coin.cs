using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    Rigidbody rigid;

    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
        //rigid.AddForce(Vector3.down * 50f, ForceMode.Force);
        rigid.velocity = new Vector3(0, -35f, 0);
    }
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        //rigid.AddForce(Vector3.down * 10f, ForceMode.Acceleration);
    }
    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Slider")
        {
            rigid.constraints = RigidbodyConstraints.None;

            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
        else if (col.gameObject.tag == "Basket")
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
}
