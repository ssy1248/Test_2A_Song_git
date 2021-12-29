using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasingMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject casingPrefabs; //ź�� ������ ���
    private MemoryPool memoryPool; //ź�� ������Ʈ Ǯ

    private void Awake()
    {
        memoryPool = new MemoryPool(casingPrefabs);
    }

    public void SpawnCasing(Vector3 position, Vector3 direction)
    {
        GameObject item = memoryPool.ActivePoolItem();
        item.transform.position = position;
        item.transform.rotation = Random.rotation;
        item.GetComponent<Casing>().SetUp(memoryPool, direction);
    }
}
