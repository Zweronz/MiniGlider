using System;
using System.Collections;
using System.Xml;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
	private GameObject baseObj;

	public ArrayList dataArray = new ArrayList();

	public ArrayList goldArray = new ArrayList();

	public ArrayList goldGroundArray = new ArrayList();

	public ArrayList attributeArray = new ArrayList();

	public Hashtable hashAttribute = new Hashtable();

	public ArrayList itemOnceArray = new ArrayList();

	public ArrayList avatarArray = new ArrayList();

	private GameObject items;

	public ArrayList ItemContainer = new ArrayList();

	private GameObject player;

	private PlayerScript players;

	private float baseX;

	private float goldBaseX;

	private float groundGoldBaseX;

	private float offsetX = 40f;

	private float goldOffsetX = 30f;

	private float playerFlyDis;

	public int RefreshTime;

	private TAudioController audios;

	public Hashtable hashTask = new Hashtable();

	public ArrayList taskListArray = new ArrayList();

	private Vector3 lastCameraPoint = Vector3.zero;

	private float startTime;

	private float endTime;

	private bool slowKey;

	private float groundY;

	private RaycastHit ray_hit;

	private bool itemGoldRefreshKey;

	private int itemRefreshCount;

	private Hashtable hashItemTime = new Hashtable();

	private float m_time;

	public bool isloading = true;

	private void Start()
	{
		ItemManagerClass.body = this;
		hashTask.Clear();
		hashItemTime.Clear();
		GameObject gameObject = GameObject.Find("TAudioController");
		if (gameObject == null)
		{
			gameObject = UnityEngine.Object.Instantiate(Resources.Load("TAudioController")) as GameObject;
			gameObject.name = "TAudioController";
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		audios = gameObject.GetComponent(typeof(TAudioController)) as TAudioController;
		ReadAvatarList();
		ReadTaskInfo();
		ReadTaskList();
		ReadItemOnceList();
		ReadItemAttr_itemAttr();
	}

	public void InitStar()
	{
		MonoBehaviour.print("init star");
		AddItem("item_recruit1");
	}

	public void InitGameItems()
	{
		if (Application.loadedLevelName == "scene_game")
		{
			items = new GameObject("items");
			player = GameObject.Find("player2/Bip01");
			players = player.GetComponent(typeof(PlayerScript)) as PlayerScript;
			ReadData();
			InitContainer_new();
		}
	}

	public void ResetItems_new()
	{
		foreach (Transform item in items.transform)
		{
			ArrayList arrayList = hashAttribute[item.gameObject.name] as ArrayList;
			if (!item.gameObject.active)
			{
				continue;
			}
			item.gameObject.active = false;
			foreach (Transform item2 in item)
			{
				item2.gameObject.active = false;
			}
			arrayList.Add(item.gameObject);
		}
	}

	public GameObject GetItem_new(string prefabName)
	{
		GameObject result = null;
		ArrayList arrayList = hashAttribute[prefabName] as ArrayList;
		if (arrayList.Count < 1)
		{
			return result;
		}
		result = arrayList[0] as GameObject;
		arrayList.RemoveAt(0);
		return result;
	}

	private int GetRandomItemId()
	{
		int num = -1;
		int[] array = new int[9] { 0, 2, 6, 0, 3, 6, 0, 4, 6 };
		num = UnityEngine.Random.Range(0, array.Length);
		if (player.transform.position.x < 200f)
		{
			return array[num];
		}
		return UnityEngine.Random.Range(0, 1000) % 7;
	}

	private int GetRandomItemId_new()
	{
		int num = -1;
		int[] array = new int[9] { 0, 2, 6, 0, 3, 6, 0, 4, 6 };
		num = UnityEngine.Random.Range(0, array.Length);
		return array[num];
	}

	private void AddZombieRandom()
	{
		string prefabName = string.Empty;
		int num = -1;
		if (!(player.transform.position.y < 10f + groundY) || ray_hit.transform.parent.name == "scene_cityturnhighway")
		{
			return;
		}
		num = UnityEngine.Random.Range(0, 2);
		if (player.transform.position.x < 2000f)
		{
			num = 0;
		}
		switch (num)
		{
		case 0:
			switch (UnityEngine.Random.Range(1, 4))
			{
			case 1:
				prefabName = "item_zombies1";
				break;
			case 2:
				prefabName = "Zombie_Nurse";
				break;
			case 3:
				prefabName = "Zombie_Swat";
				break;
			}
			AddZombieGround(prefabName);
			break;
		case 1:
			prefabName = "Zombie_Batcher";
			AddItem(prefabName);
			break;
		}
		audios.PlayAudio("SVOzombiez");
	}

	private void AddItemRandom_new()
	{
		string prefabName = string.Empty;
		int num = -1;
		if (player.transform.position.y < 10f + groundY)
		{
			if (ray_hit.transform.parent.name == "scene_cityturnhighway")
			{
				return;
			}
			num = GetRandomItemId_new();
		}
		else if (player.transform.position.y > 35f + groundY && player.transform.position.y < 75f + groundY)
		{
			num = 7;
		}
		else if (player.transform.position.y > 80f + groundY && player.transform.position.y < 100f + groundY)
		{
			num = 10;
		}
		else
		{
			if (!(player.transform.position.y > 105f + groundY) || !(player.transform.position.y < 150f + groundY))
			{
				return;
			}
			num = 12;
		}
		if (num == 5 && player.transform.position.x < 2000f)
		{
			num = 1;
		}
		switch (num)
		{
		case 0:
			prefabName = "item_bomb1";
			break;
		case 2:
			prefabName = "item_bed";
			break;
		case 3:
			prefabName = "item_wagon";
			break;
		case 4:
			prefabName = "item_trampoline";
			break;
		case 6:
			prefabName = "item_landmine";
			break;
		case 7:
			prefabName = "item_bomb2";
			break;
		case 8:
			prefabName = "item_fallingstar1";
			break;
		case 9:
			prefabName = "item_gold";
			break;
		case 10:
			prefabName = "item_fireballoon";
			break;
		case 11:
			prefabName = "item_ufo";
			break;
		case 12:
			prefabName = "item_airliner";
			break;
		}
		switch (num)
		{
		case 11:
		case 12:
			AddItem(prefabName);
			break;
		case 1:
			AddZombieGround(prefabName);
			break;
		default:
			AddItem(prefabName);
			break;
		}
	}

	private void AddItemRandom()
	{
		string prefabName = string.Empty;
		int num = -1;
		if (player.transform.position.y < 10f + groundY)
		{
			num = GetRandomItemId();
		}
		else if (player.transform.position.y > 35f + groundY && player.transform.position.y < 75f + groundY)
		{
			num = 7;
		}
		else if (player.transform.position.y > 80f + groundY && player.transform.position.y < 100f + groundY)
		{
			num = 10;
		}
		else
		{
			if (!(player.transform.position.y > 105f + groundY) || !(player.transform.position.y < 150f + groundY))
			{
				return;
			}
			num = 12;
		}
		if (num == 5 && player.transform.position.x < 2000f)
		{
			num = 1;
		}
		num = 5;
		switch (num)
		{
		case 0:
			prefabName = "item_bomb1";
			break;
		case 1:
			switch (UnityEngine.Random.Range(1, 4))
			{
			case 1:
				prefabName = "item_zombies1";
				break;
			case 2:
				prefabName = "Zombie_Nurse";
				break;
			case 3:
				prefabName = "Zombie_Swat";
				break;
			}
			audios.PlayAudio("SVOzombiez");
			break;
		case 2:
			prefabName = "item_bed";
			break;
		case 3:
			prefabName = "item_wagon";
			break;
		case 4:
			prefabName = "item_trampoline";
			break;
		case 5:
			prefabName = "Zombie_Batcher";
			break;
		case 6:
			prefabName = "item_landmine";
			break;
		case 7:
			prefabName = "item_bomb2";
			break;
		case 8:
			prefabName = "item_fallingstar1";
			break;
		case 9:
			prefabName = "item_gold";
			break;
		case 10:
			prefabName = "item_fireballoon";
			break;
		case 11:
			prefabName = "item_ufo";
			break;
		case 12:
			prefabName = "item_airliner";
			break;
		}
		switch (num)
		{
		case 11:
		case 12:
			if (UnityEngine.Random.Range(0, 100) < 30)
			{
				AddItem(prefabName);
			}
			break;
		case 1:
			AddZombieGround(prefabName);
			break;
		default:
			AddItem(prefabName);
			break;
		}
	}

	private void AddZombieGround(string prefabName)
	{
		if (ray_hit.transform.parent.name == "scene_cityturnhighway")
		{
			return;
		}
		Vector3 position = player.transform.position;
		position.y = 0f + groundY;
		position.z = 0f;
		position.x += 20f;
		int num = 0;
		num = ((!(player.transform.position.x > 1500f)) ? UnityEngine.Random.Range(1, 2) : UnityEngine.Random.Range(3, 5));
		Vector3 pos = position;
		for (int i = 0; i < num; i++)
		{
			float num2 = UnityEngine.Random.Range(2f, 5f);
			pos += new Vector3(num2, 0f, 0f);
			string prefabName2 = string.Empty;
			switch (UnityEngine.Random.Range(1, 4))
			{
			case 1:
				prefabName2 = "item_zombies1";
				break;
			case 2:
				prefabName2 = "Zombie_Nurse";
				break;
			case 3:
				prefabName2 = "Zombie_Swat";
				break;
			}
			AddOneZombie(prefabName2, pos);
			baseX += num2;
		}
	}

	private void AddZombie_Batcher()
	{
		Vector3 position = player.transform.position;
		Vector3 velocity = player.GetComponent<Rigidbody>().velocity;
		float num = (velocity.y + Mathf.Sqrt(velocity.y * velocity.y + 19.6f * (position.y - groundY))) / 9.8f;
		Vector3 zero = Vector3.zero;
		zero.x = position.x + velocity.x * num + 3.2f;
		zero.y = groundY;
		zero.z = position.z;
		string prefabName = "Zombie_Batcher";
		GameObject item_new = GetItem_new(prefabName);
		if (item_new == null)
		{
			return;
		}
		item_new.name = prefabName;
		item_new.active = true;
		foreach (Transform item in item_new.transform)
		{
			item.gameObject.active = true;
		}
		ZombieBossScript zombieBossScript = item_new.GetComponent(typeof(ZombieBossScript)) as ZombieBossScript;
		zombieBossScript.AttackOntime(num - 42f * Time.deltaTime);
		item_new.transform.position = zero;
		audios.PlayAudio("Ani_zombie_run");
	}

	private void AddOneZombie(string prefabName, Vector3 pos)
	{
		GameObject item_new = GetItem_new(prefabName);
		if (item_new == null)
		{
			return;
		}
		if (prefabName != "Zombie_Batcher")
		{
			item_new.GetComponent<Collider>().enabled = true;
		}
		item_new.name = prefabName;
		item_new.active = true;
		foreach (Transform item in item_new.transform)
		{
			item.gameObject.active = true;
		}
		int num = UnityEngine.Random.Range(-1, 1);
		if (num == 0)
		{
			num = 1;
		}
		if (prefabName != "Zombie_Batcher")
		{
			item_new.GetComponent<Rigidbody>().velocity = new Vector3(5 * num, 0f, 0f);
			Vector3 eulerAngles = item_new.transform.eulerAngles;
			eulerAngles.y = 180 + 90 * num * -1;
			item_new.transform.eulerAngles = eulerAngles;
		}
		item_new.transform.position = pos;
		SetItemAttr(item_new);
		audios.PlayAudio("Ani_zombie_run");
	}

	private void AddGoldGround()
	{
		Vector3 position = player.transform.position;
		position.z = 0f;
		int num = UnityEngine.Random.Range(3, 5);
		float num2 = 15f;
		Vector3 velocity = player.GetComponent<Rigidbody>().velocity;
		float num3 = Vector3.Angle(Vector3.up, velocity);
		num3 += UnityEngine.Random.Range(-10f, 10f);
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.Sin(num3 * ((float)Math.PI / 180f)) * num2;
		zero.y = Mathf.Cos(num3 * ((float)Math.PI / 180f)) * num2;
		Vector3 vector = position + zero;
		if (!(vector.y <= 10f + groundY))
		{
			for (int i = 0; i < num; i++)
			{
				Vector3 pos = vector + new Vector3(2 * i, 0f, 0f);
				AddOneGold("item_gold", pos);
			}
		}
	}

	public void InitContainer_new()
	{
		hashAttribute.Clear();
		int num = 10;
		InitItemData_new("item_gold", num * 9);
		InitItemData_new("item_fallingstar1", num);
		InitItemData_new("item_bomb1", num);
		InitItemData_new("item_bomb2", num);
		InitItemData_new("item_zombies1", num);
		InitItemData_new("Zombie_Nurse", num);
		InitItemData_new("Zombie_Swat", num);
		InitItemData_new("item_recruit1", num);
		InitItemData_new("item_gold2", num * 3);
		InitItemData_new("item_gold3", num * 3);
		InitItemData_new("item_bed", num);
		InitItemData_new("item_wagon", num);
		InitItemData_new("item_trampoline", num);
		InitItemData_new("Zombie_Batcher", num);
		InitItemData_new("item_fireballoon", num);
		InitItemData_new("item_bird1", num);
		InitItemData_new("item_airliner", num);
		InitItemData_new("item_ufo", num);
		InitItemData_new("item_landmine", num);
	}

	public void SetBaseX(float posx)
	{
		baseX = posx + offsetX;
		if (posx == 0f)
		{
			goldBaseX = posx + offsetX;
		}
		playerFlyDis = 0f;
	}

	private void InitItemData_new(string prefabName, int count)
	{
		baseObj = Resources.Load("Prefab/" + prefabName) as GameObject;
		if (!baseObj)
		{
			return;
		}
		ArrayList arrayList = null;
		if (!hashAttribute.Contains(prefabName))
		{
			arrayList = new ArrayList();
			hashAttribute.Add(prefabName, arrayList);
		}
		else
		{
			arrayList = hashAttribute[prefabName] as ArrayList;
		}
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(baseObj) as GameObject;
			ItemScript itemScript = gameObject.AddComponent(typeof(ItemScript)) as ItemScript;
			gameObject.name = baseObj.name;
			ItemType itemStateByName = GetItemStateByName(gameObject.name);
			itemScript.state = itemStateByName;
			gameObject.layer = LayerMask.NameToLayer("items");
			gameObject.transform.parent = items.transform;
			gameObject.transform.position = new Vector3(-10000f, 0f, 0f);
			gameObject.active = false;
			foreach (Transform item in gameObject.transform)
			{
				item.gameObject.active = false;
			}
			arrayList.Add(gameObject);
		}
	}

	private void AddOneGold(string prefabName, Vector3 pos)
	{
		GameObject item_new = GetItem_new(prefabName);
		item_new.GetComponent<Collider>().enabled = true;
		item_new.name = prefabName;
		item_new.active = true;
		item_new.transform.position = pos;
		SetItemAttr(item_new);
	}

	private void AddOneGold(string prefabName)
	{
		Vector3 position = player.transform.position;
		position.z = 0f;
		int num = UnityEngine.Random.Range(0, 3);
		float num2 = 15f;
		Vector3 velocity = player.GetComponent<Rigidbody>().velocity;
		float num3 = Vector3.Angle(Vector3.up, velocity);
		num3 += UnityEngine.Random.Range(-10f, 10f);
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < num; i++)
		{
			switch (i)
			{
			case 1:
				num3 += 30f;
				break;
			case 2:
				num3 -= 30f;
				break;
			}
			zero.x = Mathf.Sin(num3 * ((float)Math.PI / 180f)) * num2;
			zero.y = Mathf.Cos(num3 * ((float)Math.PI / 180f)) * num2;
			Vector3 position2 = position + zero;
			if (!(position2.y <= 10f + groundY))
			{
				GameObject item_new = GetItem_new(prefabName);
				item_new.GetComponent<Collider>().enabled = true;
				item_new.name = prefabName;
				item_new.active = true;
				item_new.transform.position = position2;
				SetItemAttr(item_new);
			}
		}
	}

	private void AddOneGold_group()
	{
		Vector3 position = player.transform.position;
		position.z = 0f;
		int num = UnityEngine.Random.Range(0, 3);
		float num2 = 15f;
		Vector3 velocity = player.GetComponent<Rigidbody>().velocity;
		float num3 = Vector3.Angle(Vector3.up, velocity);
		num3 += UnityEngine.Random.Range(-10f, 10f);
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < num; i++)
		{
			switch (i)
			{
			case 1:
				num3 += 30f;
				break;
			case 2:
				num3 -= 30f;
				break;
			}
			zero.x = Mathf.Sin(num3 * ((float)Math.PI / 180f)) * num2;
			zero.y = Mathf.Cos(num3 * ((float)Math.PI / 180f)) * num2;
			Vector3 offset = position + zero;
			if (!(offset.y <= 10f + groundY))
			{
				int index = UnityEngine.Random.Range(0, goldArray.Count - 1);
				ArrayList groundArray = goldArray[index] as ArrayList;
				GameObject gameObject = new GameObject();
				ShowGoldGround showGoldGround = gameObject.AddComponent(typeof(ShowGoldGround)) as ShowGoldGround;
				showGoldGround.StartShow(groundArray, offset);
			}
		}
	}

	private float GetRandTime(string prefabName)
	{
		float result = 0f;
		switch (prefabName)
		{
		case "item_airliner":
			result = UnityEngine.Random.Range(4f, 6f);
			break;
		case "item_ufo":
			result = UnityEngine.Random.Range(15f, 20f);
			break;
		case "item_bomb2":
			result = UnityEngine.Random.Range(2f, 4f);
			break;
		case "item_fireballoon":
			result = UnityEngine.Random.Range(3f, 5f);
			break;
		case "item_gold3":
			result = UnityEngine.Random.Range(2f, 4f);
			break;
		}
		return result;
	}

	public void AddItem(string prefabName)
	{
		float num = 0f;
		if (hashItemTime.Contains(prefabName))
		{
			num = (float)hashItemTime[prefabName];
			if (!(m_time >= num))
			{
				return;
			}
			hashItemTime[prefabName] = m_time + GetRandTime(prefabName);
		}
		else
		{
			hashItemTime.Add(prefabName, m_time + GetRandTime(prefabName));
		}
		Vector3 position = Camera.main.transform.position;
		position.z = 0f;
		position.y += UnityEngine.Random.Range(-5, 5);
		float num2 = 15f;
		Vector3 velocity = player.GetComponent<Rigidbody>().velocity;
		float num3 = Vector3.Angle(Vector3.up, velocity);
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.Sin(num3 * ((float)Math.PI / 180f)) * num2;
		zero.y = Mathf.Cos(num3 * ((float)Math.PI / 180f)) * num2;
		Vector3 zero2 = Vector3.zero;
		ItemType itemStateByName = GetItemStateByName(prefabName);
		switch (itemStateByName)
		{
		case ItemType.AIRLINER:
			position.x -= 30f;
			zero2 = position + zero;
			audios.PlayAudio("FXairplane", player.transform);
			break;
		case ItemType.UFO:
			position.x -= 30f;
			zero2 = position + zero;
			break;
		case ItemType.DAODAN:
			position.x -= 30f;
			zero2 = position + zero;
			if (zero2.y < 35f + groundY)
			{
				return;
			}
			audios.PlayAudio("FXbomb_fly", player.transform);
			break;
		case ItemType.FIREBALLOON:
			position.x += 20f;
			zero2 = position + zero;
			break;
		case ItemType.ZOMBIES1:
			position.y = 0f + groundY;
			position.x += 20f;
			audios.PlayAudio("Ani_zombie_run");
			zero2 = position;
			break;
		case ItemType.ZHADAN:
			position.y = 0.4753061f + groundY;
			position.x += 20f;
			zero2 = position;
			break;
		case ItemType.LANDMINE:
			position.y = 0.3106229f + groundY;
			position.x += 20f;
			zero2 = position;
			break;
		case ItemType.BED:
			position.y = 1.177445f + groundY;
			position.x += 20f;
			zero2 = position;
			break;
		case ItemType.WAGON:
			position.y = 0.73f + groundY;
			position.x += 20f;
			zero2 = position;
			break;
		case ItemType.TRAMPOLINE:
			position.y = 0.50165f + groundY;
			position.x += 20f;
			zero2 = position;
			break;
		case ItemType.CLOACA:
			position.y = 0f + groundY;
			position.x += 23.5f;
			zero2 = position;
			break;
		case ItemType.STAR:
			zero2 = position + zero;
			break;
		case ItemType.GUANGHUAN:
			position.x = 13.92987f;
			position.y = 23.16217f;
			zero2 = position;
			break;
		default:
			zero2 = position + zero;
			break;
		}
		GameObject item_new = GetItem_new(prefabName);
		if (item_new == null)
		{
			return;
		}
		item_new.name = prefabName;
		item_new.active = true;
		if (prefabName != "Zombie_Batcher")
		{
			item_new.GetComponent<Collider>().enabled = true;
		}
		else
		{
			ZombieBossScript zombieBossScript = item_new.GetComponent(typeof(ZombieBossScript)) as ZombieBossScript;
			float num4 = (zero2.x - player.transform.position.x) / velocity.x;
			zombieBossScript.AttackOntime(num4 - 42f * Time.deltaTime);
		}
		foreach (Transform item in item_new.transform)
		{
			item.gameObject.active = true;
		}
		item_new.transform.position = zero2;
		switch (itemStateByName)
		{
		case ItemType.ZOMBIES1:
		{
			int num5 = UnityEngine.Random.Range(-1, 1);
			if (num5 == 0)
			{
				num5 = 1;
			}
			item_new.GetComponent<Rigidbody>().velocity = new Vector3(5 * num5, 0f, 0f);
			Vector3 eulerAngles = item_new.transform.eulerAngles;
			eulerAngles.y = 180 + 90 * num5 * -1;
			item_new.transform.eulerAngles = eulerAngles;
			break;
		}
		case ItemType.BED:
		{
			float y2 = UnityEngine.Random.Range(150, 220);
			item_new.transform.eulerAngles = new Vector3(270f, y2, 0f);
			break;
		}
		case ItemType.WAGON:
		{
			float y = UnityEngine.Random.Range(-50, 40);
			item_new.transform.eulerAngles = new Vector3(270f, y, 0f);
			break;
		}
		case ItemType.STAR:
			foreach (Transform item2 in item_new.transform)
			{
				item2.gameObject.active = true;
			}
			break;
		}
		SetItemAttr(item_new);
	}

	public ItemAttribute GetAttributeByName(string prefabName)
	{
		for (int i = 0; i < attributeArray.Count; i++)
		{
			if ((attributeArray[i] as ItemAttribute).type == prefabName)
			{
				return attributeArray[i] as ItemAttribute;
			}
		}
		return null;
	}

	public int GetItemLevelByName(string prefabName)
	{
		return 0;
	}

	private ItemType GetItemStateByName(string prefabName)
	{
		ItemType result = ItemType.NONE;
		switch (prefabName)
		{
		case "item_recruit1":
			result = ItemType.GUANGHUAN;
			break;
		case "item_bomb1":
			result = ItemType.ZHADAN;
			break;
		case "item_bomb2":
			result = ItemType.DAODAN;
			break;
		case "item_zombies1":
		case "Zombie_Nurse":
		case "Zombie_Swat":
			result = ItemType.ZOMBIES1;
			break;
		case "item_fallingstar1":
			result = ItemType.STAR;
			break;
		case "item_gold":
			result = ItemType.GOLD;
			break;
		case "item_gold2":
			result = ItemType.GOLD2;
			break;
		case "item_gold3":
			result = ItemType.GOLD3;
			break;
		case "item_cloud":
			result = ItemType.CLOUD;
			break;
		case "item_bed":
			result = ItemType.BED;
			break;
		case "item_wagon":
			result = ItemType.WAGON;
			break;
		case "item_trampoline":
			result = ItemType.TRAMPOLINE;
			break;
		case "Zombie_Batcher":
			result = ItemType.CLOACA;
			break;
		case "item_fireballoon":
			result = ItemType.FIREBALLOON;
			break;
		case "item_bird1":
			result = ItemType.BIRD1;
			break;
		case "item_airliner":
			result = ItemType.AIRLINER;
			break;
		case "item_ufo":
			result = ItemType.UFO;
			break;
		case "item_landmine":
			result = ItemType.LANDMINE;
			break;
		}
		return result;
	}

	public void SlowGame()
	{
	}

	public void SetItemAttr(GameObject items)
	{
		ItemScript itemScript = items.GetComponent(typeof(ItemScript)) as ItemScript;
		itemScript.Init();
	}

	private void ReadTaskInfo()
	{
		XmlDocument xmlDocument = new XmlDocument();
		TextAsset textAsset = Resources.Load("taskInfo") as TextAsset;
		xmlDocument.LoadXml(textAsset.text);
		XmlNode xmlNode = xmlDocument.SelectSingleNode("root");
		XmlNodeList childNodes = xmlNode.ChildNodes;
		hashTask.Clear();
		for (int i = 0; i < childNodes.Count; i++)
		{
			taskInfo taskInfo2 = new taskInfo();
			XmlElement xmlElement = (XmlElement)childNodes.Item(i);
			taskInfo2.id = xmlElement.GetAttribute("id");
			taskInfo2.info = xmlElement.GetAttribute("info");
			taskInfo2.info = taskInfo2.info.Replace("\\n", "\n");
			taskInfo2.value = float.Parse(xmlElement.GetAttribute("value"));
			taskInfo2.golds = int.Parse(xmlElement.GetAttribute("golds"));
			hashTask.Add(taskInfo2.id, taskInfo2);
		}
	}

	private void ReadTaskList()
	{
		XmlDocument xmlDocument = new XmlDocument();
		TextAsset textAsset = Resources.Load("taskList") as TextAsset;
		xmlDocument.LoadXml(textAsset.text);
		XmlNode xmlNode = xmlDocument.SelectSingleNode("root");
		XmlNodeList childNodes = xmlNode.ChildNodes;
		taskListArray.Clear();
		for (int i = 0; i < childNodes.Count; i++)
		{
			XmlNode xmlNode2 = childNodes.Item(i);
			XmlNodeList childNodes2 = xmlNode2.ChildNodes;
			ArrayList arrayList = new ArrayList();
			for (int j = 0; j < childNodes2.Count; j++)
			{
				arrayList.Add(childNodes2.Item(j).InnerText);
			}
			taskListArray.Add(arrayList);
		}
	}

	private void ReadAvatarList()
	{
		XmlDocument xmlDocument = new XmlDocument();
		TextAsset textAsset = Resources.Load("avatarList") as TextAsset;
		xmlDocument.LoadXml(textAsset.text);
		XmlNode xmlNode = xmlDocument.SelectSingleNode("root");
		XmlNodeList childNodes = xmlNode.ChildNodes;
		avatarArray.Clear();
		globalVal.g_avatar_isbuy.Clear();
		for (int i = 0; i < childNodes.Count; i++)
		{
			XmlElement xmlElement = childNodes.Item(i) as XmlElement;
			AvatarAttribute avatarAttribute = new AvatarAttribute();
			avatarAttribute.name = xmlElement.GetAttribute("name");
			avatarAttribute.picname = xmlElement.GetAttribute("picname");
			avatarAttribute.info = xmlElement.GetAttribute("info");
			avatarAttribute.info = avatarAttribute.info.Replace("\\n", "\n");
			avatarAttribute.price = int.Parse(xmlElement.GetAttribute("price"));
			avatarAttribute.modelname = xmlElement.GetAttribute("modelname");
			if (i == 0)
			{
				avatarAttribute.isbuy = 1;
			}
			else
			{
				avatarAttribute.isbuy = 0;
			}
			globalVal.g_avatar_isbuy.Add(avatarAttribute.isbuy);
			avatarArray.Add(avatarAttribute);
		}
	}

	private void ReadItemOnceList()
	{
		XmlDocument xmlDocument = new XmlDocument();
		TextAsset textAsset = Resources.Load("itemAttr_Once") as TextAsset;
		xmlDocument.LoadXml(textAsset.text);
		XmlNode xmlNode = xmlDocument.SelectSingleNode("root");
		XmlNodeList childNodes = xmlNode.ChildNodes;
		itemOnceArray.Clear();
		globalVal.g_item_once_count.Clear();
		StatisticsData.data.d_itemBuyCount.Clear();
		StatisticsData.data.d_itemUsedCount.Clear();
		for (int i = 0; i < childNodes.Count; i++)
		{
			XmlElement xmlElement = childNodes.Item(i) as XmlElement;
			ItemOnceAttribute itemOnceAttribute = new ItemOnceAttribute();
			itemOnceAttribute.name = xmlElement.GetAttribute("name");
			itemOnceAttribute.picname = xmlElement.GetAttribute("picname");
			itemOnceAttribute.info = xmlElement.GetAttribute("info");
			itemOnceAttribute.info = itemOnceAttribute.info.Replace("\\n", "\n");
			itemOnceAttribute.price = int.Parse(xmlElement.GetAttribute("price"));
			itemOnceAttribute.value = float.Parse(xmlElement.GetAttribute("value"));
			itemOnceAttribute.smallpic = xmlElement.GetAttribute("smallpic");
			globalVal.g_item_once_count.Add(0);
			StatisticsData.data.d_itemBuyCount.Add(0);
			StatisticsData.data.d_itemUsedCount.Add(0);
			itemOnceArray.Add(itemOnceAttribute);
		}
	}

	private void ReadItemAttr_itemAttr()
	{
		XmlDocument xmlDocument = new XmlDocument();
		attributeArray.Clear();
		TextAsset textAsset = Resources.Load("itemAttr") as TextAsset;
		xmlDocument.LoadXml(textAsset.text);
		XmlNode xmlNode = xmlDocument.SelectSingleNode("root");
		XmlNodeList childNodes = xmlNode.ChildNodes;
		globalVal.g_itemlevel.Clear();
		for (int i = 0; i < childNodes.Count; i++)
		{
			ItemAttribute itemAttribute = new ItemAttribute();
			XmlElement xmlElement = (XmlElement)childNodes.Item(i);
			itemAttribute.type = xmlElement.GetAttribute("type");
			if (xmlElement.GetAttribute("inshop") == "1")
			{
				itemAttribute.inshop = true;
			}
			itemAttribute.index = i;
			itemAttribute.colliderCount = 0;
			if (itemAttribute.type == "equip_rocket" || itemAttribute.type == "equip_glider")
			{
				globalVal.g_itemlevel.Add(1);
			}
			else
			{
				globalVal.g_itemlevel.Add(0);
			}
			itemAttribute.level.Clear();
			XmlNodeList childNodes2 = xmlElement.ChildNodes;
			for (int j = 0; j < childNodes2.Count; j++)
			{
				ItemSubAttr itemSubAttr = new ItemSubAttr();
				XmlElement xmlElement2 = (XmlElement)childNodes2.Item(j);
				if (xmlElement2.GetAttribute("level") != string.Empty)
				{
					itemSubAttr.level = int.Parse(xmlElement2.GetAttribute("level"));
				}
				itemSubAttr.name = xmlElement2.GetAttribute("name");
				itemSubAttr.picname = xmlElement2.GetAttribute("picname");
				if (xmlElement2.GetAttribute("price") != string.Empty)
				{
					itemSubAttr.price = int.Parse(xmlElement2.GetAttribute("price"));
				}
				if (xmlElement2.GetAttribute("dir") != string.Empty)
				{
					itemSubAttr.dir = float.Parse(xmlElement2.GetAttribute("dir"));
				}
				if (xmlElement2.GetAttribute("strength") != string.Empty)
				{
					itemSubAttr.strength = float.Parse(xmlElement2.GetAttribute("strength"));
				}
				if (xmlElement2.GetAttribute("length") != string.Empty)
				{
					itemSubAttr.length = float.Parse(xmlElement2.GetAttribute("length"));
				}
				if (xmlElement2.GetAttribute("value") != string.Empty)
				{
					itemSubAttr.value = float.Parse(xmlElement2.GetAttribute("value"));
				}
				itemSubAttr.info = xmlElement2.GetAttribute("info");
				itemSubAttr.info = itemSubAttr.info.Replace("\\n", "\n");
				itemAttribute.level.Add(itemSubAttr);
			}
			attributeArray.Add(itemAttribute);
		}
		globalVal.ReadFile("saveData.txt");
		isloading = false;
	}

	private void ReadData()
	{
		XmlDocument xmlDocument = new XmlDocument();
		dataArray.Clear();
		TextAsset textAsset = Resources.Load("data") as TextAsset;
		xmlDocument.LoadXml(textAsset.text);
		XmlNode xmlNode = xmlDocument.SelectSingleNode("root");
		XmlNodeList childNodes = xmlNode.ChildNodes;
		for (int i = 0; i < childNodes.Count; i++)
		{
			ArrayList arrayList = new ArrayList();
			XmlNode xmlNode2 = childNodes.Item(i);
			XmlNodeList childNodes2 = xmlNode2.ChildNodes;
			for (int j = 0; j < childNodes2.Count; j++)
			{
				XmlElement xmlElement = (XmlElement)childNodes2.Item(j);
				string attribute = xmlElement.GetAttribute("type");
				Vector3 zero = Vector3.zero;
				zero.x = float.Parse(xmlElement.GetAttribute("x")) * 1f;
				zero.y = float.Parse(xmlElement.GetAttribute("y")) * 1f;
				zero.z = float.Parse(xmlElement.GetAttribute("z")) * 1f;
				PointInfo pointInfo = new PointInfo();
				pointInfo.name = attribute;
				pointInfo.pos = zero;
				arrayList.Add(pointInfo);
			}
			dataArray.Add(arrayList);
		}
		MonoBehaviour.print(dataArray.Count);
		goldArray.Clear();
		textAsset = Resources.Load("golddata") as TextAsset;
		xmlDocument.LoadXml(textAsset.text);
		xmlNode = xmlDocument.SelectSingleNode("root");
		childNodes = xmlNode.ChildNodes;
		for (int k = 0; k < childNodes.Count; k++)
		{
			ArrayList arrayList2 = new ArrayList();
			XmlNode xmlNode3 = childNodes.Item(k);
			XmlNodeList childNodes3 = xmlNode3.ChildNodes;
			for (int l = 0; l < childNodes3.Count; l++)
			{
				XmlElement xmlElement2 = (XmlElement)childNodes3.Item(l);
				string attribute2 = xmlElement2.GetAttribute("type");
				Vector3 zero2 = Vector3.zero;
				zero2.x = float.Parse(xmlElement2.GetAttribute("x")) * 1f;
				zero2.y = float.Parse(xmlElement2.GetAttribute("y")) * 1f;
				zero2.z = float.Parse(xmlElement2.GetAttribute("z")) * 1f;
				PointInfo pointInfo2 = new PointInfo();
				pointInfo2.name = attribute2;
				pointInfo2.pos = zero2;
				arrayList2.Add(pointInfo2);
			}
			goldArray.Add(arrayList2);
		}
		MonoBehaviour.print(goldArray.Count);
		goldGroundArray.Clear();
		textAsset = Resources.Load("goldground") as TextAsset;
		xmlDocument.LoadXml(textAsset.text);
		xmlNode = xmlDocument.SelectSingleNode("root");
		childNodes = xmlNode.ChildNodes;
		for (int m = 0; m < childNodes.Count; m++)
		{
			ArrayList arrayList3 = new ArrayList();
			XmlNode xmlNode4 = childNodes.Item(m);
			XmlNodeList childNodes4 = xmlNode4.ChildNodes;
			for (int n = 0; n < childNodes4.Count; n++)
			{
				XmlElement xmlElement3 = (XmlElement)childNodes4.Item(n);
				string attribute3 = xmlElement3.GetAttribute("type");
				Vector3 zero3 = Vector3.zero;
				zero3.x = float.Parse(xmlElement3.GetAttribute("x")) * 1f;
				zero3.y = float.Parse(xmlElement3.GetAttribute("y")) * 1f;
				zero3.z = float.Parse(xmlElement3.GetAttribute("z")) * 1f;
				PointInfo pointInfo3 = new PointInfo();
				pointInfo3.name = attribute3;
				pointInfo3.pos = zero3;
				arrayList3.Add(pointInfo3);
			}
			goldGroundArray.Add(arrayList3);
		}
		MonoBehaviour.print(goldGroundArray.Count);
	}

	public int GetTypeMinPrice(string typeName)
	{
		int num = 9999999;
		switch (typeName)
		{
		case "thestash_avatar":
		{
			for (int j = 0; j < avatarArray.Count; j++)
			{
				if ((int)globalVal.g_avatar_isbuy[j] == 0)
				{
					AvatarAttribute avatarAttribute = avatarArray[j] as AvatarAttribute;
					if (avatarAttribute.price < num)
					{
						num = avatarAttribute.price;
					}
				}
			}
			break;
		}
		case "thestash_upgrades":
		{
			for (int k = 0; k < attributeArray.Count; k++)
			{
				ItemAttribute itemAttribute = attributeArray[k] as ItemAttribute;
				if (!itemAttribute.inshop)
				{
					continue;
				}
				int num2 = (int)globalVal.g_itemlevel[itemAttribute.index] + 1;
				if (num2 < itemAttribute.level.Count)
				{
					ItemSubAttr itemSubAttr = itemAttribute.level[num2] as ItemSubAttr;
					if (itemSubAttr.price < num)
					{
						num = itemSubAttr.price;
					}
				}
			}
			break;
		}
		case "thestash_items":
		{
			for (int i = 0; i < itemOnceArray.Count; i++)
			{
				ItemOnceAttribute itemOnceAttribute = itemOnceArray[i] as ItemOnceAttribute;
				if (itemOnceAttribute.price < num)
				{
					num = itemOnceAttribute.price;
				}
			}
			break;
		}
		}
		return num;
	}

	public int GetMinPrice()
	{
		int num = 0;
		int num2 = 9999999;
		num = GetTypeMinPrice("thestash_avatar");
		if (num < num2)
		{
			num2 = num;
		}
		num = GetTypeMinPrice("thestash_upgrades");
		if (num < num2)
		{
			num2 = num;
		}
		num = GetTypeMinPrice("thestash_items");
		if (num < num2)
		{
			num2 = num;
		}
		return num2;
	}

	private void ShowData_new(int index)
	{
		ArrayList groundArray = dataArray[index] as ArrayList;
		Vector3 velocity = players.GetVelocity();
		float num = Vector3.Distance(Vector3.zero, velocity);
		float num2 = Vector3.Angle(Vector3.up, velocity);
		Vector3 zero = Vector3.zero;
		num = 20f;
		zero.x = Mathf.Sin(num2 * ((float)Math.PI / 180f)) * num;
		zero.y = Mathf.Cos(num2 * ((float)Math.PI / 180f)) * num;
		if (RefreshTime == 0)
		{
			lastCameraPoint = Camera.main.transform.position + zero;
		}
		Vector3 offset = lastCameraPoint + new Vector3(offsetX * (float)RefreshTime, 0f, 0f);
		GameObject gameObject = new GameObject();
		ShowGoldGround showGoldGround = gameObject.AddComponent(typeof(ShowGoldGround)) as ShowGoldGround;
		showGoldGround.StartShow(groundArray, offset);
	}

	private void ShowData_gold(int index)
	{
		ArrayList groundArray = goldArray[index] as ArrayList;
		Vector3 position = player.transform.position;
		position.z = 0f;
		float num = 10f;
		Vector3 velocity = player.GetComponent<Rigidbody>().velocity;
		float num2 = Vector3.Angle(Vector3.up, velocity);
		num2 += UnityEngine.Random.Range(-10f, 10f);
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.Sin(num2 * ((float)Math.PI / 180f)) * num;
		zero.y = Mathf.Cos(num2 * ((float)Math.PI / 180f)) * num;
		Vector3 offset = position + zero;
		GameObject gameObject = new GameObject();
		ShowGoldGround showGoldGround = gameObject.AddComponent(typeof(ShowGoldGround)) as ShowGoldGround;
		showGoldGround.StartShow(groundArray, offset);
	}

	private void ShowData_gold(int index, Vector3 pos)
	{
		ArrayList groundArray = goldArray[index] as ArrayList;
		GameObject gameObject = new GameObject();
		ShowGoldGround showGoldGround = gameObject.AddComponent(typeof(ShowGoldGround)) as ShowGoldGround;
		showGoldGround.StartShow(groundArray, pos);
	}

	private void ShowData_groundgold(int index)
	{
		if (!(ray_hit.transform.parent.name == "scene_cityturnhighway"))
		{
			ArrayList groundArray = goldGroundArray[index] as ArrayList;
			Vector3 position = player.transform.position;
			position.y = 0f + groundY;
			position.x += 20f;
			position.z = 0f;
			GameObject gameObject = new GameObject();
			ShowGoldGround showGoldGround = gameObject.AddComponent(typeof(ShowGoldGround)) as ShowGoldGround;
			showGoldGround.StartShow(groundArray, position);
		}
	}

	private void ShowData(int index)
	{
		ArrayList arrayList = dataArray[index] as ArrayList;
		Vector3 velocity = players.GetVelocity();
		float num = Vector3.Distance(Vector3.zero, velocity);
		float num2 = Vector3.Angle(Vector3.up, velocity);
		Vector3 zero = Vector3.zero;
		num = 20f;
		zero.x = Mathf.Sin(num2 * ((float)Math.PI / 180f)) * num;
		zero.y = Mathf.Cos(num2 * ((float)Math.PI / 180f)) * num;
		if (RefreshTime == 0)
		{
			lastCameraPoint = Camera.main.transform.position + zero;
		}
		for (int i = 0; i < arrayList.Count; i++)
		{
			PointInfo pointInfo = arrayList[i] as PointInfo;
			if (pointInfo != null)
			{
				GameObject item_new = GetItem_new(pointInfo.name);
				if (!(item_new == null))
				{
					item_new.name = pointInfo.name;
					item_new.active = true;
					Vector3 position = lastCameraPoint + new Vector3(offsetX * (float)RefreshTime, 0f, 0f) + pointInfo.pos;
					position.y -= 100f;
					position.z = 0f;
					item_new.transform.position = position;
				}
			}
		}
	}

	public ItemAttribute GetItemAttribute(int index)
	{
		ItemAttribute itemAttribute = null;
		int num = 0;
		for (int i = 0; i < attributeArray.Count; i++)
		{
			itemAttribute = attributeArray[i] as ItemAttribute;
			if (itemAttribute.inshop)
			{
				if (num == index)
				{
					return itemAttribute;
				}
				num++;
			}
		}
		return itemAttribute;
	}

	private void FixedUpdate()
	{
		if (!players)
		{
			return;
		}
		int layerMask = 1 << LayerMask.NameToLayer("floor");
		if (Physics.Raycast(players.transform.position, Vector3.down, out ray_hit, 100f, layerMask) && ray_hit.transform != null)
		{
			groundY = ray_hit.point.y;
		}
		m_time += Time.fixedDeltaTime;
		if (baseX < Camera.main.transform.position.x)
		{
			if (players.slingshot.playerAnim == AnimList.ANIM2)
			{
				baseX = Camera.main.transform.position.x + offsetX;
				ShowData_new(UnityEngine.Random.Range(0, dataArray.Count - 1));
				RefreshTime++;
				if (RefreshTime > 2)
				{
					RefreshTime = 0;
				}
			}
			else
			{
				float x = Camera.main.transform.position.x;
				if (player.transform.position.y > 10f + groundY && baseX > 20f)
				{
					baseX = x + offsetX * 0.25f + x * 0.001f * 3f;
					AddItemRandom_new();
				}
				else
				{
					if (player.transform.position.x > 500f)
					{
						if (itemRefreshCount == 0)
						{
							AddZombieRandom();
							if (player.transform.position.x <= 1000f)
							{
								itemRefreshCount = UnityEngine.Random.Range(1, 5);
							}
							else
							{
								itemRefreshCount = UnityEngine.Random.Range(1, 3);
							}
						}
						else
						{
							AddItemRandom_new();
							itemRefreshCount--;
						}
					}
					else if (baseX > 20f)
					{
						AddItemRandom_new();
					}
					float num = offsetX * 0.5f + x * 0.001f * 3f;
					if (num > 25f)
					{
						num = 25f;
					}
					baseX = x + num;
				}
			}
		}
		if (groundGoldBaseX < Camera.main.transform.position.x && player.transform.position.y < 10f)
		{
			float x2 = Camera.main.transform.position.x;
			if (player.transform.position.x > 500f)
			{
				groundGoldBaseX = x2 + UnityEngine.Random.Range(30f, 60f);
				ShowData_groundgold(UnityEngine.Random.Range(0, goldArray.Count - 1));
			}
		}
		if (goldBaseX < PlayerScriptClass.playerInfo.playerFlyDis && players.slingshot.playerAnim != AnimList.ANIM2 && player.transform.position.y > 10f + groundY)
		{
			float num2 = PlayerScriptClass.playerInfo.playerFlyDis;
			goldBaseX = num2 + UnityEngine.Random.Range(50f, 80f);
			Vector3 position = player.transform.position;
			Vector3 velocity = player.GetComponent<Rigidbody>().velocity;
			int num3 = 3;
			if (player.transform.position.x > 500f && player.transform.position.x <= 1000f)
			{
				num3++;
			}
			else if (player.transform.position.x > 1000f)
			{
				num3 += 2;
			}
			int num4 = UnityEngine.Random.Range(3, num3 + 1);
			float num5 = 10f;
			for (int i = 0; i < num4; i++)
			{
				num5 += 4f;
				float num6 = num5 / velocity.x;
				float num7 = velocity.y * num6 + -4.9f * num6 * num6;
				Vector3 pos = new Vector3(position.x + num5, position.y + num7, position.z);
				if (pos.y > 10f + groundY)
				{
					if (UnityEngine.Random.Range(0, 10000) % 2 == 0)
					{
						pos.y += UnityEngine.Random.Range(-2f, 2f);
					}
					if (UnityEngine.Random.Range(0, 2) == 1 && player.transform.position.x > 1000f)
					{
						AddOneGold("item_gold2", pos);
					}
					else
					{
						ShowData_gold(UnityEngine.Random.Range(0, goldArray.Count - 1), pos);
					}
				}
			}
		}
		if (slowKey)
		{
			if (startTime > endTime)
			{
				Time.timeScale = 1f;
				slowKey = false;
			}
			startTime += Time.deltaTime;
		}
	}

	public void SetPlayerSleep(bool sleep)
	{
		Component[] componentsInChildren = player.transform.GetComponentsInChildren(typeof(Transform));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Transform transform = (Transform)componentsInChildren[i];
			if ((bool)transform.GetComponent<Rigidbody>())
			{
				if (sleep)
				{
					transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
				}
				transform.GetComponent<Rigidbody>().isKinematic = sleep;
			}
		}
	}

	public void SetPlayerAddForce(Vector3 force)
	{
		Component[] componentsInChildren = player.transform.GetComponentsInChildren(typeof(Transform));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Transform transform = (Transform)componentsInChildren[i];
			if ((bool)transform.GetComponent<Rigidbody>())
			{
				transform.GetComponent<Rigidbody>().isKinematic = false;
				transform.GetComponent<Rigidbody>().velocity = force * UnityEngine.Random.Range(0.8f, 1.2f);
			}
		}
	}
}
