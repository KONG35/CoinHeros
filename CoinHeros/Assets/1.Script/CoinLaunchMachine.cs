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

    [SerializeField]
    private Transform launchPoint;

    [SerializeField]
    private Transform leftBar;
    [SerializeField]
    private Transform rightBar;
    private void Awake()
    {
        
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
    [Button]
    private void InsertCoin()
    {
        Instantiate()
    }
}
