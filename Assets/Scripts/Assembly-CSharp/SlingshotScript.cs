using System;
using System.Collections;
using System.Xml;
using UnityEngine;

public class SlingshotScript : MonoBehaviour
{
	public TUIRect uiRect;

	public GameObject obj;

	private Transform air;

	public Vector3 shotPower;

	public AnimList playerAnim;

	private Vector3[] m_vector;

	public Transform point1;

	public Transform point2;

	private RaycastHit ray_hit;

	private Ray ray;

	public IdleState state;

	private Rect viewOffset = new Rect(0f, 0f, 0f, 0f);

	private float baseFloorY = 30f;

	private float baseCameraZ = -13f;

	public Vector3 bodyoffset = new Vector3(0f, -0.2783813f, 0f);

	private GameObject aniTrans;

	private TAudioController audios;

	private int touchId = -1;

	private Vector3 localPoint = Vector3.zero;

	private float rand;

	private bool randKey;

	private bool skipkey;

	private GameObject UIs;

	private TUIMeshText label_dis;

	private string startScene = "scene_suburb";

	private Transform listener;

	private int floorLayer;

	private int shootLayer;

	private ArrayList posTrack = new ArrayList();

	private float trackPosCur;

	private float trackPosRec;

	private float trackInterval = 3f;

	private Transform folTrans;

	public bool isAniBody;

	private Vector3 playerFlyPoint = Vector3.zero;

	private Transform leftHand;

	private Transform rightHand;

	private float ttime;

	private bool inhelp;

	private float helpTime;

	private float helpEndTime;

	private bool aniEndKey;

	private ArrayList buffTransArray = new ArrayList();

	private UFOScript ufo;

	private float VoiceTime;

	private float VoiceEndTime;

	private bool VoiceKey;

	private bool isup;

	private bool isdown;

	private bool gameTimeKey;

	private float gameTime;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("TAudioController");
		if (gameObject == null)
		{
			gameObject = UnityEngine.Object.Instantiate(Resources.Load("TAudioController")) as GameObject;
			gameObject.name = "TAudioController";
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		ItemManagerClass.body.InitGameItems();
		audios = gameObject.GetComponent(typeof(TAudioController)) as TAudioController;
		audios.ResetFollowData();
		floorLayer = 1 << LayerMask.NameToLayer("floor");
		shootLayer = 1 << LayerMask.NameToLayer("Default");
		Physics.gravity = new Vector3(0f, -9.8f, 0f);
		UIs = GameObject.Find("TUI/TUIControl");
		label_dis = UIs.transform.Find("title/label_distance").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
		LoadPhysicsXML();
		state = IdleState.NORMAL;
		globalVal.ReadLastTrack("Record.txt");
		InitScene();
		leftHand = obj.transform.parent.Find("ani/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm/Bip01 L Hand");
		rightHand = obj.transform.parent.Find("ani/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand");
		ufo = obj.transform.parent.Find("effect_ufo").GetComponent(typeof(UFOScript)) as UFOScript;
		ItemManagerClass.body.ResetItems_new();
		ItemManagerClass.body.InitStar();
		MonoBehaviour.print("start");
		if (globalVal.isskipkey)
		{
			SetAvatar(globalVal.g_avatar_id);
			InitStart();
		}
		else
		{
			SetAvatar(globalVal.g_avatar_id);
			StartStory();
			UIs.transform.Find("skip_btn").localPosition = new Vector3(192f, 132.5f, -1f);
		}
	}

	private void LoadPhysicsXML()
	{
		XmlDocument xmlDocument = new XmlDocument();
		TextAsset textAsset = Resources.Load("physics") as TextAsset;
		xmlDocument.LoadXml(textAsset.text);
		XmlNode xmlNode = xmlDocument.SelectSingleNode("root");
		XmlNodeList childNodes = xmlNode.ChildNodes;
		for (int i = 0; i < childNodes.Count; i++)
		{
			XmlElement xmlElement = childNodes.Item(i) as XmlElement;
			string attribute = xmlElement.GetAttribute("name");
			GameObject gameObject = Resources.Load("Prefab/" + attribute) as GameObject;
			BoxCollider boxCollider = gameObject.GetComponent(typeof(BoxCollider)) as BoxCollider;
			PhysicMaterial sharedMaterial = boxCollider.sharedMaterial;
			sharedMaterial.dynamicFriction = float.Parse(xmlElement.GetAttribute("dynamicFriction"));
			sharedMaterial.staticFriction = float.Parse(xmlElement.GetAttribute("staticFriction"));
			sharedMaterial.bounciness = float.Parse(xmlElement.GetAttribute("bounciness"));
			sharedMaterial.frictionCombine = GetPhysicCombine(int.Parse(xmlElement.GetAttribute("frictionCombine")));
			sharedMaterial.bounceCombine = GetPhysicCombine(int.Parse(xmlElement.GetAttribute("bounceCombine")));
		}
	}

	private PhysicMaterialCombine GetPhysicCombine(int id)
	{
		PhysicMaterialCombine result = PhysicMaterialCombine.Average;
		switch (id)
		{
		case 0:
			result = PhysicMaterialCombine.Average;
			break;
		case 1:
			result = PhysicMaterialCombine.Maximum;
			break;
		case 2:
			result = PhysicMaterialCombine.Minimum;
			break;
		case 3:
			result = PhysicMaterialCombine.Multiply;
			break;
		}
		return result;
	}

