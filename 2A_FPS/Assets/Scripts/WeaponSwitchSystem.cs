using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitchSystem : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private PlayerHUD playerHUD;

    [SerializeField]
    private WeaponBase[] weapons; //�������� ���� 4����

    private WeaponBase currentWeapon; //���� �������� ����
    private WeaponBase previousWeapon; //���� ������ ����

    private void Awake()
    {
        //���� ���� ����� ���� ���� �������� ��� ���� �̺�Ʈ ���
        playerHUD.SetUpAllWeapons(weapons);

        //���� �������� ��� ���⸦ ������ �ʰ� ����
        for (int i = 0; i < weapons.Length; ++i)
        {
            if (weapons[i].gameObject != null)
            {
                weapons[i].gameObject.SetActive(false);
            }
        }

        //Main ���⸦ ���� ��� ����� ����
        SwitchingWeapon(WeaponType.Main);
    }

    private void Update()
    {
        UpdateSwitch();
    }

    private void UpdateSwitch()
    {
        if (!Input.anyKeyDown)
            return;

        //1~4����Ű�� ������ ���� ��ü
        int inputIndex = 0;
        if (int.TryParse(Input.inputString, out inputIndex) && (inputIndex > 0 && inputIndex < 5))
        {
            SwitchingWeapon((WeaponType)(inputIndex - 1));
        }
    }

    private void SwitchingWeapon(WeaponType weaponType)
    {
        //��ü ������ ���Ⱑ ������ ����
        if (weapons[(int)weaponType] == null)
        {
            return;
        }

        //���� ������� ���Ⱑ ������ ���� ���� ������ ����
        if (currentWeapon != null)
        {
            previousWeapon = currentWeapon;
        }

        //���� ��ü
        currentWeapon = weapons[(int)weaponType];

        //���� ������� ����� ��ü�Ϸ��� �� �� ����
        if (currentWeapon == previousWeapon)
        {
            return;
        }

        //���⸦ ����ϴ� �÷��̾���Ʈ�ѷ�, �÷��̾� hud �� ���� ���� ���� ����
        playerController.SwitchWeapon(currentWeapon);
        playerHUD.SwitchWeapon(currentWeapon);

        //������ ����ϴ� ���� ��Ȱ��ȭ
        if (previousWeapon != null)
        {
            previousWeapon.gameObject.SetActive(false);
        }

        //���� ����ϴ� ���� Ȱ��ȭ
        currentWeapon.gameObject.SetActive(true);
    }
}

