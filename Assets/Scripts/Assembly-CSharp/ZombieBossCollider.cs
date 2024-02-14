using UnityEngine;

public class ZombieBossCollider : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.transform.name == "Bip01")
		{
			collider.transform.SendMessage("PlayerDeadBreak");
		}
	}
}
