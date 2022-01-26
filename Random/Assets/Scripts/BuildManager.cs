using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public GameObject SelectNode;
    public GameObject Tower;
    public static BuildManager build;

    private void Start()
    {
        build = this;
    }

    public void BuildToTower()
    {
        Instantiate(Tower, new Vector3(SelectNode.transform.position.x
            , SelectNode.transform.position.y + 0.5f,
            SelectNode.transform.position.z), Quaternion.identity);
    }
}
