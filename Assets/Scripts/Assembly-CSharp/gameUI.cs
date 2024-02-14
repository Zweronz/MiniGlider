using System.Collections;
using UnityEngine;

public class gameUI : MonoBehaviour
{
	private bool[] task_complete_state = new bool[3];

	private float task_show_time;

	private float task_cur_time;

	private float drawpoint_curtime;

	private float drawpoint_showtime;

	private bool timeUpdateKey;

	private float x1 = 2000f;

	private float x2 = 1000f;

	private float y1 = 200f;

	private float y2 = 150f;

	private TAudioController audios;

	public int selectIndex = -1;

	public UILayer UIParentID = UILayer.INGAME;

	private UIEventManager ui_event;

	private bool CanDraw;

	private int DrawPointCount;

	private float DrawDis = 250f;

	private float DrawHei = 90f;

	private Transform trackStart;

	private Transform trackBeshHeight;

	private Transform trackBeshHeight2;

	private Transform trackDead;

	private Transform trackDead2;

	private Transform[] trackHuaxiang = new Transform[2];

	private Transform[] trackHuojian = new Transform[10];

	private int trackHuaxiang_count;

	private int trackHuojian_count;

	private SlingshotScript slingshot;

	private DrawPointManager lastLine;

	private DrawPointManager curLine;

	private bool stopHelpKey;

	public HelpState uitype;

	public bool isUiShaking;

	private bool buttonPopKey;

	private bool isBestDis;

