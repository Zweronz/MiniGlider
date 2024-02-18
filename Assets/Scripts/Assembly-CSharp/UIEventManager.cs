using UnityEngine;

public class UIEventManager : MonoBehaviour, TUIHandler
{
	private TUI m_tui;

	private TAudioController audios;

	private int purchasetype = -1;

	private bool purchaseKey;

	private bool restoreKey;

	private float purchaseOutTime;

	public void Start()
	{
		Application.targetFrameRate = 240;
		m_tui = TUI.Instance("TUI");
		m_tui.SetHandler(this);
		GameObject gameObject = GameObject.Find("TAudioController");
		if (gameObject == null)
		{
			gameObject = Object.Instantiate(Resources.Load("TAudioController")) as GameObject;
			gameObject.name = "TAudioController";
			Object.DontDestroyOnLoad(gameObject);
		}
		audios = gameObject.GetComponent(typeof(TAudioController)) as TAudioController;
	}

	public void Update()
	{
		TUIInput[] input = TUIInputManager.GetInput();
		for (int i = 0; i < input.Length; i++)
		{
			m_tui.HandleInput(input[i]);
		}
		if (!purchaseKey)
		{
			return;
		}
		purchaseOutTime += Time.deltaTime;
		if (purchaseOutTime > 30f)
		{
			HideIndicator();
			purchasetype = -1;
			purchaseKey = false;
			restoreKey = false;
			if (Utils.ShowMessageBox1(string.Empty, "Connection timed out!", "OK") == 0)
			{
				return;
			}
		}
		if (restoreKey)
		{
			int num = -1000;
			switch (IAPPlugin.DoRestoreStatus())
			{
			case 0:
				break;
			case 1:
				restoreKey = false;
				purchaseKey = false;
				HideIndicator();
				Restore();
				break;
			case -1:
				restoreKey = false;
				purchaseKey = false;
				HideIndicator();
				break;
			}
			return;
		}
		int num2 = -1000;
		num2 = IAPPlugin.GetPurchaseStatus();
		MonoBehaviour.print(num2);
		switch (num2)
		{
		case -3:
			HideIndicator();
			purchasetype = -1;
			purchaseKey = false;
			Utils.ShowMessageBox1(string.Empty, "Sorry, the connection failed!", "OK");
			break;
		case -2:
			HideIndicator();
			purchasetype = -1;
			purchaseKey = false;
			break;
		case -1:
			HideIndicator();
			purchasetype = -1;
			purchaseKey = false;
			break;
		case 0:
			break;
		case 1:
		{
			HideIndicator();
			globalVal.PurchaseFunction(purchasetype);
			purchasetype = -1;
			purchaseKey = false;
			menu menu2 = null;
			GameObject gameObject = GameObject.Find("TUI/TUIControl");
			menu2 = gameObject.GetComponent(typeof(menu)) as menu;
			menu2.InitTbank();
			break;
		}
		}
	}

	private void Restore()
	{
		string[] array = IAPPlugin.DoRestoreGetProductId();
		foreach (string text in array)
		{
			if (text == "com.trinitigame.miniglider.199cents2")
			{
				purchasetype = 3;
				globalVal.PurchaseFunction(purchasetype);
				Utils.ShowMessageBox1(string.Empty, "Item restored.", "OK");
				return;
			}
		}
		Utils.ShowMessageBox1(string.Empty, "You can't restore unpurchased items.", "OK");
	}

