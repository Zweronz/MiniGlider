using UnityEngine;

public class BackGroundScript_RepeatY : MonoBehaviour
{
	public float totalLength = 300f;

	private void Start()
	{
	}

	private void Update()
	{
		Vector3 position = Camera.main.transform.position;
		if (!(position.y < 275f))
		{
			if (base.transform.position.y < position.y - totalLength / 2f)
			{
				Vector3 position2 = base.transform.position;
				position2.y += totalLength;
				base.transform.position = position2;
			}
			else if (base.transform.position.y > position.y + totalLength / 2f)
			{
				Vector3 position3 = base.transform.position;
				position3.y -= totalLength;
				base.transform.position = position3;
			}
		}
	}
}
