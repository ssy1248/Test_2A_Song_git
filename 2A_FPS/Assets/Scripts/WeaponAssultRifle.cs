using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[System.Serializable]
//public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
//[System.Serializable]
//public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public class WeaponAssultRifle : WeaponBase
{
    [Header("Fire Effects")]
    [SerializeField]
    private GameObject muzzleFlashEffect; //�ѱ� ����Ʈ 

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint; //ź�� ���� ��ġ
    [SerializeField]
    private Transform bulletSpawnPoint; //�Ѿ� ���� ��ġ

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapon; //���� ���� ����
    [SerializeField]
    private AudioClip audioClipFire; //���� ����
    [SerializeField]
    private AudioClip audioClipReload; //������ ����

    [Header("Aim UI")]
    [SerializeField]
    private Image imageAim; //aim��忡 ���� ������ �̹���

    private bool isModeChange = false; //��� ��ȯ ���� üũ��
    private float defaultModeFov = 60; //�⺻��忡���� ī�޶� FOV
    private float aimModeFov = 30; //���Ӹ�忡���� ī�޶� fov

    private CasingMemoryPool casingMemoryPool; //ź�� ���� �� Ȱ��/��Ȱ��ȭ ����
    private ImpactMemoryPool impactMemoryPool; //���� ȿ�� ���� �� Ȱ��/��Ȱ�� ����
    private Camera mainCamera; //�����ɽ�Ʈ ����

    private void Awake()
    {
        //��� Ŭ������ �ʱ�ȭ�� ���� SetUp �޼ҵ� ȣ��
        base.SetUp();

        casingMemoryPool = GetComponent<CasingMemoryPool>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;

        //ó�� źâ ���� �ִ�� ����
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        //ó�� ź ���� �ִ�� ����
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        //���� ���� ���� ���
        PlaySound(audioClipTakeOutWeapon);
        //�ѱ� ����Ʈ ��Ȱ��ȭ
        muzzleFlashEffect.SetActive(false);

        //���Ⱑ Ȱ��ȭ �ɶ� �ش� ������ źâ ������ �����Ѵ�.
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);
        //���Ⱑ Ȱ��ȭ �ɶ� �ش� ������ ź �� ������ �����Ѵ�.
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

        ResetVariable();
    }

    public override void StartWeaponAction(int type = 0)
    {
        //������ ���� ���� ���� �׼��� �Ҽ� ����.
        if (isReload == true)
            return;

        //��� ��ȯ���̸� ���� �׼� ��� �Ұ�
        if (isModeChange == true)
            return;

        //���콺 ���� Ŭ��(���� ����)
        if(type == 0)
        {
            //����
            if(weaponSetting.isAutomaticAttack == true)
            {
                isAttack = true;
                StartCoroutine("OnAttackLoop");
            }
            //�ܹ�
            else
            {
                OnAttack();
            }
        }
        //���콺 ������ Ŭ��(��� ��ȯ)
        else
        {
            //���� ���϶��� ��� ��ȯ x
            if (isAttack == true)
                return;

            StartCoroutine("OnModeChange");
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
        //���� ����
        if(type == 0)
        {
            isAttack = false;
            StopCoroutine("OnAttackLoop");
        }
    }

    public override void StartReload()
    {
        //���� ���������̸� ������ �Ұ�
        if (isReload == true || weaponSetting.currentMagazine <= 0)
            return;

        //���� �׼� ���߿� RŰ�� ������ ������ �õ�
        StopWeaponAction();

        StartCoroutine("OnReload");
    }

    private IEnumerator OnAttackLoop()
    {
        while(true)
        {
            OnAttack();

            yield return null;
        }
    }

    public void OnAttack()
    {
        if(Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            //�޸��� ���� �� ���� �Ұ�
            if(animator.MoveSpeed > 0.5f)
            {
                return;
            }

            //���� �ֱⰡ �Ǿ�� ���� �����ϱ⿡ ���� �ð� ����
            lastAttackTime = Time.time;

            //ź�� ���ٸ� ���� �Ұ�
            if(weaponSetting.currentAmmo <= 0)
            {
                return;
            }

            //���ݽ� currentAmmo 1����
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            //���� �ִϸ��̼� ���
            //animator.Play("Fire", -1, 0);
            string animation = animator.AimModeIs == true ? "AimFire" : "Fire";
            animator.Play(animation, -1, 0);
            //�ѱ� ����Ʈ ���
            if(animator.AimModeIs == false)
                StartCoroutine("OnMuzzleFlashEffect");
            //���� ���� ���
            PlaySound(audioClipFire);
            //ź�� ����
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

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

        //������ �ִϸ��̼� ���� ���
        animator.OnReload();
        PlaySound(audioClipReload);

        while(true)
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
        if(Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
        {
            targetPoint = hit.point;
        }
        //�ִ� ��Ÿ��� �ε����� ������Ʈ�� ������ targetpoint�� �ִ� ��Ÿ� ��ġ
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }

        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

        //ù��° raycast�������� ����� targetPoint�� ��ǥ�������� �����ϰ�,
        //�ѱ��� ������������ �Ͽ� raycast ����
        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if(Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
        {
            impactMemoryPool.SpawnImpact(hit);

            if(hit.transform.CompareTag("ImpactEnemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
            }
            else if(hit.transform.CompareTag("InteractionObject"))
            {
                hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
            }
        }
        Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);
    }

    private IEnumerator OnModeChange()
    {
        float current = 0;
        float percent = 0;
        float time = 0.35f;

        animator.AimModeIs = !animator.AimModeIs;
        imageAim.enabled = !imageAim.enabled;

        float start = mainCamera.fieldOfView;
        float end = animator.AimModeIs == true ? aimModeFov : defaultModeFov;

        isModeChange = true;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            //mode�� ���� ī�޶��� �þ߰��� ����
            mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);

            yield return null;
        }

        isModeChange = false;
    }

    private void ResetVariable()
    {
        isReload = false;
        isAttack = false;
        isModeChange = false;
    }

    public void IncreaseMagazine(int magazine)
    {
        weaponSetting.currentMagazine = CurrentMagazine + magazine > MaxMagazine ? MaxMagazine : CurrentMagazine + magazine;

        onMagazineEvent.Invoke(CurrentMagazine);
    }
}
