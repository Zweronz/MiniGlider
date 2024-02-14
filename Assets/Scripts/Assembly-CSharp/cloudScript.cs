using UnityEngine;

public class cloudScript : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.name == "Bip01")
		{
			EffectManagerClass.body.PlayEffect("effect_cloudtouch", other.transform);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.name == "Bip01")
		{
			EffectManagerClass.body.PlayEffect("effect_cloudcrosse", other.transform);
		}
	}
}
