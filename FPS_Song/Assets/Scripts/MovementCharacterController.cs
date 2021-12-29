using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementCharacterController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed; //�̵��ӵ�
    private Vector3 moveForce; //�̵� ��

    [SerializeField]
    private float jumpForce; //���� ��
    [SerializeField]
    private float gravityForce; //�߷� 
    public float MoveSpeed //�ܺο��� �Է��� �Ͽ� �޼ҵ� ����� ���� �� �� ����
    {
        set => moveSpeed = Mathf.Max(0, value); //�������� ������ �ȵǰ� ����
        get => moveSpeed;
    }

    private CharacterController characterController; //�÷��̾��� �̵� ��� ���� ĳ���� ��Ʈ�ѷ�

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        //�ٴڰ� ���� �ʾ����� �߷¸�ŭ y�� ����
        if(!characterController.isGrounded)
        {
            moveForce.y += gravityForce * Time.deltaTime;
        }

        //�ʴ� ����ŭ �̵�
        characterController.Move(moveForce * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction)
    {
        //�̵����� - ĳ������ ȸ�� * ����
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        //�̵� �� = �̵����� * �ӵ�
        moveForce = new Vector3(direction.x * moveSpeed, moveForce.y, direction.z * moveSpeed);
    }

    public void Jump()
    {
        //�÷��̾ �ٴڿ� ����������� ���� ����
        if(characterController.isGrounded)
        {
            moveForce.y = jumpForce;
        }
    }
}
