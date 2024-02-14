using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class globalVal
{
	public static UILayer UIState = UILayer.MENU;

	public static int g_reveiw_count = 0;

	public static int g_gold = 0;

	public static float g_best_dis = 0f;

	public static float g_best_height = 0f;

	public static float g_best_speed = 0f;

	public static ulong g_best_playcount = 0uL;

	public static double g_best_totaldis = 0.0;

	public static int g_best_taskcomplete = 0;

	public static ulong g_best_zombiebreak = 0uL;

	public static bool g_gold_isdoubel = false;

	public static int g_avatar_id = 0;

	public static ArrayList g_itemlevel = new ArrayList();

	public static ArrayList g_item_once_count = new ArrayList();

	public static int[] cur_task_id = new int[3];

	public static ArrayList g_avatar_isbuy = new ArrayList();

	public static SceneForwardState g_scene_state = SceneForwardState.NONE;

	public static float changeDis = 1000f;

	public static Transform sceneLastTrans = null;

	public static bool isskipkey = false;

	public static string random = "0";

	public static bool secondKey = false;

	public static bool music = true;

	public static bool sound = true;

	public static ArrayList LastTrack = new ArrayList();

	public static ArrayList CurrentTrack = new ArrayList();

	public static TrackInfo curBestHeight = new TrackInfo();

	public static TrackInfo lastBestHeight = new TrackInfo();

	public static bool[] g_help_key = new bool[4];

	public static bool[] g_achievement_key = new bool[13];

	public static string loadingSceneName = string.Empty;

	public static bool OpenClickKey = false;

	public static bool twitterKey = false;

	public static string twitterDay = string.Empty;

	private static bool PluginKey = false;

	private static bool GameCenterKey = false;

	public static ulong g_totalPlayTime = 0uL;

	public static ulong g_totalGold = 0uL;

	public static void InitPlugins()
	{
		if (!PluginKey)
		{
			PluginKey = true;
			if (GetFirstDay() == string.Empty)
			{
				SaveFirstDay();
			}
			StatisticsData.data.ReadDataManager();
			StatisticsData.data.t_cur_sys_time = StatisticsData.data.GetSystemTime_Second();
			string systemDay = StatisticsData.data.GetSystemDay();
			string path = getSavePath() + systemDay;
			if (File.Exists(path))
			{
				StatisticsData.data.ReadDailyData(systemDay);
			}
			else
			{
				StatisticsData.data.InitDailyData(systemDay);
			}
			StatisticsData.data.d_loginCount++;
			StatisticsData.data.SaveDailyData(systemDay);
			StatisticsData.data.SendStatisticsDataDaily();
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				OpenClickPlugin.Initialize("64900DC8-B003-44E6-816D-59C2817F5507");
			}
		}
	}

	public static void InitGameCenterPlugin()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer && !GameCenterKey)
		{
			GameCenterKey = true;
			GameCenterPlugin.Initialize();
			if (GameCenterPlugin.IsSupported() && !GameCenterPlugin.IsLogin())
			{
				GameCenterPlugin.Login();
			}
			ReadReviewCount();
			if (g_reveiw_count < 4)
			{
				g_reveiw_count++;
			}
			SaveReviewCount();
			if (g_reveiw_count == 3 && Utils.ShowMessageBox2(string.Empty, "Having fun? Rate this app!", "YES", "LATER") == 0)
			{
				Application.OpenURL("http://itunes.apple.com/us/app/miniglider/id518241576?ls=1&mt=8");
			}
		}
	}

	public static void SetOpenClickShow(bool key)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer && key != OpenClickKey)
		{
			if (key)
			{
				OpenClickPlugin.Show(false);
			}
			else
			{
				OpenClickPlugin.Hide();
			}
			OpenClickKey = key;
		}
	}

	public static string getSavePath()
	{
		string text = Application.dataPath;
		if (!Application.isEditor)
		{
			text = text.Substring(0, text.LastIndexOf('/'));
			text = text.Substring(0, text.LastIndexOf('/'));
		}
		return text + "/Documents/";
	}

	public static string GetFirstDay()
	{
		string result = string.Empty;
		string path = getSavePath() + "first.sav";
		if (File.Exists(path))
		{
			FileStream fileStream = new FileStream(path, FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			result = binaryReader.ReadString();
			binaryReader.Close();
			fileStream.Close();
		}
		return result;
	}

	public static void SaveFirstDay()
	{
		string path = getSavePath() + "first.sav";
		string empty = string.Empty;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		num = Utils.GetIOSYear();
		num2 = Utils.GetIOSMonth();
		num3 = Utils.GetIOSDay();
		num4 = Utils.GetIOSHour();
		num5 = Utils.GetIOSMin();
		num6 = Utils.GetIOSSec();
		empty = string.Empty + num + "-" + num2 + "-" + num3 + "|" + num4 + "-" + num5 + "-" + num6;
		FileStream fileStream = new FileStream(path, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		binaryWriter.Write(empty);
		binaryWriter.Close();
		fileStream.Close();
	}

	public static void ShowAllChild(Transform trans, bool isshow)
	{
		Transform[] components = trans.GetComponents<Transform>();
		Transform[] array = components;
		foreach (Transform transform in array)
		{
			MeshRenderer meshRenderer = transform.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
			if (meshRenderer != null)
			{
				meshRenderer.enabled = isshow;
			}
		}
	}

	public static void SaveTrack(string filename)
	{
		string path = getSavePath() + filename;
		lastBestHeight.pos = Vector3.zero;
		curBestHeight.pos = Vector3.zero;
		try
		{
			FileStream fileStream = new FileStream(path, FileMode.Create);
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			int value = 2;
			binaryWriter.Write(value);
			binaryWriter.Write(LastTrack.Count);
			int index = 0;
			for (int i = 0; i < LastTrack.Count; i++)
			{
				TrackInfo trackInfo = LastTrack[i] as TrackInfo;
				if (trackInfo.pos.y >= lastBestHeight.pos.y)
				{
					lastBestHeight.pos = trackInfo.pos;
					index = i;
				}
				binaryWriter.Write(trackInfo.pos.x);
				binaryWriter.Write(trackInfo.pos.y);
				binaryWriter.Write(trackInfo.pos.z);
				binaryWriter.Write(trackInfo.type);
			}
			lastBestHeight.index = index;
			binaryWriter.Write(CurrentTrack.Count);
			for (int j = 0; j < CurrentTrack.Count; j++)
			{
				TrackInfo trackInfo2 = CurrentTrack[j] as TrackInfo;
				if (trackInfo2.pos.y >= curBestHeight.pos.y)
				{
					curBestHeight.pos = trackInfo2.pos;
					index = j;
				}
				binaryWriter.Write(trackInfo2.pos.x);
				binaryWriter.Write(trackInfo2.pos.y);
				binaryWriter.Write(trackInfo2.pos.z);
				binaryWriter.Write(trackInfo2.type);
			}
			curBestHeight.index = index;
			binaryWriter.Close();
			fileStream.Close();
		}
		catch (IOException ex)
		{
			Debug.Log(ex.ToString());
		}
	}

	public static void ReadLastTrack(string filename)
	{
		string path = getSavePath() + filename;
		try
		{
			FileStream fileStream = new FileStream(path, FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			int num = binaryReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				int num2 = binaryReader.ReadInt32();
				ArrayList arrayList = new ArrayList();
				for (int j = 0; j < num2; j++)
				{
					TrackInfo trackInfo = new TrackInfo();
					trackInfo.pos.x = binaryReader.ReadSingle();
					trackInfo.pos.y = binaryReader.ReadSingle();
					trackInfo.pos.z = binaryReader.ReadSingle();
					trackInfo.type = binaryReader.ReadInt32();
					arrayList.Add(trackInfo);
				}
				if (i == 0)
				{
					LastTrack = (ArrayList)arrayList.Clone();
				}
				if (i == 1)
				{
					CurrentTrack = (ArrayList)arrayList.Clone();
				}
			}
			binaryReader.Close();
			fileStream.Close();
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
		}
	}

	public static void ReadReviewCount()
	{
		string text = getSavePath() + "reveiw.dat";
		if (File.Exists(text))
		{
			Debug.Log(text);
			FileStream fileStream = new FileStream(text, FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			g_reveiw_count = binaryReader.ReadInt32();
			binaryReader.Close();
			fileStream.Close();
		}
		else
		{
			SaveReviewCount();
		}
	}

	public static void SaveReviewCount()
	{
		string path = getSavePath() + "reveiw.dat";
		try
		{
			FileStream fileStream = new FileStream(path, FileMode.Create);
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(g_reveiw_count);
			binaryWriter.Close();
			fileStream.Close();
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
		}
	}

	public static void ReadTwitterDay()
	{
		string text = "twitter.dat";
		string path = getSavePath() + text;
		if (File.Exists(path))
		{
			FileStream fileStream = new FileStream(path, FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			twitterDay = binaryReader.ReadString();
			binaryReader.Close();
			fileStream.Close();
		}
		else
		{
			SaveTwitterDay();
		}
	}

	public static void SaveTwitterDay()
	{
		string text = "twitter.dat";
		string path = getSavePath() + text;
		try
		{
			FileStream fileStream = new FileStream(path, FileMode.Create);
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(twitterDay = string.Empty + Utils.GetIOSYear() + Utils.GetIOSMonth() + Utils.GetIOSDay());
			binaryWriter.Close();
			fileStream.Close();
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
		}
	}

	public static void SaveFile(string filename)
	{
		string path = getSavePath() + filename;
		try
		{
			FileStream fileStream = new FileStream(path, FileMode.Create);
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(g_gold);
			binaryWriter.Write(g_avatar_id);
			binaryWriter.Write(g_gold_isdoubel);
			binaryWriter.Write(g_best_dis);
			for (int i = 0; i < g_itemlevel.Count; i++)
			{
				binaryWriter.Write((int)g_itemlevel[i]);
			}
			for (int j = 0; j < cur_task_id.Length; j++)
			{
				binaryWriter.Write(cur_task_id[j]);
			}
			for (int k = 0; k < g_avatar_isbuy.Count; k++)
			{
				int value = (int)g_avatar_isbuy[k];
				binaryWriter.Write(value);
			}
			for (int l = 0; l < g_item_once_count.Count; l++)
			{
				int value2 = (int)g_item_once_count[l];
				binaryWriter.Write(value2);
			}
			for (int m = 0; m < g_help_key.Length; m++)
			{
				binaryWriter.Write(g_help_key[m]);
			}
			for (int n = 0; n < g_achievement_key.Length; n++)
			{
				binaryWriter.Write(g_achievement_key[n]);
			}
			binaryWriter.Write(g_best_height);
			binaryWriter.Write(g_best_speed);
			binaryWriter.Write(g_best_playcount);
			binaryWriter.Write(g_best_totaldis);
			binaryWriter.Write(g_best_taskcomplete);
			binaryWriter.Write(g_best_zombiebreak);
			binaryWriter.Write(g_totalGold);
			binaryWriter.Write(g_totalPlayTime);
			binaryWriter.Close();
			fileStream.Close();
		}
		catch (IOException ex)
		{
			Debug.Log(ex.ToString());
		}
	}

	public static void ReadFile(string filename)
	{
		string text = getSavePath() + filename;
		try
		{
			if (File.Exists(text))
			{
				Debug.Log(text);
				FileStream fileStream = new FileStream(text, FileMode.Open);
				BinaryReader binaryReader = new BinaryReader(fileStream);
				g_gold = binaryReader.ReadInt32();
				g_avatar_id = binaryReader.ReadInt32();
				g_gold_isdoubel = binaryReader.ReadBoolean();
				g_best_dis = binaryReader.ReadSingle();
				int num = 0;
				for (int i = 0; i < g_itemlevel.Count; i++)
				{
					g_itemlevel[i] = binaryReader.ReadInt32();
					num++;
				}
				for (int j = 0; j < cur_task_id.Length; j++)
				{
					cur_task_id[j] = binaryReader.ReadInt32();
				}
				for (int k = 0; k < g_avatar_isbuy.Count; k++)
				{
					int num2 = binaryReader.ReadInt32();
					g_avatar_isbuy[k] = num2;
				}
				for (int l = 0; l < g_item_once_count.Count; l++)
				{
					int num3 = binaryReader.ReadInt32();
					g_item_once_count[l] = num3;
				}
				for (int m = 0; m < g_help_key.Length; m++)
				{
					g_help_key[m] = binaryReader.ReadBoolean();
				}
				for (int n = 0; n < g_achievement_key.Length; n++)
				{
					g_achievement_key[n] = binaryReader.ReadBoolean();
				}
				g_best_height = binaryReader.ReadSingle();
				g_best_speed = binaryReader.ReadSingle();
				g_best_playcount = binaryReader.ReadUInt64();
				g_best_totaldis = binaryReader.ReadDouble();
				g_best_taskcomplete = binaryReader.ReadInt32();
				g_best_zombiebreak = binaryReader.ReadUInt64();
				g_totalGold = binaryReader.ReadUInt64();
				g_totalPlayTime = binaryReader.ReadUInt64();
				binaryReader.Close();
				fileStream.Close();
				ReadTwitterDay();
			}
			else
			{
				SaveFile(filename);
			}
		}
		catch (IOException ex)
		{
			Debug.Log(ex.ToString());
		}
	}

	public static void PurchaseFunction(int type)
	{
		switch (type)
		{
		case 1:
			g_gold += 10000;
			g_totalGold += 10000uL;
			StatisticsData.data.d_IapGold += 10000;
			StatisticsData.data.d_iapCount[1]++;
			break;
		case 2:
			g_gold += 22000;
			g_totalGold += 22000uL;
			StatisticsData.data.d_IapGold += 22000;
			StatisticsData.data.d_iapCount[2]++;
			break;
		case 3:
			g_gold_isdoubel = true;
			StatisticsData.data.d_iapCount[0]++;
			break;
		case 4:
			g_gold += 33000;
			g_totalGold += 33000uL;
			StatisticsData.data.d_IapGold += 33000;
			StatisticsData.data.d_iapCount[3]++;
			break;
		case 5:
			g_gold += 55000;
			g_totalGold += 55000uL;
			StatisticsData.data.d_IapGold += 55000;
			StatisticsData.data.d_iapCount[4]++;
			break;
		case 6:
			g_gold += 125000;
			g_totalGold += 125000uL;
			StatisticsData.data.d_IapGold += 125000;
			StatisticsData.data.d_iapCount[5]++;
			break;
		}
		g_achievement_key[11] = true;
		StatisticsData.data.SaveDailyData(StatisticsData.data.GetSystemDay());
		SaveFile("saveData.txt");
		GameCenterPlugin.SubmitAchievement("com.trinitigame.miniglider.a12", 100);
	}
}
