using System.Collections;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
	private bool updateKey;

	private GameObject player;

	private GameObject effectObj;

	private float startTime;

	private float endTime;

	private Transform trans;

	private GameObject InstantObj;

	public GameObject[] effData = new GameObject[0];

	public ArrayList effGold = new ArrayList();

	public ArrayList effRedGold = new ArrayList();

	private GameObject effground;

	private ArrayList effect_follow = new ArrayList();

	private ArrayList follow_target = new ArrayList();

	private ArrayList array_jiesuan = new ArrayList();

	private TAudioController audios;

	private void Start()
	{
		EffectManagerClass.body = this;
		GameObject gameObject = GameObject.Find("TAudioController");
		if (gameObject == null)
		{
			gameObject = Object.Instantiate(Resources.Load("TAudioController")) as GameObject;
			gameObject.name = "TAudioController";
			Object.DontDestroyOnLoad(gameObject);
		}
		effground = new GameObject("eff");
		InitGoldEffectContainer();
		InitRedGoldContainer();
		player = GameObject.Find("player2/Bip01");
		updateKey = false;
		effect_follow.Clear();
		follow_target.Clear();
		audios = gameObject.GetComponent(typeof(TAudioController)) as TAudioController;
	}

	private void Update()
	{
		if (updateKey)
		{
			startTime += Time.deltaTime;
			if (startTime < endTime)
			{
				effectObj.transform.position = player.transform.position;
			}
			else
			{
				updateKey = false;
			}
		}
		if (trans != null && InstantObj != null)
		{
			InstantObj.transform.position = trans.position;
		}
		for (int i = 0; i < effect_follow.Count; i++)
		{
			Transform transform = follow_target[i] as Transform;
			if (transform == null)
			{
				follow_target.RemoveAt(i);
				effect_follow.RemoveAt(i);
			}
			Transform transform2 = effect_follow[i] as Transform;
			if (transform2 == null)
			{
				effect_follow.RemoveAt(i);
				follow_target.RemoveAt(i);
			}
			else
			{
				transform2.position = transform.position;
			}
		}
	}

	private void InitGoldEffectContainer()
	{
		effGold.Clear();
		for (int i = 0; i < 20; i++)
		{
			GameObject gameObject = Object.Instantiate(effData[3]) as GameObject;
			gameObject.transform.parent = effground.transform;
			gameObject.name = effData[3].name;
			gameObject.active = false;
			effGold.Add(gameObject);
		}
	}

	private void InitRedGoldContainer()
	{
		effRedGold.Clear();
		for (int i = 0; i < 20; i++)
		{
			GameObject gameObject = Object.Instantiate(effData[19]) as GameObject;
			gameObject.transform.parent = effground.transform;
			gameObject.name = effData[19].name;
			gameObject.active = false;
			effRedGold.Add(gameObject);
		}
	}

	private GameObject GetGoldFlyEff()
	{
		GameObject result = null;
		if (effGold.Count < 1)
		{
			return result;
		}
		result = effGold[0] as GameObject;
		effGold.RemoveAt(0);
		return result;
	}

	private GameObject GetRedGoldFlyEff()
	{
		GameObject result = null;
		if (effRedGold.Count < 1)
		{
			return result;
		}
		result = effRedGold[0] as GameObject;
		effRedGold.RemoveAt(0);
		return result;
	}

	public void PlayGoldFlyEffect(Transform itemTrans, int value)
	{
		GameObject goldFlyEff = GetGoldFlyEff();
		if (goldFlyEff != null)
		{
			goldFlyEff.active = true;
			GoldEffectScript goldEffectScript = goldFlyEff.GetComponent(typeof(GoldEffectScript)) as GoldEffectScript;
			goldEffectScript.GoldFly(Camera.main.WorldToViewportPoint(itemTrans.position), value);
		}
	}

	public void PlayRedGoldFlyEffect(Transform itemTrans, int value)
	{
		GameObject redGoldFlyEff = GetRedGoldFlyEff();
		if (redGoldFlyEff != null)
		{
			redGoldFlyEff.active = true;
			GoldEffectScript goldEffectScript = redGoldFlyEff.GetComponent(typeof(GoldEffectScript)) as GoldEffectScript;
			goldEffectScript.GoldFly(Camera.main.WorldToViewportPoint(itemTrans.position), value);
		}
	}

	public void PlayGoldFlyEffect(Transform itemTrans, int value, bool isbig)
	{
		GameObject goldFlyEff = GetGoldFlyEff();
		if (goldFlyEff != null)
		{
			goldFlyEff.active = true;
			GoldEffectScript goldEffectScript = goldFlyEff.GetComponent(typeof(GoldEffectScript)) as GoldEffectScript;
			if (isbig)
			{
				goldEffectScript.GoldFly_big(Camera.main.WorldToViewportPoint(itemTrans.position), value);
			}
			else
			{
				goldEffectScript.GoldFly(Camera.main.WorldToViewportPoint(itemTrans.position), value);
			}
		}
	}

	public GameObject PlayGoldFlyEffect_UI(Transform itemTrans, int value)
	{
		GameObject goldFlyEff = GetGoldFlyEff();
		if (goldFlyEff != null)
		{
			goldFlyEff.active = true;
			GoldEffectScript goldEffectScript = goldFlyEff.GetComponent(typeof(GoldEffectScript)) as GoldEffectScript;
			Camera cameraByName = GetCameraByName("TUICamera");
			goldEffectScript.GoldFly_UI(cameraByName.WorldToViewportPoint(itemTrans.position), value);
		}
		return goldFlyEff;
	}

	private Camera GetCameraByName(string name)
	{
		Camera[] allCameras = Camera.allCameras;
		Camera camera = null;
		for (int i = 0; i < allCameras.Length; i++)
		{
			camera = allCameras[i];
			if (camera.name == name)
			{
				return camera;
			}
		}
		return camera;
	}

	private void playCloudCrosse(GameObject obj)
	{
		startTime = 0f;
		endTime = 3f;
		updateKey = true;
		effectObj = obj;
	}

	private void playGoodStarEffect(GameObject obj, float time)
	{
		startTime = 0f;
		endTime = time;
		updateKey = true;
		effectObj = obj;
	}

	public void PlayEffect(string name, Transform itemTrans, bool isUpdata)
	{
		GameObject original = null;
		switch (name)
		{
		case "effect_bleeding":
			original = effData[0];
			break;
		case "effect_fireballoon":
			original = effData[15];
			break;
		case "effect_goldtouch":
			original = effData[18];
			break;
		case "effect_mask":
			original = effData[21];
			break;
		case "effect_flaming":
			original = effData[22];
			break;
		}
		InstantObj = Object.Instantiate(original) as GameObject;
		InstantObj.transform.position = itemTrans.position;
		InstantObj.transform.rotation = itemTrans.rotation;
		if (isUpdata)
		{
			effect_follow.Add(InstantObj.transform);
			follow_target.Add(itemTrans);
		}
	}

	public void PlayEffect_fireballoon(Transform itemTrans, int type)
	{
		Quaternion identity = Quaternion.identity;
		identity = itemTrans.rotation;
		GameObject gameObject = null;
		gameObject = effData[15];
		Vector3 eulerAngles = itemTrans.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.y = 180f;
		identity.eulerAngles = eulerAngles;
		GameObject gameObject2 = Object.Instantiate(gameObject) as GameObject;
		gameObject2.transform.position = itemTrans.position;
		gameObject2.transform.rotation = identity;
		if (type == 1)
		{
			gameObject2.GetComponent<Animation>()["crash"].speed = 2f;
			gameObject2.GetComponent<Animation>().Play("crash");
		}
		if (type == 2)
		{
			gameObject2.GetComponent<Animation>()["drop2"].speed = 2f;
			gameObject2.GetComponent<Animation>().Play("drop2");
		}
		gameObject2.GetComponent<Rigidbody>().velocity = player.GetComponent<Rigidbody>().velocity * 1.1f;
		gameObject2.AddComponent(typeof(fireballoonScript));
	}

	public void RemoveEffect_jiesuan()
	{
		for (int i = 0; i < array_jiesuan.Count; i++)
		{
			GameObject obj = array_jiesuan[i] as GameObject;
			Object.Destroy(obj);
		}
	}

	public GameObject PlayEffect_gantanhao()
	{
		GameObject gameObject = null;
		gameObject = effData[25];
		InstantObj = Object.Instantiate(gameObject) as GameObject;
		return InstantObj;
	}

	public void PlayEffect(string name, Transform itemTrans)
	{
		Quaternion identity = Quaternion.identity;
		identity = itemTrans.rotation;
		GameObject gameObject = null;
		switch (name)
		{
		case "effect_bleeding":
			gameObject = effData[0];
			break;
		case "effect_blowup":
			gameObject = effData[1];
			break;
		case "effect_blowup_trampoline":
			gameObject = effData[7];
			break;
		case "effect_blowup_bomb1":
		{
			gameObject = effData[20];
			Vector3 position = itemTrans.position;
			position.y = 0.01f;
			identity = Quaternion.identity;
			gameObject.transform.position = position;
			break;
		}
		case "effect_fallingstar":
			gameObject = effData[2];
			break;
		case "effect_qiliu_1":
			gameObject = effData[3];
			break;
		case "effect_qiliu_2":
			gameObject = effData[4];
			break;
		case "effect_rockfire":
			gameObject = effData[5];
			break;
		case "effect_cannonfire":
			gameObject = effData[7];
			break;
		case "effect_zombie_dead":
		{
			gameObject = effData[6];
			Vector3 position2 = itemTrans.position;
			position2.y = 0f;
			gameObject.transform.position = position2;
			identity = Quaternion.identity;
			break;
		}
		case "effect_cloudtouch":
			gameObject = effData[8];
			break;
		case "effect_cloudcrosse":
			gameObject = effData[9];
			break;
		case "effect_bed":
		{
			gameObject = effData[10];
			Vector3 eulerAngles4 = identity.eulerAngles;
			eulerAngles4.x = 0f;
			identity.eulerAngles = eulerAngles4;
			break;
		}
		case "effect_wagon":
		{
			gameObject = effData[11];
			Vector3 eulerAngles3 = itemTrans.eulerAngles;
			eulerAngles3.y += 180f;
			if (eulerAngles3.y >= 360f)
			{
				eulerAngles3.y -= 360f;
			}
			if (eulerAngles3.y < 0f)
			{
				eulerAngles3.y += 360f;
			}
			identity.eulerAngles = eulerAngles3;
			break;
		}
		case "effect_airliner":
			gameObject = effData[12];
			break;
		case "effect_ufo":
			gameObject = effData[13];
			break;
		case "effect_trampoline":
		{
			gameObject = effData[14];
			Vector3 eulerAngles2 = itemTrans.eulerAngles;
			eulerAngles2.x = 0f;
			identity.eulerAngles = eulerAngles2;
			break;
		}
		case "effect_fireballoon":
		{
			gameObject = effData[15];
			Vector3 eulerAngles = itemTrans.eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.y = 90f;
			identity.eulerAngles = eulerAngles;
			break;
		}
		case "effect_speed":
			gameObject = effData[16];
			break;
		case "effect_goodstartouch":
			gameObject = effData[17];
			break;
		case "effect_goldtouch":
			gameObject = effData[18];
			break;
		case "effect_mask":
			gameObject = effData[21];
			break;
		case "effect_luodi":
			gameObject = effData[23];
			identity.eulerAngles = Vector3.zero;
			break;
		case "effect_jiesuan":
			gameObject = effData[24];
			break;
		case "eff_zombies1":
			gameObject = effData[26];
			break;
		case "eff_Zombie_Nurse":
			gameObject = effData[27];
			break;
		case "eff_Zombie_Swat":
			gameObject = effData[28];
			break;
		case "effect_ui_02":
			gameObject = effData[29];
			break;
		}
		GameObject gameObject2 = Object.Instantiate(gameObject) as GameObject;
		gameObject2.transform.position = itemTrans.position;
		gameObject2.transform.rotation = identity;
		gameObject2.name = gameObject.name;
		if (name == "effect_jiesuan")
		{
			array_jiesuan.Add(gameObject2);
		}
		if (name == "effect_ui_02")
		{
			gameObject2.transform.parent = itemTrans;
		}
		if (name == "effect_zombie_dead")
		{
			gameObject2.GetComponent<Animation>()["Take 001"].speed = 0.5f;
		}
		if (name == "effect_wagon")
		{
			gameObject2.GetComponent<Animation>()["idle"].speed = 0.5f;
		}
		if (name == "effect_bed")
		{
			gameObject2.GetComponent<Animation>()["idle"].speed = 0.5f;
		}
		if (name == "effect_airliner")
		{
			gameObject2.transform.parent = effground.transform;
			gameObject2.GetComponent<Rigidbody>().velocity = new Vector3(player.GetComponent<Rigidbody>().velocity.x * 1.5f, 0f, player.GetComponent<Rigidbody>().velocity.x * 0.3f);
			AirlineEffectScript airlineEffectScript = gameObject2.AddComponent(typeof(AirlineEffectScript)) as AirlineEffectScript;
			airlineEffectScript.RotateOnTime();
		}
		if (name == "effect_ufo")
		{
			gameObject2.GetComponent<Rigidbody>().velocity = new Vector3(75f, -30f, 0f);
		}
		if (name == "effect_cloudcrosse")
		{
			playCloudCrosse(gameObject2);
		}
		if (name == "effect_goodstartouch")
		{
			trans = null;
			playGoodStarEffect(gameObject2, 3f);
		}
		if (name == "effect_bleeding")
		{
			float num = 0f;
			int layerMask = 1 << LayerMask.NameToLayer("floor");
			RaycastHit hitInfo;
			if (Physics.Raycast(gameObject2.transform.position, Vector3.down, out hitInfo, 100f, layerMask) && hitInfo.transform != null)
			{
				num = hitInfo.point.y;
			}
			Vector3 position3 = gameObject2.transform.position;
			position3.y = 0.07f + num;
			gameObject2.transform.position = position3;
			gameObject2.transform.rotation = Quaternion.identity;
		}
		switch (name)
		{
		case "eff_zombies1":
		case "eff_Zombie_Nurse":
		case "eff_Zombie_Swat":
		{
			int num2 = Random.Range(1, 2);
			gameObject2.GetComponent<Animation>().Play("Eat0" + num2);
			audios.PlayAudio("Ani_zombie_eat0" + num2);
			audios.transform.Find("Audio/Ani_zombie_eat0" + num2).localPosition = base.transform.position;
			break;
		}
		}
	}
}
