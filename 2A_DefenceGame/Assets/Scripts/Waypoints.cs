using UnityEngine;

public class Waypoints : MonoBehaviour
{

	private LineRenderer lineRenderer;
	public static Transform[] points;
	public Transform StartPos;

	void Awake ()
	{
		lineRenderer = GetComponent<LineRenderer>();
		points = new Transform[transform.childCount];
		for (int i = 0; i < points.Length; i++)
		{
			points[i] = transform.GetChild(i);
		}
	}

	void Update()
	{
		lineRenderer.SetWidth(1f, 1f);

		lineRenderer.SetPosition(0, StartPos.position);
		lineRenderer.SetPosition(1, points[0].position);

		for (int i = 1; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i - 1].position);
            lineRenderer.SetPosition(i + 1, points[i].position);
        }
    }
}
