using UnityEngine;

public class avatarAniScript : MonoBehaviour
{
	private enum STATE
	{
		IDLE = 0,
		RUN = 1,
		PANIC = 2
	}

	private STATE state;

	private float startTime;

	private float endTime;

	private void Start()
	{
		idle();
	}

	private void FixedUpdate()
	{
		switch (state)
		{
		case STATE.IDLE:
			startTime += Time.fixedDeltaTime;
			if (startTime > endTime)
			{
				int num = Random.Range(0, 100000) % 5;
				MonoBehaviour.print(num);
				if (num < 3)
				{
					run();
				}
				else
				{
					panic();
				}
			}
			break;
		case STATE.RUN:
			startTime += Time.fixedDeltaTime;
			if (startTime > endTime)
			{
				idle();
			}
			break;
		case STATE.PANIC:
			if (base.GetComponent<Animation>()["idle2"].length <= base.GetComponent<Animation>()["idle2"].time)
			{
				idle();
			}
			break;
		}
	}

	private void idle()
	{
		base.GetComponent<Animation>()["idle1"].wrapMode = WrapMode.Loop;
		base.GetComponent<Animation>().CrossFade("idle1");
		state = STATE.IDLE;
		startTime = 0f;
		endTime = Random.Range(3f, 5f);
	}

	private void run()
	{
		base.GetComponent<Animation>()["run2"].wrapMode = WrapMode.Loop;
		base.GetComponent<Animation>().CrossFade("run2");
		state = STATE.RUN;
		endTime = Random.Range(5f, 7f);
	}

	private void panic()
	{
		base.GetComponent<Animation>()["idle2"].wrapMode = WrapMode.Loop;
		base.GetComponent<Animation>().CrossFade("idle2");
		state = STATE.PANIC;
	}
}
