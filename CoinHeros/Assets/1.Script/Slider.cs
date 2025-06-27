using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Slider : MonoBehaviour
{
    [Header("Position Z Settings")]
    [SerializeField]
    private float startZ;
    [SerializeField]
    private float endZ;

    private float duration = 2f;  // 편도에 걸리는 시간 (속도 조절용)
    private float stopDuration = 0.5f; // 양 끝에서 멈추는 시간

    private Vector3 startpos;
    private Vector3 endpos;
    Rigidbody rig;
    public void Awake()
    {
        rig = GetComponent<Rigidbody>();
        startpos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, startZ);
        endpos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, endZ);
    }
    private void Start()
    {
        StartCoroutine(MoveLoop());
        
    }

    private IEnumerator MoveLoop()
    {
        while (true)
        {
            // 이동: start → end
            yield return MoveOverTime(startpos, endpos, duration);

            // 정지
            yield return new WaitForSeconds(stopDuration);

            // 이동: end → start
            yield return MoveOverTime(endpos, startpos, duration);

            // 정지
            yield return new WaitForSeconds(stopDuration);
        }
    }
    private IEnumerator MoveOverTime(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        Vector3 pos = Vector3.zero;


        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t)); // Clamp 보정

            pos = Vector3.Lerp(from, to, t);
            rig.MovePosition(pos);

            yield return new WaitForFixedUpdate();
            elapsed += Time.fixedDeltaTime;

            Debug.Log(rig.velocity.magnitude);
        }

        // 정확히 도착지점으로 보정
        rig.MovePosition(to);
    }
}