	private int m_lastPointId;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("TAudioController");
		if (gameObject == null)
		{
			gameObject = Object.Instantiate(Resources.Load("TAudioController")) as GameObject;
			gameObject.name = "TAudioController";
			Object.DontDestroyOnLoad(gameObject);
		}
		globalVal.SetOpenClickShow(false);
		audios = gameObject.GetComponent(typeof(TAudioController)) as TAudioController;
		ui_event = Camera.main.GetComponent(typeof(UIEventManager)) as UIEventManager;
		slingshot = GameObject.Find("Slingshot").GetComponent(typeof(SlingshotScript)) as SlingshotScript;
		InitAllMenu();
		curLine = GameObject.Find("TUI/TUIControl/gameover_page/listground/DrawPointManager").GetComponent(typeof(DrawPointManager)) as DrawPointManager;
		lastLine = GameObject.Find("TUI/TUIControl/gameover_page/listground/DrawPointManager2").GetComponent(typeof(DrawPointManager)) as DrawPointManager;
		MonoBehaviour.print("mini price : " + ItemManagerClass.body.GetMinPrice());
	}

	private void Update()
	{
		if (timeUpdateKey)
		{
			task_cur_time += Time.deltaTime;
			if (task_cur_time > task_show_time)
			{
				timeUpdateKey = false;
				CompleteTask();
			}
		}
		if (!CanDraw)
		{
			return;
		}
		drawpoint_curtime += Time.deltaTime;
		if (!(drawpoint_curtime > drawpoint_showtime))
		{
			return;
		}
		drawpoint_showtime += Time.deltaTime;
		int num = ((globalVal.CurrentTrack.Count <= globalVal.LastTrack.Count) ? globalVal.LastTrack.Count : globalVal.CurrentTrack.Count);
		Vector3 vector = ((!(globalVal.curBestHeight.pos.y > globalVal.lastBestHeight.pos.y)) ? globalVal.lastBestHeight.pos : globalVal.curBestHeight.pos);
		float num2 = DrawDis / (float)num;
		float num3 = DrawHei / vector.y;
		if (DrawPointCount < num)
		{
			if (DrawPointCount < globalVal.CurrentTrack.Count)
			{
				int num4 = 0;
				while (!AddCurPointToTrack())
				{
					num4++;
				}
			}
			else
			{
				CanDraw = false;
				base.transform.Find("gameover_page/gameover_tap").localPosition = new Vector3(-1000f, 0f, 0f);
				NewBest();
			}
		}
		else
		{
			CanDraw = false;
			base.transform.Find("gameover_page/gameover_tap").localPosition = new Vector3(-1000f, 0f, 0f);
			NewBest();
		}
	}

	private void OnGUI()
	{
	}

	private bool AddCurPointToTrack()
	{
		bool result = false;
		int num = ((globalVal.CurrentTrack.Count <= globalVal.LastTrack.Count) ? globalVal.LastTrack.Count : globalVal.CurrentTrack.Count);
		Vector3 vector = ((!(globalVal.curBestHeight.pos.y > globalVal.lastBestHeight.pos.y)) ? globalVal.lastBestHeight.pos : globalVal.curBestHeight.pos);
		float num2 = DrawDis / (float)num;
		float num3 = DrawHei / vector.y;
		if (DrawPointCount > globalVal.CurrentTrack.Count - 1)
		{
			return true;
		}
		TrackInfo trackInfo = globalVal.CurrentTrack[DrawPointCount] as TrackInfo;
		Vector3 pos = trackInfo.pos;
		pos.x = (float)DrawPointCount * num2;
		pos.y *= num3;
		if (DrawPointCount > 0)
		{
			TrackInfo trackInfo2 = globalVal.CurrentTrack[m_lastPointId] as TrackInfo;
			Vector3 pos2 = trackInfo2.pos;
			pos2.x = (float)m_lastPointId * num2;
			pos2.y *= num3;
			if (!(Mathf.Abs(pos2.x - pos.x) < 0.5f) || trackInfo.type != 2)
			{
				curLine.AddLine(pos, pos2, Vector3.up, Vector3.right, 1f, Color.white);
				m_lastPointId = DrawPointCount;
				result = true;
			}
		}
		Vector3 vector2 = new Vector3(0f, 0f, -1f);
		if (globalVal.curBestHeight.index == DrawPointCount)
		{
			trackBeshHeight.position = curLine.GetPos() + pos + vector2;
		}
		if (trackInfo.type == 1)
		{
			trackStart.position = curLine.GetPos() + pos + vector2;
		}
		else if (trackInfo.type == 4)
		{
			trackHuaxiang[trackHuaxiang_count].position = curLine.GetPos() + pos + vector2;
			trackHuaxiang_count++;
		}
		else if (trackInfo.type == 3)
		{
			trackHuojian[trackHuojian_count].position = curLine.GetPos() + pos + vector2;
			trackHuojian_count++;
		}
		if (DrawPointCount == globalVal.CurrentTrack.Count - 1)
		{
			trackDead.position = curLine.GetPos() + pos + vector2;
		}
		DrawPointCount++;
		return result;
	}

	private void CompleteLastLineImmediate()
	{
		int num = ((globalVal.CurrentTrack.Count <= globalVal.LastTrack.Count) ? globalVal.LastTrack.Count : globalVal.CurrentTrack.Count);
		Vector3 vector = ((!(globalVal.curBestHeight.pos.y > globalVal.lastBestHeight.pos.y)) ? globalVal.lastBestHeight.pos : globalVal.curBestHeight.pos);
		float num2 = DrawDis / (float)num;
		float num3 = DrawHei / vector.y;
		DrawPointCount = 0;
		int num4 = 0;
		int num5 = 0;
		for (; DrawPointCount < num; DrawPointCount++)
		{
			if (DrawPointCount >= globalVal.LastTrack.Count)
			{
				continue;
			}
			TrackInfo trackInfo = globalVal.LastTrack[DrawPointCount] as TrackInfo;
			Vector3 pos = trackInfo.pos;
			pos.x = (float)DrawPointCount * num2;
			pos.y *= num3;
			if (DrawPointCount > 0)
			{
				TrackInfo trackInfo2 = globalVal.LastTrack[num4] as TrackInfo;
				Vector3 pos2 = trackInfo2.pos;
				pos2.x = (float)num4 * num2;
				if (Mathf.Abs(pos2.x - pos.x) < 0.5f && trackInfo.type == 2)
				{
					continue;
				}
				pos2.y *= num3;
				lastLine.AddLine2(pos, pos2, Vector3.up, Vector3.right, 1f, Color.white);
				num4 = DrawPointCount;
			}
			Vector3 vector2 = new Vector3(0f, 0f, -1f);
			if (globalVal.lastBestHeight.index == DrawPointCount)
			{
				trackBeshHeight2.position = lastLine.GetPos() + pos + vector2;
			}
			else if (trackInfo.type == 6)
			{
				trackDead2.position = lastLine.GetPos() + pos + vector2;
			}
		}
		lastLine.SetLineMesh();
	}

	public void CompleteDrawImmediate()
	{
		if (CanDraw)
		{
			base.transform.Find("gameover_page/gameover_tap").localPosition = new Vector3(-1000f, 0f, 0f);
			CanDraw = false;
			int num = ((globalVal.CurrentTrack.Count <= globalVal.LastTrack.Count) ? globalVal.LastTrack.Count : globalVal.CurrentTrack.Count);
			Vector3 vector = ((!(globalVal.curBestHeight.pos.y > globalVal.lastBestHeight.pos.y)) ? globalVal.lastBestHeight.pos : globalVal.curBestHeight.pos);
			float num2 = DrawDis / (float)num;
			float num3 = DrawHei / vector.y;
			int drawPointCount = DrawPointCount;
			int num4 = 0;
			for (; DrawPointCount < num; DrawPointCount++)
			{
				if (DrawPointCount >= globalVal.CurrentTrack.Count)
				{
					continue;
				}
				TrackInfo trackInfo = globalVal.CurrentTrack[DrawPointCount] as TrackInfo;
				Vector3 pos = trackInfo.pos;
				pos.x = (float)DrawPointCount * num2;
				pos.y *= num3;
				if (DrawPointCount > 0)
				{
					TrackInfo trackInfo2 = globalVal.CurrentTrack[m_lastPointId] as TrackInfo;
					Vector3 pos2 = trackInfo2.pos;
					pos2.x = (float)m_lastPointId * num2;
					pos2.y *= num3;
					if (Mathf.Abs(pos2.x - pos.x) < 0.5f && trackInfo.type == 2)
					{
						continue;
					}
					curLine.AddLine2(pos, pos2, Vector3.up, Vector3.right, 1f, Color.white);
					m_lastPointId = DrawPointCount;
				}
				Vector3 vector2 = new Vector3(0f, 0f, -1f);
				if (globalVal.curBestHeight.index == DrawPointCount)
				{
					trackBeshHeight.position = curLine.GetPos() + pos + vector2;
				}
				if (trackInfo.type == 1)
				{
					trackStart.position = curLine.GetPos() + pos + vector2;
				}
				else if (trackInfo.type == 4)
				{
					trackHuaxiang[trackHuaxiang_count].position = curLine.GetPos() + pos + vector2;
					trackHuaxiang_count++;
				}
				else if (trackInfo.type == 3)
				{
					trackHuojian[trackHuojian_count].position = curLine.GetPos() + pos + vector2;
					trackHuojian_count++;
				}
				else if (trackInfo.type == 6)
				{
					trackDead.position = curLine.GetPos() + pos + vector2;
				}
			}
			curLine.SetLineMesh();
		}
		NewBest();
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			StatisticsData.data.SaveDailyData(StatisticsData.data.GetSystemDay());
		}
		if (globalVal.UIState != UILayer.GAMEOVER && pause)
		{
			OnPauseDown();
		}
	}

	public void InitGameUI()
	{
		MonoBehaviour.print("InitGameUI");
		InitTitle();
		InitPowerSlider();
		globalVal.UIState = UILayer.INGAME;
		curLine.ClearMesh();
		lastLine.ClearMesh();
	}

	public void InitAllMenu()
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

	private void InitTitle()
	{
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("title");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetEndPos(0f, 0f, 0f);
		uIMoveControl.UpToDown(0.1f, transform);
		TUIMeshText tUIMeshText = transform.Find("label_glod").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
		tUIMeshText.text = string.Empty + globalVal.g_gold;
		Transform transform2 = transform.Find("list_ground");
		foreach (Transform item in transform2)
		{
			Object.Destroy(item.gameObject);
		}
		GameObject original = transform.Find("item_state").gameObject;
		int num = 0;
		for (int i = 0; i < globalVal.g_item_once_count.Count; i++)
		{
			if ((int)globalVal.g_item_once_count[i] > 0)
			{
				GameObject gameObject = Object.Instantiate(original) as GameObject;
				gameObject.transform.parent = transform2;
				gameObject.transform.localPosition = new Vector3(0 + num * -22, 0f, 0f);
				TUIMeshSprite tUIMeshSprite = gameObject.GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
				ItemOnceAttribute itemOnceAttribute = ItemManagerClass.body.itemOnceArray[i] as ItemOnceAttribute;
				tUIMeshSprite.frameName = itemOnceAttribute.smallpic;
				num++;
			}
		}
	}

	public void InitPowerSlider()
	{
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("PowerSlider");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetEndPos(211f, -31f, 0f);
		uIMoveControl.RightToLeft(0f, transform);
	}

	public void InitControl()
	{
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("control");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetEndPos(0f, 0f, 0f);
		uIMoveControl.DownToUp(0f, transform);
		foreach (Transform item in transform)
		{
			switch (item.name)
			{
			case "control_Slider":
			{
				Vector3 localPosition = item.localPosition;
				localPosition.y = -200f;
				item.localPosition = localPosition;
				break;
			}
			case "left_item":
			{
				ItemAttribute attributeByName2 = ItemManagerClass.body.GetAttributeByName("equip_rocket");
				int num2 = (int)globalVal.g_itemlevel[attributeByName2.index];
				if (num2 == 0)
				{
					item.localPosition = new Vector3(-1000f, 0f, 0f);
					transform.Find("control_left").localPosition = new Vector3(-1000f, 0f, 0f);
					transform.Find("left_item_count").localPosition = new Vector3(-1000f, 0f, 0f);
				}
				else
				{
					item.localPosition = new Vector3(-200f, -122.2f, -1f);
					transform.Find("control_left").localPosition = new Vector3(-200f, -122.2f, 0f);
					transform.Find("left_item_count").localPosition = new Vector3(-180.6149f, -148.574f, -2f);
				}
				transform.Find("circle").localPosition = new Vector3(-200f, -122.3f, 0f);
				DrawCircleClass.circle.InitCircleProgress(32f, 18f, Color.white);
				DrawCircleClass.circle.SetProgress(100);
				ItemSubAttr itemSubAttr2 = attributeByName2.level[num2] as ItemSubAttr;
				string picname2 = itemSubAttr2.picname;
				TUIMeshSprite tUIMeshSprite2 = item.GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
				tUIMeshSprite2.frameName = picname2 + "_a";
				break;
			}
			case "right_item":
			{
				ItemAttribute attributeByName = ItemManagerClass.body.GetAttributeByName("equip_glider");
				int num = (int)globalVal.g_itemlevel[attributeByName.index];
				if (num == 0)
				{
					item.localPosition = new Vector3(-1000f, 0f, 0f);
					transform.Find("control_right").localPosition = new Vector3(-1000f, 0f, 0f);
					transform.Find("right_item_count").localPosition = new Vector3(-1000f, 0f, 0f);
				}
				else
				{
					item.localPosition = new Vector3(200f, -122.2f, -1f);
					transform.Find("control_right").localPosition = new Vector3(200f, -122.2f, 0f);
					transform.Find("right_item_count").localPosition = new Vector3(222.3494f, -146.4656f, -2f);
				}
				ItemSubAttr itemSubAttr = attributeByName.level[num] as ItemSubAttr;
				string picname = itemSubAttr.picname;
				TUIMeshSprite tUIMeshSprite = item.GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
				tUIMeshSprite.frameName = picname + "_a";
				break;
			}
			}
		}
	}

	public void RemoveControl()
	{
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("control");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetEndPos(0f, 0f, 0f);
		uIMoveControl.DownToUp_back(0.1f, transform);
	}

	public void HidePowerSlider()
	{
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("PowerSlider");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.RightToLeft_back(0.1f, transform);
	}

	public void InitGameOver()
	{
		globalVal.SetOpenClickShow(true);
		InitAllMenu();
		StatisticsData.data.d_UITapCount[4]++;
		StatisticsData.data.d_UiLastTap = 4;
		StatisticsData.data.SaveDailyData(StatisticsData.data.GetSystemDay());
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("gameover_page");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetEndPos(0f, 0f, 0f);
		uIMoveControl.RightToLeft(0.1f, transform);
		uIMoveControl.SetCallBack(base.transform, "GameOverCallBack");
		curLine.ClearMesh();
		lastLine.ClearMesh();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			switch (child.name)
			{
			case "label_speed":
			{
				TUIMeshText tUIMeshText = child.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				tUIMeshText.text = string.Empty + (int)PlayerScriptClass.playerInfo.speed + " m/s";
				break;
			}
			case "label_height":
			{
				TUIMeshText tUIMeshText4 = child.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				tUIMeshText4.text = string.Empty + (int)PlayerScriptClass.playerInfo.height + " m";
				break;
			}
			case "label_distance":
			{
				TUIMeshText tUIMeshText3 = child.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				tUIMeshText3.text = string.Empty + (int)PlayerScriptClass.playerInfo.distance + " m";
				break;
			}
			case "label_gold":
			{
				TUIMeshText tUIMeshText2 = child.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				float distance = PlayerScriptClass.playerInfo.distance;
				float num = Mathf.Log(x1 / x2, y1 / y2);
				float num2 = x1 / Mathf.Pow(y1, num);
				float num3 = Mathf.Pow(distance / num2, 1f / num);
				int num4 = PlayerScriptClass.playerInfo.gold + (int)num3;
				tUIMeshText2.text = string.Empty + num4;
				globalVal.g_gold += num4;
				globalVal.g_totalGold += (ulong)num4;
				break;
			}
			case "listground":
			{
				Transform transform2 = child.Find("track_start");
				transform2.transform.localPosition = new Vector3(-1000f, 0f, -1f);
				trackStart = transform2.transform;
				Transform transform3 = child.Find("track_huaxiang");
				transform3.transform.localPosition = new Vector3(-1000f, 0f, -1f);
				trackHuaxiang[0] = transform3.transform;
				Transform transform4 = child.Find("track_huaxiang1");
				transform4.transform.localPosition = new Vector3(-1000f, 0f, -1f);
				trackHuaxiang[1] = transform4.transform;
				Transform transform5 = child.Find("track_curbestheight");
				transform5.transform.localPosition = new Vector3(-1000f, 0f, -1f);
				trackBeshHeight = transform5.transform;
				Transform transform6 = child.Find("track_lastbestheight");
				transform6.transform.localPosition = new Vector3(-1000f, 0f, -1f);
				trackBeshHeight2 = transform6.transform;
				Transform transform7 = child.Find("track_dead");
				transform7.transform.localPosition = new Vector3(-1000f, 0f, -1f);
				trackDead = transform7.transform;
				Transform transform8 = child.Find("track_dead2");
				transform8.transform.localPosition = new Vector3(-1000f, 0f, -1f);
				trackDead2 = transform8.transform;
				for (int j = 0; j < trackHuojian.Length; j++)
				{
					Transform transform9 = child.Find("track_huojian" + j);
					transform9.transform.localPosition = new Vector3(-1000f, 0f, -1f);
					trackHuojian[j] = transform9.transform;
				}
				break;
			}
			case "gameover_menu":
			case "gameover_retry":
			case "gameover_thestash":
			case "newbest":
				child.localPosition = new Vector3(-1000f, 0f, 0f);
				break;
			case "gameover_tap":
				child.localPosition = new Vector3(5f, 0f, -4.1f);
				break;
			}
		}
		TUIMeshText tUIMeshText5 = transform.Find("title/label_glod").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
		tUIMeshText5.text = string.Empty + globalVal.g_gold;
	}

	private void GameOverCallBack()
	{
		CompleteLastLineImmediate();
		slingshot.state = IdleState.GAMEOVERCALLBACK;
		ItemManagerClass.body.ResetItems_new();
		m_lastPointId = 0;
		CanDraw = true;
		trackHuojian_count = 0;
		trackHuaxiang_count = 0;
		drawpoint_curtime = 0f;
		drawpoint_showtime = 0f;
		DrawPointCount = 0;
		buttonPopKey = false;
		isBestDis = false;
		globalVal.g_best_playcount++;
		StatisticsData.data.d_playCount++;
		if (globalVal.g_best_height < PlayerScriptClass.playerInfo.height)
		{
			globalVal.g_best_height = PlayerScriptClass.playerInfo.height;
		}
		if (globalVal.g_best_speed < PlayerScriptClass.playerInfo.speed)
		{
			globalVal.g_best_speed = PlayerScriptClass.playerInfo.speed;
		}
		if (globalVal.g_best_dis < PlayerScriptClass.playerInfo.distance)
		{
			globalVal.g_best_dis = PlayerScriptClass.playerInfo.distance;
			isBestDis = true;
		}
		globalVal.g_best_zombiebreak += PlayerScriptClass.playerInfo.breakZombiesCount;
		globalVal.g_best_totaldis += PlayerScriptClass.playerInfo.distance * 0.001f;
		StatisticsData.data.SaveDailyData(StatisticsData.data.GetSystemDay());
		globalVal.SaveFile("saveData.txt");
	}

	private void TwitterDialog()
	{
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("twitter");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetPos(-229f, 100f, -9f);
		TUIButtonClick tUIButtonClick = transform.GetComponent(typeof(TUIButtonClick)) as TUIButtonClick;
		string text = string.Empty + Utils.GetIOSYear() + Utils.GetIOSMonth() + Utils.GetIOSDay();
		if (globalVal.twitterDay != text && !globalVal.twitterKey)
		{
			globalVal.twitterKey = true;
			if (Utils.ShowMessageBox2(string.Empty, "Get 1000 gold every day you post your score on Twitter!", "GET IT NOW", "CANCEL") == 0)
			{
				if (TweetPlugin.CanTweet())
				{
					globalVal.g_gold += 1000;
					globalVal.g_totalGold += 1000uL;
					StatisticsData.data.d_gameGold += 1000;
					globalVal.SaveTwitterDay();
					globalVal.SaveFile("saveData.txt");
					TweetPlugin.SendMsg("I just flew " + (int)globalVal.g_best_dis + " m in MiniGlider! Can you beat that? That??s a fanny game.Get the it at http://bit.ly/Mz3kMO!");
					switch (Utils.ShowMessageBox2(string.Empty, "Woohoo! You just got 1000 gold! Follow us on Twitter for the latest news and free games!", "FOLLOW", "CANCEL"))
					{
					case 0:
						TweetPlugin.FollowUser("@TrinitiGames");
						break;
					}
				}
				else
				{
					Utils.ShowMessageBox1(string.Empty, "Please log in to Twitter to post your score.", "CANCEL");
				}
			}
		}
		if (globalVal.twitterDay != text && globalVal.twitterKey)
		{
			tUIButtonClick.SetDisabled(false);
		}
		else
		{
			tUIButtonClick.SetDisabled(true);
		}
	}

	private void NewBest()
	{
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		if (isBestDis)
		{
			transform = base.transform.Find("gameover_page");
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				switch (child.name)
				{
				case "newbest":
					uIMoveControl = child.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
					uIMoveControl.SetTargetPos(0f, 17.44024f, -2.55f);
					uIMoveControl.ScaleRotate_newbest(0.1f, child);
					uIMoveControl.SetCallBack(base.transform, "CallbackInitTask");
					EffectManagerClass.body.PlayEffect("effect_jiesuan", child);
					break;
				}
			}
			audios.PlayAudio("UInewbest");
		}
		else
		{
			CallbackInitTask();
		}
		GameCenterPlugin.SubmitScore("com.trinitigame.miniglider.l1", (int)globalVal.g_best_dis);
	}

	private void GameOverButton()
	{
		if (buttonPopKey)
		{
			return;
		}
		buttonPopKey = true;
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("gameover_page");
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			switch (child.name)
			{
			case "gameover_menu":
				uIMoveControl = child.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(-133f, -93f, -4.1f);
				uIMoveControl.DownToUp(0.1f, child);
				break;
			case "gameover_retry":
				uIMoveControl = child.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(150f, -95f, -4.1f);
				uIMoveControl.DownToUp(0.1f, child);
				break;
			case "gameover_thestash":
				uIMoveControl = child.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(5f, -95f, -4.1f);
				uIMoveControl.DownToUp(0.1f, child);
				uIMoveControl.SetCallBack(base.transform, "CallbackThestash");
				if (globalVal.g_gold >= ItemManagerClass.body.GetMinPrice())
				{
					GameObject gameObject = EffectManagerClass.body.PlayEffect_gantanhao();
					gameObject.transform.parent = child;
					gameObject.transform.localPosition = new Vector3(48.41553f, 7.317734f, -0.1f);
					stashFlash stashFlash2 = child.GetComponent(typeof(stashFlash)) as stashFlash;
					stashFlash2.BeginFlash();
				}
				break;
			}
		}
	}

	public void CallbackThestash()
	{
		if (!globalVal.g_help_key[3])
		{
			InitHelp(HelpState.BUYFIRST);
		}
		TwitterDialog();
	}

	public void CallbackInitTask()
	{
		GameObject gameObject = GameObject.Find("TUI/TUIControl/task_page");
		TaskManager taskManager = gameObject.GetComponent(typeof(TaskManager)) as TaskManager;
		if (slingshot.state == IdleState.GAMEOVER || slingshot.state == IdleState.GAMEOVERCALLBACK)
		{
			taskManager.InitTaskList();
		}
	}

	public void CheckTaskCompleteState()
	{
		GameObject gameObject = GameObject.Find("TUI/TUIControl/task_page");
		TaskManager taskManager = gameObject.GetComponent(typeof(TaskManager)) as TaskManager;
		for (int i = 0; i < task_complete_state.Length; i++)
		{
			task_complete_state[i] = false;
		}
		bool flag = false;
		for (int j = 0; j < ItemManagerClass.body.taskListArray.Count; j++)
		{
			ArrayList arrayList = ItemManagerClass.body.taskListArray[j] as ArrayList;
			if (globalVal.cur_task_id[j] > arrayList.Count - 1)
			{
				continue;
			}
			string key = arrayList[globalVal.cur_task_id[j]] as string;
			taskInfo taskInfo2 = ItemManagerClass.body.hashTask[key] as taskInfo;
			switch (taskInfo2.id.Substring(0, 1))
			{
			case "A":
				if (PlayerScriptClass.playerInfo.distance >= taskInfo2.value)
				{
					task_complete_state[j] = true;
				}
				break;
			case "B":
				if (PlayerScriptClass.playerInfo.height >= taskInfo2.value)
				{
					task_complete_state[j] = true;
				}
				break;
			case "G":
				if (PlayerScriptClass.playerInfo.speed >= taskInfo2.value)
				{
					task_complete_state[j] = true;
				}
				MonoBehaviour.print("speed " + PlayerScriptClass.playerInfo.speed);
				break;
			case "C":
				if ((float)PlayerScriptClass.playerInfo.goldCount >= taskInfo2.value)
				{
					task_complete_state[j] = true;
				}
				break;
			case "D":
				if ((float)PlayerScriptClass.playerInfo.heightUp10Distance >= taskInfo2.value)
				{
					task_complete_state[j] = true;
				}
				break;
			case "E":
				if (PlayerScriptClass.playerInfo.distance - (float)PlayerScriptClass.playerInfo.firstTouchFloorDistance >= taskInfo2.value)
				{
					task_complete_state[j] = true;
				}
				break;
			case "F":
				if ((float)globalVal.g_best_zombiebreak >= taskInfo2.value)
				{
					task_complete_state[j] = true;
				}
				break;
			}
			if (task_complete_state[j])
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (slingshot.state == IdleState.GAMEOVER || slingshot.state == IdleState.GAMEOVERCALLBACK)
			{
				taskManager.PopTaskList();
			}
		}
		else
		{
			GameOverButton();
		}
	}

	public void CompleteTask()
	{
		GameObject gameObject = GameObject.Find("TUI/TUIControl/task_page");
		TaskManager taskManager = gameObject.GetComponent(typeof(TaskManager)) as TaskManager;
		for (int i = 0; i < task_complete_state.Length; i++)
		{
			if (task_complete_state[i])
			{
				task_complete_state[i] = false;
				task_show_time = task_cur_time + 1f;
				timeUpdateKey = true;
				taskManager.TaskComplete(i + 1);
				return;
			}
		}
		GameOverButton();
	}

	public void RemoveGameOver()
	{
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("gameover_page");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetEndPos(0f, 0f, 0f);
		uIMoveControl.RightToLeft_back(0.1f, transform);
		transform = base.transform.Find("control");
		transform.localPosition = new Vector3(-1000f, -1000f, 0f);
		transform = base.transform.Find("task_page");
		transform.localPosition = new Vector3(-1000f, -1000f, 0f);
		transform = base.transform.Find("twitter");
		transform.localPosition = new Vector3(-1000f, -1000f, 0f);
	}

	public void OnPauseDown()
	{
		if (uitype != 0)
		{
			return;
		}
		Time.timeScale = 0f;
		globalVal.UIState = UILayer.PAUSE;
		globalVal.SetOpenClickShow(true);
		ui_event.UpdateMusicState_pause();
		ui_event.UpdateSoundState_pause();
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			transform = base.transform.GetChild(i);
			switch (base.transform.GetChild(i).name)
			{
			case "title":
				uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.UpToDown_back(0f, transform);
				break;
			case "pause_page":
			{
				uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetEndPos(0f, 0f, 0f);
				uIMoveControl.RightToLeft(0f, transform);
				TaskManager taskManager = transform.Find("task_page").GetComponent(typeof(TaskManager)) as TaskManager;
				taskManager.InitTaskList_pause();
				break;
			}
			case "control":
			case "PowerSlider":
				uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetPos(-1000f, -1000f, 0f);
				break;
			}
		}
	}

	private void RemoveTitle()
	{
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("title");
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.UpToDown_back(0f, transform);
	}

	public void OnResumeDown()
	{
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			transform = base.transform.GetChild(i);
			switch (base.transform.GetChild(i).name)
			{
			case "title":
				if (slingshot.state != 0)
				{
					InitTitle();
				}
				break;
			case "task_page":
				uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetPos(-1000f, -1000f, 0f);
				break;
			case "control":
				if (slingshot.state == IdleState.SHOT_FLYING)
				{
					InitControl();
				}
				if (slingshot.playerAnim != 0)
				{
					transform.Find("control_Slider").localPosition = new Vector3(0f, -105f, 0f);
				}
				break;
			case "PowerSlider":
				if (slingshot.state == IdleState.STAND)
				{
					InitPowerSlider();
				}
				break;
			}
		}
		globalVal.UIState = UILayer.INGAME;
	}

	public void RemovePause()
	{
		globalVal.UIState = UILayer.INGAME;
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			transform = base.transform.GetChild(i);
			switch (base.transform.GetChild(i).name)
			{
			case "pause_page":
				uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.RightToLeft_back(0.1f, transform);
				break;
			}
		}
	}

	public void InitHelp(HelpState uistate)
	{
		int num = (int)(uistate - 1);
		if (!globalVal.g_help_key[num])
		{
			stopHelpKey = false;
		}
		if (stopHelpKey)
		{
			return;
		}
		uitype = uistate;
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("help");
		transform.localPosition = Vector3.zero;
		foreach (Transform item in transform)
		{
			switch (item.name)
			{
			case "finger":
				uIMoveControl = item.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				switch (uistate)
				{
				case HelpState.TAP:
					uIMoveControl.SetPos(-14f, 10f, -2.1f);
					break;
				case HelpState.HUOJIAN:
					uIMoveControl.SetPos(-174.8f, -145f, -2.2f);
					break;
				case HelpState.HUAXIANG:
					uIMoveControl.SetPos(222.8f, -145f, -2.2f);
					break;
				case HelpState.BUYFIRST:
					uIMoveControl.SetPos(38f, -114f, -10f);
					break;
				}
				uIMoveControl.OnTimeCallBack(0.2f, base.transform, "FingerTapDown");
				break;
			case "help_bg":
				if (uistate == HelpState.BUYFIRST)
				{
					item.localPosition = new Vector3(0f, 0f, -9.8f);
				}
				else
				{
					item.localPosition = new Vector3(0f, 0f, -2.2f);
				}
				break;
			case "help_click":
				item.localPosition = new Vector3(-1000f, -1000f, -2f);
				break;
			case "label_text":
			{
				uIMoveControl = item.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				TUIMeshText tUIMeshText = item.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				switch (uistate)
				{
				case HelpState.TAP:
				{
					uIMoveControl.SetPos(-14f, 114f, -2.3f);
					string text3 = "Pull back, aim, and let go to fling! The bar on the right shows \nyour flinging force; try to time your release just right \nfor the longest flight!";
					tUIMeshText.text = text3.Replace("\\n", "\n");
					break;
				}
				case HelpState.HUOJIAN:
				{
					uIMoveControl.SetPos(-14f, 10f, -2.3f);
					string text2 = "Tap the button in the lower left to rocket boost. You??ll start \nto blink when you??re about to run out of time.";
					tUIMeshText.text = text2.Replace("\\n", "\n");
					break;
				}
				case HelpState.HUAXIANG:
				{
					uIMoveControl.SetPos(-14f, 10f, -2.3f);
					string text = "Once you reach at least 100 M of altitude, tap the button \nin the lower left to use a hang glider.";
					tUIMeshText.text = text.Replace("\\n", "\n");
					break;
				}
				case HelpState.BUYFIRST:
				{
					uIMoveControl.SetPos(-14f, 10f, -2.3f);
					string empty = string.Empty;
					tUIMeshText.text = empty.Replace("\\n", "\n");
					break;
				}
				}
				break;
			}
			default:
				item.localPosition = new Vector3(-1000f, -1000f, 0f);
				break;
			}
		}
	}

	public void FingerTapDown()
	{
		if (!stopHelpKey)
		{
			Transform transform = null;
			UIMoveControl uIMoveControl = null;
			transform = base.transform.Find("help/finger");
			TUIButtonPush tUIButtonPush = transform.GetComponent(typeof(TUIButtonPush)) as TUIButtonPush;
			tUIButtonPush.SetPressed(true);
			uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
			if (uitype == HelpState.TAP)
			{
				uIMoveControl.SetTargetPos(-112f, -53f, -2.1f);
				uIMoveControl.CurToTargetMove(0.5f, transform, 0.5f);
				uIMoveControl.SetCallBack(base.transform, "FingerTapUp");
			}
			else if (uitype == HelpState.BUYFIRST)
			{
				uIMoveControl.OnTimeCallBack(1f, base.transform, "FingerTapUp");
			}
			else if (uitype == HelpState.HUOJIAN || uitype == HelpState.HUAXIANG)
			{
				uIMoveControl.OnTimeCallBack(1f, base.transform, "FingerTapUp");
			}
			transform = base.transform.Find("help/tap");
			uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
			if (uitype == HelpState.TAP)
			{
				uIMoveControl.SetEndPos(-41f, 32f, -2.1f);
			}
			else if (uitype == HelpState.HUOJIAN)
			{
				uIMoveControl.SetEndPos(-197f, -124f, -2.2f);
			}
			else if (uitype == HelpState.HUAXIANG)
			{
				uIMoveControl.SetEndPos(197f, -124f, -2.2f);
			}
			else if (uitype == HelpState.BUYFIRST)
			{
				uIMoveControl.SetEndPos(14f, -95f, -9.9f);
			}
			uIMoveControl.ScaleNoMove(0f, transform);
			uIMoveControl.SetCallBack(base.transform, "RemoveHelpTap");
		}
	}

	public void RemoveHelpTap()
	{
		if (!stopHelpKey)
		{
			Transform transform = null;
			transform = base.transform.Find("help/tap");
			transform.localPosition = new Vector3(-1000f, -1000f, 0f);
		}
	}

	public void FingerTapUp()
	{
		if (!stopHelpKey)
		{
			Transform transform = null;
			UIMoveControl uIMoveControl = null;
			transform = base.transform.Find("help/finger");
			TUIButtonPush tUIButtonPush = transform.GetComponent(typeof(TUIButtonPush)) as TUIButtonPush;
			tUIButtonPush.SetPressed(false);
			uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
			uIMoveControl.OnTimeCallBack(0.5f, base.transform, "InitHelp", uitype);
		}
	}

	public void StopHelp(HelpState uistate)
	{
		int num = (int)(uistate - 1);
		if (!globalVal.g_help_key[num])
		{
			Transform transform = null;
			transform = base.transform.Find("help");
			transform.localPosition = new Vector3(-1000f, -1000f, 0f);
			stopHelpKey = true;
			globalVal.g_help_key[num] = true;
			if (uitype == HelpState.TAP)
			{
				uitype = HelpState.NONE;
			}
			else
			{
				StartCoroutine(InitShake(1f));
			}
		}
	}

	private IEnumerator InitShake(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		HelpShake();
	}

	private void HelpShake()
	{
		isUiShaking = true;
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform.Find("help");
		transform.localPosition = Vector3.zero;
		Time.timeScale = 0f;
		foreach (Transform item in transform)
		{
			switch (item.name)
			{
			case "shake":
				HelpReshake();
				break;
			case "label_text":
			{
				uIMoveControl = item.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				TUIMeshText tUIMeshText = item.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				uIMoveControl.SetPos(-14f, 50f, -2.3f);
				string text = "Tilt to glide up and down! You??ll start to blink \nwhen you??re about to run out of time.";
				tUIMeshText.text = text.Replace("\\n", "\n");
				break;
			}
			case "help_bg":
				item.localPosition = new Vector3(0f, 0f, -2.2f);
				break;
			case "help_click":
				item.localPosition = new Vector3(0f, 0f, -2f);
				break;
			default:
				item.localPosition = new Vector3(-1000f, -1000f, 0f);
				break;
			}
		}
	}

	public void HelpReshake()
	{
		if (isUiShaking)
		{
			Transform transform = null;
			UIMoveControl uIMoveControl = null;
			transform = base.transform.Find("help/shake");
			uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
			uIMoveControl.SetEndPos(0f, -61f, 0f);
			uIMoveControl.Rotate(0f, transform);
			uIMoveControl.SetCallBack(base.transform, "HelpReshake");
		}
	}

	public void StopShake()
	{
		Transform transform = null;
		transform = base.transform.Find("help");
		transform.localPosition = new Vector3(-1000f, -1000f, 0f);
		Time.timeScale = 1f;
		uitype = HelpState.NONE;
		isUiShaking = false;
		transform = base.transform.Find("help/shake");
		transform.localPosition = new Vector3(-1000f, -1000f, 0f);
	}

	private void RemoveAllList(Transform root)
	{
		foreach (Transform item in root)
		{
			Object.Destroy(item.gameObject);
		}
	}
}
