using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllorCharacter : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private float speed = 5f;

    private CharacterController characterController;
    private Vector3 calcVelocity = Vector3.zero;


    [SerializeField]
    private float jumpHeight = 2f;

    [SerializeField]
    private float gravity = -9.81f;

    [SerializeField]
    private LayerMask groundLayerMask;

    [SerializeField]
    private float dashDistance = 5f;

    [SerializeField]
    private Vector3 drags;  // 저항값

    [SerializeField]
    private float groundCheckDistance = 0.2f;

    private bool isGrounded = true;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check ground!!
        bool isGrounded = characterController.isGrounded;
        if (isGrounded && calcVelocity.y < 0)
        {
            // 멈춰!
            calcVelocity.y = 0f;
        }

        // 사용자 입력값을 받아오고, 처리(이동)
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        characterController.Move(move * Time.deltaTime * speed);
        if (move != Vector3.zero)
        {
            transform.forward = move;
        }

        // 사용자 입력값을 받아오고, 처리(쩜프)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            calcVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 사용자 입력값을 받아오고, 처리(대쉬)
        if (Input.GetButtonDown("Dash"))
        {
            calcVelocity += Vector3.Scale(
                transform.forward, dashDistance * new Vector3(
                    (Mathf.Log(1f / (Time.deltaTime * drags.x + 1)) / -Time.deltaTime),
                0,
                (Mathf.Log(1f / (Time.deltaTime * drags.z + 1)) / -Time.deltaTime)
                    )
                );

        }

        // 중력 적용 처리
        calcVelocity.y += gravity * Time.deltaTime;

        // 저항값 처리
        calcVelocity.x /= 1 + drags.x * Time.deltaTime;
        calcVelocity.y /= 1 + drags.y * Time.deltaTime;
        calcVelocity.z /= 1 + drags.z * Time.deltaTime;

        characterController.Move(calcVelocity * Time.deltaTime);
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
