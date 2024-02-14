using UnityEngine;

public class stashFlash : MonoBehaviour
{
	private float time;

	private float flashOffset = 1f;

	private float flashTime;

	private bool key;

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		if (!key)
		{
			return;
		}
		time += Time.fixedDeltaTime;
		if (time > flashTime)
		{
			flashTime += flashOffset;
			int num = (int)flashTime;
			if (num % 2 == 0)
			{
				TUIMeshSprite tUIMeshSprite = base.transform.Find("Normal").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
				tUIMeshSprite.frameName = base.name;
			}
			else
			{
				TUIMeshSprite tUIMeshSprite2 = base.transform.Find("Normal").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
				tUIMeshSprite2.frameName = base.name + "_d";
			}
		}
	}

	public void BeginFlash()
	{
		time = 0f;
		key = true;
		flashOffset = 1f;
		flashTime = 0f;
	}

	public void BeginFlash(float offset)
	{
		time = 0f;
		key = true;
		flashOffset = offset;
		flashTime = 0f;
	}

	public void StopFlash()
	{
		key = false;
		TUIMeshSprite tUIMeshSprite = base.transform.Find("Normal").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
		tUIMeshSprite.frameName = base.name;
	}
}
