using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusBasket : MonoBehaviour
{
    public float startX = -30f;     // a값
    public float endX = -12f;        // b값

    public float period = 6f;  // 왕복에 걸리는 전체 시간 (속도 조절용)

    private float timer = 0f;
    Rigidbody rig;
    public void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;
        float normalizedTime = Mathf.PingPong(timer / period, 1f);  // 0~1 왕복

        // 양 끝에서 천천히, 중간에서 빠르게 움직이게 만듦
        float t = Mathf.SmoothStep(0f, 1f, normalizedTime);

        float x = Mathf.Lerp(startX, endX, t);

        rig.MovePosition(new Vector3(x, transform.position.y, transform.position.z));
    }
}
