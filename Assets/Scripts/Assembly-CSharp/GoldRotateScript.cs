using UnityEngine;

public class GoldRotateScript : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Rotate(Vector3.forward, 13f);
	}
}
