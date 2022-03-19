using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyCharacter : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private float speed = 5f;

    private Rigidbody rigidbody;
    private Vector3 inputDirection = Vector3.zero;

    [SerializeField]
    private float jumpHeight = 2f;

    [SerializeField]
    private float groundCheckDistance = 0.2f;

    [SerializeField]
    private LayerMask groundLayerMask;

    private bool isGrounded = true;

    [SerializeField]
    private float dashDistance = 5f;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
