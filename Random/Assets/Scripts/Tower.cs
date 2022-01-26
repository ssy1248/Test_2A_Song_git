using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Animator anim;
    public GameObject Target;
    public float Range; //����

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

    //���� ���� ������ ����, ���ϸ����� �������� ����
    void UpdateTarget() //Ÿ�� ������Ʈ 
    {
        if(Target == null)
        {
            GameObject[] Monsters = GameObject.FindGameObjectsWithTag("Monster");
            float shortest = Mathf.Infinity; //���� ª�� �Ÿ�
            GameObject nearestMonster = null; //���� ����� ����
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
