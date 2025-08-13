using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 물체를 밀어내기 위한 콜라이더
public class WaterSpoutCollider : MonoBehaviour
{
    private float pushForce;
    // 물체와 충돌했을 때 밀어내는 기능
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody otherRigid = other.GetComponent<Rigidbody>();
        if (otherRigid != null && other.tag == "Coin")
        {
            // 위쪽으로 밀어내기
            Vector3 pushDirection = Vector3.up + Random.insideUnitSphere * 0.3f; // 약간의 랜덤성 추가
            pushDirection.Normalize();
            otherRigid.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
    }
    public void SetForce(float force)
    {
        pushForce = force;
    }

}
