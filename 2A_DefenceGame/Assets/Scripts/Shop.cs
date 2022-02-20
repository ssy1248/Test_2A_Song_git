using UnityEngine;

public class Shop : MonoBehaviour {

	public TurretBlueprint[] turret; //랜덤 타워가 들어갈 배열

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
