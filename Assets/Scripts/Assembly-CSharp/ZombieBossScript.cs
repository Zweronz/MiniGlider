using System.Collections;
using UnityEngine;

public class ZombieBossScript : MonoBehaviour
{
	private enum STATE
	{
		IDLE = 0,
		ATTACK = 1
	}

	private STATE state;

	private float m_waitTime;

	private bool attackkey;

	private Transform trans;

	private TAudioController audios;

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
		trans = base.transform.Find("Bip01/Bip01 Prop1");
		Idle();
		if (m_waitTime < 0f)
		{
			Attack();
			base.GetComponent<Animation>()["attack"].time = m_waitTime * -1f;
			attackkey = true;
		}
	}

	private void Update()
	{
		STATE sTATE = state;
		if (sTATE != 0 && sTATE == STATE.ATTACK)
		{
			if (!base.GetComponent<Animation>().isPlaying)
			{
				Idle();
			}
			CheckOnAttack();
		}
	}

	private void Idle()
	{
		base.GetComponent<Animation>()["idle"].wrapMode = WrapMode.Loop;
		base.GetComponent<Animation>().Play("idle");
		state = STATE.IDLE;
	}

	private void CheckOnAttack()
	{
		if (attackkey && base.GetComponent<Animation>()["attack"].time > Time.deltaTime * 42f)
		{
			trans.gameObject.active = false;
			attackkey = false;
		}
	}

	private void Attack()
	{
		state = STATE.ATTACK;
		trans.gameObject.active = true;
		base.GetComponent<Animation>().Play("attack");
		audios.PlayAudio("Ani_Zombie_bossattack", trans);
	}

	public void AttackOntime(float waitTime)
	{
		if (waitTime < 0f)
		{
			m_waitTime = waitTime;
		}
		else
		{
			StartCoroutine(OnTime(waitTime));
		}
	}

	private IEnumerator OnTime(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		Attack();
		attackkey = true;
	}
}
