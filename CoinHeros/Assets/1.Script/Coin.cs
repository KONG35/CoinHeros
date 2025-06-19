using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    Rigidbody rigid;
    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        rigid.AddForce(Vector3.down * 10f, ForceMode.Acceleration);
    }
    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Slider")
        {
            rigid.constraints = RigidbodyConstraints.None;
        }
    }
}
