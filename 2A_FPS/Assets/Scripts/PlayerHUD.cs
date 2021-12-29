using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    private WeaponBase weapon; //���� ������ ��µǴ� ����

    [Header("Components")]
    [SerializeField]
    private Status status; //�÷��̾��� ����(�̵��ӵ�, ü��)

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI textWeaponName; //�����̸�
    [SerializeField]
    private Image imageWeaponIcon; //���� ������
    [SerializeField]
    private Sprite[] spriteWeaponIcons; //���� �����ܿ� ���Ǵ� ��������Ʈ �迭
    [SerializeField]
    private Vector2[] sizeWeaponIcons; //���� �������� UIũ�� �迭

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI textAmmo; //���� ź��/ �ִ�ź�� ���

    [Header("Magazine")]
    [SerializeField]
    private GameObject magazineUIPrefab; //źâ ui ������
    [SerializeField]
    private Transform magazineParent; //źâ ui�� ��ġ�Ǵ� panel
    [SerializeField]
    private int maxMagazineCount; //ó�� �����ϴ� �ִ� źâ ��

    private List<GameObject> magazineList; //źâ ui ����Ʈ

    [Header("HP & BloodScreen UI")]
    [SerializeField]
    private TextMeshProUGUI textHP; //�÷��̾��� ü���� ����ϴ� Text
    [SerializeField]
    private Image imageBloodScreen; //�÷��̾ ���ݹ޾��� �� ȭ�鿡 ǥ�õǴ� image
    [SerializeField]
    private AnimationCurve cureveBloodScreen;

    private void Awake()
    {
        status.onHPEvent.AddListener(UpdateHPHUD);
    }

    public void SetUpAllWeapons(WeaponBase[] weapons)
    {
        SetUpMagazine();

        //��밡���� ��� ������ �̺�Ʈ ���
        for(int i = 0;  i < weapons.Length; ++i)
        {
            weapons[i].onAmmoEvent.AddListener(UpdateAmmoHUD);
            weapons[i].onMagazineEvent.AddListener(UpdateMagazineHUD);
        }
    }

    public void SwitchWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;

        SetUpWeapon();
    }

    private void SetUpWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName];
        imageWeaponIcon.rectTransform.sizeDelta = sizeWeaponIcons[(int)weapon.WeaponName];
    }

    private void SetUpMagazine()
    {
        //waepon�� ��ϵǾ� �ִ� �ִ� źâ ������ŭ image icon�� ����
        //magazineParent ������Ʈ�� �ڽ����� ��� �� ��� ��Ȱ��ȭ/����Ʈ�� ����
        magazineList = new List<GameObject>();
        for(int i = 0; i < maxMagazineCount; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazineParent);


            clone.SetActive(false);

            magazineList.Add(clone);
        }
    }

    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}";
    }

    private void UpdateMagazineHUD(int currentMagazine)
    {
        for(int i = 0; i < magazineList.Count; ++i)
        {
            magazineList[i].SetActive(false);
        }
        for(int i = 0; i < currentMagazine; ++i)
        {
            magazineList[i].SetActive(true);
        }
    }

    private void UpdateHPHUD(int previous, int current)
    {
        textHP.text = "HP " + current;

        //ü���� �������� ���� ȭ�鿡 �߰��� �̹����� ������� �ʵ��� return
        if (previous <= current)
            return;

        if(previous - current > 0)
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }
    }

    private IEnumerator OnBloodScreen()
    {
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1, 0, cureveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;

            yield return null;
        }
    }
}
