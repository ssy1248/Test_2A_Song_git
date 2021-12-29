using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode keycoderun = KeyCode.LeftShift; //�޸��� Ű
    [SerializeField]
    private KeyCode keycodeJump = KeyCode.Space; //���� Ű
    [SerializeField]
    private KeyCode keycodeReload = KeyCode.R; //������ Ű

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioclipWalk;
    [SerializeField]
    private AudioClip audioclipRun;

    private RotateToMouse rotateToMouse; //���콺 �̵� -> ī�޶� ȸ��
    private MovementCharacterController movement; //Ű���� �Է����� ĳ���� �̵�
    private Status status; //�̵��ӵ� ���� �÷��̾� �������ͽ�
    private AudioSource audioSource;
    private WeaponBase weapon; //��� ���Ⱑ ��ӹ޴� ��� Ŭ����

    private void Awake()
    {
        //���콺 Ŀ���� ������ �ʰ� ����, ���� ��ġ ����
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateToMouse>();
        movement = GetComponent<MovementCharacterController>();
        status = GetComponent<Status>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        UpdateRot();
        UpdateMove();
        UpdateJump();
        UpdateWeaponAction();
    }

    private void UpdateRot()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }

    private void UpdateMove()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //�̵���
        if( x != 0 || z != 0)
        {
            bool isRun = false;

            //���̳� �ڷ� �޸��� �Ұ�
            if (z > 0)
                isRun = Input.GetKey(keycoderun);

            movement.MoveSpeed = isRun == true ? status.RunSpeed : status.WalkSpeed;
            audioSource.clip = isRun == true ? audioclipRun : audioclipWalk;

            //����Ű �Է� ���δ� �� �����׿��� Ȯ�� �ϱ� ������ ������϶� �ٽ� ��� ���ϵ��� �Ұ� Ȱ��
            if(audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        //���ڸ��� ����������
        else
        {
            movement.MoveSpeed = 0;

            if(audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
        }

        movement.MoveTo(new Vector3(x, 0, z));
    }

    private void UpdateJump()
    {
        if(Input.GetKey(keycodeJump))
        {
            movement.Jump();
        }
    }

    private void UpdateWeaponAction()
    {
        if(Input.GetMouseButtonDown(0))
        {
            weapon.StartWeaponAction();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            weapon.StopWeaponAction();
        }

        if(Input.GetMouseButtonDown(1))
        {
            weapon.StartWeaponAction(1);
        }
        else if(Input.GetMouseButtonUp(1))
        {
            weapon.StopWeaponAction(1);
        }

        if(Input.GetKeyDown(keycodeReload))
        {
            weapon.StartReload();
        }
    }

    public void TakeDamage(int damage)
    {
        bool isDie = status.DecreaseHP(damage);

        if(isDie == true)
        {
            Debug.Log("Game Over");
        }
    }

    public void SwitchWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;
    }

}
