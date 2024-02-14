using UnityEngine;

public class AirlineEffectScript : MonoBehaviour
{
	private bool updateRotate;

	private float startTime;

	private float endTime;

	private void Start()
	{
	}

	private void Update()
	{
		if (updateRotate)
		{
			startTime += Time.deltaTime;
			if (startTime < endTime)
			{
				base.transform.Rotate(Vector3.up, -60f * Time.deltaTime, Space.Self);
			}
			else
			{
				updateRotate = false;
			}
		}
		if (Vector3.Distance(base.transform.position, Camera.main.transform.position) > 150f)
		{
			Object.Destroy(this);
			Object.Destroy(base.gameObject);
		}
	}

	public void RotateOnTime()
	{
		updateRotate = true;
		startTime = 0f;
		endTime = 1f;
	}
}
