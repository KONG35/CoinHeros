using System.Collections;
using UnityEngine;

public enum CoinEnum
{
    Copper=0,
    Silver,
    Gold,
    Diamond,
    Count
}
public class Coin : MonoBehaviour, IPoolable
{
    [SerializeField]
    private PoolDataSO poolDataSO;
    public PoolDataSO PoolData => poolDataSO;

    [SerializeField] private CoinEnum coinEnum;
    public CoinEnum CoinEnum => coinEnum;
    public float impactThreshold = 2f; // 이 값 이상 충격이 오면 쓰러짐
    public float collapseRadius = 1f;  // 붕괴 반경
    private Rigidbody rigid;
    private Coroutine flyCor;
    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
        flyCor = null;
    }
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Basket"))
        {
            ResetRigidbody(); 
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag=="Slider")
        {
            ResetRigidbody(); 
        }
        else if(col.gameObject.tag=="Spin")
        {
            RouletteManager.Instance.InputCoin(coinEnum);
            CoinSpawnManager.Instance.ReturnCoin(PoolData, this);
        }
        else if (col.gameObject.tag == "Outside")
        {
            BattleManager.Instance.CharacterAction((int)coinEnum);
            CoinSpawnManager.Instance.ReturnCoin(PoolData, this);
        }
    }
    public void OnSpawn()
    {
        rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        rigid.velocity = new Vector3(0, -35f, 0);
    }

    public void OnDespawn()
    {
    }
    public void ResetRigidbody()
    {
        rigid.constraints = RigidbodyConstraints.None;
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }
    public void SetVelocity(Vector3 vec)
    {
        rigid.velocity = vec;
    }
    public bool IsResetRigidbody()
    {
        return rigid.constraints == RigidbodyConstraints.None;
    }
    public void StartFly(float _upTime)
    {
        flyCor = StartCoroutine(FlyCor(_upTime));
    }
    public void FinishFly(bool _isPlaced)
    {
        if (flyCor == null) return;

        StopCoroutine(flyCor);
        flyCor = null;

        if (_isPlaced)
        {
            rigid.rotation = Quaternion.identity;
        }
        ResetRigidbody();
    }
    IEnumerator FlyCor(float _upTime)
    {
        float height = Random.Range(14f, 16f);
        float margin = Random.Range(0.1f, 1f);
        Vector3 originPos = gameObject.transform.position; 
        Vector3 startPos = originPos + Vector3.up*height;
        Vector3 endPos = startPos + Vector3.up*margin;

        float upTime = _upTime;  // 올라가는 시간
        float duration = 3f; // 왕복 시간
        float totalTime = 20f; // 총 실행 시간
        float elapsed = 0f;
        
        while(elapsed < totalTime)
        {
            if(elapsed < upTime)
            {
                // 올라가기
                float t = elapsed / upTime;
                Vector3 pos = Vector3.Lerp(originPos, startPos, t);
                rigid.MovePosition(pos);
                
                yield return new WaitForFixedUpdate();
                elapsed += Time.fixedDeltaTime;
            }
            else
            {
                // 사인파를 사용해서 자연스러운 왕복
                float t = elapsed / duration;
                float sinValue = Mathf.Sin(t * Mathf.PI * 2f); // -1 ~ 1
                float pingPong = (sinValue + 1f) * 0.5f; // 0 ~ 1로 변환
                
                Vector3 pos = Vector3.Lerp(startPos, endPos, pingPong);
                rigid.MovePosition(pos);

                yield return new WaitForFixedUpdate();
                elapsed += Time.fixedDeltaTime;
            }
        }
        yield return null;
    }
}
