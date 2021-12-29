using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatiorController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        //"Player" ������Ʈ �������� �ڽ� ������Ʈ��
        //"arms_assult" ������Ʈ�� animator ������Ʈ���ִ�.
        animator = GetComponentInChildren<Animator>();
    }

    public float MoveSpeed
    {
        set => animator.SetFloat("movementSpeed", value);
        get => animator.GetFloat("movementSpeed");
    }

    //���콺 ������ Ŭ�� �׼�(default/aim mode)
    public bool AimModeIs
    {
        set => animator.SetBool("IsAimMode", value);
        get => animator.GetBool("IsAimMode");
    }

    public void OnReload()
    {
        animator.SetTrigger("OnReload"); //�¸��ε��� Ʈ���Ÿ� On��Ų��
    }

    public void Play(string stateName, int layer, float normalizedTime)
    {
        animator.Play(stateName, layer, normalizedTime);      
    }

    public bool CurrentAnimationIs(string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name); //���� ��������� Ȯ���ϰ� �� ���� ��ȯ
    }
}
