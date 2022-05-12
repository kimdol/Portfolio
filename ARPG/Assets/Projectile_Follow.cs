using ARPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Follow : Projectile
{
    public float destroyDelay = 5.0f;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(DestroyParticle(destroyDelay));
    }

    protected override void FixedUpdate()
    {
        if (target)
        {
            Vector3 dest = target.transform.position;
            dest.y += 1.5f;
            transform.LookAt(dest);
        }

        base.FixedUpdate();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }
}
