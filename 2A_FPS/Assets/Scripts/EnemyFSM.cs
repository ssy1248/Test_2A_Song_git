using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { None = -1, Idle = 0, Wander, Pursuit, Attack,}

public class EnemyFSM : MonoBehaviour
{
    [Header("Pursuit")]
    [SerializeField]
    private float targetRecognitionRange = 8; //�ν� ���� (�̹����ȿ� ������pursuit���·� ����)
    [SerializeField]
    private float pursiotLimitRange = 10; //���� ����(�� ���� �ٱ����� ������ wander ���·� ����)

    [Header("Attack")]
    [SerializeField]
    private GameObject projecttilePrefab; //�߻�ü ������
    [SerializeField]
    private Transform projecttileSpawnPoint; //�߻�ü ���� ��ġ
    [SerializeField]
    private float attackRange = 5; //���� ���� (���� �ȿ� ������ Attack���·� ����)
    [SerializeField]
    private float attackRate = 1; //���� �ӵ�

    private EnemyState enemyState = EnemyState.None; //���� ���� �ൿ
    private float lastAttackTime = 0; //���� �ֱ� ���� ����

    private Status status; //�̵��ӵ� ���� ����
    private NavMeshAgent navMeshAgent; //�̵���� ���� navigation
    private Transform target; //���� ���� ���(�÷��̾�)
    private EnemyMemoryPool enemyMemoryPool; //�� �޸� Ǯ(�� ������Ʈ ��Ȱ��ȭ�� ���)

    //private void Awake()
    public void SetUp(Transform target, EnemyMemoryPool enemyMemoryPool)
    {
        status = GetComponent<Status>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        this.target = target;
        this.enemyMemoryPool = enemyMemoryPool;

        //NavMeshAgent ������Ʈ���� ȸ���� ������Ʈ���� �ʵ��� ����
        navMeshAgent.updateRotation = false;
    }

    private void OnEnable()
    {
        //���� Ȱ��ȭ �� �� ���� ���¸� "���"�� ����
        ChangeState(EnemyState.Idle);
    }

    private void OnDisable()
    {
        //���� ��Ȱ��ȭ �ɶ� ���� ������� ���¸� �����ϰ�, ���¸� "None"���� ����
        StopCoroutine(enemyState.ToString());

        enemyState = EnemyState.None;
    }

    public void ChangeState(EnemyState newState)
    {
        //���� ������� ���¿� �ٲٷ��� �ϴ� ���°� ������ �ٲ� �ʿ䰡 ����.
        if (newState == enemyState)
            return;

        //������ ������̴� ���� ����
        StopCoroutine(enemyState.ToString());
        //���� ���� ���¸� newState�� ����
        enemyState = newState;
        //���ο� ���� ���
        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        //n�� �Ŀ� "��ȸ" ���·� �����ϴ� �ڷ�ƾ ����
        StartCoroutine("AutoChangeFromIdleToWander");

        while(true)
        {
            //"���" ���� �϶� �ϴ� �ൿ
            //Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����(��ȸ, �߰�, ���Ÿ� ����)
            CalculateDistacveToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        //1~4�� �ð� ���
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        //���¸� "��ȸ"�� ����
        ChangeState(EnemyState.Wander);
    }

    private IEnumerator Wander()
    {
        float currentTime = 0;
        float maxTime = 10;

        //�̵��ӵ� ����
        navMeshAgent.speed = status.WalkSpeed;

        //��ǥ ��ġ ����
        navMeshAgent.SetDestination(CalculateWanderPosition());

        //��ǥ��ġ�� ȸ��
        Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to = from);

        while(true)
        {
            currentTime += Time.deltaTime;

            //��ǥ��ġ�� �����ϰ� �����ϰų� �ʹ� �����ð����� ��ȸ�ϱ� ���¿� �ӹ��� ������
            to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);
            if((to-from).sqrMagnitude < 0.01f || currentTime >= maxTime)
            {
                //���¸� "���"�� ����
                ChangeState(EnemyState.Idle);
            }

            CalculateDistacveToTargetAndSelectState();

            yield return null;
        }
    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10; //���� ��ġ�� �������� �ϴ� ���� ������
        int wanderJitter = 0; //���õ� ����(wanderJitterMin ~ wanderJitterMax)
        int wanderJitterMin = 0; //�ּ� ����
        int wanderJitterMax = 360; //�ִ� ����

