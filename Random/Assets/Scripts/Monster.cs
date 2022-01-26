using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public float StartHp;
    public float Hp;

    public GameObject DamageText;
    public GameObject TextPos;

    public GameObject HPBar;

    private void Start()
    {
        StartHp = 100f;
    }

    public void GetDamage(int damage)
    {
        GameObject dmgText = Instantiate(DamageText, TextPos.transform.position, TextPos.transform.rotation);
        dmgText.GetComponent<Text>().text = damage.ToString();

        Hp -= damage;
        HPBar.GetComponent<Image>().fillAmount = Hp / StartHp;

        Destroy(dmgText, 1f);
    }
}
