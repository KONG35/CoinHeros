using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class CoinLaunchMachine : MonoBehaviour
{
    [SerializeField]
    private Coin copperCoin;
    [SerializeField]
    private Coin silverCoin;
    [SerializeField]
    private Coin goldCoin;
    [SerializeField]
    private Coin diamondCoin;
    
    [Space(5)]
    [SerializeField]
    private Transform launchPointT;

    [SerializeField]
    private Transform leftBarT;
    [SerializeField]
    private Transform rightBarT;

    [SerializeField]
    private Transform leftPoint;
    [SerializeField]
    private Transform rightPoint;

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
        launchPointT.transform.position = (leftPoint.position + rightPoint.position) / 2f;

        if (Input.GetKeyDown("1"))
        {
            InsertCoin(1);
        }
        else if (Input.GetKeyDown("2"))
        {
            InsertCoin(2);
        }
        else if (Input.GetKeyDown("3"))
        {
            InsertCoin(3);
        }
        else if (Input.GetKeyDown("4"))
        {
            InsertCoin(4);
        }
    }
    [Button]
    private void InsertCoin(int num = 1)
    {
        switch(num)
        {
            case 1:
                {
                    var go = Instantiate(copperCoin.gameObject);
                    go.transform.position = launchPointT.position;
                }
                break;

            case 2:
                {
                    var go = Instantiate(silverCoin.gameObject);
                    go.transform.position = launchPointT.position;
                }
                break;

            case 3:
                {
                    var go = Instantiate(goldCoin.gameObject);
                    go.transform.position = launchPointT.position;

                }
                break;

            case 4:
                {
                    var go = Instantiate(diamondCoin.gameObject);
                    go.transform.position = launchPointT.position;

                }
                break;

        }

    }
}
