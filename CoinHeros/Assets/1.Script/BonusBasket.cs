using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusBasket : MonoBehaviour
{
    public float startX = -30f;     // a��
    public float endX = -12f;        // b��

    public float period = 6f;  // �պ��� �ɸ��� ��ü �ð� (�ӵ� ������)

    private float timer = 0f;
    Rigidbody rig;
    public void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;
        float normalizedTime = Mathf.PingPong(timer / period, 1f);  // 0~1 �պ�

        // �� ������ õõ��, �߰����� ������ �����̰� ����
        float t = Mathf.SmoothStep(0f, 1f, normalizedTime);

        float x = Mathf.Lerp(startX, endX, t);

        rig.MovePosition(new Vector3(x, transform.position.y, transform.position.z));
    }
}
