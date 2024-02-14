using UnityEngine;

public class UFOScript : MonoBehaviour
{
	private enum UfoState
	{
		NONE = 0,
		STAND = 1,
		START = 2,
		MOVE1 = 3,
		FOLLOW = 4,
		ATTACK = 5,
		LEAVE = 6,
		REST = 7,
		RESTOVER = 8,
		HEIDONG = 9
	}

	private UfoState state;

	private Transform player;

	public Vector3 offset = Vector3.zero;

	private Vector3 basePos = Vector3.zero;

	private Vector3 targetPos = Vector3.zero;

	private bool moveKey;

	private float startTime;

	private float endTime;

	private string endCallback = string.Empty;

	private bool isActive;

	public bool onWake;

	private float activeHight = 160f;

	private float deactiveHight = 80f;

	private float attackY;

	private Transform effectTrans;

	private Transform heidong;

	private Vector3 heidongPos = Vector3.zero;

	private SlingshotScript slingshot;

	private TAudioController audios;

	private float groundY;

	private RaycastHit ray_hit;

	private int floorLayer;

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
		state = UfoState.RESTOVER;
		onWake = false;
		player = base.transform.parent.Find("Bip01");
		offset = new Vector3(-33f, 10f, 70f);
		effectTrans = base.transform.Find("effect_ufoxi");
		effectTrans.localPosition = new Vector3(-1000f, 0f, -1000f);
		heidong = base.transform.parent.Find("effect_heidong");
		heidong.localPosition = new Vector3(-1000f, 0f, -1000f);
		slingshot = GameObject.Find("Slingshot").GetComponent(typeof(SlingshotScript)) as SlingshotScript;
		floorLayer = 1 << LayerMask.NameToLayer("floor");
	}

	private void Update()
	{
		if (globalVal.UIState == UILayer.PAUSE)
		{
			return;
		}
		if (moveKey)
		{
			startTime += Time.deltaTime;
			if (startTime < endTime)
			{
				float t = startTime / endTime;
				offset = Vector3.Lerp(basePos, targetPos, t);
			}
			else
			{
				offset = targetPos;
				moveKey = false;
				if (endCallback != string.Empty)
				{
					SendMessage(endCallback);
				}
			}
		}
		if (Physics.Raycast(player.position, Vector3.down, out ray_hit, 100f, floorLayer) && ray_hit.transform != null)
		{
			groundY = ray_hit.point.y;
		}
		if (player.position.y < deactiveHight + groundY && isActive)
		{
			isActive = false;
			AttackEnd();
			return;
		}
		if (onWake && state == UfoState.LEAVE)
		{
			onWake = false;
		}
		if (player.position.y > activeHight + groundY)
		{
			isActive = true;
		}
		else
		{
			isActive = false;
		}
		if (isActive && !onWake && state == UfoState.RESTOVER && slingshot.playerAnim == AnimList.NONE)
		{
			Hidong();
		}
		switch (state)
		{
		case UfoState.ATTACK:
		{
			float num = attackY + Mathf.Sin(startTime * 3f);
			float num2 = num - player.position.y;
			player.GetComponent<Rigidbody>().AddForce(new Vector3(0f, num2 * 500f, 0f));
			break;
		}
		case UfoState.HEIDONG:
			heidong.position = player.position + heidongPos;
			break;
		}
		base.transform.position = player.position + offset;
		base.transform.Rotate(Vector3.forward, 26f);
	}

	private void StartMove(Vector3 target, float time, string callback)
	{
		basePos = offset;
		targetPos = target;
		startTime = 0f;
		endTime = time;
		endCallback = callback;
		moveKey = true;
	}

	private void Stand()
	{
		SetHeiDongPos(new Vector3(-1000f, 0f, 0f));
		state = UfoState.START;
		Vector3 zero = Vector3.zero;
		zero.x = Random.Range(-25f, 25f);
		zero.y = Random.Range(-12f, 12f);
		zero.z = 34f;
		StartMove(zero, Random.Range(0.5f, 1f), "StartEnd");
	}

	private void StartEnd()
	{
		state = UfoState.MOVE1;
		Vector3 zero = Vector3.zero;
		zero.x = Random.Range(-15f, 15f);
		zero.y = Random.Range(-8f, 8f);
		zero.z = 15f;
		StartMove(zero, Random.Range(0.5f, 1f), "Move1End");
	}

	private void Move1End()
	{
		state = UfoState.FOLLOW;
		Vector3 zero = Vector3.zero;
		zero.x = 0f;
		zero.y = 4.3f;
		zero.z = 5f;
		StartMove(zero, Random.Range(0.2f, 0.5f), "FollowEnd");
	}

	private void FollowEnd()
	{
		Vector3 zero = Vector3.zero;
		zero.x = 0f;
		zero.y = 2.3f;
		zero.z = 0f;
		StartMove(zero, Random.Range(0.2f, 0.3f), "StandbyAttack");
	}

	private void StandbyAttack()
	{
		state = UfoState.ATTACK;
		Vector3 zero = Vector3.zero;
		zero = offset;
		ItemAttribute attributeByName = ItemManagerClass.body.GetAttributeByName("item_ufo");
		int index = (int)globalVal.g_itemlevel[attributeByName.index];
		ItemSubAttr itemSubAttr = attributeByName.level[index] as ItemSubAttr;
		float value = itemSubAttr.value;
		attackY = player.position.y;
		effectTrans.localPosition = Vector3.zero;
		StartMove(zero, value, "AttackEnd");
		audios.PlayAudio("FXufo_drag", player);
		playAudioFall();
	}

	private void AttackEnd()
	{
		state = UfoState.LEAVE;
		Vector3 zero = Vector3.zero;
		zero.x = Random.Range(-45f, 45f);
		zero.y = Random.Range(0f, 20f);
		zero.z = 70f;
		effectTrans.localPosition = new Vector3(-10000f, 0f, -10000f);
		StartMove(zero, Random.Range(2f, 2.5f), "Rest");
		audios.StopAudio("FXufo_drag");
		audios.PlayAudio("FXufo_go", player);
	}

	private void Rest()
	{
		state = UfoState.REST;
		Vector3 zero = Vector3.zero;
		zero = offset;
		StartMove(zero, Random.Range(15f, 20f), "RestOver");
	}

	private void RestOver()
	{
		state = UfoState.RESTOVER;
	}

	private void SetHeiDongPos(Vector3 pos)
	{
		heidongPos = pos;
	}

	private void Hidong()
	{
		if (player.position.y > activeHight)
		{
			onWake = true;
			state = UfoState.HEIDONG;
			Vector3 zero = Vector3.zero;
			zero = offset;
			zero.z = 66.2f;
			heidong.GetComponent<Animation>().Play("effect_heidong");
			SetHeiDongPos(zero);
			zero.z = 64f;
			StartMove(zero, 1.5f, "Stand");
			audios.PlayAudio("FXufo_come", player);
		}
	}

	private void playAudioFall()
	{
		switch (globalVal.g_avatar_id)
		{
		case 0:
			audios.PlayAudio("SVOfall", base.transform);
			break;
		case 1:
			audios.PlayAudio("SVOtintin_fall", base.transform);
			break;
		case 2:
			audios.PlayAudio("SVOindiana J_fall", base.transform);
			break;
		case 3:
			audios.PlayAudio("SVOsoldier_fall", base.transform);
			break;
		case 4:
			audios.PlayAudio("SVOterminator_fall", base.transform);
			break;
		case 5:
			audios.PlayAudio("SVOhalo_fall", base.transform);
			break;
		}
	}
}
