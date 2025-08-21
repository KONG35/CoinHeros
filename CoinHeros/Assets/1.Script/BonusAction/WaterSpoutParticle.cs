using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class WaterSpoutParticle : MonoBehaviour, IPoolable
{    
    [SerializeField] PoolDataSO poolData;
    public PoolDataSO PoolData => poolData;
    [SerializeField] private WaterSpoutCollider wsCollider;
    [Header("상승 시간은 duration의 절반")]
    [SerializeField] private float duration;
    [SerializeField] private float maxHeight = 5f; // 최대 높이
    [SerializeField] private float pushForce;
    private ParticleSystem particle;
    private Collider colCollider; 
    private Transform colTr;
    private Vector3 originLocalPos;
    private void Awake()
    {
        particle = gameObject.GetComponent<ParticleSystem>();
        colCollider = wsCollider.GetComponent<Collider>();
        colTr = wsCollider.gameObject.transform;

        originLocalPos = colTr.localPosition;
        
        wsCollider.SetForce(pushForce);
        
        // 초기에는 콜라이더 비활성화
        if (colCollider != null)
            colCollider.enabled = false;
    }
    
    [Button]
    public void Play()
    {
        particle.Play();
        StartCoroutine(WaterSpoutCoroutine());
    }
    
    private IEnumerator WaterSpoutCoroutine()
    {
        // 초기 설정
        colTr.localPosition = originLocalPos;
        
        // 상승 단계
        if (colCollider != null)
            colCollider.enabled = true;
            
        float elapsed = 0f;
        Vector3 targetPos = originLocalPos + Vector3.up * maxHeight;
        
        while (elapsed < duration * 0.5f) // 상승 시간은 전체 시간의 절반
        {
            float t = elapsed / (duration * 0.5f);
            // 부드러운 상승을 위해 easeOut 사용
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);
            colTr.localPosition = Vector3.Lerp(originLocalPos, targetPos, smoothT);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 최고점 도달
        colTr.localPosition = targetPos;
        
        // 하강 단계
        if (colCollider != null)
            colCollider.enabled = false;
            
        elapsed = 0f;
        while (elapsed < duration * 0.5f) // 하강 시간은 전체 시간의 절반
        {
            float t = elapsed / (duration * 0.5f);
            // 부드러운 하강을 위해 easeIn 사용
            float smoothT = Mathf.Pow(t, 3f);
            colTr.localPosition = Vector3.Lerp(targetPos, originLocalPos, smoothT);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 원래 위치로 복원
        colTr.localPosition = originLocalPos;
        ObjectManager.Instance.Return<WaterSpoutParticle>(poolData, this);
    }
    public void OnSpawn()
    {
        
    }

    public void OnDespawn()
    {
    }
}
