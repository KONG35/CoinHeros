using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Net;
using Unity.VisualScripting;
public class CoinLaunchMachine : MonoBehaviour
{
    //[SerializeField]
    //private Coin copperCoin;
    //[SerializeField]
    //private Coin silverCoin;
    //[SerializeField]
    //private Coin goldCoin;
    //[SerializeField]
    //private Coin diamondCoin;

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
    [Header("룰렛 보너스")]
    [SerializeField]
    private Transform[] bonusStartTr;
    [SerializeField]
    private Transform bonusApexTr;
    [SerializeField]
    private Transform bonusEndTr;


    private float swingAngle = 45f;
    private Quaternion initRot;
    private void Awake()
    {
        initRot = leftBarT.rotation;

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

        if (Input.GetKeyDown("1"))
        {
            InsertCoin(CoinEnum.Copper);
        }
        else if (Input.GetKeyDown("2"))
        {
            InsertCoin(CoinEnum.Silver);
        }
        else if (Input.GetKeyDown("3"))
        {
            InsertCoin(CoinEnum.Gold);
        }
        else if (Input.GetKeyDown("4"))
        {
            InsertCoin(CoinEnum.Diamond);
        }
    }
    [Button]
    private void InsertCoin(CoinEnum _cEnum)
    {
        var coin = CoinSpawnManager.Instance.GetCoin(_cEnum);
        coin.transform.position = launchPoint.position;
    }
    public void BonusCoin(BonusEnum _bonus, CoinEnum _coin)
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
            var rb = coin.GetComponent<Rigidbody>();

            rb.constraints = RigidbodyConstraints.None;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            coin.transform.position = bonusStartTr[idx].position;

            Vector3 v = CalculateVelocityFromThreePoints(bonusStartTr[idx].position, bonusApexTr.position, bonusEndTr.position);
            rb.velocity = v;
        }
    }
    
    private Vector3 CalculateVelocityFromThreePoints(Vector3 start, Vector3 apex, Vector3 end)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);

        // 1. 상승 시간
        float heightUp = apex.y - start.y;
        if (heightUp < 0.01f)
            heightUp = 0.01f;  // 최소 보정

        float timeToApex = Mathf.Sqrt(2f * heightUp / gravity);

        // 2. 하강 시간
        float heightDown = apex.y - end.y;
        if (heightDown < 0.01f)
            heightDown = 0.01f;

        float timeFromApex = Mathf.Sqrt(2f * heightDown / gravity);

        // 3. 총 체공 시간
        float totalTime = timeToApex + timeFromApex;

        // 4. 수평 방향 속도
        Vector3 displacementXZ = new Vector3(end.x - start.x, 0f, end.z - start.z);
        Vector3 velocityXZ = displacementXZ / totalTime;

        // 5. 수직 방향 속도 (중력 가속도에 맞게)
        float velocityY = gravity * timeToApex;

        return velocityXZ + Vector3.up * velocityY;
    }
    
}
