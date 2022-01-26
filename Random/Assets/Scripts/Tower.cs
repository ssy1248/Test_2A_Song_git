using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Animator anim;
    public GameObject Target;
    public float Range; //범위

    public GameObject Splash;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        UpdateTarget();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, Range);
    }

    //범위 내의 공격은 수정, 에니메이터 변경점도 수정
    void UpdateTarget() //타켓 업데이트 
    {
        if(Target == null)
        {
            GameObject[] Monsters = GameObject.FindGameObjectsWithTag("Monster");
            float shortest = Mathf.Infinity; //가장 짧은 거리
            GameObject nearestMonster = null; //가장 가까운 몬스터
            foreach(GameObject Monster in Monsters)
            {
                float DistanceToMonsters = Vector3.Distance(transform.position, Monster.transform.position);

                if(DistanceToMonsters < shortest)
                {
                    shortest = DistanceToMonsters;
                    nearestMonster = Monster;
                }

                if(nearestMonster != null && shortest <= Range)
                {
                    Target = nearestMonster;
                    Attack();
                }
                else
                {
                    Idle();
                    Target = null;
                }
            }
        }
    }

    public void SplashDamage()
    {
        Instantiate(Splash, transform.position, Quaternion.identity);
    }

    public void Attack()
    {
        anim.SetBool("IsAttack", true);
    }

    public void Idle()
    {
        anim.SetBool("IsAttack", false);
    }
}
