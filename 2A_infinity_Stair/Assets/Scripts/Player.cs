using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator animator; //캐릭터가 죽거나 이동관련 애니메이터
    public bool isleft = true; //캐릭터가 바라보는 방향을 위한 bool값
    public bool isDie = false; //캐릭터가 죽었는지 체크하는 bool값
    public int characterIndex; //캐릭터 추가를 위해 무슨 캐릭터가 선택되었는지 체크를 위한 변수
    public int stairIndex; //계단
    public int money; //코인

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Climb(bool isChange)
    {
        
    }
}
