using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class AgentControllorCharacter : MonoBehaviour
{
    #region Variables
    private CharacterController characterController;

    [SerializeField]
    private LayerMask groundLayerMask;

    private NavMeshAgent agent;
    private new Camera camera;

    [SerializeField]
    private float groundCheckDistance = 0.2f;

    private bool isGrounded = false;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();

        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = true;

        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // 사용자 클릭 입력값을 받아온다
        if (Input.GetMouseButtonDown(0))
        {
            // Screen pos에서 world pos으로 변경
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            // Check hit!
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, groundLayerMask))
            {
                Debug.Log("We hit " + hit.collider.name + " " + hit.point);

                // Hit한 곳에 이동
                agent.SetDestination(hit.point);
            }
        }
        // 아직 가야할 거리가 남을 때 이동시킴
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            characterController.Move(agent.velocity * Time.deltaTime);
        }
        else
        {
            characterController.Move(Vector3.zero);
        }
    }

    private void LateUpdate()
    {
        transform.position = agent.nextPosition;
    }
}
