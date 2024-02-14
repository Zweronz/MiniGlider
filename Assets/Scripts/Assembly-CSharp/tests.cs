using UnityEngine;

public class tests : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		Vector3 position = Camera.main.transform.position;
		position.x += 0.1f;
		if (position.x > -39.5f)
		{
			position.x = -57f;
		}
		Camera.main.transform.position = position;
	}
}