	private void TwitterDialog()
	{
		if (Utils.ShowMessageBox2(string.Empty, "Get 1000 gold every day you post your score on Twitter!", "GET IT NOW", "CANCEL") != 0)
		{
			return;
		}
		if (TweetPlugin.CanTweet())
		{
			globalVal.g_gold += 1000;
			globalVal.g_totalGold += 1000uL;
			StatisticsData.data.d_gameGold += 1000;
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

	public void HandleEvent(TUIControl control, int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3 && control.GetType() == typeof(TUIButtonClick))
		{
			switch (control.name)
			{
			case "twitter":
				TwitterDialog();
				break;
			case "Button":
				break;
			case "options":
			{
				globalVal.UIState = UILayer.OPTIONS;
				GameObject gameObject23 = GameObject.Find("TUI/TUIControl/fade_bg");
				TUIFade tUIFade10 = gameObject23.GetComponent(typeof(TUIFade)) as TUIFade;
				tUIFade10.FadeOut("scene_ui_option");
				audios.PlayAudio("UIclick");
				break;
			}
			case "ranking":
				GameCenterPlugin.OpenLeaderboard("com.trinitigame.miniglider.l1");
				audios.PlayAudio("UIclick");
				break;
			case "trophy":
				GameCenterPlugin.OpenAchievement();
				audios.PlayAudio("UIclick");
				break;
			case "option_credits":
			{
				globalVal.UIState = UILayer.CREDITS;
				GameObject gameObject19 = GameObject.Find("TUI/TUIControl/fade_bg");
				TUIFade tUIFade7 = gameObject19.GetComponent(typeof(TUIFade)) as TUIFade;
				tUIFade7.FadeOut("scene_ui_credits");
				audios.PlayAudio("UIclick");
				break;
			}
			case "option_howto":
			{
				globalVal.UIState = UILayer.HOWTO;
				GameObject gameObject17 = GameObject.Find("TUI/TUIControl/fade_bg");
				TUIFade tUIFade6 = gameObject17.GetComponent(typeof(TUIFade)) as TUIFade;
				tUIFade6.FadeOut("scene_ui_howto");
				audios.PlayAudio("UIclick");
				break;
			}
			case "option_review":
				Application.OpenURL("http://itunes.apple.com/us/app/miniglider/id518241576?ls=1&mt=8");
				audios.PlayAudio("UIclick");
				break;
			case "option_support":
				Application.OpenURL("http://www.trinitigame.com/support?game=minig&version=1.0");
				audios.PlayAudio("UIclick");
				break;
			case "playNow":
				globalVal.loadingSceneName = "scene_game";
				Application.LoadLevel("scene_ui_loading");
				audios.PlayAudio("UIclick");
				globalVal.isskipkey = false;
				break;
			case "theStash":
			{
				globalVal.UIState = UILayer.THESTASH;
				GameObject gameObject4 = GameObject.Find("TUI/TUIControl/fade_bg");
				TUIFade tUIFade2 = gameObject4.GetComponent(typeof(TUIFade)) as TUIFade;
				tUIFade2.FadeOut("scene_ui_thestash");
				audios.PlayAudio("UIclick");
				break;
			}
			case "gameover_menu":
			{
				GameObject gameObject22 = GameObject.Find("TUI/TUIControl");
				gameUI gameUI11 = gameObject22.GetComponent(typeof(gameUI)) as gameUI;
				if (gameUI11.uitype != HelpState.BUYFIRST)
				{
					globalVal.UIState = UILayer.MENU;
					audios.StopAllAudio();
					audios.ReplayMusic();
					globalVal.loadingSceneName = "scene_ui_menu";
					Application.LoadLevel("scene_ui_loading");
					audios.PlayAudio("UIclick");
					Time.timeScale = 1f;
				}
				break;
			}
			case "gameover_retry":
			{
				GameObject gameObject18 = GameObject.Find("TUI/TUIControl");
				gameUI gameUI10 = gameObject18.GetComponent(typeof(gameUI)) as gameUI;
				if (gameUI10.uitype != HelpState.BUYFIRST)
				{
					UIMoveControl uIMoveControl = gameObject18.transform.Find("task_page").GetComponent(typeof(UIMoveControl)) as UIMoveControl;
					uIMoveControl.StopMove();
					globalVal.SetOpenClickShow(false);
					gameUI10.RemoveGameOver();
					gameUI10.OnResumeDown();
					Time.timeScale = 1f;
					EffectManagerClass.body.RemoveEffect_jiesuan();
					gameObject18 = GameObject.Find("/Slingshot");
					if ((bool)gameObject18)
					{
						SlingshotScript slingshotScript2 = gameObject18.GetComponent(typeof(SlingshotScript)) as SlingshotScript;
						slingshotScript2.ReStart();
					}
					audios.PlayAudio("UIclick");
				}
				break;
			}
			case "gameover_thestash":
			{
				GameObject gameObject11 = GameObject.Find("TUI/TUIControl");
				gameUI gameUI8 = gameObject11.GetComponent(typeof(gameUI)) as gameUI;
				if (gameUI8.uitype == HelpState.BUYFIRST)
				{
					globalVal.g_help_key[3] = true;
				}
				stashFlash stashFlash2 = control.GetComponent(typeof(stashFlash)) as stashFlash;
				stashFlash2.StopFlash();
				globalVal.UIState = UILayer.THESTASH;
				globalVal.loadingSceneName = "scene_ui_thestash";
				Application.LoadLevel("scene_ui_loading");
				audios.PlayAudio("UIclick");
				break;
			}
			case "gameover_tap":
			{
				GameObject gameObject10 = GameObject.Find("TUI/TUIControl");
				gameUI gameUI7 = gameObject10.GetComponent(typeof(gameUI)) as gameUI;
				gameUI7.CompleteDrawImmediate();
				control.transform.localPosition = new Vector3(-1000f, 0f, 0f);
				break;
			}
			case "pause_music":
				globalVal.music = !globalVal.music;
				audios.m_music = globalVal.music;
				TAudioManager.instance.isMusicOn = globalVal.music;
				audios.PlayAudio("UIclick");
				UpdateMusicState_pause();
				if (!globalVal.music)
				{
					audios.StopAllMusic();
				}
				else
				{
					audios.ReplayMusic();
				}
				break;
			case "pause_sound":
				globalVal.sound = !globalVal.sound;
				audios.m_sound = globalVal.sound;
				TAudioManager.instance.isSoundOn = globalVal.sound;
				audios.PlayAudio("UIclick");
				UpdateSoundState_pause();
				break;
			case "pause_btn":
			{
				GameObject gameObject6 = GameObject.Find("TUI/TUIControl");
				gameUI gameUI4 = gameObject6.GetComponent(typeof(gameUI)) as gameUI;
				gameUI4.OnPauseDown();
				audios.PlayAudio("UIclick");
				break;
			}
			case "pause_resume":
			{
				GameObject gameObject5 = GameObject.Find("TUI/TUIControl");
				gameUI gameUI3 = gameObject5.GetComponent(typeof(gameUI)) as gameUI;
				gameUI3.OnResumeDown();
				gameUI3.RemovePause();
				Time.timeScale = 1f;
				audios.PlayAudio("UIback");
				globalVal.SetOpenClickShow(false);
				break;
			}
			case "pause_menu":
				Time.timeScale = 1f;
				globalVal.UIState = UILayer.MENU;
				audios.StopAllAudio();
				audios.ReplayMusic();
				globalVal.loadingSceneName = "scene_ui_menu";
				Application.LoadLevel("scene_ui_loading");
				audios.PlayAudio("UIclick");
				globalVal.SetOpenClickShow(false);
				break;
			case "pause_retry":
				Time.timeScale = 1f;
				audios.StopAllAudio();
				audios.ReplayMusic();
				Application.LoadLevel("scene_game");
				audios.PlayAudio("UIclick");
				globalVal.SetOpenClickShow(false);
				break;
			case "stash_back":
				if (globalVal.UIState == UILayer.GAMEOVER)
				{
					GameObject gameObject2 = GameObject.Find("TUI/TUIControl");
					gameUI gameUI2 = gameObject2.GetComponent(typeof(gameUI)) as gameUI;
					gameUI2.InitGameOver();
				}
				else
				{
					globalVal.UIState = UILayer.MENU;
					GameObject gameObject3 = GameObject.Find("TUI/TUIControl/fade_bg");
					TUIFade tUIFade = gameObject3.GetComponent(typeof(TUIFade)) as TUIFade;
					tUIFade.FadeOut("scene_ui_menu");
				}
				audios.PlayAudio("UIback");
				break;
			case "stash_getmore":
			{
				globalVal.UIState = UILayer.TBANK;
				GameObject gameObject27 = GameObject.Find("TUI/TUIControl/fade_bg");
				TUIFade tUIFade13 = gameObject27.GetComponent(typeof(TUIFade)) as TUIFade;
				tUIFade13.FadeOut("scene_ui_tbank");
				audios.PlayAudio("UIclick");
				break;
			}
			case "thestash_play":
				globalVal.loadingSceneName = "scene_game";
				Application.LoadLevel("scene_ui_loading");
				audios.PlayAudio("UIclick");
				break;
			case "option_back":
			{
				globalVal.UIState = UILayer.MENU;
				GameObject gameObject26 = GameObject.Find("TUI/TUIControl/fade_bg");
				TUIFade tUIFade12 = gameObject26.GetComponent(typeof(TUIFade)) as TUIFade;
				tUIFade12.FadeOut("scene_ui_menu");
				audios.PlayAudio("UIback");
				break;
			}
			case "howto_back":
			{
				globalVal.UIState = UILayer.OPTIONS;
				GameObject gameObject25 = GameObject.Find("TUI/TUIControl/fade_bg");
				TUIFade tUIFade11 = gameObject25.GetComponent(typeof(TUIFade)) as TUIFade;
				tUIFade11.FadeOut("scene_ui_option");
				audios.PlayAudio("UIback");
				break;
			}
			case "help_click":
			{
				GameObject gameObject24 = GameObject.Find("TUI/TUIControl");
				gameUI gameUI12 = gameObject24.GetComponent(typeof(gameUI)) as gameUI;
				gameUI12.StopShake();
				break;
			}
			case "avatar_back":
			case "items_back":
			case "upgrades_back":
			case "tbank_back":
			case "profile_back":
				if (!purchaseKey)
				{
					if (globalVal.UIState == UILayer.GAMEOVER)
					{
						globalVal.UIState = UILayer.THESTASH;
						Application.LoadLevel("scene_ui_thestash");
					}
					else
					{
						globalVal.UIState = UILayer.THESTASH;
						GameObject gameObject21 = GameObject.Find("TUI/TUIControl/fade_bg");
						TUIFade tUIFade9 = gameObject21.GetComponent(typeof(TUIFade)) as TUIFade;
						tUIFade9.FadeOut("scene_ui_thestash");
					}
					audios.PlayAudio("UIback");
				}
				break;
			case "thestash_profile":
			{
				globalVal.UIState = UILayer.PROFILE;
				GameObject gameObject20 = GameObject.Find("TUI/TUIControl/fade_bg");
				TUIFade tUIFade8 = gameObject20.GetComponent(typeof(TUIFade)) as TUIFade;
				tUIFade8.FadeOut("scene_ui_profile");
				audios.PlayAudio("UIclick");
				break;
			}
			case "pop_dialog_buy":
			{
				MonoBehaviour.print(control.name);
				int num = 0;
				GameObject gameObject12 = GameObject.Find("TUI/TUIControl");
				menu menu3 = null;
				gameUI gameUI9 = null;
				UILayer uILayer2 = UILayer.NONE;
				if (globalVal.UIState == UILayer.GAMEOVER)
				{
					gameUI9 = gameObject12.GetComponent(typeof(gameUI)) as gameUI;
					uILayer2 = gameUI9.UIParentID;
					num = gameUI9.selectIndex;
				}
				else
				{
					menu3 = gameObject12.GetComponent(typeof(menu)) as menu;
					uILayer2 = globalVal.UIState;
					num = menu3.selectIndex;
				}
				int num2 = 0;
				switch (uILayer2)
				{
				case UILayer.UPGRADES:
				{
					ItemAttribute itemAttribute = ItemManagerClass.body.GetItemAttribute(num);
					if (itemAttribute == null)
					{
						break;
					}
					int num3 = (int)globalVal.g_itemlevel[itemAttribute.index];
					menu3.InitAllItems();
					if (num3 + 1 < itemAttribute.level.Count)
					{
						ItemSubAttr itemSubAttr = itemAttribute.level[num3 + 1] as ItemSubAttr;
						if (globalVal.g_gold >= itemSubAttr.price)
						{
							globalVal.g_gold -= itemSubAttr.price;
							globalVal.g_itemlevel[itemAttribute.index] = num3 + 1;
							StatisticsData.data.d_UsedGold += itemSubAttr.price;
							StatisticsData.data.SaveDailyData(StatisticsData.data.GetSystemDay());
							globalVal.SaveFile("saveData.txt");
							menu3.InitUpgradesList();
							audios.PlayAudio("UIbuy");
							num2 = (int)(control.transform.localPosition.y / 50f);
							num2 *= -1;
							menu3.ShowDialog(true, num);
						}
						else if (Utils.ShowMessageBox2("Not Enough Coins!", "Would you like to get more coins?", "OK", "CANCEL") == 0)
						{
							globalVal.UIState = UILayer.TBANK;
							GameObject gameObject16 = GameObject.Find("TUI/TUIControl/fade_bg");
							TUIFade tUIFade5 = gameObject16.GetComponent(typeof(TUIFade)) as TUIFade;
							tUIFade5.FadeOut("scene_ui_tbank");
							audios.PlayAudio("UIclick");
						}
						else
						{
							num2 *= -1;
							menu3.ShowDialog(true, num);
						}
					}
					break;
				}
				case UILayer.ITEMS:
				{
					ItemOnceAttribute itemOnceAttribute = ItemManagerClass.body.itemOnceArray[num] as ItemOnceAttribute;
					if (itemOnceAttribute != null)
					{
						menu3.InitAllItems();
						if (globalVal.g_gold >= itemOnceAttribute.price && (int)globalVal.g_item_once_count[num] < 999)
						{
							globalVal.g_gold -= itemOnceAttribute.price;
							globalVal.g_item_once_count[num] = (int)globalVal.g_item_once_count[num] + 1;
							MonoBehaviour.print("StatisticsData.data.d_itemBuyCount : " + StatisticsData.data.d_itemBuyCount.Count);
							StatisticsData.data.d_itemBuyCount[num] = (int)StatisticsData.data.d_itemBuyCount[num] + 1;
							StatisticsData.data.d_UsedGold += itemOnceAttribute.price;
							StatisticsData.data.SaveDailyData(StatisticsData.data.GetSystemDay());
							globalVal.SaveFile("saveData.txt");
							menu3.InitItemList();
							audios.PlayAudio("UIbuy");
							num2 = (int)(control.transform.localPosition.y / 50f);
							num2 *= -1;
							menu3.ShowDialog(true, num);
						}
						else if (Utils.ShowMessageBox2("Not Enough Coins!", "Would you like to get more coins?", "OK", "CANCEL") == 0)
						{
							globalVal.UIState = UILayer.TBANK;
							GameObject gameObject15 = GameObject.Find("TUI/TUIControl/fade_bg");
							TUIFade tUIFade4 = gameObject15.GetComponent(typeof(TUIFade)) as TUIFade;
							tUIFade4.FadeOut("scene_ui_tbank");
							audios.PlayAudio("UIclick");
						}
						else
						{
							num2 *= -1;
							menu3.ShowDialog(true, num);
						}
					}
					break;
				}
				case UILayer.AVATAR:
				{
					AvatarAttribute avatarAttribute = ItemManagerClass.body.avatarArray[num] as AvatarAttribute;
					if (avatarAttribute == null)
					{
						break;
					}
					menu3.InitAllItems();
					if (globalVal.g_gold >= avatarAttribute.price && (int)globalVal.g_avatar_isbuy[num] == 0)
					{
						globalVal.g_gold -= avatarAttribute.price;
						globalVal.g_avatar_isbuy[num] = 1;
						StatisticsData.data.d_UsedGold += avatarAttribute.price;
						StatisticsData.data.SaveDailyData(StatisticsData.data.GetSystemDay());
						globalVal.SaveFile("saveData.txt");
						menu3.InitAvatarList();
						num2 = (int)(control.transform.localPosition.y / 50f);
						audios.PlayAudio("UIbuy");
						num2 *= -1;
						menu3.ShowDialog(false, 0);
					}
					else if (globalVal.g_gold < avatarAttribute.price && (int)globalVal.g_avatar_isbuy[num] == 0)
					{
						if (Utils.ShowMessageBox2("Not Enough Coins!", "Would you like to get more coins?", "OK", "CANCEL") == 0)
						{
							globalVal.UIState = UILayer.TBANK;
							GameObject gameObject13 = GameObject.Find("TUI/TUIControl/fade_bg");
							TUIFade tUIFade3 = gameObject13.GetComponent(typeof(TUIFade)) as TUIFade;
							tUIFade3.FadeOut("scene_ui_tbank");
							audios.PlayAudio("UIclick");
						}
						else
						{
							num2 *= -1;
							menu3.ShowDialog(false, 0);
						}
					}
					if ((int)globalVal.g_avatar_isbuy[num] == 1)
					{
						globalVal.g_avatar_id = num;
						globalVal.SaveFile("saveData.txt");
						menu3.InitAvatarList();
						menu3.ShowDialog(false, 0);
						audios.PlayAudio("UIavatar");
					}
					GameObject gameObject14 = GameObject.Find("AvatarCamera");
					if (gameObject14 != null)
					{
						gameObject14.GetComponent<Camera>().enabled = false;
						gameObject14.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
					}
					break;
				}
				}
				break;
			}
			case "pop_dialog_close":
			{
				GameObject gameObject9 = GameObject.Find("TUI/TUIControl");
				menu menu2 = null;
				gameUI gameUI6 = null;
				UILayer uILayer = UILayer.NONE;
				if (globalVal.UIState == UILayer.GAMEOVER)
				{
					gameUI6 = gameObject9.GetComponent(typeof(gameUI)) as gameUI;
					uILayer = gameUI6.UIParentID;
				}
				else
				{
					menu2 = gameObject9.GetComponent(typeof(menu)) as menu;
					uILayer = globalVal.UIState;
				}
				menu2.InitAllItems();
				switch (uILayer)
				{
				case UILayer.UPGRADES:
					menu2.InitUpgradesList();
					break;
				case UILayer.ITEMS:
					menu2.InitItemList();
					break;
				case UILayer.AVATAR:
					menu2.InitAvatarList();
					break;
				}
				menu2.ShowDialog(false, 0);
				HideAvatarCamera();
				audios.PlayAudio("UIback");
				break;
			}
			case "control_rejump":
			{
				GameObject gameObject8 = GameObject.Find("player2/Bip01");
				PlayerScript playerScript = gameObject8.GetComponent(typeof(PlayerScript)) as PlayerScript;
				playerScript.OnRejump();
				break;
			}
			case "task_button":
			{
				GameObject gameObject7 = GameObject.Find("TUI/TUIControl");
				gameUI gameUI5 = gameObject7.GetComponent(typeof(gameUI)) as gameUI;
				if (!(gameUI5 != null) || gameUI5.uitype != HelpState.BUYFIRST)
				{
					gameObject7 = GameObject.Find("TUI/TUIControl/task_page");
					TaskManager taskManager = gameObject7.GetComponent(typeof(TaskManager)) as TaskManager;
					taskManager.PopTaskList();
				}
				break;
			}
			case "tbank_1":
				if (!purchaseKey)
				{
					purchasetype = 1;
					purchaseKey = true;
					ShowIndicator();
					IAPPlugin.NowPurchaseProduct("com.trinitigame.miniglider.099cents", "1");
				}
				break;
			case "tbank_2":
				if (!purchaseKey)
				{
					purchasetype = 2;
					purchaseKey = true;
					ShowIndicator();
					IAPPlugin.NowPurchaseProduct("com.trinitigame.miniglider.199cents1", "1");
				}
				break;
			case "tbank_3":
				if (!purchaseKey)
				{
					purchasetype = 3;
					purchaseKey = true;
					ShowIndicator();
					IAPPlugin.NowPurchaseProduct("com.trinitigame.miniglider.199cents2", "1");
				}
				break;
			case "tbank_4":
				if (!purchaseKey)
				{
					purchasetype = 4;
					purchaseKey = true;
					ShowIndicator();
					IAPPlugin.NowPurchaseProduct("com.trinitigame.miniglider.299cents", "1");
				}
				break;
			case "tbank_5":
				if (!purchaseKey)
				{
					purchasetype = 5;
					purchaseKey = true;
					ShowIndicator();
					IAPPlugin.NowPurchaseProduct("com.trinitigame.miniglider.499cents", "1");
				}
				break;
			case "tbank_6":
				if (!purchaseKey)
				{
					purchasetype = 6;
					purchaseKey = true;
					ShowIndicator();
					IAPPlugin.NowPurchaseProduct("com.trinitigame.miniglider.999cents", "1");
				}
				break;
			case "tbank_restore":
				if (!purchaseKey)
				{
					purchaseKey = true;
					restoreKey = true;
					IAPPlugin.DoRestoreProduct();
					ShowIndicator();
				}
				break;
			case "skip_btn":
			{
				GameObject gameObject = GameObject.Find("/Slingshot");
				if ((bool)gameObject)
				{
					SlingshotScript slingshotScript = gameObject.GetComponent(typeof(SlingshotScript)) as SlingshotScript;
					slingshotScript.CheckSkip();
				}
				break;
			}
			}
		}
		else if (eventType == 3 && control.GetType() == typeof(TUIButtonClick_Pressed))
		{
			switch (control.name)
			{
			case "thestash_avatar":
			{
				MonoBehaviour.print(globalVal.UIState.ToString());
				globalVal.UIState = UILayer.AVATAR;
				GameObject gameObject33 = GameObject.Find("TUI/TUIControl/fade_bg");
				TUIFade tUIFade15 = gameObject33.GetComponent(typeof(TUIFade)) as TUIFade;
				tUIFade15.FadeOut("scene_ui_avatar");
				audios.PlayAudio("UIclick");
				break;
			}
			case "thestash_items":
			{
				globalVal.UIState = UILayer.ITEMS;
				GameObject gameObject30 = GameObject.Find("TUI/TUIControl/fade_bg");
				TUIFade tUIFade14 = gameObject30.GetComponent(typeof(TUIFade)) as TUIFade;
				tUIFade14.FadeOut("scene_ui_items");
				audios.PlayAudio("UIclick");
				break;
			}
			case "thestash_upgrades":
			{
				globalVal.UIState = UILayer.UPGRADES;
				GameObject gameObject34 = GameObject.Find("TUI/TUIControl/fade_bg");
				TUIFade tUIFade16 = gameObject34.GetComponent(typeof(TUIFade)) as TUIFade;
				tUIFade16.FadeOut("scene_ui_upgrade");
				audios.PlayAudio("UIclick");
				break;
			}
			case "control_left":
			{
				GameObject gameObject31 = GameObject.Find("player2/Bip01");
				PlayerScript playerScript3 = gameObject31.GetComponent(typeof(PlayerScript)) as PlayerScript;
				playerScript3.OnHuoJian();
				if (!globalVal.g_help_key[1])
				{
					GameObject gameObject32 = GameObject.Find("TUI/TUIControl");
					gameUI gameUI14 = gameObject32.GetComponent(typeof(gameUI)) as gameUI;
					gameUI14.StopHelp(HelpState.HUOJIAN);
					Time.timeScale = 1f;
				}
				break;
			}
			case "control_right":
			{
				GameObject gameObject28 = GameObject.Find("player2/Bip01");
				PlayerScript playerScript2 = gameObject28.GetComponent(typeof(PlayerScript)) as PlayerScript;
				playerScript2.OnHuaXiang();
				if (!globalVal.g_help_key[2])
				{
					GameObject gameObject29 = GameObject.Find("TUI/TUIControl");
					gameUI gameUI13 = gameObject29.GetComponent(typeof(gameUI)) as gameUI;
					gameUI13.StopHelp(HelpState.HUAXIANG);
					Time.timeScale = 1f;
				}
				break;
			}
			}
		}
		else if (eventType == 1 && control.GetType() == typeof(TUIButtonSelect))
		{
			switch (control.name)
			{
			case "avatar_item":
			{
				int num4 = (int)(control.transform.localPosition.y / 50f);
				MonoBehaviour.print("item index = " + num4);
				num4 *= -1;
				GameObject gameObject35 = GameObject.Find("TUI/TUIControl");
				menu menu4 = gameObject35.GetComponent(typeof(menu)) as menu;
				menu4.ShowDialog(true, num4);
				break;
			}
			}
		}
		else
		{
			if ((eventType != 1 || control.GetType() != typeof(TUIButtonPush)) && (eventType != 2 || control.GetType() != typeof(TUIButtonPush)))
			{
				return;
			}
			switch (control.name)
			{
			case "onoff_music_1":
			case "onoff_music_2":
				globalVal.music = !globalVal.music;
				audios.m_music = globalVal.music;
				TAudioManager.instance.isMusicOn = globalVal.music;
				audios.PlayAudio("UIclick");
				UpdateMusicState();
				if (!globalVal.music)
				{
					audios.StopAllMusic();
				}
				else
				{
					audios.ReplayMusic();
				}
				break;
			case "onoff_sound_1":
			case "onoff_sound_2":
				globalVal.sound = !globalVal.sound;
				audios.m_sound = globalVal.sound;
				Debug.Log("m_sound :  " + audios.m_sound);
				TAudioManager.instance.isSoundOn = globalVal.sound;
				audios.PlayAudio("UIclick");
				UpdateSoundState();
				break;
			}
		}
	}

	private void HideAvatarCamera()
	{
		GameObject gameObject = GameObject.Find("AvatarCamera");
		if (gameObject != null)
		{
			gameObject.GetComponent<Camera>().enabled = false;
			gameObject.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
		}
	}

	private void ShowIndicator()
	{
		purchaseOutTime = 0f;
		GameObject gameObject = GameObject.Find("TUI/TUIControl/tbank");
		Transform transform = gameObject.transform.Find("hui");
		transform.localPosition = new Vector3(0f, 0f, -29f);
		Utils.ShowIndicatorSystem_int(1, Utils.ScreenType(), 0f, 0f, 0f, 0f);
	}

	private void HideIndicator()
	{
		GameObject gameObject = GameObject.Find("TUI/TUIControl/tbank");
		Transform transform = gameObject.transform.Find("hui");
		transform.localPosition = new Vector3(-1000f, 0f, -29f);
		Utils.HideIndicatorSystem();
	}

	public void UpdateMusicState_pause()
	{
		GameObject gameObject = GameObject.Find("TUI/TUIControl");
		TUIMeshSprite tUIMeshSprite = null;
		if (globalVal.UIState == UILayer.PAUSE)
		{
			tUIMeshSprite = gameObject.transform.Find("pause_page/pause_music_state").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
			string empty = string.Empty;
			empty = ((!globalVal.music) ? "pause_sound_off" : "pause_sound_on");
			tUIMeshSprite.frameName = empty;
		}
	}

	public void UpdateSoundState_pause()
	{
		GameObject gameObject = GameObject.Find("TUI/TUIControl");
		TUIMeshSprite tUIMeshSprite = null;
		if (globalVal.UIState == UILayer.PAUSE)
		{
			tUIMeshSprite = gameObject.transform.Find("pause_page/pause_sound_state").GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
			string empty = string.Empty;
			empty = ((!globalVal.sound) ? "pause_music_off" : "pause_music_on");
			tUIMeshSprite.frameName = empty;
		}
	}

	public void UpdateMusicState()
	{
		GameObject gameObject = GameObject.Find("TUI/TUIControl");
		TUIButtonPush tUIButtonPush = null;
		if (globalVal.UIState == UILayer.PAUSE)
		{
			tUIButtonPush = gameObject.transform.Find("pause_page/onoff_music_1").GetComponent(typeof(TUIButtonPush)) as TUIButtonPush;
			tUIButtonPush.SetPressed(globalVal.music);
			tUIButtonPush = gameObject.transform.Find("pause_page/onoff_music_2").GetComponent(typeof(TUIButtonPush)) as TUIButtonPush;
			tUIButtonPush.SetPressed(!globalVal.music);
			MonoBehaviour.print(globalVal.music);
		}
		else if (globalVal.UIState == UILayer.OPTIONS)
		{
			tUIButtonPush = gameObject.transform.Find("option/onoff_music_1").GetComponent(typeof(TUIButtonPush)) as TUIButtonPush;
			tUIButtonPush.SetPressed(globalVal.music);
			tUIButtonPush = gameObject.transform.Find("option/onoff_music_2").GetComponent(typeof(TUIButtonPush)) as TUIButtonPush;
			tUIButtonPush.SetPressed(!globalVal.music);
		}
	}

	public void UpdateSoundState()
	{
		GameObject gameObject = GameObject.Find("TUI/TUIControl");
		TUIButtonPush tUIButtonPush = null;
		if (globalVal.UIState == UILayer.PAUSE)
		{
			tUIButtonPush = gameObject.transform.Find("pause_page/onoff_sound_1").GetComponent(typeof(TUIButtonPush)) as TUIButtonPush;
			tUIButtonPush.SetPressed(globalVal.sound);
			tUIButtonPush = gameObject.transform.Find("pause_page/onoff_sound_2").GetComponent(typeof(TUIButtonPush)) as TUIButtonPush;
			tUIButtonPush.SetPressed(!globalVal.sound);
			MonoBehaviour.print(globalVal.sound);
		}
		else if (globalVal.UIState == UILayer.OPTIONS)
		{
			tUIButtonPush = gameObject.transform.Find("option/onoff_sound_1").GetComponent(typeof(TUIButtonPush)) as TUIButtonPush;
			tUIButtonPush.SetPressed(globalVal.sound);
			tUIButtonPush = gameObject.transform.Find("option/onoff_sound_2").GetComponent(typeof(TUIButtonPush)) as TUIButtonPush;
			tUIButtonPush.SetPressed(!globalVal.sound);
		}
	}
}
