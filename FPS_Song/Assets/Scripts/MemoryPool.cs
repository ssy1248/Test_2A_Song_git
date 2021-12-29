using System.Collections.Generic;
using UnityEngine;

public class MemoryPool
{
    //메모리 풀로 관리되는 오브젝트 정보
    private class PoolItem
    {
        public bool isActive;
        public GameObject gameObject;
    }

    private int increaseCount = 5; //오브젝트가 부족 시 Instantiate()로 추가 생성되는 오브젝트 갯수
    private int maxCount; //현재 리스트에 등록되어 있는 오브젝트 갯수
    private int activeCount; //현재 게임에 사용되고 있는(활성화) 오브젝트 갯수

    private GameObject poolObject; //오브젝트 풀링에서 관리하는 게임 오브젝트 프리팹
    private List<PoolItem> poolItemList; //관리되는 모든 오브젝트 저장 리스트

    public int MaxCount => maxCount; //외부에서 현재 리스트에 등록되어 있는 오브젝트 갯수 확인을 위한 프로퍼티
    public int ActiveCount => activeCount; //외부에서 현재 활성화 되어 있는 오브젝트 개수 확인을 위한 프로퍼티

    //오브젝트가 임시로 보관되는 위치
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
        if (poolItemList == null) //관리중인 오브젝트가 없다
            return null;

        if(maxCount == activeCount) //모든 오브젝트와 활성화 오브젝트 갯수가 같으면
        {
            InstantiateObject(); //추가 생성
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

    //사용이 완료된 오브젝트를 비활성화 상태로 변경
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

    //게임에 사용중인 모든 오브젝트를 비활성화 상태로 설정
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
