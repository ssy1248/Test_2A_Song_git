using UnityEngine;
using System.Collections;

[System.Serializable]
public class TurretBlueprint {

	public GameObject prefab;
	public int cost;

	public GameObject upgradedPrefab;
	public int upgradeCost;

	//Ÿ���� �־ Ÿ�� ���ʽ� ��ũ��Ʈ�� ���� UI���� 
	public TowerType type1;
	public TowerType type2;

	public int GetSellAmount ()
	{
		return cost / 2;
	}

}