        //���� �� ĳ���Ͱ� �ִ� ������ �߽� ��ġ�� ũ��(������ ��� �ൿ�� ���� �ʵ���)
        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        //�ڽ��� ��ġ�� �߽����� ������ �Ÿ�, ���õ� ������ ��ġ�� ��ǥ�� ��ǥ�������� ����
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        //������ ��ǥ��ġ�� �ڽ��� �̵������� ����� �ʰ� ����
        targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x - rangeScale.x * 0.5f, rangePosition.x + rangeScale.x * 0.5f);
        targetPosition.y = 0.0f;
        targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z - rangeScale.z * 0.5f, rangePosition.z + rangeScale.z * 0.5f);

        return targetPosition;
    }

    private Vector3 SetAngle(float radius, int angle)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }

    private IEnumerator Pursuit()
    {
        while(true)
        {
            //�̵� �ӵ� ����(��ȸ�� ���� �ȴ� �ӵ��� �̵�, ��ô�Ҷ��� �ٴ� �ӵ��� �̵�)
            navMeshAgent.speed = status.RunSpeed;

            //��ǥ��ġ�� ���� �÷��̾��� ��ġ�� ����
            navMeshAgent.SetDestination(target.position);

            //Ÿ�� ������ ��� �ֽ��ϵ��� ��
            LockRotationToTarget();

            //Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����
            CalculateDistacveToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator Attack()
    {
        //�����Ҷ��� �̵��� ���ߵ��� ����
        navMeshAgent.ResetPath();

        while(true)
        {
            //Ÿ�� ���� �ֽ�
            LockRotationToTarget();

            //Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����
            CalculateDistacveToTargetAndSelectState();

            if(Time.time - lastAttackTime > attackRate)
            {
                //���� �ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
                lastAttackTime = Time.time;

                //�߻�ü ����
                GameObject clone = Instantiate(projecttilePrefab, projecttileSpawnPoint.position, projecttileSpawnPoint.rotation);
                clone.GetComponent<EnemyProjectTile>().SetUp(target.position);
            }

            yield return null;
        }
    }

    private void LockRotationToTarget()
    {
        //��ǥ ��ġ
        Vector3 to = new Vector3(target.position.x, 0, target.position.z);
        //�� ��ġ
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        //�ٷ� ����
        transform.rotation = Quaternion.LookRotation(to - from);
        //������ ����
        //Quaternion rotaion = Quaternion.LookRotation(to - from);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotaion, 0.01f);
    }

    private void CalculateDistacveToTargetAndSelectState()
    {
        if (target == null)
            return;

        //�÷��̾�(target)�� ���� �Ÿ� ��� �� �Ÿ��� ���� �ൿ ����
        float distance = Vector3.Distance(target.position, transform.position);

        if(distance <= attackRange)
        {
            ChangeState(EnemyState.Attack);
        }
        else if(distance <= targetRecognitionRange)
        {
            ChangeState(EnemyState.Pursuit);
        }
        else if (distance >= targetRecognitionRange)
        {
            ChangeState(EnemyState.Wander);
        }
    }

    private void OnDrawGizmos()
    {
        //"��ȸ" ������ �� �̵��� ��� ǥ��
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);

        //��ǥ �ν� ����
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        //���� ����
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursiotLimitRange);

        //���� ����
        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void TakeDamage(int damage)
    {
        bool isDie = status.DecreaseHP(damage);

        if(isDie == true)
        {
            enemyMemoryPool.DeactivateEnemy(gameObject);
        }
    }
}
