using UnityEngine;

public class EffectClearSelfOnAniEnd : MonoBehaviour
{
	private void Start()
	{
	}

	private void FixedUpdate()
	{
		if (!base.GetComponent<Animation>().isPlaying)
		{
			Object.Destroy(this);
			Object.Destroy(base.gameObject);
		}
	}
}
