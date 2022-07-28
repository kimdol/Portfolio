using ARPG.Characters;
using ARPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Variables

    public float speed;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    public AudioClip shotSFX;
    public AudioClip hitSFX;

    protected bool collided;
    protected Rigidbody rigidbody;

    [HideInInspector]
    public AttackBehaviour attackBehaviour;

    [HideInInspector]
    public GameObject owner;

    [HideInInspector]
    public GameObject target;

    #endregion Variables

    protected virtual void Start()
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

        if (shotSFX != null && GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().PlayOneShot(shotSFX);
        }

        if (target != null)
        {
            Vector3 dest = target.transform.position;
            dest.y += 1.5f;
            transform.LookAt(dest);
        }
    }

    protected virtual void FixedUpdate()
    {
        if (speed != 0 && rigidbody != null)
        {
            rigidbody.position += (transform.forward) * (speed * Time.deltaTime);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
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

        ContactPoint contact = collision.contacts[0];
        Quaternion contactRotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 contactPosition = contact.point;

        if (hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, contactPosition, contactRotation) as GameObject;

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

        // 발사체가 캐릭터에 닿았을 때 데미지 처리함
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
        if (damagable != null)
        {
            damagable.TakeDamage(attackBehaviour?.damage ?? 0, owner, null);
        }

        StartCoroutine(DestroyParticle(0.1f));
    }

    public IEnumerator DestroyObject(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (transform.childCount > 0 && waitTime != 0)
        {
            List<Transform> childs = new List<Transform>();

            foreach (Transform t in transform.GetChild(0).transform)
            {
                childs.Add(t);
            }

            while (transform.GetChild(0).localScale.x > 0)
            {
                yield return new WaitForSeconds(0.1f);
                transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                for (int i = 0; i < childs.Count; i++)
                {
                    childs[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }

        Destroy(gameObject);
    }

    public IEnumerator DestroyParticle(float waitTime)
    {
        if (transform.childCount > 0 && waitTime != 0)
        {
            List<Transform> childs = new List<Transform>();

            foreach (Transform t in transform.GetChild(0).transform)
            {
                childs.Add(t);
            }

            while (transform.GetChild(0).localScale.x > 0)
            {
                yield return new WaitForSeconds(0.01f);
                transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                for (int i = 0; i < childs.Count; i++)
                {
                    childs[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }

        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
