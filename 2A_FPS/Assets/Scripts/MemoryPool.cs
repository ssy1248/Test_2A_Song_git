using System.Collections.Generic;
using UnityEngine;

public class MemoryPool
{
    //�޸� Ǯ�� �����Ǵ� ������Ʈ ����
    private class PoolItem
    {
        public bool isActive;
        public GameObject gameObject;
    }

    private int increaseCount = 5; //������Ʈ�� ���� �� Instantiate()�� �߰� �����Ǵ� ������Ʈ ����
    private int maxCount; //���� ����Ʈ�� ��ϵǾ� �ִ� ������Ʈ ����
    private int activeCount; //���� ���ӿ� ���ǰ� �ִ�(Ȱ��ȭ) ������Ʈ ����

    private GameObject poolObject; //������Ʈ Ǯ������ �����ϴ� ���� ������Ʈ ������
    private List<PoolItem> poolItemList; //�����Ǵ� ��� ������Ʈ ���� ����Ʈ

    public int MaxCount => maxCount; //�ܺο��� ���� ����Ʈ�� ��ϵǾ� �ִ� ������Ʈ ���� Ȯ���� ���� ������Ƽ
    public int ActiveCount => activeCount; //�ܺο��� ���� Ȱ��ȭ �Ǿ� �ִ� ������Ʈ ���� Ȯ���� ���� ������Ƽ

    //������Ʈ�� �ӽ÷� �����Ǵ� ��ġ
    private Vector3 tempPosition = new Vector3(48, 1, 48);

    public MemoryPool(GameObject poolObject)
    {
        maxCount = 0;
        activeCount = 0;
        this.poolObject = poolObject;

        poolItemList = new List<PoolItem>();

        InstantiateObject();
    }

    public void InstantiateObject()
    {
        maxCount += increaseCount;
        
        for(int i = 0; i < increaseCount; ++i)
        {
            PoolItem poolItem = new PoolItem();

            poolItem.isActive = false;
            poolItem.gameObject = GameObject.Instantiate(poolObject);
            poolItem.gameObject.transform.position = tempPosition;
            poolItem.gameObject.SetActive(false);

            poolItemList.Add(poolItem);
        }
    }

    public void DestroyObjects()
    {
        if (poolItemList == null)
            return;

        int count = poolItemList.Count;
        for(int i = 0; i < count; ++i)
        {
            GameObject.Destroy(poolItemList[i].gameObject);
        }

        poolItemList.Clear();
    }

    public GameObject ActivePoolItem()
    {
        if (poolItemList == null) //�������� ������Ʈ�� ����
            return null;

        if(maxCount == activeCount) //��� ������Ʈ�� Ȱ��ȭ ������Ʈ ������ ������
        {
            InstantiateObject(); //�߰� ����
        }

        int count = poolItemList.Count;

        for(int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if(poolItem.isActive == false)
            {
                activeCount++;

                poolItem.isActive = true;
                poolItem.gameObject.SetActive(true);

                return poolItem.gameObject;
            }
        }

        return null;
    }

    //����� �Ϸ�� ������Ʈ�� ��Ȱ��ȭ ���·� ����
    public void DeactivePoolItem(GameObject removeObject)
    {
        if (poolItemList == null || removeObject == null)
            return;

        int count = poolItemList.Count;
        for(int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if(poolItem.gameObject == removeObject)
            {
                activeCount--;

                poolItem.gameObject.transform.position = tempPosition;
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);

                return;
            }
        }
    }

    //���ӿ� ������� ��� ������Ʈ�� ��Ȱ��ȭ ���·� ����
    public void DeactivateAllPoolItem()
    {
        if (poolItemList == null)
            return;

        int count = poolItemList.Count;

        for(int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if(poolItem.gameObject != null && poolItem.isActive == true)
            {
                poolItem.gameObject.transform.position = tempPosition;
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);
            }
        }

        activeCount = 0;
    }
}
