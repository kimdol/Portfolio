using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Animator))]
public class PlayerCharacter : MonoBehaviour
{
    #region Variables

    private CharacterController controller;
    [SerializeField]
    private LayerMask groundLayerMask;

    private NavMeshAgent agent;
    private Camera camera;

    [SerializeField]
    private Animator animator;

    readonly int moveHash = Animator.StringToHash("Move");

    #endregion

    #region Main Methods
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();

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
                // Debug.Log("We hit " + hit.collider.name + " " + hit.point);

                // Hit한 곳에 이동
                agent.SetDestination(hit.point);
            }
        }

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            controller.Move(agent.velocity * Time.deltaTime);
            animator.SetBool(moveHash, true);
        }
        else
        {
            controller.Move(Vector3.zero);
            animator.SetBool(moveHash, false);
        }


    }

    private void OnAnimatorMove()
    {
        Vector3 position = agent.nextPosition;
        animator.rootPosition = agent.nextPosition;
        transform.position = position;
    }
    #endregion Main Methods

}
