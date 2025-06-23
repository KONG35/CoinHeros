using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusBasket : MonoBehaviour
{
    public float startX = -30f;     // a값
    public float endX = -12f;        // b값

    private float period = 3f;  // 편도에 걸리는 시간 (속도 조절용)

    private float timer = 0f;
    Rigidbody rig;
    public void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;
        float t = Mathf.PingPong(timer / period, 1f);
        float x = Mathf.SmoothStep(startX, endX, t);
        rig.MovePosition(new Vector3(x, transform.position.y, transform.position.z));
    }

}
