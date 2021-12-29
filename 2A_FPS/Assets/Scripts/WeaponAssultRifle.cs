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
    private GameObject muzzleFlashEffect; //총구 이펙트 

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint; //탄피 생성 위치
    [SerializeField]
    private Transform bulletSpawnPoint; //총알 생성 위치

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapon; //무기 장착 사운드
    [SerializeField]
    private AudioClip audioClipFire; //공격 사운드
    [SerializeField]
    private AudioClip audioClipReload; //재장전 사운드

    [Header("Aim UI")]
    [SerializeField]
    private Image imageAim; //aim모드에 따라 보여줄 이미지

    private bool isModeChange = false; //모드 전환 여부 체크용
    private float defaultModeFov = 60; //기본모드에서의 카메라 FOV
    private float aimModeFov = 30; //에임모드에서의 카메라 fov

    private CasingMemoryPool casingMemoryPool; //탄피 생성 후 활성/비활성화 관리
    private ImpactMemoryPool impactMemoryPool; //공격 효과 생성 후 활성/비활성 관리
    private Camera mainCamera; //레이케스트 관리

    private void Awake()
    {
        //기반 클래스의 초기화를 위한 SetUp 메소드 호출
        base.SetUp();

        casingMemoryPool = GetComponent<CasingMemoryPool>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;

        //처음 탄창 수는 최대로 설정
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        //처음 탄 수는 최대로 설정
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        //무기 장착 사운드 재생
        PlaySound(audioClipTakeOutWeapon);
        //총구 이펙트 비활성화
        muzzleFlashEffect.SetActive(false);

        //무기가 활성화 될때 해당 무기의 탄창 정보를 갱신한다.
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);
        //무기가 활성화 될때 해당 무기의 탄 수 정보를 갱신한다.
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

        ResetVariable();
    }

    public override void StartWeaponAction(int type = 0)
    {
        //재장전 중일 때는 무기 액션을 할수 없다.
        if (isReload == true)
            return;

        //모드 전환중이면 무기 액션 사용 불가
        if (isModeChange == true)
            return;

        //마우스 왼쪽 클릭(공격 시작)
        if(type == 0)
        {
            //연발
            if(weaponSetting.isAutomaticAttack == true)
            {
                isAttack = true;
                StartCoroutine("OnAttackLoop");
            }
            //단발
            else
            {
                OnAttack();
            }
        }
        //마우스 오른쪽 클릭(모드 전환)
        else
        {
            //공격 중일때는 모드 전환 x
            if (isAttack == true)
                return;

            StartCoroutine("OnModeChange");
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
        //공격 종료
        if(type == 0)
        {
            isAttack = false;
            StopCoroutine("OnAttackLoop");
        }
    }

    public override void StartReload()
    {
        //현재 재장전중이면 재장전 불가
        if (isReload == true || weaponSetting.currentMagazine <= 0)
            return;

        //무기 액션 도중에 R키를 누르면 재장전 시도
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
            //달리고 있을 시 공격 불가
            if(animator.MoveSpeed > 0.5f)
            {
                return;
            }

            //공격 주기가 되어야 공격 가능하기에 현재 시간 저장
            lastAttackTime = Time.time;

            //탄이 없다면 공격 불가
            if(weaponSetting.currentAmmo <= 0)
            {
                return;
            }

            //공격시 currentAmmo 1감소
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            //무기 애니메이션 재생
            //animator.Play("Fire", -1, 0);
            string animation = animator.AimModeIs == true ? "AimFire" : "Fire";
            animator.Play(animation, -1, 0);
            //총구 이펙트 재생
            if(animator.AimModeIs == false)
                StartCoroutine("OnMuzzleFlashEffect");
            //공격 사운드 재생
            PlaySound(audioClipFire);
            //탄피 생성
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

            //광선을 발사해 원하는 위치 공격
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

        //재장전 애니메이션 사운드 재생
        animator.OnReload();
        PlaySound(audioClipReload);

        while(true)
        {
            //사운드가 재생중이 아니고, 현재 애니메이션이 movement이면
            //재장전 애니메이션, 사운드재생이 종료되었다.
            if (audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
            {
                isReload = false;

                //현재 탄창 수를 1감소시키고 text에 업데이트
                weaponSetting.currentMagazine--;
                onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                //현재 탄 수를 최대로 설정하고, 바뀐 탄수 정보를 Text에 업데이트
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

        //화면의 중앙 좌표 (aim 기준으로 raycast 연산)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        //공격 사거리(attackdistance)안에 부딪히는 오브젝트가 있으면 targetpoint는 광선에 부딪힌 위치
        if(Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
        {
            targetPoint = hit.point;
        }
        //최대 사거리에 부딪히는 오브젝트가 없으면 targetpoint는 최대 사거리 위치
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }

        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

        //첫번째 raycast연산으로 얻어진 targetPoint를 목표지점으로 설정하고,
        //총구를 시작지점으로 하여 raycast 연산
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

            //mode에 따라 카메라의 시야각을 변경
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
