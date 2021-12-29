using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementCharacterController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed; //이동속도
    private Vector3 moveForce; //이동 힘

    [SerializeField]
    private float jumpForce; //점프 힘
    [SerializeField]
    private float gravityForce; //중력 
    public float MoveSpeed //외부에서 입력을 하여 메소드 사용을 위한 겟 셋 생성
    {
        set => moveSpeed = Mathf.Max(0, value); //음수값이 적용이 안되게 설정
        get => moveSpeed;
    }

    private CharacterController characterController; //플레이어의 이동 제어를 위한 캐릭터 컨트롤러

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        //바닥과 닿지 않았으면 중력만큼 y축 감소
        if(!characterController.isGrounded)
        {
            moveForce.y += gravityForce * Time.deltaTime;
        }

        //초당 힘만큼 이동
        characterController.Move(moveForce * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction)
    {
        //이동방향 - 캐릭터의 회전 * 방향
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        //이동 힘 = 이동방향 * 속도
        moveForce = new Vector3(direction.x * moveSpeed, moveForce.y, direction.z * moveSpeed);
    }

    public void Jump()
    {
        //플레이어가 바닥에 닿아있을때만 점프 가능
        if(characterController.isGrounded)
        {
            moveForce.y = jumpForce;
        }
    }
}
