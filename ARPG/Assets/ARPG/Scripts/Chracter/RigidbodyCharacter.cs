using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyCharacter : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private float speed = 5f;

    private new Rigidbody rigidbody;
    private Vector3 inputDirection = Vector3.zero;

    [SerializeField]
    private float jumpHeight = 2f;

    [SerializeField]
    private float groundCheckDistance = 0.2f;

    [SerializeField]
    private LayerMask groundLayerMask;

    private bool isGrounded = false;

    [SerializeField]
    private float dashDistance = 5f;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>(); 
    }

    // Update is called once per frame
    void Update()
    {
        CheckGroundStatus();

        // 사용자 입력값을 받아오고, 처리(이동)
        inputDirection = Vector3.zero;
        inputDirection = Vector3.zero;
        inputDirection.x = Input.GetAxis("Horizontal");
        inputDirection.z = Input.GetAxis("Vertical");
        if (inputDirection != Vector3.zero)
        {
            transform.forward = inputDirection;
        }

        // 사용자 입력값을 받아오고, 처리(쩜프)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rigidbody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }

        // 사용자 입력값을 받아오고, 처리(대쉬)
        if (Input.GetButtonDown("Dash"))
        {
            Vector3 dashVelocity = Vector3.Scale(transform.forward, dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * rigidbody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * rigidbody.drag + 1)) / -Time.deltaTime)));
            rigidbody.AddForce(dashVelocity, ForceMode.VelocityChange);
        }
    }
    // Fixed Timestep 기반으로 호출되는 함수
    private void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + inputDirection * speed * Time.fixedDeltaTime);
    }
    #region Helper Methods
    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // Ground check Ray
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), 
            transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif
        // Check ground!
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), 
            Vector3.down, out hitInfo, groundCheckDistance, groundLayerMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    #endregion Helper Methods

}
