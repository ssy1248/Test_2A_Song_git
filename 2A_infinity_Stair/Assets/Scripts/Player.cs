using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator animator; //ĳ���Ͱ� �װų� �̵����� �ִϸ�����
    public bool isleft = true; //ĳ���Ͱ� �ٶ󺸴� ������ ���� bool��
    public bool isDie = false; //ĳ���Ͱ� �׾����� üũ�ϴ� bool��
    public int characterIndex; //ĳ���� �߰��� ���� ���� ĳ���Ͱ� ���õǾ����� üũ�� ���� ����
    public int stairIndex; //���
    public int money; //����

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Climb(bool isChange)
    {
        
    }
}
