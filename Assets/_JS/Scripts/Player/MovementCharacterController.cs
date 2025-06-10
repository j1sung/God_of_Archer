using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementCharacterController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;    //이동속도
    private Vector3 moveForce;  //이동 힘

    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float gravity;

    public bool IsGrounded => characterController.isGrounded;

    public float MoveSpeed
    {
        set => moveSpeed = Mathf.Max(0, value);
        get => moveSpeed;
    }
    private CharacterController characterController; //플레이어 이동 제어를 위한 컴포넌트

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if(!characterController.isGrounded)
        {
            moveForce.y += gravity * Time.deltaTime;
        }

        //1초당 moveForce 속력으로 이동
        characterController.Move(moveForce * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction)
    {
        //이동 방향
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        //이동 힘
        moveForce = new Vector3(direction.x * moveSpeed, moveForce.y, direction.z * moveSpeed);
    }

    public void Jump()
    {
        if(characterController.isGrounded) 
        {
            moveForce.y = jumpForce;
        }
    }
}
