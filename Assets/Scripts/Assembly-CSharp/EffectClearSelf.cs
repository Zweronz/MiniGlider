using UnityEngine;

public class EffectClearSelf : MonoBehaviour
{
	private void Start()
	{
	}

	private void FixedUpdate()
	{
		if (base.transform.GetChildCount() < 1)
		{
			Object.Destroy(this);
			Object.Destroy(base.gameObject);
		}
	}
}
