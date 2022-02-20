using UnityEngine;

public class Shop : MonoBehaviour {

	public TurretBlueprint[] turret; //���� Ÿ���� �� �迭

	BuildManager buildManager;

	private Tower[] availableTowerArray;

	void Start ()
	{
		buildManager = BuildManager.instance;
	}

	public void SelectRandomTurret()
    {
		Debug.Log("Random Tower Selected");
		int rand = Random.Range(0, turret.Length);
		buildManager.SelectTowerToBuild(turret, rand);
    }

}
