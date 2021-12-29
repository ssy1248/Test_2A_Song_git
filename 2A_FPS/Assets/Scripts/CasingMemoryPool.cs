using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasingMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject casingPrefabs; //≈∫«« «¡∏Æ∆È µÓ∑œ
    private MemoryPool memoryPool; //≈∫«« ø¿∫Í¡ß∆Æ «Æ

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
