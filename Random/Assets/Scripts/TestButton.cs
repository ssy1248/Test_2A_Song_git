using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestButton : MonoBehaviour
{
    public GameObject TestObj;

    public void Attack()
    {
        TestObj.GetComponent<Tower>().Attack();
    }

    public void Idle()
    {
        TestObj.GetComponent<Tower>().Idle();
    }
}
