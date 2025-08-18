using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Net;
using Unity.VisualScripting;
using UnityEngine.Purchasing;
public class CoinLaunchMachine : MonoBehaviour
{
    
    [Space(5)]
    [SerializeField]
    private Transform launchPoint;

    [SerializeField]
    private Transform leftBarT;
    [SerializeField]
    private Transform rightBarT;

    [SerializeField]
    private Transform leftPoint;
    [SerializeField]
    private Transform rightPoint;


    [Space(5)]
    [Header("보너스 코인 던지는 위치 그룹")]
    [SerializeField] private Transform[] bonusStartTr;

    [Space(5)]
    [Header("보너스 코인 떨어지는 위치 그룹")]
    [SerializeField] private Transform bonusApexTr;
    [SerializeField] private Transform bonusEndTr;

    private List<Vector3> velocityPoints;
    private float swingAngle = 45f;
    private Quaternion initRot;
    
    private void Awake()
    {
        initRot = leftBarT.rotation;
        InitVelocity();
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        float angle = Mathf.Sin(Time.time * 1.5f) * swingAngle;

        Quaternion offset = Quaternion.AngleAxis(angle, Vector3.forward);
        leftBarT.rotation = initRot * offset;
        rightBarT.rotation = initRot * offset;
        launchPoint.transform.position = (leftPoint.position + rightPoint.position) / 2f;

    }
    [Button]
    public void InsertCoin(CoinEnum _cEnum)
    {
        var coin = CoinSpawnManager.Instance.GetCoin(_cEnum);
        coin.transform.position = launchPoint.position;
    }
    public void ShowBonusCoin(BonusEnum _bonus, CoinEnum _coin)
    {
        int count = 0;
        switch (_bonus)
        {
            case BonusEnum.Coin3:
                count = 3;
                break;
            case BonusEnum.Coin6:
                count = 6;
                break;
            case BonusEnum.Coin9:
                count = 9;
                break;
        }
        int temp = (int)Random.Range(0, bonusStartTr.Length);
        for (int i = 0; i < count; i++)
        {
            int idx = (temp + i) % bonusStartTr.Length;
            var coin = CoinSpawnManager.Instance.GetCoin(_coin);
            coin.ResetRigidbody();

            coin.transform.position = bonusStartTr[idx].position;

            coin.SetVelocity(velocityPoints[idx]);
        }
    }
    private void InitVelocity()
    {
        velocityPoints = new List<Vector3>();
        for(int i=0;i<bonusStartTr.Length;i++)
        {
            velocityPoints.Add(CalculateVelocityFromThreePoints(bonusStartTr[i].position, bonusApexTr.position, bonusEndTr.position));
        }
    }
    private Vector3 CalculateVelocityFromThreePoints(Vector3 start, Vector3 apex, Vector3 end)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);

        // end 위치의 x값을 일정 범위에서 랜덤하게 조정
        float randomXOffset = Random.Range(-5f, 5f);
        Vector3 randomizedEnd = new Vector3(end.x + randomXOffset, end.y, end.z);

        // 1. 올라가는 시간
        float heightUp = apex.y - start.y;
        if (heightUp < 0.01f)
            heightUp = 0.01f;  // 최소 높이

        float timeToApex = Mathf.Sqrt(2f * heightUp / gravity);

        // 2. 내려가는 시간
        float heightDown = apex.y - randomizedEnd.y;
        if (heightDown < 0.01f)
            heightDown = 0.01f;

        float timeFromApex = Mathf.Sqrt(2f * heightDown / gravity);

        // 3. 전체 비행 시간
        float totalTime = timeToApex + timeFromApex;

        // 4. 수평 방향 속도
        Vector3 displacementXZ = new Vector3(randomizedEnd.x - start.x, 0f, randomizedEnd.z - start.z);
        Vector3 velocityXZ = displacementXZ / totalTime;

        // 5. 수직 방향 속도 (중력 가속도로부터 역산)
        float velocityY = gravity * timeToApex;

        return velocityXZ + Vector3.up * velocityY;
    }
    
}
