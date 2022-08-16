using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Actor : MonoBehaviour
{
    [SerializeField]
    protected bool isDead = false;

    public bool IsDead
    {
        get
        {
            return isDead;
        }
    }

    //protected int actorInstanceID = 0;

    //public int ActorInstanceID
    //{
    //    get
    //    {
    //        return actorInstanceID;
    //    }
    //}


    // Start is called before the first frame update
    protected virtual void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateActor();
    }

    protected virtual void UpdateActor()
    {

    }

    protected virtual void OnDead()
    {
        Debug.Log(name + " OnDead");
        isDead = true;
    }

    public void SetPosition(Vector3 position)
    {

    }

    public void CmdSetPosition(Vector3 position)
    {

    }

    public void RpcSetPosition(Vector3 position)
    {

    }

    public void RpcSetActive(bool value)
    {

    }

    public void RpcSetActorInstanceID(int instID)
    {

    }
}
