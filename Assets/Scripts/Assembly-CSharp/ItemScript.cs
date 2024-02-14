using System;
using System.Collections;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
	public ItemType state;

	private bool moveup;

	private Vector3 basePoint;

	private GameObject player;

	private PlayerScript players;

	private TAudioController audios;

	private GameObject UIs;

	private TUIMeshText label_gold;

	private bool isDestory;

	private Transform audioTrans;

	private Transform followTrans;

	private AudioSource audioSource;

	private bool follow;

	private ArrayList audioArray = new ArrayList();

	private bool colliderkey;

	private void Start()
	{
		basePoint = base.transform.position;
		GameObject gameObject = GameObject.Find("TAudioController");
		if (gameObject == null)
		{
			gameObject = UnityEngine.Object.Instantiate(Resources.Load("TAudioController")) as GameObject;
			gameObject.name = "TAudioController";
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		UIs = GameObject.Find("TUI/TUIControl");
		audios = gameObject.GetComponent(typeof(TAudioController)) as TAudioController;
		label_gold = UIs.transform.Find("title/label_glod").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
		player = GameObject.Find("player2/Bip01");
		players = player.GetComponent(typeof(PlayerScript)) as PlayerScript;
		audioArray.Clear();
	}

	public void Init()
	{
		colliderkey = false;
		basePoint = base.transform.position;
		switch (state)
		{
		case ItemType.DAODAN:
		{
			player = GameObject.Find("player2/Bip01");
			float num = player.GetComponent<Rigidbody>().velocity.x;
			if (num < 30f)
			{
				num = 30f;
			}
			base.GetComponent<Rigidbody>().velocity = new Vector3((num + UnityEngine.Random.Range(-3f, 3f)) * 1.8f, 0f, 0f);
			break;
		}
		case ItemType.FIREBALLOON:
		{
			player = GameObject.Find("player2/Bip01");
			float num2 = player.GetComponent<Rigidbody>().velocity.x;
			if (num2 < 20f)
			{
				num2 = 20f;
			}
			base.GetComponent<Rigidbody>().velocity = new Vector3((num2 + UnityEngine.Random.Range(-3f, 3f)) * 0.5f, 0f, 0f);
			break;
		}
		case ItemType.STAR:
		{
			Vector3 velocity = new Vector3(-3f, -6f, 0f);
			base.GetComponent<Rigidbody>().velocity = velocity;
			break;
		}
		case ItemType.GOLD:
		case ItemType.GOLD2:
		case ItemType.GOLD3:
			if (base.transform.GetComponent(typeof(GoldRotateScript)) == null)
			{
				base.gameObject.AddComponent(typeof(GoldRotateScript));
			}
			break;
		case ItemType.AIRLINER:
			player = GameObject.Find("player2/Bip01");
			base.GetComponent<Rigidbody>().velocity = new Vector3(player.GetComponent<Rigidbody>().velocity.x * 1.5f, 0f, 0f);
			break;
		case ItemType.UFO:
			base.GetComponent<Rigidbody>().velocity = new Vector3(75f, -30f, 0f);
			break;
		}
	}

	private void FixedUpdate()
	{
		ItemType itemType = state;
		if (itemType == ItemType.GUANGHUAN)
		{
			if (base.transform.position.y < basePoint.y - 2f)
			{
				moveup = true;
			}
			if (base.transform.position.y > basePoint.y + 2f)
			{
				moveup = false;
			}
			Vector3 zero = Vector3.zero;
			if (moveup)
			{
				zero = base.transform.position;
				zero.y += 0.05f;
				base.transform.position = zero;
			}
			else
			{
				zero = base.transform.position;
				zero.y -= 0.05f;
				base.transform.position = zero;
			}
		}
	}

	private void Update()
	{
		Vector3 position = Camera.main.transform.position;
		ItemType itemType = state;
		if (itemType == ItemType.AIRLINER || itemType == ItemType.UFO || itemType == ItemType.DAODAN)
		{
			if (base.transform.position.x > position.x + Mathf.Abs(position.z))
			{
				RecoverGameObject_new();
			}
		}
		else if (base.transform.position.x < position.x - Mathf.Abs(position.z))
		{
			RecoverGameObject_new();
		}
		if (isDestory && (base.transform.position.x > position.x + Mathf.Abs(position.z) || base.transform.position.x < position.x - Mathf.Abs(position.z)))
		{
			RecoverGameObject_new();
			isDestory = false;
		}
		for (int i = 0; i < audioArray.Count; i++)
		{
			Transform transform = audioArray[i] as Transform;
			transform.localPosition = followTrans.position;
			if (!(transform.GetComponent(typeof(AudioSource)) as AudioSource).isPlaying)
			{
				audioArray.RemoveAt(i);
			}
		}
	}

	private void SetAudioFollow(string name)
	{
		audioTrans = audios.transform.Find("Audio/" + name);
		if (audioTrans != null)
		{
			audioArray.Add(audioTrans);
		}
	}

	private void RecoverGameObject_new()
	{
		ArrayList arrayList = ItemManagerClass.body.hashAttribute[base.name] as ArrayList;
		base.gameObject.active = false;
		foreach (Transform item in base.gameObject.transform)
		{
			item.gameObject.active = false;
		}
		arrayList.Add(base.gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (colliderkey)
		{
			return;
		}
		RecoverGameObject_new();
		colliderkey = true;
		ItemAttribute attributeByName = ItemManagerClass.body.GetAttributeByName(base.name);
		int index = (int)globalVal.g_itemlevel[attributeByName.index];
		if (!attributeByName.inshop)
		{
			index = 0;
		}
		Vector3 velocity = players.GetVelocity();
		float num = Vector3.Distance(Vector3.zero, velocity);
		float num2 = Vector3.Angle(Vector3.up, velocity);
		float num3 = 1f;
		float num4 = num;
		ItemSubAttr itemSubAttr = attributeByName.level[index] as ItemSubAttr;
		if (itemSubAttr.dir != 0f)
		{
			num2 = itemSubAttr.dir;
		}
		if (itemSubAttr.length != 0f)
		{
			num = ((!(itemSubAttr.length > num4)) ? num4 : itemSubAttr.length);
		}
		if (itemSubAttr.strength != 0f)
		{
			num3 = itemSubAttr.strength;
		}
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.Sin(num2 * ((float)Math.PI / 180f)) * num * num3;
		zero.y = Mathf.Cos(num2 * ((float)Math.PI / 180f)) * num * num3;
		attributeByName.colliderCount++;
		base.GetComponent<Collider>().enabled = false;
		switch (state)
		{
		case ItemType.DAODAN:
			EffectManagerClass.body.PlayEffect("effect_blowup", base.transform);
			audios.PlayAudio("FXbomb_explodes", base.transform);
			audios.PlayAudio("FXblowup_bomb", base.transform);
			playAudioFall();
			if (attributeByName.colliderCount == 0)
			{
				ItemManagerClass.body.SlowGame();
			}
			ItemManagerClass.body.SetPlayerSleep(true);
			ItemManagerClass.body.SetPlayerAddForce(zero);
			break;
		case ItemType.TRAMPOLINE:
			EffectManagerClass.body.PlayEffect("effect_trampoline", base.transform);
			if (num4 > 16f)
			{
				audios.PlayAudio("FXCrash_bang", player.transform);
				playAudioSpring();
			}
			else if (num4 > 10f && num4 <= 16f)
			{
				audios.PlayAudio("FXCrash_Hit", player.transform);
				playAudioSpring();
			}
			else
			{
				audios.PlayAudio("FXCrash_Flip", player.transform);
			}
			audios.PlayAudio("FXspringbed");
			audios.transform.Find("Audio/FXspringbed").localPosition = base.transform.position;
			if (attributeByName.colliderCount == 0)
			{
				ItemManagerClass.body.SlowGame();
			}
			MonoBehaviour.print("last angles : " + num2 + "   length : " + num + "  strength : " + num3);
			if (num2 > 70f)
			{
				num2 = 70f;
			}
			if (num < 20f)
			{
				num = 20f;
			}
			MonoBehaviour.print("angles : " + num2 + "   length : " + num + "  strength : " + num3);
			zero = Vector3.zero;
			zero.x = Mathf.Sin(num2 * ((float)Math.PI / 180f)) * num * num3;
			zero.y = Mathf.Cos(num2 * ((float)Math.PI / 180f)) * num * num3;
			ItemManagerClass.body.SetPlayerSleep(true);
			ItemManagerClass.body.SetPlayerAddForce(zero);
			break;
		case ItemType.ZHADAN:
			EffectManagerClass.body.PlayEffect("effect_blowup_bomb1", base.transform);
			audios.PlayAudio("FXbomb_explodes", base.transform);
			audios.PlayAudio("FXblowup_bomb", player.transform);
			playAudioFall();
			if (attributeByName.colliderCount == 0)
			{
				ItemManagerClass.body.SlowGame();
			}
			ItemManagerClass.body.SetPlayerSleep(true);
			ItemManagerClass.body.SetPlayerAddForce(zero);
			break;
		case ItemType.LANDMINE:
			EffectManagerClass.body.PlayEffect("effect_blowup_trampoline", base.transform);
			audios.PlayAudio("FXbomb_explodes", base.transform);
			audios.PlayAudio("FXblowup_bomb", player.transform);
			if (attributeByName.colliderCount == 0)
			{
				ItemManagerClass.body.SlowGame();
			}
			playAudioFall();
			ItemManagerClass.body.SetPlayerSleep(true);
			ItemManagerClass.body.SetPlayerAddForce(zero);
			break;
		case ItemType.ZOMBIES1:
		{
			EffectManagerClass.body.PlayEffect("effect_bleeding", base.transform);
			audios.PlayAudio("FXzombiesmash", base.transform);
			playAudioHurt();
			if (attributeByName.colliderCount == 0)
			{
				ItemManagerClass.body.SlowGame();
			}
			if (num4 > 4f)
			{
				EffectManagerClass.body.PlayEffect("effect_zombie_dead", base.transform);
				globalVal.g_best_zombiebreak++;
				ItemManagerClass.body.SetPlayerSleep(true);
				ItemManagerClass.body.SetPlayerAddForce(zero);
				break;
			}
			string text = string.Empty;
			switch (base.name)
			{
			case "item_zombies1":
				text = "eff_zombies1";
				break;
			case "Zombie_Nurse":
				text = "eff_Zombie_Nurse";
				break;
			case "Zombie_Swat":
				text = "eff_Zombie_Swat";
				break;
			}
			EffectManagerClass.body.PlayEffect(text, base.transform);
			audios.PlayAudio("Ani_zombie_attack");
			audios.transform.Find("Audio/Ani_zombie_attack").localPosition = base.transform.position;
			audios.PlayAudio("SVOzombiez");
			audios.transform.Find("Audio/SVOzombiez").localPosition = base.transform.position;
			isDestory = true;
			base.GetComponent<Collider>().enabled = false;
			ItemManagerClass.body.SetPlayerSleep(true);
			ItemManagerClass.body.SetPlayerAddForce(Vector3.zero);
			return;
		}
		case ItemType.BED:
			EffectManagerClass.body.PlayEffect("effect_bed", base.transform);
			if (num4 > 16f)
			{
				audios.PlayAudio("FXCrash_bang", player.transform);
				playAudioSpring();
			}
			else if (num4 > 10f && num4 <= 16f)
			{
				audios.PlayAudio("FXCrash_Hit", player.transform);
				playAudioSpring();
			}
			else
			{
				audios.PlayAudio("FXCrash_Flip", player.transform);
			}
			audios.PlayAudio("FXbed", base.transform);
			if (attributeByName.colliderCount == 0)
			{
				ItemManagerClass.body.SlowGame();
			}
			MonoBehaviour.print("last angles : " + num2 + "   length : " + num + "  strength : " + num3);
			if (num2 > 70f)
			{
				num2 = 70f;
			}
			if (num < 20f)
			{
				num = 20f;
			}
			MonoBehaviour.print("angles : " + num2 + "   length : " + num + "  strength : " + num3);
			zero = Vector3.zero;
			zero.x = Mathf.Sin(num2 * ((float)Math.PI / 180f)) * num * num3;
			zero.y = Mathf.Cos(num2 * ((float)Math.PI / 180f)) * num * num3;
			ItemManagerClass.body.SetPlayerSleep(true);
			ItemManagerClass.body.SetPlayerAddForce(zero);
			MonoBehaviour.print("bed : power " + zero);
			break;
		case ItemType.WAGON:
			EffectManagerClass.body.PlayEffect("effect_wagon", base.transform);
			audios.PlayAudio("FXpolicecar", base.transform);
			if (num4 > 16f)
			{
				audios.PlayAudio("FXCrash_bang", player.transform);
				playAudioHurt();
			}
			else if (num4 > 10f && num4 <= 16f)
			{
				audios.PlayAudio("FXCrash_Hit", player.transform);
				playAudioHurt();
			}
			else
			{
				audios.PlayAudio("FXCrash_Flip", player.transform);
			}
			if (attributeByName.colliderCount == 0)
			{
				ItemManagerClass.body.SlowGame();
			}
			if (num2 > 70f)
			{
				num2 = 70f;
			}
			if (num < 20f)
			{
				num = 20f;
			}
			MonoBehaviour.print("angles : " + num2 + "   length : " + num + "  strength : " + num3);
			zero = Vector3.zero;
			zero.x = Mathf.Sin(num2 * ((float)Math.PI / 180f)) * num * num3;
			zero.y = Mathf.Cos(num2 * ((float)Math.PI / 180f)) * num * num3;
			MonoBehaviour.print("WAGON : power " + zero);
			ItemManagerClass.body.SetPlayerSleep(true);
			ItemManagerClass.body.SetPlayerAddForce(zero);
			break;
		case ItemType.GOLD:
		case ItemType.GOLD3:
			EffectManagerClass.body.PlayGoldFlyEffect(base.transform, (int)itemSubAttr.value);
			EffectManagerClass.body.PlayEffect("effect_goldtouch", player.transform, true);
			audios.PlayAudio("FXobtain_coin", base.transform);
			break;
		case ItemType.GOLD2:
			EffectManagerClass.body.PlayRedGoldFlyEffect(base.transform, (int)itemSubAttr.value);
			EffectManagerClass.body.PlayEffect("effect_goldtouch", player.transform, true);
			audios.PlayAudio("FXobtain_coin", base.transform);
			break;
		case ItemType.FIREBALLOON:
			if (other.transform.position.y > base.transform.position.y - 3f)
			{
				EffectManagerClass.body.PlayEffect_fireballoon(base.transform, 1);
				audios.PlayAudio("FXblowup_balloon_up", base.transform);
			}
			else
			{
				EffectManagerClass.body.PlayEffect_fireballoon(base.transform, 2);
				audios.PlayAudio("FXblowup_balloon_bottom", base.transform);
			}
			playAudioHurt();
			if (attributeByName.colliderCount == 0)
			{
				ItemManagerClass.body.SlowGame();
			}
			ItemManagerClass.body.SetPlayerSleep(true);
			ItemManagerClass.body.SetPlayerAddForce(zero);
			break;
		case ItemType.AIRLINER:
			ItemManagerClass.body.SetPlayerSleep(true);
			ItemManagerClass.body.SetPlayerAddForce(zero);
			EffectManagerClass.body.PlayEffect("effect_airliner", base.transform);
			audios.PlayAudio("Fxblowaway_jet", player.transform);
			audios.PlayAudio("FXCrash_bang", player.transform);
			playAudioHurt();
			break;
		case ItemType.UFO:
			ItemManagerClass.body.SetPlayerSleep(true);
			ItemManagerClass.body.SetPlayerAddForce(zero);
			EffectManagerClass.body.PlayEffect("effect_ufo", base.transform);
			playAudioHurt();
			audios.PlayAudio("FXblowup_bomb", player.transform);
			audios.PlayAudio("FXCrash_bang", player.transform);
			break;
		case ItemType.GUANGHUAN:
			if (attributeByName.colliderCount == 0)
			{
				ItemManagerClass.body.SlowGame();
			}
			ItemManagerClass.body.SetPlayerSleep(true);
			ItemManagerClass.body.SetPlayerAddForce(zero);
			EffectManagerClass.body.PlayEffect("effect_goodstartouch", player.transform);
			audios.PlayAudio("FXobtain_star", base.transform);
			break;
		case ItemType.STAR:
			if (attributeByName.colliderCount == 0)
			{
				ItemManagerClass.body.SlowGame();
			}
			ItemManagerClass.body.SetPlayerSleep(true);
			ItemManagerClass.body.SetPlayerAddForce(zero);
			audios.PlayAudio("FXobtain_slowdown", player.transform);
			audios.PlayAudio("FXCrash_bang", player.transform);
			audios.PlayAudio("SVOhurt", player.transform);
			break;
		}
		if ((bool)base.GetComponent<Rigidbody>())
		{
			base.GetComponent<Rigidbody>().Sleep();
		}
	}

	private void stopAudioCheer()
	{
		switch (globalVal.g_avatar_id)
		{
		case 0:
			audios.StopAudio("SVOcheer");
			break;
		case 1:
			audios.StopAudio("SVOtintin_cheer");
			break;
		case 2:
			audios.StopAudio("SVOindiana J_cheer");
			break;
		case 3:
			audios.StopAudio("SVOsoldier_cheer");
			break;
		case 4:
			audios.StopAudio("SVOterminator_cheer");
			break;
		case 5:
			audios.StopAudio("SVOhalo_cheer");
			break;
		}
	}

	private void stopAudioFall()
	{
		switch (globalVal.g_avatar_id)
		{
		case 0:
			audios.StopAudio("SVOfall");
			break;
		case 1:
			audios.StopAudio("SVOtintin_fall");
			break;
		case 2:
			audios.StopAudio("SVOindiana J_fall");
			break;
		case 3:
			audios.StopAudio("SVOsoldier_fall");
			break;
		case 4:
			audios.StopAudio("SVOterminator_fall");
			break;
		case 5:
			audios.StopAudio("SVOhalo_fall");
			break;
		}
	}

	private void playAudioHurt()
	{
		stopAudioCheer();
		stopAudioFall();
		switch (globalVal.g_avatar_id)
		{
		case 0:
		{
			int num = UnityEngine.Random.Range(0, 100) % 10;
			if (num > 6)
			{
				MonoBehaviour.print("VO hurt");
				audios.PlayAudio("VOhurt", player.transform);
			}
			else
			{
				audios.PlayAudio("SVOhurt", player.transform);
			}
			break;
		}
		case 1:
			audios.PlayAudio("SVOtintin_hurt", player.transform);
			break;
		case 2:
			audios.PlayAudio("SVOindiana J_hurt", player.transform);
			break;
		case 3:
			audios.PlayAudio("SVOsoldier_hurt", player.transform);
			break;
		case 4:
			audios.PlayAudio("SVOterminator_hurt", player.transform);
			break;
		case 5:
			audios.PlayAudio("SVOhalo_hurt", player.transform);
			break;
		}
	}

	private void playAudioSpring()
	{
		switch (globalVal.g_avatar_id)
		{
		case 0:
			audios.PlayAudio("SVOspring", player.transform);
			break;
		case 1:
			audios.PlayAudio("SVOtintin_spring", player.transform);
			break;
		case 2:
			audios.PlayAudio("SVOindiana J_spring", player.transform);
			break;
		case 3:
			audios.PlayAudio("SVOsoldier_spring", player.transform);
			break;
		case 4:
			audios.PlayAudio("SVOterminator_spring", player.transform);
			break;
		case 5:
			audios.PlayAudio("SVOhalo_spring", player.transform);
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
}
