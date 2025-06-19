using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineBar : MonoBehaviour
{
    [SerializeField]
    private Transform barT;

    private float swingAngle = 45f;
    private Quaternion initRot;
    // Start is called before the first frame update
    void Awake()
    {
        initRot = barT.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        float angle = Mathf.Sin(Time.time * 1.5f) * swingAngle;

        Quaternion offset = Quaternion.AngleAxis(angle, Vector3.forward);
        barT.rotation = initRot * offset;
    }
}
