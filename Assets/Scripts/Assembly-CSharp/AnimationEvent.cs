using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
	private enum StoryAniState
	{
		NONE = 0,
		RUN = 1,
		JUMP = 2,
		JUMP2 = 3
	}

	private float startTime;

	private float endTime;

	private bool updateKey;

	private Vector3 startPos = Vector3.zero;

	private Vector3 endPos = Vector3.zero;

	private TAudioController audios;

	private StoryAniState state;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("TAudioController");
		if (gameObject == null)
		{
			gameObject = Object.Instantiate(Resources.Load("TAudioController")) as GameObject;
			gameObject.name = "TAudioController";
			Object.DontDestroyOnLoad(gameObject);
		}
		audios = gameObject.GetComponent(typeof(TAudioController)) as TAudioController;
	}

	private void Update()
	{
		if (!updateKey)
		{
			return;
		}
		if (globalVal.isskipkey)
		{
			updateKey = false;
			return;
		}
		startTime += Time.deltaTime;
		float num = 0f;
		num = startTime / endTime;
		if (num > 1f)
		{
			num = 1f;
			updateKey = false;
			base.transform.localPosition = Vector3.Lerp(startPos, endPos, num);
			MoveEndCallBack();
		}
		else
		{
			base.transform.localPosition = Vector3.Lerp(startPos, endPos, num);
		}
	}

	private void AddAttackEvent()
	{
	}

	private void PlayAnimationOnEnd(string name)
	{
		base.GetComponent<Animation>()[name].speed = 0.1f;
		base.GetComponent<Animation>().PlayQueued(name);
	}

	private void PlayAnimation(string name)
	{
		MonoBehaviour.print("story jump 2 " + name);
		base.GetComponent<Animation>().Play(name);
		StartMove(endpos: new Vector3(0f, -2.672f, 3.83f), startpos: base.transform.localPosition, endtime: 0.5f);
		state = StoryAniState.JUMP2;
	}

	private void PlayEffectOnEnd(string name)
	{
	}

	private void ClimbStopEvent()
	{
		base.GetComponent<Animation>().Play("storyrun", AnimationPlayMode.Queue);
		base.transform.localPosition = new Vector3(1.250087f, -6.257381f, -5.021229f);
		base.transform.localEulerAngles = Vector3.zero;
		StartMove(endpos: new Vector3(0f, -6.257381f, 2.78f), startpos: base.transform.localPosition, endtime: 1f);
		state = StoryAniState.RUN;
	}

	private void PlayAudio(string name)
	{
		GameObject gameObject = GameObject.Find("Slingshot");
		if (gameObject != null)
		{
			SlingshotScript slingshotScript = gameObject.GetComponent(typeof(SlingshotScript)) as SlingshotScript;
			if (slingshotScript != null && slingshotScript.state == IdleState.GAMEOVERCALLBACK)
			{
				return;
			}
		}
		audios.PlayAudio(name);
		audios.transform.Find("Audio/" + name).localPosition = base.transform.position;
	}

	private void StoryJump()
	{
		base.transform.localPosition = new Vector3(0f, -5.925913f, 2.78f);
		base.GetComponent<Animation>().Play("storyjump_copy");
		state = StoryAniState.JUMP;
	}

	private void MoveEndCallBack()
	{
		switch (state)
		{
		case StoryAniState.RUN:
			StoryJump();
			break;
		case StoryAniState.JUMP2:
			base.GetComponent<Animation>().Play("ready");
			base.transform.localPosition = new Vector3(0f, -0.9192619f, 3.83f);
			MonoBehaviour.print(base.transform.position);
			break;
		case StoryAniState.JUMP:
			break;
		}
	}

	private void StartMove(Vector3 startpos, Vector3 endpos, float endtime)
	{
		startPos = startpos;
		endPos = endpos;
		endTime = endtime;
		startTime = 0f;
		updateKey = true;
	}
}
