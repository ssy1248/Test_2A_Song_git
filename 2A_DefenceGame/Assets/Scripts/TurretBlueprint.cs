using UnityEngine;
using System.Collections;

[System.Serializable]
public class TurretBlueprint {

	public GameObject prefab;
	public int cost;

	public GameObject upgradedPrefab;
	public int upgradeCost;

	//타입을 넣어서 타워 보너스 스크립트를 만들어서 UI제작 
	public TowerType type1;
	public TowerType type2;

	public int GetSellAmount ()
	{
		return cost / 2;
	}

}