	private void ResetScene()
	{
		GameObject gameObject = GameObject.Find("scene");
		if (!(gameObject != null))
		{
			return;
		}
		foreach (Transform item in gameObject.transform)
		{
			item.localPosition = new Vector3(-1000f, 0f, 0f);
			BackGroundScript backGroundScript = item.GetComponent(typeof(BackGroundScript)) as BackGroundScript;
			backGroundScript.state = SceneForwardState.NONE;
		}
	}

	public RaycastHit GetRayhit()
	{
		return ray_hit;
	}

	private void InitScene()
	{
		ResetScene();
		GameObject gameObject = GameObject.Find("scene");
		if (!(gameObject != null))
		{
			return;
		}
		int num = 0;
		foreach (Transform item in gameObject.transform)
		{
			if (item.name == startScene)
			{
				item.localPosition = new Vector3(320 * num, 0f, 0f);
				BackGroundScript backGroundScript = item.GetComponent(typeof(BackGroundScript)) as BackGroundScript;
				backGroundScript.state = SceneForwardState.SUBURB;
				globalVal.sceneLastTrans = item;
				num++;
			}
		}
	}

	private void SetAvatar(int index)
	{
		AvatarAttribute avatarAttribute = ItemManagerClass.body.avatarArray[index] as AvatarAttribute;
		GameObject original = Resources.Load("Prefab/avatar/" + avatarAttribute.modelname) as GameObject;
		GameObject partModel = UnityEngine.Object.Instantiate(original) as GameObject;
		GameObject gameObject = obj.transform.parent.Find("ani").gameObject;
		gameObject.transform.parent = null;
		AvatarMounter.MountSkinnedMesh(gameObject, partModel, avatarAttribute.modelname);
		AvatarMounter.MountSkinnedMesh(obj.transform.parent.gameObject, partModel, avatarAttribute.modelname);
		gameObject.transform.parent = obj.transform.parent;
		UnityEngine.Object.Destroy(partModel);
		Resources.UnloadUnusedAssets();
		PlayerScript playerScript = obj.GetComponent(typeof(PlayerScript)) as PlayerScript;
		playerScript.CheckSpeedShow();
	}

	private void StartStory()
	{
		audios.PlayMusic("BGMgamestart");
		TUIFade tUIFade = UIs.transform.Find("fade_bg").GetComponent(typeof(TUIFade)) as TUIFade;
		tUIFade.FadeIn();
		listener = GameObject.Find("AudioListener").transform;
		listener.position = Camera.main.transform.position;
		state = IdleState.STORY;
		ShowAnimation(true);
		aniTrans = obj.transform.parent.Find("ani").gameObject;
		aniTrans.transform.localPosition = new Vector3(5.469332f, -21.50789f, -27.858f);
		GameObjectMove gameObjectMove = aniTrans.AddComponent(typeof(GameObjectMove)) as GameObjectMove;
		GameObject gameObject = GameObject.Find("GameObjectMoveData");
		GameObjectMoveData movedata = gameObject.GetComponent(typeof(GameObjectMoveData)) as GameObjectMoveData;
		gameObjectMove.StartMove(movedata);
		aniTrans.GetComponent<Animation>().Play("run2");
		GameObject gameObject2 = GameObject.Find("visit_zombies1");
		gameObjectMove = gameObject2.AddComponent(typeof(GameObjectMove)) as GameObjectMove;
		gameObject = GameObject.Find("GameObjectMoveData_zombie1");
		movedata = gameObject.GetComponent(typeof(GameObjectMoveData)) as GameObjectMoveData;
		gameObjectMove.StartMove(movedata);
		GameObject gameObject3 = GameObject.Find("visit_zombies2");
		gameObjectMove = gameObject3.AddComponent(typeof(GameObjectMove)) as GameObjectMove;
		gameObject = GameObject.Find("GameObjectMoveData_zombie2");
		movedata = gameObject.GetComponent(typeof(GameObjectMoveData)) as GameObjectMoveData;
		gameObjectMove.StartMove(movedata);
		StoryCameraScript storyCameraScript = Camera.main.gameObject.AddComponent(typeof(StoryCameraScript)) as StoryCameraScript;
		storyCameraScript.SetCameraState(StoryCameraState.FOLLOW);
		gameObject = GameObject.Find("equip_apartment_ani");
		gameObject.GetComponent<Animation>().Play("idle");
		Vector3 position = point1.position;
		position.z = 0f;
		localPoint = position;
	}

	public void CheckSkip()
	{
		if (!skipkey)
		{
			skipkey = true;
			Camera.main.SendMessage("SetCameraState", StoryCameraState.NONE);
			GameObjectMove gameObjectMove = aniTrans.GetComponent(typeof(GameObjectMove)) as GameObjectMove;
			gameObjectMove.StopMove();
			InitStart();
		}
	}

