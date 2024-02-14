using System;
using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
	public SlingshotScript slingshot;

	public ArrayList attributeArray = new ArrayList();

	public GameObject m_obj;

	private bool onceKey;

	private float lastx = -100f;

	private Vector3 lastVelocity = Vector3.zero;

	private float itemTime;

	private float tempTime;

	private float veloTime;

	private bool canAcceleration;

	private Vector3 tacc = Vector3.zero;

	private bool SpeedShowKey;

	private TAudioController audios;

	private GameObject UIs;

	private TUIMeshText label_dis;

	private TUIButtonClick_Pressed left_btn;

	private TUIButtonClick_Pressed right_btn;

	private TUIMeshSprite left_btn_img;

	private TUIMeshSprite right_btn_img;

	private TUIMeshSprite left_btn_count_img;

	private TUIMeshSprite right_btn_count_img;

	private TUIRect controlRect;

	private Transform slider_point;

	private bool heightUp10Key;

	private float heightUp10Pos;

	private bool heightUp1500Key;

	private float heightUp1500Time;

	private bool heightDown50Key;

	private float heightDown50Pos;

	private bool firstTouchFloorKey;

	private float lastRectWidth;

	private float level2 = 20f;

	private float level1 = 10f;

	private bool isup;

	private bool isdown;

	private float minSpeed = 20f;

	private float flyMaxSpeed = 50f;

	private float maxSpeed = 80f;

	private bool playerIsBreak;

	private gameUI ui_game;

	private GameObject airObj;

	private float targetAngle;

	private bool downState;

	private float m_down_time;

	private float mountlength;

	private UFOScript ufostate;

	private bool UIhelpKey;

	private ArrayList lengthArray = new ArrayList();

	private float groundY;

	private RaycastHit ray_hit;

	private int floorLayer;

	private void Start()
	{
		m_obj = base.gameObject;
		GameObject gameObject = GameObject.Find("TAudioController");
		if (gameObject == null)
		{
			gameObject = UnityEngine.Object.Instantiate(Resources.Load("TAudioController")) as GameObject;
			gameObject.name = "TAudioController";
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		ResetPrivateValue();
		UIs = GameObject.Find("TUI/TUIControl");
		label_dis = UIs.transform.Find("title/label_distance").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
		left_btn = UIs.transform.Find("control/control_left").GetComponent(typeof(TUIButtonClick_Pressed)) as TUIButtonClick_Pressed;
		left_btn_img = left_btn.transform.parent.Find("left_item").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
		left_btn_count_img = left_btn.transform.parent.Find("left_item_count").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
		right_btn = UIs.transform.Find("control/control_right").GetComponent(typeof(TUIButtonClick_Pressed)) as TUIButtonClick_Pressed;
		right_btn_img = right_btn.transform.parent.Find("right_item").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
		right_btn_count_img = right_btn.transform.parent.Find("right_item_count").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
		controlRect = UIs.transform.Find("control/control_Slider/control_Clip").GetComponent(typeof(TUIRect)) as TUIRect;
		slider_point = UIs.transform.Find("control/control_Slider/Slider_point");
		ui_game = UIs.GetComponent(typeof(gameUI)) as gameUI;
		ufostate = base.transform.parent.Find("effect_ufo").GetComponent(typeof(UFOScript)) as UFOScript;
		floorLayer = 1 << LayerMask.NameToLayer("floor");
		audios = gameObject.GetComponent(typeof(TAudioController)) as TAudioController;
		CheckSpeedShow();
	}

	public void ResetPrivateValue()
	{
		onceKey = false;
		lastx = -100f;
		lastVelocity = Vector3.zero;
		itemTime = 0f;
		tempTime = 0f;
		veloTime = 0f;
		canAcceleration = false;
		tacc = Vector3.zero;
		SpeedShowKey = false;
		heightUp10Key = false;
		heightUp10Pos = 0f;
		heightUp1500Key = false;
		heightUp1500Time = 0f;
		heightDown50Key = false;
		heightDown50Pos = 0f;
		firstTouchFloorKey = false;
		level2 = 10f;
		level1 = 4f;
		playerIsBreak = false;
	}

	private void Update()
	{
		IdleState state = slingshot.state;
		if (state != IdleState.SHOT_FLYING || globalVal.UIState == UILayer.PAUSE || !canAcceleration)
		{
			return;
		}
		tacc = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f);
		if (Screen.orientation == ScreenOrientation.LandscapeLeft)
		{
			tacc.y *= -1f;
		}
		if (ui_game.isUiShaking && Mathf.Abs(tacc.y) > 0.5f)
		{
			ui_game.isUiShaking = false;
			ui_game.StopShake();
		}
		if (tacc.y > 0.5f)
		{
			tacc.y = 0.5f;
		}
		else if (tacc.y < -0.5f)
		{
			tacc.y = -0.5f;
		}
		if (slingshot.playerAnim == AnimList.ANIM2)
		{
			Vector3 velocity = airObj.GetComponent<Rigidbody>().velocity;
			velocity.z = 0f;
			float num = Vector3.Angle(Vector3.up, velocity);
			float num2 = Vector3.Distance(Vector3.zero, velocity);
			if (base.transform.position.y > 200f + groundY)
			{
				Vector3 zero = Vector3.zero;
				zero.y = velocity.y * Time.deltaTime + -40.8f * Time.deltaTime * Time.deltaTime;
				zero.x = velocity.x * Time.deltaTime;
				num2 = Vector3.Distance(Vector3.zero, zero) / Time.deltaTime;
				num = Vector3.Angle(Vector3.up, zero);
			}
			else
			{
				num += tacc.y * 5f;
				float num3 = minSpeed + Mathf.Abs(90f - num) * (maxSpeed - minSpeed) / 45f;
				if (num > 135f)
				{
					num = 135f;
				}
				if (num < 45f)
				{
					num = 45f;
				}
				num2 = num3;
				if (num2 > flyMaxSpeed)
				{
					num2 = flyMaxSpeed;
				}
				if (num2 < minSpeed)
				{
					num2 = minSpeed;
				}
			}
			Vector3 zero2 = Vector3.zero;
			zero2.x = Mathf.Sin(num * ((float)Math.PI / 180f)) * num2;
			zero2.y = Mathf.Cos(num * ((float)Math.PI / 180f)) * num2;
			airObj.GetComponent<Rigidbody>().velocity = zero2;
			base.transform.GetComponent<Rigidbody>().velocity = zero2;
			zero2 = Vector3.zero;
			float num4 = num + 90f;
			float num5 = mountlength;
			zero2.x = Mathf.Sin(num4 * ((float)Math.PI / 180f)) * num5;
			zero2.y = Mathf.Cos(num4 * ((float)Math.PI / 180f)) * num5;
			base.transform.position = airObj.transform.position + zero2;
			Debug.DrawLine(airObj.transform.position, airObj.transform.position + airObj.GetComponent<Rigidbody>().velocity);
			Debug.DrawLine(airObj.transform.position, airObj.transform.position + zero2);
		}
		else if (slingshot.playerAnim == AnimList.ANIM1)
		{
			Vector3 velocity2 = base.transform.GetComponent<Rigidbody>().velocity;
			velocity2.z = 0f;
			float num6 = Vector3.Distance(Vector3.zero, velocity2);
			if (num6 > maxSpeed)
			{
				num6 = maxSpeed;
			}
			float num7 = Vector3.Angle(Vector3.up, velocity2);
			num7 += tacc.y * 4f;
			if (num7 < 15f)
			{
				num7 = 15f;
			}
			else if (num7 > 75f)
			{
				num7 = 75f;
			}
			Vector3 zero3 = Vector3.zero;
			zero3.x = Mathf.Sin(num7 * ((float)Math.PI / 180f)) * num6;
			zero3.y = Mathf.Cos(num7 * ((float)Math.PI / 180f)) * num6;
			base.transform.GetComponent<Rigidbody>().velocity = zero3;
		}
	}

	private void FixedUpdate()
	{
		IdleState state = slingshot.state;
		if (state != IdleState.SHOT_FLYING)
		{
			return;
		}
		AnimList playerAnim = slingshot.playerAnim;
		if (playerAnim == AnimList.ANIM1 || playerAnim == AnimList.ANIM2)
		{
			if (tempTime >= itemTime)
			{
				slingshot.ShowAnimation(false);
				canAcceleration = false;
				onceKey = false;
				base.GetComponent<Rigidbody>().useGravity = true;
				EnabledCollider(base.transform, true);
				if (PlayerScriptClass.playerInfo.leftUseTime > 0 && slingshot.playerAnim == AnimList.ANIM1)
				{
					left_btn.SetDisabled(false);
					PlayerScriptClass.playerInfo.leftUseTime--;
				}
				if (PlayerScriptClass.playerInfo.rightUseTime > 0 && slingshot.playerAnim == AnimList.ANIM2)
				{
					Debug.DrawLine(airObj.transform.position, airObj.transform.position + airObj.GetComponent<Rigidbody>().velocity);
					right_btn.SetDisabled(false);
					PlayerScriptClass.playerInfo.rightUseTime--;
					Vector3 velocity = airObj.GetComponent<Rigidbody>().velocity;
					ItemManagerClass.body.SetPlayerSleep(true);
					ItemManagerClass.body.SetPlayerAddForce(velocity);
					RemoveAirObj();
					MonoBehaviour.print(" left use time : " + PlayerScriptClass.playerInfo.rightUseTime);
				}
				if (slingshot.playerAnim == AnimList.ANIM1)
				{
					Transform transform = base.transform.parent.Find("ani/effect_qiliu_2");
					if ((bool)transform)
					{
						foreach (Transform item in transform)
						{
							item.gameObject.active = false;
						}
					}
					audios.StopAudio("FXprop_rocket");
				}
				else if (slingshot.playerAnim == AnimList.ANIM2)
				{
					Transform transform3 = base.transform.parent.Find("ani/effect_qiliu_3");
					if ((bool)transform3)
					{
						foreach (Transform item2 in transform3)
						{
							item2.gameObject.active = false;
						}
					}
				}
				slingshot.playerAnim = AnimList.NONE;
				ItemAttribute attributeByName = ItemManagerClass.body.GetAttributeByName("equip_rocket");
				int num = (int)globalVal.g_itemlevel[attributeByName.index];
				ItemSubAttr itemSubAttr = attributeByName.level[num] as ItemSubAttr;
				string picname = itemSubAttr.picname;
				Transform transform5 = base.transform.parent.Find("ani/Bip01/" + picname);
				if ((bool)transform5)
				{
					UnityEngine.Object.Destroy(transform5.gameObject);
				}
				string text = string.Empty;
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				switch (num)
				{
				case 1:
					text = "effect_kele";
					break;
				case 2:
					text = "effect_yangqi";
					break;
				case 3:
					text = "effect_baozu";
					break;
				case 4:
					text = "effect_hangtian";
					break;
				case 5:
					text = "effect_zhandou";
					break;
				}
				Transform transform6 = base.transform.parent.Find("ani/Bip01/" + text);
				if ((bool)transform6)
				{
					foreach (Transform item3 in transform6)
					{
						UnityEngine.Object.Destroy(item3.gameObject);
					}
					UnityEngine.Object.Destroy(transform6.gameObject);
				}
				ItemAttribute attributeByName2 = ItemManagerClass.body.GetAttributeByName("equip_glider");
				int index = (int)globalVal.g_itemlevel[attributeByName2.index];
				ItemSubAttr itemSubAttr2 = attributeByName2.level[index] as ItemSubAttr;
				string picname2 = itemSubAttr2.picname;
				Transform transform8 = base.transform.parent.Find("ani/Bip01/" + picname2);
				if ((bool)transform8)
				{
					UnityEngine.Object.Destroy(transform8.gameObject);
				}
				Transform transform9 = UIs.transform.Find("control/control_Slider");
				transform9.localPosition = new Vector3(0f, -200f, 0f);
			}
			else
			{
				tempTime += Time.fixedDeltaTime;
				if (Physics.Raycast(base.transform.position, Vector3.down, out ray_hit, 100f, floorLayer) && ray_hit.transform != null)
				{
					groundY = ray_hit.point.y;
				}
				if (base.transform.position.y < 5f + groundY && slingshot.playerAnim == AnimList.ANIM2)
				{
					tempTime = itemTime;
				}
				UpdateControlSlider();
			}
			updateBtnState();
		}
		else
		{
			updateBtnState();
		}
		if (onceKey)
		{
			if (Mathf.Abs(base.GetComponent<Rigidbody>().velocity.x - lastVelocity.x) <= 2f && Mathf.Abs(base.GetComponent<Rigidbody>().velocity.y - lastVelocity.y) <= 2f)
			{
				onceKey = false;
			}
			else
			{
				veloTime += Time.fixedDeltaTime;
				if (veloTime > 1f)
				{
					veloTime = 1f;
				}
				Vector3 velocity2 = Vector3.Lerp(base.GetComponent<Rigidbody>().velocity, lastVelocity, veloTime);
				base.GetComponent<Rigidbody>().velocity = velocity2;
			}
		}
		Vector3 velocity3 = base.transform.GetComponent<Rigidbody>().velocity;
		velocity3.z = 0f;
		float num2 = Vector3.Distance(Vector3.zero, velocity3);
		if (num2 > maxSpeed)
		{
			num2 = maxSpeed;
			velocity3.Normalize();
			base.transform.GetComponent<Rigidbody>().velocity = velocity3 * num2;
		}
		UpdateDistance();
		UpdatePlayerVoice();
		if (PlayerScriptClass.playerInfo.speed < num2)
		{
			PlayerScriptClass.playerInfo.speed = num2;
		}
		if (PlayerScriptClass.playerInfo.height < base.transform.position.y)
		{
			PlayerScriptClass.playerInfo.height = base.transform.position.y;
		}
		if (PlayerScriptClass.playerInfo.distance < base.transform.position.x)
		{
			PlayerScriptClass.playerInfo.distance = base.transform.position.x;
		}
		if (base.transform.position.y > 10f + groundY && !heightUp10Key)
		{
			heightUp10Key = true;
			heightUp10Pos = base.transform.position.x;
		}
		else if (base.transform.position.y <= 10f + groundY && heightUp10Key)
		{
			heightUp10Key = false;
			float num3 = base.transform.position.x - heightUp10Pos;
			if ((float)PlayerScriptClass.playerInfo.heightUp10Distance < num3)
			{
				PlayerScriptClass.playerInfo.heightUp10Distance = (int)num3;
			}
		}
		if (base.transform.position.y > 250f + groundY && !heightUp1500Key)
		{
			heightUp1500Key = true;
			MonoBehaviour.print("height 1500 time : " + heightUp1500Time);
		}
		else if (base.transform.position.y <= 250f + groundY && heightUp1500Key)
		{
			heightUp1500Key = false;
			PlayerScriptClass.playerInfo.heightUp1500Time = heightUp1500Time;
		}
		if (heightUp1500Key)
		{
			heightUp1500Time += Time.fixedDeltaTime;
		}
		if (base.transform.position.y < 150f + groundY && !heightDown50Key)
		{
			heightDown50Key = true;
			heightDown50Pos = base.transform.position.x;
		}
		else if (base.transform.position.y >= 150f + groundY && heightDown50Key)
		{
			heightDown50Key = false;
			float num4 = base.transform.position.x - heightDown50Pos;
			PlayerScriptClass.playerInfo.heightDown50Distance += num4;
		}
		TestGameOver();
	}

	private void UpdatePlayerVoice()
	{
		Vector3 velocity = base.transform.GetComponent<Rigidbody>().velocity;
		velocity.z = 0f;
		float num = Vector3.Distance(Vector3.zero, velocity);
		float num2 = Vector3.Angle(Vector3.up, velocity);
		if (base.transform.position.y > 150f && num2 < 60f && !isup && num > 15f)
		{
			isup = true;
			playAudioCheer();
		}
		else if (num2 > 90f)
		{
			isup = false;
		}
		if (num2 > 135f && num >= 50f && !isdown && base.transform.position.y > 80f)
		{
			isdown = true;
			playAudioFall();
		}
		else if (num2 < 90f && base.transform.position.y > 80f)
		{
			isdown = false;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.name == "scene_floor")
		{
			if (!firstTouchFloorKey)
			{
				firstTouchFloorKey = true;
				PlayerScriptClass.playerInfo.firstTouchFloorDistance = (int)base.transform.position.x;
			}
			float num = Vector3.Distance(Vector3.zero, base.transform.GetComponent<Rigidbody>().velocity);
			string objName = string.Empty;
			if (num > 0f && num <= level1)
			{
				objName = "FXCrash_Flip";
			}
			else if (num > level1 && num <= level2)
			{
				EffectManagerClass.body.PlayEffect("effect_luodi", base.transform);
				objName = "FXCrash_Hit";
				playAudioHurt();
			}
			else if (num > level2)
			{
				EffectManagerClass.body.PlayEffect("effect_bleeding", base.transform);
				objName = "FXCrash_bang";
				playAudioHurt();
			}
			audios.PlayAudio(objName, base.transform);
		}
	}

	private void playAudioHurt()
	{
		switch (globalVal.g_avatar_id)
		{
		case 0:
		{
			int num = UnityEngine.Random.Range(0, 100) % 10;
			if (num > 6)
			{
				audios.PlayAudio("VOhurt", base.transform);
			}
			else
			{
				audios.PlayAudio("SVOhurt", base.transform);
			}
			break;
		}
		case 1:
			audios.PlayAudio("SVOtintin_hurt", base.transform);
			break;
		case 2:
			audios.PlayAudio("SVOindiana J_hurt", base.transform);
			break;
		case 3:
			audios.PlayAudio("SVOsoldier_hurt", base.transform);
			break;
		case 4:
			audios.PlayAudio("SVOterminator_hurt", base.transform);
			break;
		case 5:
			audios.PlayAudio("SVOhalo_hurt", base.transform);
			break;
		}
	}

	public void OnChangeAnim(AnimList anim)
	{
		slingshot.ShowAnimation(true);
		string empty = string.Empty;
		switch (anim)
		{
		case AnimList.ANIM1:
			empty = "flyupsupperman";
			break;
		case AnimList.ANIM2:
		{
			ItemAttribute attributeByName = ItemManagerClass.body.GetAttributeByName("equip_glider");
			int index = (int)globalVal.g_itemlevel[attributeByName.index];
			ItemSubAttr itemSubAttr = attributeByName.level[index] as ItemSubAttr;
			switch (itemSubAttr.picname)
			{
			default:
			{
				int num = 0;
				empty = ((num != 1) ? "fly2" : "flyupsupperman");
				break;
			}
			case "equip_glide1":
				empty = "fly";
				break;
			}
			break;
		}
		default:
			empty = "ready";
			break;
		}
		if (empty != string.Empty)
		{
			Transform transform = base.transform.parent.Find("ani");
			if ((bool)transform)
			{
				transform.GetComponent<Animation>().Play(empty);
			}
		}
	}

	private void UpdateDistance()
	{
		label_dis.text = (int)base.transform.position.x + "m";
	}

	public void CheckSpeedShow()
	{
		Vector3 velocity = base.transform.GetComponent<Rigidbody>().velocity;
		velocity.z = 0f;
		if (label_dis == null)
		{
			return;
		}
		label_dis.text = (int)base.transform.position.x + "m";
		float num = Vector3.Distance(Vector3.zero, velocity);
		if (num >= 30f && !SpeedShowKey)
		{
			Transform transform = null;
			Transform transform2 = null;
			if (slingshot.playerAnim == AnimList.ANIM2)
			{
				transform = base.transform.parent.Find("ani/effect_qiliu_3");
				transform2 = base.transform.parent.Find("ani/effect_qiliu_2");
			}
			else
			{
				transform = base.transform.parent.Find("ani/effect_qiliu_2");
				transform2 = base.transform.parent.Find("ani/effect_qiliu_3");
			}
			foreach (Transform item in transform)
			{
				item.gameObject.active = true;
			}
			foreach (Transform item2 in transform2)
			{
				item2.gameObject.active = false;
			}
			SpeedShowKey = true;
		}
		if ((!(num < 30f) || !SpeedShowKey) && (!(base.transform.position.y < 20f) || !SpeedShowKey))
		{
			return;
		}
		Transform transform5 = null;
		Transform transform6 = null;
		SpeedShowKey = false;
		transform5 = base.transform.parent.Find("ani/effect_qiliu_3");
		transform6 = base.transform.parent.Find("ani/effect_qiliu_2");
		transform5.gameObject.active = false;
		foreach (Transform item3 in transform5)
		{
			item3.gameObject.active = false;
		}
		foreach (Transform item4 in transform6)
		{
			item4.gameObject.active = false;
		}
	}

	public bool GetControlBtnState()
	{
		return UIhelpKey;
	}

	private void updateBtnState()
	{
		bool flag = false;
		if (ufostate.onWake)
		{
			flag = true;
		}
		if (slingshot.playerAnim != 0)
		{
			flag = true;
		}
		string frameName = left_btn_img.frameName;
		frameName = frameName.Substring(0, frameName.LastIndexOf('_'));
		string frameName2 = right_btn_img.frameName;
		frameName2 = frameName2.Substring(0, frameName2.LastIndexOf('_'));
		if (PlayerScriptClass.playerInfo.leftUseTime > 0 && !flag)
		{
			left_btn.SetDisabled_Pressed(false);
			left_btn_img.frameName = frameName + "_a";
			left_btn_count_img.frameName = "X" + PlayerScriptClass.playerInfo.leftUseTime;
		}
		else
		{
			left_btn.SetDisabled_Pressed(true);
			left_btn_img.frameName = frameName + "_h";
			left_btn_count_img.frameName = string.Empty;
		}
		if (base.transform.position.y > 30f + groundY && PlayerScriptClass.playerInfo.rightUseTime > 0 && !flag)
		{
			right_btn.SetDisabled_Pressed(false);
			right_btn_img.frameName = frameName2 + "_a";
			right_btn_count_img.frameName = "X" + PlayerScriptClass.playerInfo.rightUseTime;
		}
		else
		{
			right_btn.SetDisabled_Pressed(true);
			right_btn_img.frameName = frameName2 + "_h";
			right_btn_count_img.frameName = string.Empty;
		}
		UIhelpKey = !flag;
	}

	public void OnHuoJian()
	{
		if (slingshot.playerAnim != 0 || slingshot.state == IdleState.GAMEOVER || slingshot.state == IdleState.GAMEOVERCALLBACK)
		{
			return;
		}
		MonoBehaviour.print("left use count : " + PlayerScriptClass.playerInfo.leftUseTime + "  " + (int)globalVal.g_item_once_count[2]);
		if (PlayerScriptClass.playerInfo.leftUseTime == 1)
		{
			if ((int)globalVal.g_item_once_count[2] > 0)
			{
				slingshot.playBuffEff(2);
			}
			PlayerScriptClass.playerInfo.huojianKey = true;
			PlayerScriptClass.playerInfo.huojianAddLv++;
			PlayerScriptClass.playerInfo.huojianAddAgain = 0;
			DrawCircleClass.circle.SetProgress(0);
			Transform transform = GameObject.Find("TUI/TUIControl/control/circle").transform;
			Transform transform2 = transform.Find("effect_ui_02");
			if (transform2 != null)
			{
				transform2.parent = null;
				MonoBehaviour.print("destroy : " + transform2.name);
				UnityEngine.Object.Destroy(transform2.gameObject);
			}
		}
		left_btn.SetDisabled(true);
		Vector3 velocity = base.GetComponent<Rigidbody>().velocity;
		float num = Vector3.Distance(Vector3.zero, velocity);
		num = 80f;
		velocity = new Vector3(1f, 1f, 0f);
		velocity.Normalize();
		velocity *= num;
		if (num < 25f)
		{
			num = 25f;
		}
		SetVelocity(velocity * 2f);
		canAcceleration = true;
		base.GetComponent<Rigidbody>().useGravity = false;
		Transform transform3 = base.transform.parent.Find("ani").transform;
		transform3.eulerAngles = new Vector3(-45f, 90f, 0f);
		slingshot.playerAnim = AnimList.ANIM1;
		slingshot.AddTrackPoint(3);
		OnChangeAnim(slingshot.playerAnim);
		ItemAttribute attributeByName = ItemManagerClass.body.GetAttributeByName("equip_rocket");
		int num2 = (int)globalVal.g_itemlevel[attributeByName.index];
		ItemSubAttr itemSubAttr = attributeByName.level[num2] as ItemSubAttr;
		string picname = itemSubAttr.picname;
		tempTime = 0f;
		itemTime = itemSubAttr.value;
		MonoBehaviour.print("level " + num2 + "   " + itemTime);
		GameObject original = Resources.Load("Prefab/" + picname) as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.transform.parent = base.transform.parent.Find("ani/Bip01").transform;
		switch (picname)
		{
		case "equip_rocket1":
			gameObject.transform.localPosition = new Vector3(-0.2f, -0.22f, -0.32f);
			gameObject.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
			break;
		case "equip_rocket2":
			gameObject.transform.localPosition = new Vector3(-0.1674747f, -0.1635681f, -0.2773766f);
			gameObject.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
			break;
		case "equip_rocket3":
			gameObject.transform.localPosition = new Vector3(0.2300384f, -0.08356297f, -0.1332729f);
			gameObject.transform.localEulerAngles = new Vector3(0f, 90f, -90f);
			break;
		case "equip_rocket4":
			gameObject.transform.localPosition = new Vector3(-0.1704238f, -0.134294f, -0.2754612f);
			gameObject.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
			break;
		case "equip_rocket5":
			gameObject.transform.localPosition = new Vector3(-0.1988136f, -0.1635683f, -0.293009f);
			gameObject.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
			break;
		}
		gameObject.name = picname;
		Transform transform4 = UIs.transform.Find("control/control_Slider");
		transform4.localPosition = new Vector3(0f, -105f, 0f);
		EffectManagerClass.body.PlayEffect("effect_mask", base.transform, true);
		Transform transform5 = base.transform.parent.Find("ani/effect_qiliu_2");
		if ((bool)transform5)
		{
			foreach (Transform item in transform5)
			{
				item.gameObject.active = true;
			}
		}
		string text = string.Empty;
		Vector3 localPosition = Vector3.zero;
		Vector3 zero = Vector3.zero;
		switch (num2)
		{
		case 1:
			text = "effect_kele";
			break;
		case 2:
			text = "effect_yangqi";
			break;
		case 3:
			text = "effect_baozu";
			localPosition = new Vector3(0.3015943f, 0f, 0f);
			break;
		case 4:
			text = "effect_hangtian";
			localPosition = new Vector3(0.3015943f, 0f, 0f);
			break;
		case 5:
			text = "effect_zhandou";
			break;
		}
		original = Resources.Load("Prefab/" + text) as GameObject;
		gameObject = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.name = text;
		gameObject.transform.parent = base.transform.parent.Find("ani/Bip01").transform;
		gameObject.transform.localPosition = localPosition;
		gameObject.transform.localEulerAngles = zero;
		audios.PlayAudio("FXprop_rocket_launch", base.transform);
		audios.PlayAudio("FXprop_rocket", base.transform);
		playAudioCheer();
	}

	private void playAudioCheer()
	{
		switch (globalVal.g_avatar_id)
		{
		case 0:
			audios.PlayAudio("SVOcheer", base.transform);
			break;
		case 1:
			audios.PlayAudio("SVOtintin_cheer", base.transform);
			break;
		case 2:
			audios.PlayAudio("SVOindiana J_cheer", base.transform);
			break;
		case 3:
			audios.PlayAudio("SVOsoldier_cheer", base.transform);
			break;
		case 4:
			audios.PlayAudio("SVOterminator_cheer", base.transform);
			break;
		case 5:
			audios.PlayAudio("SVOhalo_cheer", base.transform);
			break;
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

	private void UpdateControlSlider()
	{
		float num = tempTime / itemTime;
		controlRect.rect.width = (1f - num) * 212f;
		float num2 = (tempTime - Time.fixedDeltaTime * 3f) / itemTime;
		float num3 = (1f - num2) * 212f;
		Vector3 localPosition = controlRect.transform.localPosition;
		if (num3 >= 0f)
		{
			localPosition.x = controlRect.rect.x + num3 + localPosition.x;
		}
		localPosition.z = -1f;
		slider_point.localPosition = localPosition;
	}

	public void OnRejump()
	{
		RaycastHit rayhit = slingshot.GetRayhit();
		if (!(base.transform.position.y - rayhit.point.y > 5f) && slingshot.playerAnim == AnimList.NONE)
		{
			slingshot.playerAnim = AnimList.REJUMP;
			Vector3 velocity = base.GetComponent<Rigidbody>().velocity;
			float num = Vector3.Distance(Vector3.zero, velocity);
			float num2 = Vector3.Angle(Vector3.up, velocity);
			if (num < 30f)
			{
				num = 30f;
			}
			if (num2 > 70f)
			{
				num2 = 70f;
			}
			float num3 = 15f;
			Vector3 zero = Vector3.zero;
			zero.x = Mathf.Sin(num2 * ((float)Math.PI / 180f)) * num * num3;
			zero.y = Mathf.Cos(num2 * ((float)Math.PI / 180f)) * num * num3;
			MonoBehaviour.print("rejump shotPower : " + zero);
			base.GetComponent<Rigidbody>().AddForce(zero, ForceMode.Impulse);
			slingshot.playerAnim = AnimList.NONE;
		}
	}

	private void RemoveAirObj()
	{
		Transform transform = base.transform.parent.Find("air");
		if (transform != null)
		{
			transform.parent = null;
			UnityEngine.Object.Destroy(transform.gameObject);
		}
	}

	public void OnHuaXiang()
	{
		if (slingshot.state == IdleState.GAMEOVER || slingshot.state == IdleState.GAMEOVERCALLBACK || slingshot.playerAnim != 0)
		{
			return;
		}
		MonoBehaviour.print("right use count : " + PlayerScriptClass.playerInfo.rightUseTime);
		if (PlayerScriptClass.playerInfo.rightUseTime == 1 && (int)globalVal.g_item_once_count[3] > 0)
		{
			slingshot.playBuffEff(3);
		}
		if (base.transform.position.y > 30f + groundY)
		{
			right_btn.SetDisabled(true);
			slingshot.playerAnim = AnimList.ANIM2;
			slingshot.AddTrackPoint(4);
			OnChangeAnim(slingshot.playerAnim);
			Transform transform = UIs.transform.Find("control/control_Slider");
			transform.localPosition = new Vector3(0f, -105f, 0f);
			Vector3 velocity = base.GetComponent<Rigidbody>().velocity;
			velocity.z = 0f;
			velocity.Normalize();
			velocity *= 30f;
			base.transform.GetComponent<Rigidbody>().velocity = velocity;
			base.GetComponent<Rigidbody>().useGravity = false;
			EnabledCollider(base.transform, false);
			ItemAttribute attributeByName = ItemManagerClass.body.GetAttributeByName("equip_glider");
			int num = (int)globalVal.g_itemlevel[attributeByName.index];
			ItemSubAttr itemSubAttr = attributeByName.level[num] as ItemSubAttr;
			string picname = itemSubAttr.picname;
			airObj = new GameObject("air");
			airObj.transform.parent = base.transform.parent;
			Rigidbody rigidbody = airObj.AddComponent(typeof(Rigidbody)) as Rigidbody;
			rigidbody.velocity = base.transform.GetComponent<Rigidbody>().velocity;
			rigidbody.mass = base.transform.GetComponent<Rigidbody>().mass;
			rigidbody.drag = base.transform.GetComponent<Rigidbody>().drag;
			rigidbody.angularDrag = base.transform.GetComponent<Rigidbody>().angularDrag;
			rigidbody.useGravity = false;
			float num2 = Vector3.Angle(Vector3.up, base.transform.GetComponent<Rigidbody>().velocity) - 90f;
			float num3 = Vector3.Distance(Vector3.zero, base.transform.GetComponent<Rigidbody>().velocity);
			switch (num)
			{
			case 1:
				mountlength = 1.2f;
				break;
			case 2:
				mountlength = 1f;
				break;
			case 3:
				mountlength = 2f;
				break;
			case 4:
				mountlength = 2f;
				break;
			case 5:
				mountlength = 2f;
				break;
			}
			num3 = mountlength;
			Vector3 zero = Vector3.zero;
			zero.x = Mathf.Sin(num2 * ((float)Math.PI / 180f)) * num3;
			zero.y = Mathf.Cos(num2 * ((float)Math.PI / 180f)) * num3;
			airObj.transform.position = base.transform.position + zero;
			tempTime = 0f;
			itemTime = itemSubAttr.value;
			GameObject original = Resources.Load("Prefab/" + picname) as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.transform.parent = base.transform.parent.Find("ani/Bip01").transform;
			gameObject.transform.localPosition = GetHuojianLocalPos(picname);
			gameObject.transform.localEulerAngles = GetHuojianLocalRot(picname);
			gameObject.name = picname;
			canAcceleration = true;
			ItemManagerClass.body.SetBaseX(Camera.main.transform.position.x);
			ItemManagerClass.body.RefreshTime = 0;
			EffectManagerClass.body.PlayEffect("effect_mask", base.transform, true);
			if (itemSubAttr.picname == "equip_glide5")
			{
				audios.PlayAudio("FXprop_wing02", base.transform);
			}
			else
			{
				audios.PlayAudio("FXprop_wing", base.transform);
			}
			playAudioCheer();
		}
	}

	private Vector3 GetHuojianLocalPos(string itemname)
	{
		Vector3 result = Vector3.zero;
		switch (itemname)
		{
		case "equip_glide1":
			result = new Vector3(0.2132607f, 0.1548589f, 235f / (256f * (float)Math.PI));
			break;
		case "equip_glide2":
			result = new Vector3(-0.2631467f, 0f, 0.854638f);
			break;
		case "equip_glide3":
			result = new Vector3(0.6852147f, -0.6399125f, 0.7302642f);
			break;
		case "equip_glide4":
			result = new Vector3(-0.12f, 0f, 0.7f);
			break;
		case "equip_glide5":
			result = new Vector3(0.03737296f, -0.09874679f, 0.1342084f);
			break;
		}
		return result;
	}

	private Vector3 GetHuojianLocalRot(string itemname)
	{
		Vector3 zero = Vector3.zero;
		switch (itemname)
		{
		case "equip_glide5":
			zero = new Vector3(7.748988f, 17.55f, 278.7329f);
			break;
		case "equip_glide2":
		case "equip_glide3":
			zero = new Vector3(0f, 90f, 270f);
			break;
		case "equip_glide4":
			zero = new Vector3(51.2f, 90f, 90f);
			break;
		default:
			zero = new Vector3(0f, 180f, 90f);
			break;
		}
		return zero;
	}

	public void EnabledCollider(Transform trans, bool iscan)
	{
		foreach (Transform tran in trans)
		{
			if ((bool)tran.GetComponent<Rigidbody>())
			{
				tran.GetComponent<Collider>().enabled = iscan;
				tran.GetComponent<Rigidbody>().useGravity = iscan;
			}
			tran.gameObject.active = iscan;
			if (tran.childCount > 0)
			{
				EnabledCollider(tran, iscan);
			}
		}
	}

	public void SetVelocity(Vector3 velo)
	{
		lastVelocity = velo;
		float num = Vector3.Distance(Vector3.zero, lastVelocity);
		if (num > maxSpeed)
		{
			num = maxSpeed;
		}
		lastVelocity.Normalize();
		lastVelocity *= num;
		onceKey = true;
		veloTime = 0f;
	}

	public Vector3 GetVelocity()
	{
		return base.GetComponent<Rigidbody>().velocity;
	}

	private void TestGameOver()
	{
		if (!playerIsBreak && !base.GetComponent<Rigidbody>().isKinematic)
		{
			Vector3 position = base.transform.position;
			float num = Vector3.Distance(Vector3.zero, base.GetComponent<Rigidbody>().velocity);
			if (Mathf.Abs(position.x - lastx) / Time.fixedDeltaTime < 0.2f && num < 0.5f)
			{
				slingshot.state = IdleState.GAMEOVER;
				ItemManagerClass.body.SetPlayerSleep(true);
				ItemManagerClass.body.SetPlayerAddForce(Vector3.zero);
				slingshot.AddTrackPoint(6);
				StartCoroutine(OverOntime(2f));
				CheckSpeedShow();
			}
			position.z = 0f;
			base.transform.position = position;
			lastx = position.x;
		}
	}

	private IEnumerator OverOntime(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		slingshot.GameOver();
		if (globalVal.g_avatar_id == 0)
		{
			audios.PlayAudio("VOend", base.transform);
		}
		Time.timeScale = 1f;
	}

	public void PlayerDeadBreak()
	{
		playerIsBreak = true;
		slingshot.state = IdleState.GAMEOVER;
		Component[] componentsInChildren = base.transform.GetComponentsInChildren(typeof(Rigidbody));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Rigidbody rigidbody = (Rigidbody)componentsInChildren[i];
			rigidbody.isKinematic = true;
		}
		Component[] componentsInChildren2 = base.transform.parent.GetComponentsInChildren(typeof(SkinnedMeshRenderer));
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)componentsInChildren2[j];
			skinnedMeshRenderer.enabled = false;
		}
		if (Physics.Raycast(base.transform.position, Vector3.down, out ray_hit, 100f, floorLayer) && ray_hit.transform != null)
		{
			groundY = ray_hit.point.y;
		}
		AvatarAttribute avatarAttribute = ItemManagerClass.body.avatarArray[globalVal.g_avatar_id] as AvatarAttribute;
		string text = avatarAttribute.modelname + "_b";
		GameObject original = Resources.Load("Prefab/avatar/" + text) as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		Vector3 position = base.transform.position;
		position.y = groundY;
		gameObject.transform.position = position;
		gameObject.transform.eulerAngles = new Vector3(0f, 90f, 0f);
		gameObject.GetComponent<Animation>().Play("break");
		Time.timeScale = 0.5f;
		string text2 = avatarAttribute.modelname + "_h";
		GameObject original2 = Resources.Load("Prefab/avatar/" + text2) as GameObject;
		GameObject gameObject2 = UnityEngine.Object.Instantiate(original2) as GameObject;
		position = base.transform.position;
		position.y = groundY;
		gameObject2.transform.position = position;
		gameObject2.transform.eulerAngles = new Vector3(UnityEngine.Random.Range(180, -180), 90f, UnityEngine.Random.Range(180, -180));
		gameObject2.GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.Range(-50, 90), UnityEngine.Random.Range(130, 250)));
		audios.PlayAudio("FXzombiesmash", gameObject.transform);
		audios.PlayAudio("Fxbones", gameObject.transform);
		StartCoroutine(OverOntime(3f));
	}

	public void ResetRigid()
	{
		playerIsBreak = false;
		Component[] componentsInChildren = base.transform.GetComponentsInChildren(typeof(Rigidbody));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Rigidbody rigidbody = (Rigidbody)componentsInChildren[i];
			if (rigidbody.name != "Bip01")
			{
				rigidbody.isKinematic = false;
			}
		}
		Component[] componentsInChildren2 = base.transform.parent.GetComponentsInChildren(typeof(SkinnedMeshRenderer));
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)componentsInChildren2[j];
			skinnedMeshRenderer.enabled = true;
		}
	}
}
