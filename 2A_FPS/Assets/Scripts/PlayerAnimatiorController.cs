using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatiorController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        //"Player" 오브젝트 기준으로 자식 오브젝트인
        //"arms_assult" 오브젝트에 animator 컴포넌트가있다.
        animator = GetComponentInChildren<Animator>();
    }

    public float MoveSpeed
    {
        set => animator.SetFloat("movementSpeed", value);
        get => animator.GetFloat("movementSpeed");
    }

    //마우스 오른쪽 클릭 액션(default/aim mode)
    public bool AimModeIs
    {
        set => animator.SetBool("IsAimMode", value);
        get => animator.GetBool("IsAimMode");
    }

    public void OnReload()
    {
        animator.SetTrigger("OnReload"); //온리로드라는 트리거를 On시킨다
    }

    public void Play(string stateName, int layer, float normalizedTime)
    {
        animator.Play(stateName, layer, normalizedTime);      
    }

    public bool CurrentAnimationIs(string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name); //현재 재생중인지 확인하고 그 값을 반환
    }
}
