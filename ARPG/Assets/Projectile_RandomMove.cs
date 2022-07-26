using ARPG.Characters;
using ARPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_RandomMove : Projectile
{
    [Header("Range Settings")]
    public float rangeRadius = 5f;

    [Range(0, 360)]
    public float moveAngle = 20f;

    [SerializeField]
    private float coolTime;

    protected float calcCoolTime = 0.0f;

    public float destroyDelay = 5.0f;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(DestroyParticle(destroyDelay));
    }

    protected override void FixedUpdate()
    {
        ChangeRandomAngle();

        base.FixedUpdate();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
        if (damagable == null)
        {
            return;
        }

        LayerMask targetMask = owner.gameObject.GetComponent<AttackBehaviour>().targetMask;
        
        Collider[] targetsInViewRadius = Physics.OverlapSphere(collision.transform.position, rangeRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;

            Vector3 dirToTarget = (target.position - transform.position).normalized;
            
        }
    }

    public void ChangeRandomAngle()
    {
        if (calcCoolTime < coolTime)
        {
            calcCoolTime += Time.deltaTime;
        }

        if (calcCoolTime >= coolTime)
        {
            int randomSign = Random.Range(0, 3) - 1;
            transform.localEulerAngles = new Vector3(0, moveAngle * randomSign, 0);

            calcCoolTime = 0.0f;
        }
    }
}
