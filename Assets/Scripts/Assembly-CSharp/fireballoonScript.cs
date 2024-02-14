using UnityEngine;

public class fireballoonScript : MonoBehaviour
{
	private Transform player;

	private void Start()
	{
		player = GameObject.Find("player2/Bip01").transform;
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		float num = Vector3.Distance(player.position, base.transform.position);
		if (num > 30f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
