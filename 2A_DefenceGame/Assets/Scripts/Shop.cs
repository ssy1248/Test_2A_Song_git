using UnityEngine;

public class Shop : MonoBehaviour {

	public TurretBlueprint standardTurret;
	public TurretBlueprint missileLauncher;
	public TurretBlueprint laserBeamer;

	public TurretBlueprint[] turret;

	BuildManager buildManager;

	private Tower[] availableTowerArray;

	void Start ()
	{
		buildManager = BuildManager.instance;
	}

    public void SelectStandardTurret ()
	{
		Debug.Log("Standard Turret Selected");
		buildManager.SelectTurretToBuild(standardTurret);
	}

	public void SelectMissileLauncher()
	{
		Debug.Log("Missile Launcher Selected");
		buildManager.SelectTurretToBuild(missileLauncher);
	}

	public void SelectLaserBeamer()
	{
		Debug.Log("Laser Beamer Selected");
		buildManager.SelectTurretToBuild(laserBeamer);
	}

	public void SelectRandomTurret()
    {
		Debug.Log("Random Tower Selected");
		int rand = Random.Range(0, turret.Length);
		buildManager.SelectTowerToBuild(turret, rand);
    }

}
