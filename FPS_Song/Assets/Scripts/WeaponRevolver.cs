using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRevolver : WeaponBase
{
    [Header("Fire Effects")]
    [SerializeField]
    private GameObject muzzleFlashEffect; //�ѱ� ȭ��

    [Header("Spawn Points")]
    [SerializeField]
    private Transform bulletSpawnPoint; //�Ѿ� ���� ��ġ

    [Header("Audio clips")]
    [SerializeField]
    private AudioClip audioClipFire; //���� ����
    [SerializeField]
    private AudioClip audioClipReload; //��������

    private ImpactMemoryPool impactMemoryPool; //���� ȿ�� ���� �� Ȱ��/��Ȱ�� ����
    private Camera mainCamera; //���� �߻�

    private void OnEnable()
    {
        //�ѱ� ����Ʈ ������Ʈ ��Ȱ��ȭ
        muzzleFlashEffect.SetActive(false);

        //���Ⱑ Ȱ��ȭ �ɶ� �ش� ������ źâ ������ �����Ѵ�.
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);
        //���Ⱑ Ȱ��ȭ�� �� �ش� ������ ź �� ������ �����Ѵ�.
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

        ResetVariable();
    }

    private void Awake()
    {
        base.SetUp();

        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;

        //ó�� źâ ���� �ִ�� ����
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        //ó�� ź ���� �ִ�� ����
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    public override void StartWeaponAction(int type = 0)
    {
        if (type == 0 && isAttack == false && isReload == false)
        {
            OnAttack();
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
        isAttack = false;
    }

    public override void StartReload()
    {
        //���� ������ ���̰ų� źâ ���� 0�̸� ������ �Ұ���
        if (isReload == true || weaponSetting.currentMagazine <= 0)
            return;

        //���� �׼� ���߿� 'r'Ű�� ���� �������� �õ��ϸ� ���� �׼� ���� �� ������
        StopWeaponAction();

        StartCoroutine("OnReload");
    }

    public void OnAttack()
    {
        if(Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            //�ٰ��ִ� ���� ������ �� ����.
            if(animator.MoveSpeed > 0.5f)
            {
                return;
            }

            //�����ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
            lastAttackTime = Time.time;

            //ź ���� ������ ���� �Ұ���
            if(weaponSetting.currentAmmo <= 0)
            {

            }
            //���ݽ� currentAmmo 1 ����, ź �� UI������Ʈ
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            //���� �ִϸ��̼� ���
            animator.Play("Fire", -1, 0);
            //�ѱ� ����Ʈ ���
            StartCoroutine("OnMuzzleFlashEffect");
            //���� ���� ���
            PlaySound(audioClipFire);

            //������ �߻��� ���ϴ� ��ġ ����
            TwoStepRayCast();
        }
    }

    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

        muzzleFlashEffect.SetActive(false);
    }

    private IEnumerator OnReload()
    {
        isReload = true;

        //������ �ִϸ��̼�, ���� ���
        animator.OnReload();
        PlaySound(audioClipReload);

        while (true)
        {
            //���尡 ������� �ƴϰ�, ���� �ִϸ��̼��� movement�̸�
            //������ �ִϸ��̼�, ��������� ����Ǿ���.
            if (audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
            {
                isReload = false;

                //���� źâ ���� 1���ҽ�Ű�� text�� ������Ʈ
                weaponSetting.currentMagazine--;
                onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                //���� ź ���� �ִ�� �����ϰ�, �ٲ� ź�� ������ Text�� ������Ʈ
                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                yield break;
            }

            yield return null;
        }
    }

    private void TwoStepRayCast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        //ȭ���� �߾� ��ǥ (aim �������� raycast ����)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        //���� ��Ÿ�(attackdistance)�ȿ� �ε����� ������Ʈ�� ������ targetpoint�� ������ �ε��� ��ġ
        if (Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
        {
            targetPoint = hit.point;
        }
        //�ִ� ��Ÿ��� �ε����� ������Ʈ�� ������ targetpoint�� �ִ� ��Ÿ� ��ġ
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }

        //ù��° raycast�������� ����� targetPoint�� ��ǥ�������� �����ϰ�,
        //�ѱ��� ������������ �Ͽ� raycast ����
        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
        {
            impactMemoryPool.SpawnImpact(hit);

            if (hit.transform.CompareTag("ImpactEnemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
            }
            else if (hit.transform.CompareTag("InteractionObject"))
            {
                hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
            }
        }
    }
    private void ResetVariable()
    {
        isReload = false;
        isAttack = false;
    }
}