	private void InitHoujianEff()
	{
		Transform transform = GameObject.Find("TUI/TUIControl/control/circle").transform;
		for (int i = 0; i < transform.GetChildCount(); i++)
		{
			UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
		}
		EffectManagerClass.body.PlayEffect("effect_ui_02", transform);
	}

	private void InitStart()
	{
		skipkey = true;
		UIs.transform.Find("skip_btn").localPosition = new Vector3(-1000f, -1000f, 0f);
		globalVal.isskipkey = true;
		globalVal.secondKey = false;
		StopAllCoroutines();
		InitHoujianEff();
		listener = GameObject.Find("AudioListener").transform;
		ItemManagerClass.body.ResetItems_new();
		ItemManagerClass.body.InitStar();
		InitScene();
		ClearBuffEff();
		posTrack.Clear();
		trackPosCur = 0f;
		trackPosRec = 0f;
		base.GetComponent<Animation>().Play("action");
		base.GetComponent<Animation>()["action"].normalizedTime = 0f;
		base.GetComponent<Animation>()["action"].enabled = true;
		base.GetComponent<Animation>().Sample();
		base.GetComponent<Animation>()["action"].enabled = false;
		globalVal.changeDis = 1000f;
		gameUI gameUI2 = UIs.GetComponent(typeof(gameUI)) as gameUI;
		gameUI2.InitGameUI();
		label_dis.text = string.Empty + 0 + "m";
		uiRect.rect.height = 0f;
		if (!globalVal.g_help_key[0])
		{
			gameUI2.InitHelp(HelpState.TAP);
		}
		PlayerScriptClass.playerInfo.Reset();
		if ((int)globalVal.g_item_once_count[2] > 0)
		{
			PlayerScriptClass.playerInfo.leftUseTime = 2;
		}
		else
		{
			PlayerScriptClass.playerInfo.leftUseTime = 1;
		}
		if ((int)globalVal.g_item_once_count[3] > 0)
		{
			PlayerScriptClass.playerInfo.rightUseTime = 2;
		}
		else
		{
			PlayerScriptClass.playerInfo.rightUseTime = 1;
		}
		obj.transform.parent.position = new Vector3(6.688f, 20.85f, 0f);
		obj.transform.parent.eulerAngles = new Vector3(0f, 90f, 0f);
		obj.GetComponent<Rigidbody>().isKinematic = true;
		obj.GetComponent<Rigidbody>().useGravity = false;
		obj.transform.localPosition = new Vector3(0f, 0f, 0f);
		obj.transform.localEulerAngles = new Vector3(270f, 90f, 0f);
		if ((int)globalVal.g_item_once_count[1] > 0)
		{
			Physics.gravity = new Vector3(0f, -5.8f, 0f);
		}
		else
		{
			Physics.gravity = new Vector3(0f, -9.8f, 0f);
		}
		aniTrans = obj.transform.parent.Find("ani").gameObject;
		Transform transform = obj.transform.parent.Find("air");
		if (transform != null)
		{
			transform.parent = null;
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		Camera.main.transform.position = GameObject.Find("CameraReadyTrans").transform.position;
		Camera.main.transform.eulerAngles = new Vector3(9f, 0f, 0f);
		aniTrans.transform.position = obj.transform.position;
		aniTrans.transform.localEulerAngles = Vector3.zero;
		localPoint = aniTrans.transform.position - bodyoffset;
		aniTrans.GetComponent<Animation>().Play("ready");
		obj.SendMessage("ResetRigid");
		obj.SendMessage("ResetPrivateValue");
		ItemManagerClass.body.SetBaseX(0f);
		ShowAnimation(true);
		state = IdleState.STAND;
		audios.StopAllAudio();
		audios.ReplayMusic();
		audios.PlayMusic("BGMrandom");
		MonoBehaviour.print("avatar id : " + globalVal.g_avatar_id);
		if (globalVal.g_avatar_id == 0)
		{
			audios.PlayAudio("VOready", base.transform);
		}
		ResetColliderCount();
	}

	private void ResetColliderCount()
	{
		int count = ItemManagerClass.body.attributeArray.Count;
		for (int i = 0; i < count; i++)
		{
			ItemAttribute itemAttribute = ItemManagerClass.body.attributeArray[i] as ItemAttribute;
			itemAttribute.colliderCount = 0;
		}
	}

	private void Update()
	{
		if (listener != null)
		{
			listener.position = Camera.main.transform.position;
		}
		if (VoiceKey)
		{
			VoiceTime += Time.fixedDeltaTime;
			if (VoiceTime > VoiceEndTime)
			{
				CheckVoiceState();
			}
		}
		switch (state)
		{
		case IdleState.STORY:
			RubberbandClass.rubber.AddRubber(point1.position, localPoint, Vector3.up, Vector3.right, Color.white);
			RubberbandClass.rubber.AddRubber(point2.position, localPoint, Vector3.up, Vector3.right, Color.white);
			break;
		case IdleState.STAND:
			obj.transform.localPosition = Vector3.zero;
			UpdateInput();
			RubberbandClass.rubber.AddRubber(point1.position, rightHand.position, Vector3.up, Vector3.right, Color.white);
			RubberbandClass.rubber.AddRubber(point2.position, leftHand.position, Vector3.up, Vector3.right, Color.white);
			RubberbandClass.rubber.AddRubber(leftHand.position, rightHand.position, Vector3.up, Vector3.right, Color.white);
			break;
		case IdleState.SHOTING:
		{
			UpdateInput();
			Debug.DrawLine(Camera.main.transform.position, ray_hit.point);
			if (!Physics.Raycast(ray, out ray_hit, 100f, shootLayer))
			{
				break;
			}
			Vector3 position2 = point1.position;
			position2.z = 0f;
			position2 = ray_hit.point - position2;
			float num5 = 3f;
			float num6 = Vector3.Angle(Vector3.up, position2);
			float num7 = Vector3.Distance(Vector3.zero, position2);
			if (position2.x < 0f)
			{
				num6 = 360f - num6;
			}
			Vector3 zero5 = Vector3.zero;
			if (num7 > num5)
			{
				num7 = num5;
				zero5.x = Mathf.Sin(num6 * ((float)Math.PI / 180f)) * num7;
				zero5.y = Mathf.Cos(num6 * ((float)Math.PI / 180f)) * num7;
				Vector3 position3 = point1.position;
				position3.z = 0f;
				localPoint = zero5 + position3;
				if (rand > 0.5f)
				{
					randKey = true;
				}
				if (rand < 0f)
				{
					randKey = false;
				}
				if (randKey)
				{
					rand -= Time.deltaTime;
				}
				else
				{
					rand += Time.deltaTime;
				}
			}
			else
			{
				zero5.x = Mathf.Sin(num6 * ((float)Math.PI / 180f)) * num7;
				zero5.y = Mathf.Cos(num6 * ((float)Math.PI / 180f)) * num7;
				localPoint = ray_hit.point;
				rand = 0f;
			}
			Vector3 position4 = point1.position;
			position4.z = 0f;
			position4 -= localPoint;
			uiRect.rect.height = Vector3.Distance(Vector3.zero, position4) / num5 * (0.5f + rand) * 168f;
			if ((bool)obj)
			{
				obj.transform.position = localPoint + bodyoffset;
				aniTrans.transform.position = obj.transform.position;
			}
			if (zero5.x <= 0f)
			{
				float num8 = 0.5f;
				if ((int)globalVal.g_item_once_count[0] > 0)
				{
					num8 = 1f;
				}
				base.GetComponent<Animation>()["action"].normalizedTime = zero5.x * 0.33333f * -1f * num8;
			}
			else
			{
				base.GetComponent<Animation>()["action"].normalizedTime = 0f;
			}
			base.GetComponent<Animation>()["action"].enabled = true;
			base.GetComponent<Animation>().Sample();
			base.GetComponent<Animation>()["action"].enabled = false;
			RubberbandClass.rubber.AddRubber(point1.position, rightHand.position, Vector3.up, Vector3.right, Color.white);
			RubberbandClass.rubber.AddRubber(point2.position, leftHand.position, Vector3.up, Vector3.right, Color.white);
			RubberbandClass.rubber.AddRubber(leftHand.position, rightHand.position, Vector3.up, Vector3.right, Color.white);
			break;
		}
		case IdleState.SHOT_FLYING:
		{
			Vector3 zero = Vector3.zero;
			switch (playerAnim)
			{
			case AnimList.ANIM1:
			{
				zero = obj.transform.position;
				float num2 = Vector3.Angle(Vector3.up, obj.GetComponent<Rigidbody>().velocity);
				Vector3 zero3 = Vector3.zero;
				zero3.x = num2 - 90f;
				aniTrans.transform.localEulerAngles = zero3;
				if (!aniTrans.GetComponent<Animation>().isPlaying)
				{
					playerAnim = AnimList.NONE;
				}
				ttime += Time.fixedDeltaTime * 0.3f;
				break;
			}
			case AnimList.ANIM2:
			{
				zero = obj.transform.position;
				float num3 = Vector3.Angle(Vector3.up, obj.GetComponent<Rigidbody>().velocity);
				Vector3 zero4 = Vector3.zero;
				zero4.x = num3 - 90f;
				aniTrans.transform.localEulerAngles = zero4;
				if (!aniTrans.GetComponent<Animation>().isPlaying)
				{
					playerAnim = AnimList.NONE;
				}
				air = obj.transform.parent.Find("air");
				ttime += Time.fixedDeltaTime * 0.3f;
				break;
			}
			default:
			{
				float num = Vector3.Angle(Vector3.up, obj.GetComponent<Rigidbody>().velocity);
				Vector3 zero2 = Vector3.zero;
				zero2.x = num - 90f;
				aniTrans.transform.localEulerAngles = zero2;
				zero = obj.transform.position;
				ttime -= Time.deltaTime;
				if (ttime < 0f)
				{
					ttime = 0f;
				}
				helpTime += Time.fixedDeltaTime;
				break;
			}
			}
			zero.z = 0f;
			obj.transform.position = zero;
			aniTrans.transform.position = obj.transform.position;
			Vector3 position = Camera.main.transform.position;
			if (playerAnim == AnimList.ANIM2)
			{
				if (ttime > 1f)
				{
					ttime = 1f;
				}
				zero = Vector3.Lerp(obj.transform.position, air.position, ttime);
			}
			if (zero.x > position.x + viewOffset.x)
			{
				position.x = zero.x + viewOffset.x * -1f;
			}
			else if (zero.x < position.x + viewOffset.y && position.x > 50f)
			{
				position.x = zero.x + viewOffset.y * -1f;
			}
			if (zero.y > position.y + viewOffset.width)
			{
				position.y = zero.y + viewOffset.width * -1f;
			}
			if (zero.y < position.y + viewOffset.height)
			{
				position.y = zero.y + viewOffset.height * -1f;
			}
			if (zero.x > trackPosRec)
			{
				trackPosRec += trackInterval;
				TrackInfo trackInfo = new TrackInfo();
				trackInfo.pos = obj.transform.position;
				trackInfo.type = 2;
				posTrack.Add(trackInfo);
			}
			if (zero.y < baseFloorY)
			{
			}
			if (Physics.Raycast(obj.transform.position, Vector3.down, out ray_hit, 100f, floorLayer) && ray_hit.transform != null)
			{
				Debug.DrawLine(obj.transform.position, ray_hit.point);
				float num4 = 3.28f;
				if (position.y < ray_hit.point.y + num4)
				{
					position.y = ray_hit.point.y + num4;
				}
			}
			if (position.z > -10f)
			{
				position.z = -10f;
			}
			Camera.main.transform.position = position;
			break;
		}
		case IdleState.NORMAL:
			break;
		}
	}

	private void FixedUpdate()
	{
		gameTime += Time.fixedDeltaTime;
		switch (state)
		{
		case IdleState.SHOT_FLYING:
			if (helpTime > helpEndTime && !ufo.onWake)
			{
				if (obj.transform.position.y > 5f && !globalVal.g_help_key[1] && obj.transform.position.x > 20f && !inhelp)
				{
					ItemAttribute attributeByName = ItemManagerClass.body.GetAttributeByName("equip_rocket");
					PlayerScript playerScript = obj.GetComponent(typeof(PlayerScript)) as PlayerScript;
					bool controlBtnState = playerScript.GetControlBtnState();
					MonoBehaviour.print(controlBtnState);
					int num = (int)globalVal.g_itemlevel[attributeByName.index];
					if (num > 0 && controlBtnState)
					{
						inhelp = true;
						gameUI gameUI2 = UIs.GetComponent(typeof(gameUI)) as gameUI;
						gameUI2.InitHelp(HelpState.HUOJIAN);
						Time.timeScale = 0f;
					}
				}
				if (obj.transform.position.y > 40f && obj.transform.position.y < 100f && !globalVal.g_help_key[2] && obj.transform.position.x > 20f && !inhelp)
				{
					ItemAttribute attributeByName2 = ItemManagerClass.body.GetAttributeByName("equip_glider");
					PlayerScript playerScript2 = obj.GetComponent(typeof(PlayerScript)) as PlayerScript;
					bool controlBtnState2 = playerScript2.GetControlBtnState();
					int num2 = (int)globalVal.g_itemlevel[attributeByName2.index];
					if (num2 > 0 && controlBtnState2)
					{
						inhelp = true;
						gameUI gameUI3 = UIs.GetComponent(typeof(gameUI)) as gameUI;
						gameUI3.InitHelp(HelpState.HUAXIANG);
						Time.timeScale = 0f;
					}
				}
			}
			PlayerScriptClass.playerInfo.playerFlyDis += Vector3.Distance(playerFlyPoint, obj.transform.position);
			playerFlyPoint = obj.transform.position;
			break;
		case IdleState.GAMEOVER:
		case IdleState.GAMEOVERCALLBACK:
			break;
		case IdleState.FLYING:
			break;
		}
	}

	private void CheckVoiceState()
	{
		Vector3 velocity = obj.GetComponent<Rigidbody>().velocity;
		velocity.z = 0f;
		float num = Vector3.Distance(Vector3.zero, velocity);
		float num2 = Vector3.Angle(Vector3.up, velocity);
		if (num2 < 60f && !isup && num > 25f)
		{
			isup = true;
			if (globalVal.g_avatar_id == 0)
			{
				audios.PlayAudio("VOupward", obj.transform);
			}
			MonoBehaviour.print("--------VOupward");
		}
		else if (num2 > 135f && num >= 40f && !isdown)
		{
			isdown = true;
			if (globalVal.g_avatar_id == 0)
			{
				audios.PlayAudio("VOdownward", obj.transform);
			}
			MonoBehaviour.print("--------VOdownward");
		}
		if (isup && isdown)
		{
			VoiceKey = false;
		}
		else
		{
			VoiceEndTime += UnityEngine.Random.Range(3f, 5f);
		}
	}

	private void BeganVoice()
	{
		VoiceKey = true;
		VoiceTime = 0f;
		VoiceEndTime = UnityEngine.Random.Range(3f, 5f);
		isup = false;
		isdown = false;
	}

	public void ShowAnimation(bool iscan)
	{
		if (!iscan)
		{
			inhelp = false;
			helpTime = 0f;
			gameUI gameUI2 = UIs.GetComponent(typeof(gameUI)) as gameUI;
			if (gameUI2.uitype != HelpState.TAP)
			{
				helpEndTime = 3f;
			}
			aniEndKey = true;
		}
		ShowRagdoll(!iscan);
		ShowAniBody(iscan);
		isAniBody = iscan;
	}

	public void AddTrackPoint(int type)
	{
		trackPosRec += trackInterval;
		TrackInfo trackInfo = new TrackInfo();
		trackInfo.pos = obj.transform.position;
		trackInfo.type = type;
		posTrack.Add(trackInfo);
	}

	private void ShowRagdoll(bool iscan)
	{
		AvatarAttribute avatarAttribute = ItemManagerClass.body.avatarArray[globalVal.g_avatar_id] as AvatarAttribute;
		Transform transform = obj.transform.parent.Find(avatarAttribute.modelname);
		if ((bool)transform)
		{
			transform.GetComponent<Renderer>().enabled = iscan;
		}
	}

	private void ShowAniBody(bool iscan)
	{
		AvatarAttribute avatarAttribute = ItemManagerClass.body.avatarArray[globalVal.g_avatar_id] as AvatarAttribute;
		Transform transform = obj.transform.parent.Find("ani/" + avatarAttribute.modelname);
		if ((bool)transform)
		{
			transform.GetComponent<Renderer>().enabled = iscan;
		}
	}

	private void UpdateInput()
	{
		if (globalVal.UIState == UILayer.PAUSE)
		{
			return;
		}
		UITouchInner[] array = iPhoneInputMgr.MockTouches();
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].phase == TouchPhase.Began)
			{
				OnMouseDown(array[i].fingerId, array[i].position);
			}
			if (array[i].phase == TouchPhase.Moved)
			{
				OnMouseMove(array[i].fingerId, array[i].position);
			}
			if (array[i].phase == TouchPhase.Ended)
			{
				OnMouseUp(array[i].fingerId, array[i].position);
			}
		}
	}

	private void OnMouseDown(int fingerId, Vector2 mousePosition)
	{
		ray = Camera.main.ScreenPointToRay(mousePosition);
		if (Physics.Raycast(ray, out ray_hit, 100f))
		{
			float num = Vector3.Distance(ray_hit.point, ray_hit.transform.position);
			if (ray_hit.transform.name == "Plane" && num < 1.5f)
			{
				touchId = fingerId;
				base.GetComponent<Animation>().Play("action");
				base.GetComponent<Animation>()["action"].normalizedTime = 0f;
				state = IdleState.SHOTING;
				gameUI gameUI2 = UIs.GetComponent(typeof(gameUI)) as gameUI;
				gameUI2.StopHelp(HelpState.TAP);
				audios.PlayAudio("FXbow");
				playBuffEff(0);
			}
		}
	}

	private void OnMouseMove(int fingerId, Vector2 mousePosition)
	{
		if (fingerId == touchId)
		{
			ray = Camera.main.ScreenPointToRay(mousePosition);
			audios.PlayAudio("FXbow");
		}
	}

	private void OnMouseUp(int fingerId, Vector2 mousePosition)
	{
		if (fingerId == touchId)
		{
			touchId = -1;
			state = IdleState.SHOT_FLYING;
			ShowAnimation(false);
			gameTimeKey = true;
			gameTime = 0f;
			obj.GetComponent<Rigidbody>().useGravity = true;
			obj.GetComponent<Rigidbody>().isKinematic = false;
			Vector3 position = point1.position;
			position.z = 0f;
			position -= localPoint;
			MonoBehaviour.print("t : " + position);
			playerFlyPoint = obj.transform.position;
			ItemAttribute attributeByName = ItemManagerClass.body.GetAttributeByName("equip_dangong1");
			int index = (int)globalVal.g_itemlevel[attributeByName.index];
			ItemSubAttr itemSubAttr = attributeByName.level[index] as ItemSubAttr;
			if (rand > 0.45f)
			{
				audios.PlayAudio("UICheer");
			}
			shotPower = position * itemSubAttr.value * (1f + rand * 2f) / 3f;
			if ((int)globalVal.g_item_once_count[0] > 0)
			{
				float num = Vector3.Angle(Vector3.up, shotPower);
				float num2 = Vector3.Distance(Vector3.zero, shotPower);
				num2 += 50f;
				Vector3 zero = Vector3.zero;
				zero.x = Mathf.Sin(num * ((float)Math.PI / 180f)) * num2;
				zero.y = Mathf.Cos(num * ((float)Math.PI / 180f)) * num2;
				shotPower = zero;
			}
			if ((int)globalVal.g_item_once_count[1] > 0)
			{
				playBuffEff(1);
				shotPower *= 0.9f;
			}
			ItemManagerClass.body.SetPlayerSleep(true);
			ItemManagerClass.body.SetPlayerAddForce(shotPower);
			audios.StopAudio("FXbow");
			BeganVoice();
			TrackInfo trackInfo = new TrackInfo();
			trackInfo.pos = obj.transform.position;
			trackInfo.type = 1;
			posTrack.Add(trackInfo);
			trackPosRec = trackInfo.pos.x + trackInterval;
			float num3 = Vector3.Distance(Vector3.zero, shotPower);
			MonoBehaviour.print("shot power : " + num3);
			if (num3 > 20f && num3 < 60f)
			{
				playAudioShoot();
			}
			if (num3 >= 60f)
			{
				playAudioShootHard();
			}
			audios.PlayAudio("FXloosen", obj.transform);
			gameUI gameUI2 = UIs.GetComponent(typeof(gameUI)) as gameUI;
			gameUI2.HidePowerSlider();
			gameUI2.InitControl();
		}
	}

	public void playBuffEff(int index)
	{
		MonoBehaviour.print("index : " + index + "    count : " + (int)globalVal.g_item_once_count[index]);
		if ((int)globalVal.g_item_once_count[index] > 0)
		{
			int num = (int)globalVal.g_item_once_count[index];
			num--;
			globalVal.g_item_once_count[index] = num;
			StatisticsData.data.d_itemUsedCount[index] = (int)StatisticsData.data.d_itemUsedCount[index] + 1;
			GameObject original = Resources.Load("Prefab/tishi_" + (index + 1)) as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
			gameUI gameUI2 = UIs.GetComponent(typeof(gameUI)) as gameUI;
			gameObject.transform.parent = gameUI2.transform;
			gameObject.transform.localPosition = new Vector3(0f, -45f, 0f);
			buffTransArray.Add(gameObject.transform);
			original = Resources.Load("Prefab/tishi_p") as GameObject;
			gameObject = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.transform.parent = gameUI2.transform;
			gameObject.transform.localPosition = new Vector3(0f, -45f, 0f);
			buffTransArray.Add(gameObject.transform);
			audios.PlayAudio("UIwarn");
			StatisticsData.data.SaveDailyData(StatisticsData.data.GetSystemDay());
			globalVal.SaveFile("saveData.txt");
		}
	}

	private void ClearBuffEff()
	{
		if (buffTransArray.Count < 1)
		{
			return;
		}
		for (int i = 0; i < buffTransArray.Count; i++)
		{
			Transform transform = buffTransArray[i] as Transform;
			foreach (Transform item in transform)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		buffTransArray.Clear();
	}

	private void playAudioShoot()
	{
		switch (globalVal.g_avatar_id)
		{
		case 0:
			audios.PlayAudio("SVOshoot_normal", obj.transform);
			break;
		case 1:
			audios.PlayAudio("SVOtintin_shoot_normal", obj.transform);
			break;
		case 2:
			audios.PlayAudio("SVOindiana J_shoot_normal", obj.transform);
			break;
		case 3:
			audios.PlayAudio("SVOsoldier_shoot_normal", obj.transform);
			break;
		case 4:
			audios.PlayAudio("SVOterminator_shoot_normal", obj.transform);
			break;
		case 5:
			audios.PlayAudio("SVOhalo_shoot_normal", obj.transform);
			break;
		}
	}

	private void playAudioShootHard()
	{
		switch (globalVal.g_avatar_id)
		{
		case 0:
			audios.PlayAudio("SVOshoot_hard", obj.transform);
			break;
		case 1:
			audios.PlayAudio("SVOtintin_shoot_hard", obj.transform);
			break;
		case 2:
			audios.PlayAudio("SVOindiana J_shoot_hard", obj.transform);
			break;
		case 3:
			audios.PlayAudio("SVOsoldier_shoot_hard", obj.transform);
			break;
		case 4:
			audios.PlayAudio("SVOterminator_shoot_hard", obj.transform);
			break;
		case 5:
			audios.PlayAudio("SVOhalo_shoot_hard", obj.transform);
			break;
		}
	}

	private void audioFollow(string name)
	{
		folTrans = audios.transform.Find("Audio/" + name);
	}

	public void ReStart()
	{
		SetAvatar(globalVal.g_avatar_id);
		InitStart();
	}

	public void GameOver()
	{
		state = IdleState.GAMEOVER;
		audios.PlayMusic("BGMtitle");
		gameTimeKey = false;
		if (StatisticsData.data.d_gameLongTime < (int)gameTime)
		{
			StatisticsData.data.d_gameLongTime = (int)gameTime;
		}
		if (StatisticsData.data.d_playCount > 0)
		{
			StatisticsData.data.d_gameNormalTime = (int)((float)(StatisticsData.data.d_gameNormalTime + (int)gameTime) * 0.5f);
		}
		else
		{
			StatisticsData.data.d_gameNormalTime = (int)gameTime;
		}
		if (StatisticsData.data.d_gameShotTime > (int)gameTime)
		{
			StatisticsData.data.d_gameShotTime = (int)gameTime;
		}
		if ((float)StatisticsData.data.d_gameLongDis < PlayerScriptClass.playerInfo.distance)
		{
			StatisticsData.data.d_gameLongDis = (int)PlayerScriptClass.playerInfo.distance;
		}
		if (StatisticsData.data.d_playCount > 0)
		{
			StatisticsData.data.d_gameNormalDis = (int)((float)(StatisticsData.data.d_gameNormalDis + (int)PlayerScriptClass.playerInfo.distance) * 0.5f);
		}
		else
		{
			StatisticsData.data.d_gameNormalDis = (int)PlayerScriptClass.playerInfo.distance;
		}
		if ((float)StatisticsData.data.d_gameShotDis > PlayerScriptClass.playerInfo.distance)
		{
			StatisticsData.data.d_gameShotDis = (int)PlayerScriptClass.playerInfo.distance;
		}
		if (PlayerScriptClass.playerInfo.distance >= 5000f || globalVal.g_achievement_key[0])
		{
			GameCenterPlugin.SubmitAchievement("com.trinitigame.miniglider.a1", 100);
			globalVal.g_achievement_key[0] = true;
			MonoBehaviour.print("com.trinitigame.miniglider.a1");
		}
		if (PlayerScriptClass.playerInfo.height >= 400f || globalVal.g_achievement_key[1])
		{
			GameCenterPlugin.SubmitAchievement("com.trinitigame.miniglider.a2", 100);
			globalVal.g_achievement_key[1] = true;
			MonoBehaviour.print("com.trinitigame.miniglider.a2");
		}
		if (PlayerScriptClass.playerInfo.speed >= 60f || globalVal.g_achievement_key[2])
		{
			GameCenterPlugin.SubmitAchievement("com.trinitigame.miniglider.a3", 100);
			globalVal.g_achievement_key[2] = true;
			MonoBehaviour.print("com.trinitigame.miniglider.a3");
		}
		if (PlayerScriptClass.playerInfo.goldCount >= 100 || globalVal.g_achievement_key[3])
		{
			GameCenterPlugin.SubmitAchievement("com.trinitigame.miniglider.a4", 100);
			globalVal.g_achievement_key[3] = true;
			MonoBehaviour.print("com.trinitigame.miniglider.a4");
		}
		if (PlayerScriptClass.playerInfo.goldCount >= 500 || globalVal.g_achievement_key[4])
		{
			GameCenterPlugin.SubmitAchievement("com.trinitigame.miniglider.a5", 100);
			globalVal.g_achievement_key[4] = true;
			MonoBehaviour.print("com.trinitigame.miniglider.a5");
		}
		if (PlayerScriptClass.playerInfo.heightUp10Distance >= 1000 || globalVal.g_achievement_key[5])
		{
			GameCenterPlugin.SubmitAchievement("com.trinitigame.miniglider.a6", 100);
			globalVal.g_achievement_key[5] = true;
			MonoBehaviour.print("com.trinitigame.miniglider.a6");
		}
		if (PlayerScriptClass.playerInfo.firstTouchFloorDistance >= 3000 || globalVal.g_achievement_key[6])
		{
			GameCenterPlugin.SubmitAchievement("com.trinitigame.miniglider.a7", 100);
			globalVal.g_achievement_key[6] = true;
			MonoBehaviour.print("com.trinitigame.miniglider.a7");
		}
		if (globalVal.g_best_zombiebreak >= 50 || globalVal.g_achievement_key[7])
		{
			GameCenterPlugin.SubmitAchievement("com.trinitigame.miniglider.a8", 100);
			globalVal.g_achievement_key[7] = true;
			MonoBehaviour.print("com.trinitigame.miniglider.a8");
		}
		if (globalVal.g_best_zombiebreak >= 100 || globalVal.g_achievement_key[8])
		{
			GameCenterPlugin.SubmitAchievement("com.trinitigame.miniglider.a9", 100);
			globalVal.g_achievement_key[8] = true;
			MonoBehaviour.print("com.trinitigame.miniglider.a9");
		}
		if (PlayerScriptClass.playerInfo.heightUp1500Time >= 20f || globalVal.g_achievement_key[9])
		{
			GameCenterPlugin.SubmitAchievement("com.trinitigame.miniglider.a10", 100);
			globalVal.g_achievement_key[9] = true;
			MonoBehaviour.print("com.trinitigame.miniglider.a10");
		}
		if (PlayerScriptClass.playerInfo.heightDown50Distance >= 5000f || globalVal.g_achievement_key[10])
		{
			GameCenterPlugin.SubmitAchievement("com.trinitigame.miniglider.a11", 100);
			globalVal.g_achievement_key[10] = true;
			MonoBehaviour.print("com.trinitigame.miniglider.a11");
		}
		if (globalVal.g_achievement_key[11])
		{
			GameCenterPlugin.SubmitAchievement("com.trinitigame.miniglider.a12", 100);
			MonoBehaviour.print("com.trinitigame.miniglider.a12");
		}
		if (globalVal.g_achievement_key[12])
		{
			GameCenterPlugin.SubmitAchievement("com.trinitigame.miniglider.a13", 100);
			MonoBehaviour.print("com.trinitigame.miniglider.a13");
		}
		if (globalVal.UIState != UILayer.GAMEOVER)
		{
			globalVal.LastTrack = (ArrayList)globalVal.CurrentTrack.Clone();
			globalVal.CurrentTrack = (ArrayList)posTrack.Clone();
			StatisticsData.data.SaveDailyData(StatisticsData.data.GetSystemDay());
			globalVal.SaveTrack("Record.txt");
			globalVal.SaveFile("saveData.txt");
			globalVal.UIState = UILayer.GAMEOVER;
		}
		gameUI gameUI2 = UIs.GetComponent(typeof(gameUI)) as gameUI;
		gameUI2.InitGameOver();
	}
}
