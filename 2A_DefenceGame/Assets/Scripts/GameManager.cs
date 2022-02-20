using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static bool GameIsOver;

	public GameObject gameOverUI;
	public GameObject completeLevelUI;
	public GameObject[] bonusPanels;

	void Start ()
	{
		GameIsOver = false;

		//��� UI�г��� �� �����
		foreach (GameObject go in bonusPanels)
		{
			go.SetActive(false);
		}
	}

	void Update () {
		if (GameIsOver)
			return;

		if (PlayerStats.Lives <= 0)
		{
			EndGame();
		}
	}

	void EndGame ()
	{
		GameIsOver = true;
		gameOverUI.SetActive(true);
	}

	public void WinLevel ()
	{
		GameIsOver = true;
		completeLevelUI.SetActive(true);
	}

}
