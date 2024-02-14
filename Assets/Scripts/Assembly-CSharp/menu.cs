using System.Collections;
using UnityEngine;

public class menu : MonoBehaviour
{
	private enum ANIID
	{
		NONE = 0,
		UIFLY = 1,
		UIROTATE = 2,
		UIROTATE2 = 3
	}

	private TAudioController audios;

	public int selectIndex = -1;

	public UILayer layer;

	private Transform listener;

	private UIEventManager ui_event;

	private bool menuKey;

	private bool statisticsKey;

	private GameObject player;

	private ANIID AniState;

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
		ui_event = Camera.main.GetComponent(typeof(UIEventManager)) as UIEventManager;
		if (audios.GetMusicName() != "BGMtitle")
		{
			audios.PlayMusic("BGMtitle");
		}
//		iPhoneSettings.screenCanDarken = false;
		listener = GameObject.Find("AudioListener").transform;
		listener.position = Vector3.zero;
		InitAllMenu();
		if (layer != 0)
		{
			globalVal.UIState = layer;
		}
		switch (globalVal.UIState)
		{
		case UILayer.MENU:
			InitMenu();
			break;
		case UILayer.THESTASH:
			InitTheStash();
			break;
		case UILayer.OPTIONS:
			InitOption();
			break;
		case UILayer.AVATAR:
			InitAvatar();
			break;
		case UILayer.ITEMS:
			InitItems();
			break;
		case UILayer.UPGRADES:
			InitUpgrades();
			break;
		case UILayer.PROFILE:
			InitProfile();
			break;
		case UILayer.TBANK:
			InitTbank();
			break;
		case UILayer.HOWTO:
			InitHowto();
			break;
		case UILayer.CREDITS:
			InitCredits();
			break;
		case UILayer.INGAME:
		case UILayer.PAUSE:
		case UILayer.GAMEOVER:
			break;
		}
	}

	private void Update()
	{
		UILayer uIState = globalVal.UIState;
		if (uIState != UILayer.MENU)
		{
			return;
		}
		if (statisticsKey && StatisticsData.data != null)
		{
			statisticsKey = false;
			menuKey = true;
			GameObject gameObject = null;
			gameObject = new GameObject("ItemManager");
			gameObject.AddComponent(typeof(ItemManager));
			Object.DontDestroyOnLoad(gameObject);
		}
		if (ItemManagerClass.body != null && !ItemManagerClass.body.isloading && menuKey && !statisticsKey)
		{
			menuKey = false;
			StartMenu();
		}
		switch (AniState)
		{
		case ANIID.UIFLY:
			if (!player.GetComponent<Animation>().isPlaying)
			{
				player.GetComponent<Animation>().Play("UIrotate");
				AniState = ANIID.UIROTATE;
			}
			break;
		case ANIID.UIROTATE:
			if (!player.GetComponent<Animation>().isPlaying)
			{
				player.GetComponent<Animation>().Play("UIrotate2");
				AniState = ANIID.UIROTATE2;
				globalVal.InitGameCenterPlugin();
				GameCenterPlugin.SubmitScore("com.trinitigame.miniglider.l1", (int)globalVal.g_best_dis);
			}
			break;
		}
	}

	private void InitAllMenu()
	{
		Transform transform = null;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			transform = base.transform.GetChild(i);
			if (!(transform.name == "bg_ipads") && !(transform.name == "fade_bg"))
			{
				transform.localPosition = new Vector3(-1000f, -1000f, 0f);
			}
		}
	}

	private void InitMenu()
	{
		float num = (float)Screen.width / (float)Screen.height;
		MonoBehaviour.print(num);
		if (num == 1.3333334f || num == 0.75f)
		{
			Camera.main.fov = 32f;
		}
		else
		{
			Camera.main.fov = 26.94286f;
		}
		GameObject gameObject = GameObject.Find("ItemManager");
		if (gameObject == null)
		{
			GameObject gameObject2 = new GameObject("Notification");
			gameObject2.AddComponent(typeof(MiniGliderLocalNotification));
			gameObject2.AddComponent(typeof(PublicDailyData));
			Object.DontDestroyOnLoad(gameObject2);
			statisticsKey = true;
		}
		else
		{
			StartMenu();
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (!(StatisticsData.data == null) && !pause)
		{
			StatisticsData.data.SaveDailyData(StatisticsData.data.GetSystemDay());
		}
	}

	private void StartMenu()
	{
		MonoBehaviour.print("init ui");
		globalVal.InitPlugins();
		globalVal.SetOpenClickShow(true);
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		SetAvatar(globalVal.g_avatar_id, "jump_protagonist_1");
		player = GameObject.Find("jump_protagonist_1");
		player.GetComponent<Animation>().Play("UIfly_1");
		AniState = ANIID.UIFLY;
		StatisticsData.data.d_UITapCount[0]++;
		StatisticsData.data.d_UiLastTap = 0;
		StatisticsData.data.SaveDailyData(StatisticsData.data.GetSystemDay());
		transform = base.transform.Find("task_page");
		TaskManager taskManager = transform.GetComponent(typeof(TaskManager)) as TaskManager;
		taskManager.InitTaskList();
		transform = base.transform.Find("twitter");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetPos(-229f, 73f, -9f);
		TUIButtonClick tUIButtonClick = transform.GetComponent(typeof(TUIButtonClick)) as TUIButtonClick;
		string text = string.Empty + Utils.GetIOSYear() + Utils.GetIOSMonth() + Utils.GetIOSDay();
		if (globalVal.twitterDay != text && globalVal.twitterKey)
		{
			tUIButtonClick.SetDisabled(false);
		}
		else
		{
			tUIButtonClick.SetDisabled(true);
		}
		transform = base.transform.Find("fade_bg");
		TUIFade tUIFade = transform.GetComponent(typeof(TUIFade)) as TUIFade;
		tUIFade.FadeIn();
		transform = base.transform.Find("menu");
		transform.localPosition = Vector3.zero;
		Transform transform2 = null;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform2 = transform.GetChild(i);
			switch (transform.GetChild(i).name)
			{
			case "btn_bg":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(151.5866f, -59.4f, 0f);
				uIMoveControl.RightToLeft(1f, transform2);
				break;
			case "options":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(160.9165f, -46.95f, -1f);
				uIMoveControl.RightToLeft(1.38f, transform2);
				break;
			case "playNow":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(135.9402f, 46.06f, -1f);
				uIMoveControl.RightToLeft(1.28f, transform2);
				break;
			case "theStash":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(157.2614f, -2.3f, -1f);
				uIMoveControl.RightToLeft(1.33f, transform2);
				break;
			case "buttom_bg":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(164.1f, -125f, -2f);
				uIMoveControl.DownToUp(1.4f, transform2);
				break;
			case "ranking":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(134.6f, -104f, -1f);
				uIMoveControl.DownToUp(1.45f, transform2);
				break;
			case "trophy":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(190.57f, -103f, -1f);
				uIMoveControl.DownToUp(1.5f, transform2);
				break;
			case "game_bg":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(-43.5f, 89.8f, -2f);
				uIMoveControl.UpToDown(1.5f, transform2);
				break;
			}
		}
	}

	public void InitTbank()
	{
		globalVal.SetOpenClickShow(false);
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		StatisticsData.data.d_UITapCount[3]++;
		StatisticsData.data.d_UiLastTap = 3;
		StatisticsData.data.SaveDailyData(StatisticsData.data.GetSystemDay());
		transform = base.transform.Find("fade_bg");
		TUIFade tUIFade = transform.GetComponent(typeof(TUIFade)) as TUIFade;
		tUIFade.FadeIn();
		transform = base.transform.Find("bg");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetPos(0f, 0f, 1f);
		transform = base.transform.Find("tbank");
		transform.localPosition = Vector3.zero;
		TUIMeshText tUIMeshText = transform.Find("title/label_glod").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
		tUIMeshText.text = string.Empty + globalVal.g_gold;
		Transform transform2 = null;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform2 = transform.GetChild(i);
			switch (transform.GetChild(i).name)
			{
			case "title":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(0f, 140f, 0f);
				uIMoveControl.UpToDown(0f, transform2);
				break;
			}
		}
	}

	public void InitHowto()
	{
		globalVal.SetOpenClickShow(true);
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("fade_bg");
		TUIFade tUIFade = transform.GetComponent(typeof(TUIFade)) as TUIFade;
		tUIFade.FadeIn();
		transform = base.transform.Find("bg");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetPos(0f, 0f, 1f);
		transform = base.transform.Find("howto");
		transform.localPosition = Vector3.zero;
		Transform transform2 = null;
		TUIMeshText tUIMeshText = transform.Find("title/label_glod").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
		tUIMeshText.text = string.Empty + globalVal.g_gold;
	}

	public void InitCredits()
	{
		globalVal.SetOpenClickShow(true);
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("fade_bg");
		TUIFade tUIFade = transform.GetComponent(typeof(TUIFade)) as TUIFade;
		tUIFade.FadeIn();
		transform = base.transform.Find("bg");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetPos(0f, 0f, 1f);
		transform = base.transform.Find("credits");
		transform.localPosition = Vector3.zero;
		Transform transform2 = null;
		TUIMeshText tUIMeshText = transform.Find("title/label_glod").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
		tUIMeshText.text = string.Empty + globalVal.g_gold;
	}

	private void InitTheStash()
	{
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		StatisticsData.data.d_UITapCount[1]++;
		StatisticsData.data.d_UiLastTap = 1;
		StatisticsData.data.SaveDailyData(StatisticsData.data.GetSystemDay());
		transform = base.transform.Find("fade_bg");
		TUIFade tUIFade = transform.GetComponent(typeof(TUIFade)) as TUIFade;
		tUIFade.FadeIn();
		globalVal.SetOpenClickShow(true);
		transform = base.transform.Find("bg");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetPos(0f, 0f, 1f);
		transform = base.transform.Find("thestash");
		transform.localPosition = Vector3.zero;
		Transform transform2 = null;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform2 = transform.GetChild(i);
			switch (transform.GetChild(i).name)
			{
			case "title":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(0f, 140f, 0f);
				uIMoveControl.UpToDown(0f, transform2);
				break;
			case "thestash_avatar":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(-160f, 33f, -1f);
				uIMoveControl.LeftToRight(0.1f, transform2);
				if (globalVal.g_gold >= ItemManagerClass.body.GetTypeMinPrice(transform2.name))
				{
					GameObject gameObject2 = EffectManagerClass.body.PlayEffect_gantanhao();
					gameObject2.transform.parent = transform2;
					gameObject2.transform.localPosition = new Vector3(57.9f, -58.2f, -2.1f);
				}
				break;
			case "thestash_upgrades":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(0f, 33f, -1f);
				uIMoveControl.RightToLeft(0.2f, transform2);
				if (globalVal.g_gold >= ItemManagerClass.body.GetTypeMinPrice(transform2.name))
				{
					GameObject gameObject3 = EffectManagerClass.body.PlayEffect_gantanhao();
					gameObject3.transform.parent = transform2;
					gameObject3.transform.localPosition = new Vector3(61.47f, -58.6f, -2.1f);
				}
				break;
			case "thestash_items":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(156f, 33f, -1f);
				uIMoveControl.RightToLeft(0.3f, transform2);
				if (globalVal.g_gold >= ItemManagerClass.body.GetTypeMinPrice(transform2.name))
				{
					GameObject gameObject = EffectManagerClass.body.PlayEffect_gantanhao();
					gameObject.transform.parent = transform2;
					gameObject.transform.localPosition = new Vector3(60.92f, -58.34f, -2.1f);
				}
				break;
			case "thestash_profile":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(-146f, -98f, -1f);
				uIMoveControl.DownToUp(0.2f, transform2);
				break;
			case "thestash_play":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(143f, -100f, -1f);
				uIMoveControl.DownToUp(0.3f, transform2);
				break;
			}
		}
		TUIMeshText tUIMeshText = transform.Find("title/label_glod").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
		tUIMeshText.text = string.Empty + globalVal.g_gold;
	}

	private void InitOption()
	{
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		StatisticsData.data.d_UITapCount[2]++;
		StatisticsData.data.d_UiLastTap = 2;
		StatisticsData.data.SaveDailyData(StatisticsData.data.GetSystemDay());
		transform = base.transform.Find("fade_bg");
		TUIFade tUIFade = transform.GetComponent(typeof(TUIFade)) as TUIFade;
		tUIFade.FadeIn();
		globalVal.SetOpenClickShow(true);
		ui_event.UpdateMusicState();
		ui_event.UpdateSoundState();
		transform = base.transform.Find("bg");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetPos(0f, 0f, 1f);
		transform = base.transform.Find("option");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.RightToLeft(0f, transform);
		TUIMeshText tUIMeshText = transform.Find("title/label_glod").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
		tUIMeshText.text = string.Empty + globalVal.g_gold;
	}

	private void InitAvatar()
	{
		if (Utils.ScreenType() > 0)
		{
			globalVal.SetOpenClickShow(true);
		}
		else
		{
			globalVal.SetOpenClickShow(false);
		}
		InitAvatarList();
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("bg");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetPos(0f, 0f, 1f);
		transform = base.transform.Find("fade_bg");
		TUIFade tUIFade = transform.GetComponent(typeof(TUIFade)) as TUIFade;
		tUIFade.FadeIn();
		transform = base.transform.Find("avatar");
		transform.localPosition = Vector3.zero;
		Transform transform2 = null;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform2 = transform.GetChild(i);
			switch (transform.GetChild(i).name)
			{
			case "title":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(0f, 140f, -2.1f);
				uIMoveControl.UpToDown(0.1f, transform2);
				break;
			case "listground":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(0f, 90f, 0f);
				uIMoveControl.RightToLeft(0.1f, transform2);
				break;
			case "sliderbg":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(217f, -11f, -1f);
				uIMoveControl.RightToLeft(0.1f, transform2);
				break;
			}
		}
	}

	private void InitItems()
	{
		if (Utils.ScreenType() > 0)
		{
			globalVal.SetOpenClickShow(true);
		}
		else
		{
			globalVal.SetOpenClickShow(false);
		}
		InitItemList();
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("fade_bg");
		TUIFade tUIFade = transform.GetComponent(typeof(TUIFade)) as TUIFade;
		tUIFade.FadeIn();
		transform = base.transform.Find("bg");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetPos(0f, 0f, 1f);
		transform = base.transform.Find("items");
		transform.localPosition = Vector3.zero;
		Transform transform2 = null;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform2 = transform.GetChild(i);
			switch (transform.GetChild(i).name)
			{
			case "title":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(0f, 140f, -2.1f);
				uIMoveControl.UpToDown(0.1f, transform2);
				break;
			case "listground":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(0f, 90f, 0f);
				uIMoveControl.RightToLeft(0.1f, transform2);
				break;
			case "sliderbg":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(217f, -11f, -1f);
				uIMoveControl.RightToLeft(0.1f, transform2);
				break;
			}
		}
	}

	private void InitUpgrades()
	{
		if (Utils.ScreenType() > 0)
		{
			globalVal.SetOpenClickShow(true);
		}
		else
		{
			globalVal.SetOpenClickShow(false);
		}
		InitUpgradesList();
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("fade_bg");
		TUIFade tUIFade = transform.GetComponent(typeof(TUIFade)) as TUIFade;
		tUIFade.FadeIn();
		transform = base.transform.Find("bg");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetPos(0f, 0f, 1f);
		transform = base.transform.Find("upgrades");
		transform.localPosition = Vector3.zero;
		Transform transform2 = null;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform2 = transform.GetChild(i);
			switch (transform.GetChild(i).name)
			{
			case "title":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(0f, 140f, -2.1f);
				uIMoveControl.UpToDown(0.1f, transform2);
				break;
			case "listground":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(0f, 90f, 0f);
				uIMoveControl.RightToLeft(0.1f, transform2);
				break;
			case "sliderbg":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(217f, -11f, -1f);
				uIMoveControl.RightToLeft(0.1f, transform2);
				break;
			}
		}
	}

	public void InitItemList()
	{
		ArrayList itemOnceArray = ItemManagerClass.body.itemOnceArray;
		Transform transform = null;
		GameObject original = Resources.Load("Prefab/list_item") as GameObject;
		transform = base.transform.Find("items");
		Transform transform2 = transform.Find("listground");
		RemoveAllList(transform2);
		float num = 50f;
		Transform transform3 = transform.Find("listground_scroll");
		int num2 = 0;
		for (int i = 0; i < itemOnceArray.Count; i++)
		{
			ItemOnceAttribute itemOnceAttribute = itemOnceArray[i] as ItemOnceAttribute;
			GameObject gameObject = Object.Instantiate(original) as GameObject;
			gameObject.name = "avatar_item";
			gameObject.transform.parent = transform2;
			gameObject.transform.localPosition = new Vector3(0f, (float)(-num2) * num, 0f);
			TUIMeshText tUIMeshText = null;
			TUIMeshSprite tUIMeshSprite = null;
			tUIMeshText = gameObject.transform.Find("label").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
			tUIMeshText.text = itemOnceAttribute.name.ToUpper();
			if ((int)globalVal.g_item_once_count[i] > 0)
			{
				tUIMeshText.transform.localPosition = new Vector3(-116.834f, 17.44226f, -0.6f);
			}
			else
			{
				tUIMeshText.transform.localPosition = new Vector3(-116.834f, 8f, -0.6f);
			}
			tUIMeshText = gameObject.transform.Find("label_gold").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
			tUIMeshText.text = itemOnceAttribute.price.ToString();
			if (globalVal.g_gold >= itemOnceAttribute.price)
			{
				GameObject gameObject2 = EffectManagerClass.body.PlayEffect_gantanhao();
				gameObject2.transform.parent = gameObject.transform;
				gameObject2.transform.localPosition = new Vector3(111.6f, 0f, -0.1f);
			}
			tUIMeshSprite = gameObject.transform.Find("picture").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
			tUIMeshSprite.frameName = itemOnceAttribute.picname;
			gameObject.transform.Find("picture_bg").gameObject.SetActive(false);
			tUIMeshSprite = gameObject.transform.Find("picture_bought").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
			if ((int)globalVal.g_item_once_count[i] > 0)
			{
				tUIMeshSprite.gameObject.active = true;
			}
			else
			{
				tUIMeshSprite.gameObject.active = false;
			}
			tUIMeshSprite = gameObject.transform.Find("picture_count").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
			if ((int)globalVal.g_item_once_count[i] > 0)
			{
				tUIMeshSprite.gameObject.active = true;
			}
			else
			{
				tUIMeshSprite.gameObject.active = false;
			}
			tUIMeshText = gameObject.transform.Find("label_count").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
			tUIMeshText.text = ((int)globalVal.g_item_once_count[i]).ToString();
			if ((int)globalVal.g_item_once_count[i] > 0)
			{
				tUIMeshText.gameObject.active = true;
			}
			else
			{
				tUIMeshText.gameObject.active = false;
			}
			num2++;
		}
		TUIScroll tUIScroll = transform3.GetComponent(typeof(TUIScroll)) as TUIScroll;
		tUIScroll.rangeYMin = 90f;
		if (num2 > 5)
		{
			tUIScroll.rangeYMax = tUIScroll.rangeYMin + (float)(num2 - 5) * num;
		}
		tUIScroll.borderYMin = tUIScroll.rangeYMin - num;
		tUIScroll.borderYMax = tUIScroll.rangeYMax + num;
		TUIMeshText tUIMeshText2 = transform.Find("title/label_glod").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
		tUIMeshText2.text = string.Empty + globalVal.g_gold;
	}

	public void InitAvatarList()
	{
		ArrayList avatarArray = ItemManagerClass.body.avatarArray;
		Transform transform = null;
		GameObject original = Resources.Load("Prefab/list_avatar") as GameObject;
		transform = base.transform.Find("avatar");
		Transform transform2 = transform.Find("listground");
		RemoveAllList(transform2);
		float num = 50f;
		Transform transform3 = transform.Find("listground_scroll");
		int num2 = 0;
		for (int i = 0; i < avatarArray.Count; i++)
		{
			AvatarAttribute avatarAttribute = avatarArray[i] as AvatarAttribute;
			int num3 = (int)globalVal.g_avatar_isbuy[i];
			GameObject gameObject = Object.Instantiate(original) as GameObject;
			gameObject.name = "avatar_item";
			gameObject.transform.parent = transform2;
			gameObject.transform.localPosition = new Vector3(0f, (float)(-num2) * num, 0f);
			TUIMeshText tUIMeshText = null;
			TUIMeshSprite tUIMeshSprite = null;
			tUIMeshText = gameObject.transform.Find("label").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
			tUIMeshText.text = avatarAttribute.name.ToUpper();
			tUIMeshText = gameObject.transform.Find("label_gold").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
			tUIMeshText.text = avatarAttribute.price.ToString();
			if (num3 == 1)
			{
				tUIMeshText.gameObject.active = false;
			}
			else
			{
				tUIMeshText.gameObject.active = true;
			}
			if (num3 == 0 && globalVal.g_gold >= avatarAttribute.price)
			{
				GameObject gameObject2 = EffectManagerClass.body.PlayEffect_gantanhao();
				gameObject2.transform.parent = gameObject.transform;
				gameObject2.transform.localPosition = new Vector3(111.6f, 0f, -0.1f);
			}
			tUIMeshSprite = gameObject.transform.Find("picture").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
			tUIMeshSprite.frameName = avatarAttribute.picname;
			tUIMeshSprite = gameObject.transform.Find("pic_gold").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
			if (num3 == 1)
			{
				tUIMeshSprite.gameObject.active = false;
			}
			else
			{
				tUIMeshSprite.gameObject.active = true;
			}
			tUIMeshSprite = gameObject.transform.Find("pic_gold_bg").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
			if (num3 == 1)
			{
				tUIMeshSprite.gameObject.active = false;
			}
			else
			{
				tUIMeshSprite.gameObject.active = true;
			}
			tUIMeshSprite = gameObject.transform.Find("picture_bought").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
			if (num3 == 1)
			{
				tUIMeshSprite.frameName = "item_bought";
			}
			else
			{
				tUIMeshSprite.frameName = string.Empty;
			}
			tUIMeshSprite = gameObject.transform.Find("picture_used").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
			if (globalVal.g_avatar_id == i)
			{
				tUIMeshSprite.frameName = "gou1";
			}
			else
			{
				tUIMeshSprite.frameName = string.Empty;
			}
			num2++;
		}
		TUIScroll tUIScroll = transform3.GetComponent(typeof(TUIScroll)) as TUIScroll;
		tUIScroll.rangeYMin = 90f;
		if (num2 > 5)
		{
			tUIScroll.rangeYMax = tUIScroll.rangeYMin + (float)(num2 - 5) * num;
		}
		tUIScroll.borderYMin = tUIScroll.rangeYMin - num;
		tUIScroll.borderYMax = tUIScroll.rangeYMax + num;
		TUIMeshText tUIMeshText2 = transform.Find("title/label_glod").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
		tUIMeshText2.text = string.Empty + globalVal.g_gold;
	}

	private void RemoveAllList(Transform root)
	{
		foreach (Transform item in root)
		{
			Object.Destroy(item.gameObject);
		}
	}

	public void InitUpgradesList()
	{
		ArrayList attributeArray = ItemManagerClass.body.attributeArray;
		Transform transform = null;
		GameObject original = Resources.Load("Prefab/list_upgrade") as GameObject;
		transform = base.transform.Find("upgrades");
		Transform transform2 = transform.Find("listground");
		RemoveAllList(transform2);
		float num = 50f;
		Transform transform3 = transform.Find("listground_scroll");
		TUIRect tUIRect = transform.Find("clipRect").GetComponent(typeof(TUIRect)) as TUIRect;
		int num2 = 0;
		bool flag = true;
		for (int i = 0; i < attributeArray.Count; i++)
		{
			ItemAttribute itemAttribute = attributeArray[i] as ItemAttribute;
			int num3 = (int)globalVal.g_itemlevel[itemAttribute.index];
			if (itemAttribute.inshop)
			{
				GameObject gameObject = Object.Instantiate(original) as GameObject;
				gameObject.name = "avatar_item";
				gameObject.transform.parent = transform2;
				bool flag2 = false;
				if (num3 + 1 >= itemAttribute.level.Count)
				{
					num3 = itemAttribute.level.Count - 1;
					flag2 = true;
					MonoBehaviour.print("max");
				}
				else
				{
					num3++;
					flag = false;
				}
				gameObject.transform.localPosition = new Vector3(0f, (float)(-num2) * num, 0f);
				TUIMeshText tUIMeshText = null;
				TUIMeshSprite tUIMeshSprite = null;
				tUIMeshText = gameObject.transform.Find("label").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				tUIMeshText.text = (itemAttribute.level[num3] as ItemSubAttr).name.ToUpper();
				tUIMeshText = gameObject.transform.Find("label_gold").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				tUIMeshText.text = (itemAttribute.level[num3] as ItemSubAttr).price.ToString();
				if (flag2)
				{
					tUIMeshText.gameObject.active = false;
				}
				else
				{
					tUIMeshText.gameObject.active = true;
				}
				if (!flag2 && globalVal.g_gold >= (itemAttribute.level[num3] as ItemSubAttr).price)
				{
					GameObject gameObject2 = EffectManagerClass.body.PlayEffect_gantanhao();
					gameObject2.transform.parent = gameObject.transform;
					gameObject2.transform.localPosition = new Vector3(111.6f, 0f, -0.1f);
				}
				tUIMeshSprite = gameObject.transform.Find("picture").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
				tUIMeshSprite.frameName = (itemAttribute.level[num3] as ItemSubAttr).picname;
				tUIMeshSprite = gameObject.transform.Find("pic_gold").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
				if (flag2)
				{
					tUIMeshSprite.gameObject.active = false;
				}
				else
				{
					tUIMeshSprite.gameObject.active = true;
				}
				tUIMeshSprite = gameObject.transform.Find("pic_gold_bg").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
				if (flag2)
				{
					tUIMeshSprite.gameObject.active = false;
				}
				else
				{
					tUIMeshSprite.gameObject.active = true;
				}
				tUIMeshText = gameObject.transform.Find("label_level").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				tUIMeshText.text = "LV  " + num3;
				tUIMeshSprite = gameObject.transform.Find("picture_max").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
				if (flag2)
				{
					tUIMeshSprite.gameObject.active = true;
				}
				else
				{
					tUIMeshSprite.gameObject.active = false;
				}
				num2++;
			}
		}
		if (flag)
		{
			GameCenterPlugin.SubmitAchievement("com.trinitigame.miniglider.a13", 100);
			globalVal.g_achievement_key[12] = true;
		}
		TUIScroll tUIScroll = transform3.GetComponent(typeof(TUIScroll)) as TUIScroll;
		tUIScroll.rangeYMin = 90f;
		if (num2 > 5)
		{
			tUIScroll.rangeYMax = tUIScroll.rangeYMin + (float)(num2 - 5) * num;
		}
		tUIScroll.borderYMin = tUIScroll.rangeYMin - num;
		tUIScroll.borderYMax = tUIScroll.rangeYMax + num;
		TUIMeshText tUIMeshText2 = transform.Find("title/label_glod").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
		tUIMeshText2.text = string.Empty + globalVal.g_gold;
	}

	private void InitProfileList()
	{
		Transform transform = null;
		TUIMeshText tUIMeshText = null;
		Transform transform2 = null;
		transform = base.transform.Find("profile/profile_bg/profilelist");
		for (int i = 0; i < transform.childCount; i++)
		{
			transform2 = transform.GetChild(i);
			switch (transform2.name)
			{
			case "label_distance":
				tUIMeshText = transform2.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				tUIMeshText.text = (int)globalVal.g_best_dis + " M";
				break;
			case "label_height":
				tUIMeshText = transform2.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				tUIMeshText.text = (int)globalVal.g_best_height + " M";
				break;
			case "label_speed":
				tUIMeshText = transform2.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				tUIMeshText.text = (int)globalVal.g_best_speed + " M/S";
				break;
			case "label_count":
				tUIMeshText = transform2.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				tUIMeshText.text = globalVal.g_best_playcount.ToString();
				break;
			case "label_total_distance":
				tUIMeshText = transform2.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				tUIMeshText.text = (ulong)globalVal.g_best_totaldis + " KM";
				break;
			case "label_task":
			{
				int num = 0;
				for (int j = 0; j < ItemManagerClass.body.taskListArray.Count; j++)
				{
					ArrayList arrayList = ItemManagerClass.body.taskListArray[j] as ArrayList;
					num += arrayList.Count;
				}
				tUIMeshText = transform2.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				tUIMeshText.text = globalVal.g_best_taskcomplete + " / " + num;
				break;
			}
			case "label_zombies":
				tUIMeshText = transform2.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				tUIMeshText.text = globalVal.g_best_zombiebreak.ToString();
				break;
			}
		}
	}

	private void InitProfile()
	{
		GameObject gameObject = GameObject.Find("Avatar_Character");
		float num = (float)Screen.width / (float)Screen.height;
		MonoBehaviour.print(num);
		if (num == 1.3333334f || num == 0.75f)
		{
			gameObject.transform.position = new Vector3(-10000.59f, -0.9267035f, 0f);
		}
		else
		{
			gameObject.transform.position = new Vector3(-10000.98f, -0.9267035f, 0f);
		}
		MonoBehaviour.print(gameObject.transform.position);
		SetUIAvatar(globalVal.g_avatar_id);
		globalVal.SetOpenClickShow(true);
		InitProfileList();
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("fade_bg");
		TUIFade tUIFade = transform.GetComponent(typeof(TUIFade)) as TUIFade;
		tUIFade.FadeIn();
		transform = base.transform.Find("bg");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetPos(0f, 0f, 1f);
		transform = base.transform.Find("profile");
		transform.localPosition = Vector3.zero;
		Transform transform2 = null;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform2 = transform.GetChild(i);
			switch (transform.GetChild(i).name)
			{
			case "title":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(0f, 140f, -1f);
				uIMoveControl.UpToDown(0.1f, transform2);
				break;
			case "profile_bg":
				uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(-1.684551f, -12.76361f, -1f);
				uIMoveControl.DownToUp(0.1f, transform2);
				uIMoveControl.SetCallBack(base.transform, "ShowAvatarVeiw");
				break;
			}
		}
		TUIMeshText tUIMeshText = transform.Find("title/label_glod").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
		tUIMeshText.text = string.Empty + globalVal.g_gold;
	}

	private void InitPopInfo(int index)
	{
		switch (globalVal.UIState)
		{
		case UILayer.AVATAR:
		{
			ArrayList avatarArray = ItemManagerClass.body.avatarArray;
			Transform transform2 = null;
			transform2 = base.transform.Find("picture_bg");
			transform2.localPosition = new Vector3(0f, 0f, -4.95f);
			transform2 = base.transform.Find("popdialog");
			selectIndex = index;
			int num2 = 0;
			for (int j = 0; j < avatarArray.Count; j++)
			{
				AvatarAttribute avatarAttribute = avatarArray[j] as AvatarAttribute;
				if (num2 == index)
				{
					TUIMeshText tUIMeshText2 = null;
					transform2.Find("picture").GetComponent<Renderer>().enabled = false;
					tUIMeshText2 = transform2.Find("label_name").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
					tUIMeshText2.text = avatarAttribute.name.ToUpper();
					tUIMeshText2 = transform2.Find("label_info").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
					tUIMeshText2.text = avatarAttribute.info.ToUpper();
					tUIMeshText2 = transform2.Find("label_gold").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
					tUIMeshText2.text = avatarAttribute.price.ToString();
					transform2.Find("label_level").gameObject.active = false;
					transform2.Find("label_count").gameObject.active = false;
					TUIButtonClick tUIButtonClick2 = null;
					tUIButtonClick2 = transform2.Find("pop_dialog_buy").GetComponent(typeof(TUIButtonClick)) as TUIButtonClick;
					tUIButtonClick2.SetDisabled(false);
					tUIMeshText2 = tUIButtonClick2.transform.Find("label").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
					tUIMeshText2.color = new Color(1f, 1f, 1f);
					bool flag2 = false;
					tUIMeshText2 = transform2.Find("pop_dialog_buy/label").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
					if ((int)globalVal.g_avatar_isbuy[j] == 1)
					{
						tUIMeshText2.text = "USE";
						flag2 = true;
					}
					else
					{
						tUIMeshText2.text = "BUY";
					}
					if (flag2)
					{
						transform2.Find("label_gold").gameObject.active = false;
						transform2.Find("picture_gold").gameObject.active = false;
						transform2.Find("picture_gold_bg").gameObject.active = false;
					}
					else
					{
						transform2.Find("label_gold").gameObject.active = true;
						transform2.Find("picture_gold").gameObject.active = true;
						transform2.Find("picture_gold_bg").gameObject.active = true;
					}
					MonoBehaviour.print(globalVal.g_gold + "    " + avatarAttribute.price);
					break;
				}
				num2++;
			}
			break;
		}
		case UILayer.UPGRADES:
		{
			ArrayList attributeArray = ItemManagerClass.body.attributeArray;
			Transform transform3 = null;
			transform3 = base.transform.Find("picture_bg");
			transform3.localPosition = new Vector3(0f, 0f, -4.95f);
			transform3 = base.transform.Find("popdialog");
			selectIndex = index;
			int num3 = 0;
			for (int k = 0; k < attributeArray.Count; k++)
			{
				ItemAttribute itemAttribute = attributeArray[k] as ItemAttribute;
				if (!itemAttribute.inshop)
				{
					continue;
				}
				if (num3 == index)
				{
					TUIMeshText tUIMeshText3 = null;
					TUIMeshSprite tUIMeshSprite2 = null;
					int num4 = (int)globalVal.g_itemlevel[itemAttribute.index];
					bool flag3 = false;
					if (num4 + 1 >= itemAttribute.level.Count)
					{
						num4 = itemAttribute.level.Count - 1;
						flag3 = true;
					}
					else
					{
						num4++;
					}
					ItemSubAttr itemSubAttr = itemAttribute.level[num4] as ItemSubAttr;
					tUIMeshText3 = transform3.Find("label_name").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
					tUIMeshText3.text = itemSubAttr.name.ToUpper();
					tUIMeshText3 = transform3.Find("label_gold").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
					tUIMeshText3.text = itemSubAttr.price.ToString();
					tUIMeshText3 = transform3.Find("label_info").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
					tUIMeshText3.text = itemSubAttr.info.ToUpper();
					transform3.Find("label_count").gameObject.active = false;
					transform3.Find("label_level").gameObject.active = true;
					tUIMeshText3 = transform3.Find("label_level").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
					tUIMeshText3.text = "LV." + num4;
					transform3.Find("picture").GetComponent<Renderer>().enabled = true;
					tUIMeshSprite2 = transform3.Find("picture").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
					tUIMeshSprite2.frameName = itemSubAttr.picname + "_b";
					MonoBehaviour.print(tUIMeshSprite2.frameName);
					if (flag3)
					{
						transform3.Find("label_gold").gameObject.active = false;
						transform3.Find("picture_gold").gameObject.active = false;
						transform3.Find("picture_gold_bg").gameObject.active = false;
					}
					else
					{
						transform3.Find("label_gold").gameObject.active = true;
						transform3.Find("picture_gold").gameObject.active = true;
						transform3.Find("picture_gold_bg").gameObject.active = true;
					}
					TUIButtonClick tUIButtonClick3 = null;
					tUIButtonClick3 = transform3.Find("pop_dialog_buy").GetComponent(typeof(TUIButtonClick)) as TUIButtonClick;
					tUIMeshText3 = transform3.Find("pop_dialog_buy/label").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
					if (!flag3)
					{
						tUIButtonClick3.SetDisabled(false);
						tUIMeshText3.text = "UPGRADE";
						tUIMeshText3.gameObject.active = true;
					}
					else
					{
						tUIButtonClick3.SetDisabled(true);
						tUIMeshText3.gameObject.active = false;
					}
					break;
				}
				num3++;
			}
			break;
		}
		case UILayer.ITEMS:
		{
			ArrayList itemOnceArray = ItemManagerClass.body.itemOnceArray;
			Transform transform = null;
			transform = base.transform.Find("picture_bg");
			transform.localPosition = new Vector3(0f, 0f, -4.95f);
			transform = base.transform.Find("popdialog");
			selectIndex = index;
			int num = 0;
			for (int i = 0; i < itemOnceArray.Count; i++)
			{
				ItemOnceAttribute itemOnceAttribute = itemOnceArray[i] as ItemOnceAttribute;
				if (num == index)
				{
					TUIMeshText tUIMeshText = null;
					TUIMeshSprite tUIMeshSprite = null;
					bool flag = false;
					tUIMeshText = transform.Find("label_name").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
					tUIMeshText.text = itemOnceAttribute.name.ToUpper();
					transform.Find("label_info").localPosition = new Vector3(22.17877f, 36.7f, -5.3f);
					tUIMeshText = transform.Find("label_info").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
					tUIMeshText.text = itemOnceAttribute.info.ToUpper();
					tUIMeshText = transform.Find("label_gold").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
					tUIMeshText.text = itemOnceAttribute.price.ToString();
					transform.Find("picture").GetComponent<Renderer>().enabled = true;
					transform.Find("label_level").gameObject.active = false;
					transform.Find("label_count").gameObject.active = true;
					transform.Find("label_count").localPosition = new Vector3(22.17877f, 54.50531f, -5.3f);
					tUIMeshText = transform.Find("label_count").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
					tUIMeshText.text = "X " + (int)globalVal.g_item_once_count[i];
					if ((int)globalVal.g_item_once_count[i] >= 999)
					{
						flag = true;
					}
					if (flag)
					{
						transform.Find("label_gold").gameObject.active = false;
						transform.Find("picture_gold").gameObject.active = false;
						transform.Find("picture_gold_bg").gameObject.active = false;
					}
					else
					{
						transform.Find("label_gold").gameObject.active = true;
						transform.Find("picture_gold").gameObject.active = true;
						transform.Find("picture_gold_bg").gameObject.active = true;
					}
					TUIButtonClick tUIButtonClick = null;
					tUIButtonClick = transform.Find("pop_dialog_buy").GetComponent(typeof(TUIButtonClick)) as TUIButtonClick;
					tUIButtonClick.SetDisabled(false);
					tUIMeshText = tUIButtonClick.transform.Find("label").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
					tUIMeshText.color = new Color(1f, 1f, 1f);
					tUIMeshSprite = transform.Find("picture").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
					tUIMeshSprite.frameName = itemOnceAttribute.picname + "_b";
					MonoBehaviour.print(tUIMeshSprite.frameName);
					MonoBehaviour.print(globalVal.g_gold + "    " + itemOnceAttribute.price);
					break;
				}
				num++;
			}
			break;
		}
		}
	}

	public void ShowDialog(bool show, int index)
	{
		if (show)
		{
			InitPopInfo(index);
		}
		if (show)
		{
			audios.PlayAudio("UIpopup");
		}
		string text = string.Empty;
		switch (globalVal.UIState)
		{
		case UILayer.AVATAR:
			text = "avatar";
			break;
		case UILayer.ITEMS:
			text = "items";
			break;
		case UILayer.UPGRADES:
			text = "upgrades";
			break;
		}
		Transform transform = base.transform.Find(text + "/listground");
		Transform transform2 = base.transform.Find(text + "/listground_scroll");
		transform.gameObject.active = !show;
		transform2.gameObject.active = !show;
		TUIScroll tUIScroll = transform2.GetComponent(typeof(TUIScroll)) as TUIScroll;
		if (tUIScroll != null)
		{
			tUIScroll.Awake();
		}
		TUIContainer tUIContainer = transform.GetComponent(typeof(TUIContainer)) as TUIContainer;
		if (tUIContainer != null)
		{
			tUIContainer.Awake();
		}
		Transform transform3 = null;
		UIMoveControl uIMoveControl = null;
		transform3 = base.transform.Find("popdialog");
		uIMoveControl = transform3.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		if (show)
		{
			uIMoveControl.SetEndPos(0f, 0f, -5f);
			uIMoveControl.ScaleNoMove(0f, transform3);
			if (globalVal.UIState == UILayer.AVATAR)
			{
				SetUIAvatar(selectIndex);
				uIMoveControl.SetCallBack(base.transform, "ShowAvatarVeiw");
				GameObject gameObject = GameObject.Find("Avatar_Character");
				float num = (float)Screen.width / (float)Screen.height;
				MonoBehaviour.print(num);
				if (num == 1.3333334f || num == 0.75f)
				{
					gameObject.transform.position = new Vector3(-9999.8f, -0.9267035f, 0f);
				}
				else
				{
					gameObject.transform.position = new Vector3(-10000f, -0.9267035f, 0f);
				}
				MonoBehaviour.print(gameObject.transform.position);
			}
		}
		else
		{
			uIMoveControl.SetPos(-1000f, -1000f, 0f);
		}
		if (!show)
		{
			selectIndex = -1;
		}
	}

	public void InitAllItems()
	{
		string text = string.Empty;
		base.transform.Find("picture_bg").localPosition = new Vector3(-1000f, -1000f, 0f);
		switch (globalVal.UIState)
		{
		case UILayer.AVATAR:
			text = "avatar";
			break;
		case UILayer.ITEMS:
			text = "items";
			break;
		case UILayer.UPGRADES:
			text = "upgrades";
			break;
		}
		Transform transform = base.transform.Find(text + "/listground");
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			TUIButtonSelect tUIButtonSelect = child.GetComponent(typeof(TUIButtonSelect)) as TUIButtonSelect;
			tUIButtonSelect.SetSelected(false);
		}
	}

	private void SetUIAvatar(int index)
	{
		AvatarAttribute avatarAttribute = ItemManagerClass.body.avatarArray[index] as AvatarAttribute;
		GameObject original = Resources.Load("Prefab/avatar/" + avatarAttribute.modelname) as GameObject;
		GameObject gameObject = Object.Instantiate(original) as GameObject;
		GameObject gameObject2 = GameObject.Find("Avatar_Character");
		if (!(gameObject2 == null))
		{
			gameObject2.transform.parent = null;
			AvatarMounter.MountSkinnedMesh(gameObject2, gameObject, avatarAttribute.modelname);
			Object.Destroy(gameObject);
		}
	}

	private void SetAvatar(int index, string name)
	{
		AvatarAttribute avatarAttribute = ItemManagerClass.body.avatarArray[index] as AvatarAttribute;
		GameObject original = Resources.Load("Prefab/avatar/" + avatarAttribute.modelname) as GameObject;
		GameObject gameObject = Object.Instantiate(original) as GameObject;
		GameObject gameObject2 = GameObject.Find(name);
		if (!(gameObject2 == null))
		{
			gameObject2.transform.parent = null;
			AvatarMounter.MountSkinnedMesh(gameObject2, gameObject, avatarAttribute.modelname);
			Object.Destroy(gameObject);
		}
	}

	public void ShowAvatarVeiw()
	{
		GameObject gameObject = GameObject.Find("AvatarCamera");
		if (gameObject != null)
		{
			gameObject.GetComponent<Camera>().enabled = true;
			gameObject.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
		}
	}
}
