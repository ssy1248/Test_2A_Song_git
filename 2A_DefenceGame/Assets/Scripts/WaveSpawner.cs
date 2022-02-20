using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour {

	public static int EnemiesAlive = 0;

	public Wave[] waves;

	public Transform spawnPoint;
	//public GameObject navigator;

	public float timeBetweenWaves = 10f;
	private float countdown = 5f;

	public Text waveCountdownText;
	public Text waveText;

	public GameManager gameManager;

	//NavigatorMovement navi;

	private int waveIndex = 0;

	void Start()
    {
		//navi = NavigatorMovement.instance;
		//Instantiate(navigator, spawnPoint.position, spawnPoint.rotation);
	}

    void Update ()
	{
		//if (!navi.IsEnd) //빌드매니저 참고
		//{
		//	return;
		//}

		if (EnemiesAlive > 0)
		{
			return;
		}

		MoveMent();
	}

	void MoveMent()
    {
		if (waveIndex == waves.Length)
		{
			gameManager.WinLevel();
			this.enabled = false;
		}

		if (countdown <= 0f)
		{
			StartCoroutine(SpawnWave());
			countdown = timeBetweenWaves;
			return;
		}

		countdown -= Time.deltaTime;

		countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);

		waveCountdownText.text = string.Format("{0:00.00}", countdown);
		waveText.text = waveIndex + 1 + " Waves";
	}

	IEnumerator SpawnWave ()
	{
		PlayerStats.Rounds++;

		Wave wave = waves[waveIndex];

		EnemiesAlive = wave.count;

		for (int i = 0; i < wave.count; i++)
		{
			SpawnEnemy(wave.enemy);
			yield return new WaitForSeconds(1f / wave.rate);
		}

		waveIndex++;
	}

	void SpawnEnemy (GameObject enemy)
	{
		Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
	}

	public void SpawnMissionEnemy(GameObject enemy)
    {
		Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
    }
}
