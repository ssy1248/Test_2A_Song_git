using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode keycoderun = KeyCode.LeftShift; //달리기 키
    [SerializeField]
    private KeyCode keycodeJump = KeyCode.Space; //점프 키
    [SerializeField]
    private KeyCode keycodeReload = KeyCode.R; //재장전 키

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioclipWalk;
    [SerializeField]
    private AudioClip audioclipRun;

    private RotateToMouse rotateToMouse; //마우스 이동 -> 카메라 회전
    private MovementCharacterController movement; //키보드 입력으로 캐릭터 이동
    private Status status; //이동속도 관련 플레이어 스테이터스
    private AudioSource audioSource;
    private WeaponBase weapon; //모든 무기가 상속받는 기반 클래스

    private void Awake()
    {
        //마우스 커서를 보이지 않게 설정, 현재 위치 고정
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

        //이동중
        if( x != 0 || z != 0)
        {
            bool isRun = false;

            //옆이나 뒤로 달리기 불가
            if (z > 0)
                isRun = Input.GetKey(keycoderun);

            movement.MoveSpeed = isRun == true ? status.RunSpeed : status.WalkSpeed;
            audioSource.clip = isRun == true ? audioclipRun : audioclipWalk;

            //방향키 입력 여부는 매 프레잉에서 확인 하기 때문에 재생중일때 다시 재생 안하도록 불값 활용
            if(audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        //제자리에 멈춰있을때
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
