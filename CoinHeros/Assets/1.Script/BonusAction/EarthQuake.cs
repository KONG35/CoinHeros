using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class EarthQuake : BonusAction
{
    [Header("============= 지진 설정 ============")]
    [SerializeField]
    private Transform mainObjTr;
    
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeIntensity;
    [SerializeField] private float shakeFrequency;
    [SerializeField] private Vector3 offsetPosA;
    [SerializeField] private Vector3 offsetPosB;
    private Vector3 originalPosition;
    
    private void Awake()
    {
        if(mainObjTr)
            originalPosition = mainObjTr.position;
    }
    [Button]
    public override void Show()
    {
        StartCoroutine(EarthQuakeCor());
    }
    
    IEnumerator EarthQuakeCor()
    {
        if (mainObjTr == null)
        {
            Debug.LogWarning("mainObjTr이 설정되지 않았습니다!");
            yield break;
        }
        
        float elapsed = 0f;
        float frameTime = 1f / shakeFrequency;
        
        while (elapsed < shakeDuration)
        {
            // 시간에 따른 진동 강도 계산 (시간이 지날수록 약해짐)
            float currentIntensity = shakeIntensity * (1f - (elapsed / shakeDuration));
            
            // offsetPosA와 offsetPosB 사이에서 랜덤한 위치 계산
            float t = Random.Range(0f, 1f);
            Vector3 targetOffset = Vector3.Lerp(offsetPosA, offsetPosB, t);
            
            // 진동 강도를 적용하여 최종 위치 계산
            Vector3 randomOffset = targetOffset * currentIntensity;
            
            // 위치 업데이트
            mainObjTr.position = originalPosition + randomOffset;
            
            elapsed += frameTime;
            
            // 정확한 시간 간격으로 대기
            yield return new WaitForSeconds(frameTime);
        }
        
        // 원래 위치로 복원
        mainObjTr.position = originalPosition;
    }
}
