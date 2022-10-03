using ARPG.Characters;
using ARPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_RandomMove : Projectile
{
    [Header("Range Settings")]
    public float pbaeRangeRadius = 10f;

    [Range(0, 360)]
    public float moveAngle = 20f;
    [SerializeField]
    private float changeInterval;
    protected float calcCoolTime = 0.0f;
    public int index = 0;

    public float destroyDelay = 5.0f;

    protected override void Start()
    {
        if (owner != null)
        {
            Collider projectileCollider = GetComponent<Collider>();
            Collider[] ownerColliders = owner.GetComponentsInChildren<Collider>();
            foreach (Collider collider in ownerColliders)
            {
                Physics.IgnoreCollision(projectileCollider, collider);
            }
        }

        rigidbody = GetComponent<Rigidbody>();

        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
            ParticleSystem particleSystem = muzzleVFX.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                Destroy(muzzleVFX, particleSystem.main.duration);
            }
            else
            {
                ParticleSystem childParticleSystem = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, childParticleSystem.main.duration);
            }
        }

        if (shotSFX != null && GetComponent<AudioSource>() && index == 0)
        {
            GetComponent<AudioSource>().PlayOneShot(shotSFX);
        }

        StartCoroutine(DestroyObject(destroyDelay));
    }

    protected override void FixedUpdate()
    {
        ChangeRandomAngle();

        base.FixedUpdate();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collided)
        {
            return;
        }

        Collider projectileCollider = GetComponent<Collider>();
        projectileCollider.enabled = false;

        collided = true;

        if (hitSFX != null && GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().PlayOneShot(hitSFX);
        }

        speed = 0;
        rigidbody.isKinematic = true;

        LayerMask targetMask = owner.gameObject.GetComponent<AttackBehaviour>().targetMask;
        Collider[] targetsInViewRadius = Physics.OverlapSphere(collision.transform.position, pbaeRangeRadius, targetMask);
        
        foreach (Collider target in targetsInViewRadius)
        {
            if (hitPrefab != null)
            {
                var hitVFX = Instantiate(hitPrefab, target.transform) as GameObject;

                ParticleSystem particleSystem = hitVFX.GetComponent<ParticleSystem>();
                if (particleSystem == null)
                {
                    ParticleSystem childParticleSystem = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                    Destroy(hitVFX, childParticleSystem.main.duration);
                }
                else
                {
                    Destroy(hitVFX, particleSystem.main.duration);
                }
            }

            IDamagable damagable = target.gameObject.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(attackBehaviour?.damage ?? 0, owner, null);
            }
        }
        
        StartCoroutine(DestroyParticle(1.0f));
    }

    public void ChangeRandomAngle()
    {
        if (calcCoolTime < changeInterval)
        {
            calcCoolTime += Time.deltaTime;
        }

        if (calcCoolTime >= changeInterval)
        {
            int randomSign = Random.Range(0, 3) - 1;
            Vector3 rotatedRandomVector = new Vector3(0, moveAngle * randomSign, 0);
            transform.localEulerAngles += rotatedRandomVector;

            calcCoolTime = 0.0f;
        }
    }
}
