using System;
using System.Collections;
using System.IO;
using LitJson;
using UnityEngine;

public class PublicDailyData : MonoBehaviour
{
	public ulong d_totalTime;

	public int d_loginCount;

	public int d_gameGold;

	public int d_IapGold;

	public int d_UsedGold;

	public int[] d_iapCount = new int[6];

	public ArrayList d_itemBuyCount = new ArrayList();

	public ArrayList d_itemUsedCount = new ArrayList();

	public int[] d_UITapCount = new int[5];

	public int d_UiLastTap = -1;

	public int d_playCount;

	public int d_gameLongTime;

	public int d_gameShotTime = 100000000;

	public int d_gameNormalTime;

	public int d_gameLongDis;

	public int d_gameShotDis;

	public int d_gameNormalDis;

	public ulong t_cur_sys_time;

	public ArrayList data_saved_file = new ArrayList();

	public int requestId = -1;

	private string statistics_url = "http://account.trinitigame.com/gameapi/turboPlatform2.do?action=logAllInfo&json=";

	private int sendCount = 3;

	private int dataCount;

	public void SaveDailyData(string dataTime)
	{
		MonoBehaviour.print("------------------save----------------------");
		try
		{
			ulong systemTime_Second = GetSystemTime_Second();
			globalVal.g_totalPlayTime += systemTime_Second - t_cur_sys_time;
			d_totalTime += systemTime_Second - t_cur_sys_time;
			t_cur_sys_time = systemTime_Second;
			MonoBehaviour.print("total play time : " + globalVal.g_totalPlayTime);
			string path = globalVal.getSavePath() + dataTime;
			FileStream fileStream = new FileStream(path, FileMode.Create);
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(d_totalTime);
			MonoBehaviour.print("daytotalTime:" + d_totalTime);
			binaryWriter.Write(d_loginCount);
			MonoBehaviour.print("day login count:" + d_loginCount);
			binaryWriter.Write(d_gameGold);
			MonoBehaviour.print("day game gold" + d_gameGold);
			binaryWriter.Write(d_IapGold);
			MonoBehaviour.print("day Iap Gold" + d_IapGold);
			binaryWriter.Write(d_UsedGold);
			MonoBehaviour.print("day Used Gold : " + d_UsedGold);
			for (int i = 0; i < d_iapCount.Length; i++)
			{
				binaryWriter.Write(d_iapCount[i]);
				MonoBehaviour.print("day Iap" + i + " count : " + d_iapCount[i]);
			}
			for (int j = 0; j < d_itemBuyCount.Count; j++)
			{
				binaryWriter.Write((int)d_itemBuyCount[j]);
				MonoBehaviour.print("day item" + j + " buy count : " + (int)d_itemBuyCount[j]);
			}
			for (int k = 0; k < d_itemUsedCount.Count; k++)
			{
				binaryWriter.Write((int)d_itemUsedCount[k]);
				MonoBehaviour.print("day item" + k + " used count : " + (int)d_itemUsedCount[k]);
			}
			for (int l = 0; l < 5; l++)
			{
				binaryWriter.Write(d_UITapCount[l]);
				MonoBehaviour.print("day ui" + l + " tap count : " + d_UITapCount[l]);
			}
			binaryWriter.Write(d_UiLastTap);
			MonoBehaviour.print("day ui last tap " + d_UiLastTap);
			binaryWriter.Write(d_playCount);
			MonoBehaviour.print("day play count " + d_playCount);
			binaryWriter.Write(d_gameLongTime);
			MonoBehaviour.print("day game long time : " + d_gameLongTime);
			binaryWriter.Write(d_gameShotTime);
			MonoBehaviour.print("day game shot time : " + d_gameShotTime);
			binaryWriter.Write(d_gameNormalTime);
			MonoBehaviour.print("day game normal time: " + d_gameNormalTime);
			binaryWriter.Write(d_gameLongDis);
			MonoBehaviour.print("day game long dis : " + d_gameLongDis);
			binaryWriter.Write(d_gameShotDis);
			MonoBehaviour.print("day game shot dis : " + d_gameShotDis);
			binaryWriter.Write(d_gameNormalDis);
			MonoBehaviour.print("day game normal dis : " + d_gameNormalDis);
			binaryWriter.Close();
			fileStream.Close();
		}
		catch (IOException ex)
		{
			Debug.Log(ex.ToString());
		}
		MonoBehaviour.print("------------------ save end -------------------------");
	}

	public void ReadDailyData(string dataTime)
	{
		MonoBehaviour.print("------------------read----------------------");
		try
		{
			string path = globalVal.getSavePath() + dataTime;
			FileStream fileStream = new FileStream(path, FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			d_totalTime = binaryReader.ReadUInt64();
			MonoBehaviour.print("daytotalTime:" + d_totalTime);
			d_loginCount = binaryReader.ReadInt32();
			MonoBehaviour.print("day login count:" + d_loginCount);
			d_gameGold = binaryReader.ReadInt32();
			MonoBehaviour.print("day game gold" + d_gameGold);
			d_IapGold = binaryReader.ReadInt32();
			MonoBehaviour.print("day Iap Gold" + d_IapGold);
			d_UsedGold = binaryReader.ReadInt32();
			MonoBehaviour.print("day Used Gold : " + d_UsedGold);
			for (int i = 0; i < d_iapCount.Length; i++)
			{
				d_iapCount[i] = binaryReader.ReadInt32();
				MonoBehaviour.print("day Iap" + i + " count : " + d_iapCount[i]);
			}
			for (int j = 0; j < d_itemBuyCount.Count; j++)
			{
				d_itemBuyCount[j] = binaryReader.ReadInt32();
				MonoBehaviour.print("day item" + j + " buy count : " + (int)d_itemBuyCount[j]);
			}
			for (int k = 0; k < d_itemUsedCount.Count; k++)
			{
				d_itemUsedCount[k] = binaryReader.ReadInt32();
				MonoBehaviour.print("day item" + k + " used count : " + (int)d_itemUsedCount[k]);
			}
			for (int l = 0; l < 5; l++)
			{
				d_UITapCount[l] = binaryReader.ReadInt32();
				MonoBehaviour.print("day ui" + l + " tap count : " + d_UITapCount[l]);
			}
			d_UiLastTap = binaryReader.ReadInt32();
			MonoBehaviour.print("day ui last tap " + d_UiLastTap);
			d_playCount = binaryReader.ReadInt32();
			MonoBehaviour.print("day play count " + d_playCount);
			d_gameLongTime = binaryReader.ReadInt32();
			MonoBehaviour.print("day game long time : " + d_gameLongTime);
			d_gameShotTime = binaryReader.ReadInt32();
			MonoBehaviour.print("day game shot time : " + d_gameShotTime);
			d_gameNormalTime = binaryReader.ReadInt32();
			MonoBehaviour.print("day game normal time: " + d_gameNormalTime);
			d_gameLongDis = binaryReader.ReadInt32();
			MonoBehaviour.print("day game long dis : " + d_gameLongDis);
			d_gameShotDis = binaryReader.ReadInt32();
			MonoBehaviour.print("day game shot dis : " + d_gameShotDis);
			d_gameNormalDis = binaryReader.ReadInt32();
			MonoBehaviour.print("day game normal dis : " + d_gameNormalDis);
			binaryReader.Close();
			fileStream.Close();
		}
		catch (Exception)
		{
		}
		MonoBehaviour.print("------------------ read end -------------------------");
	}

	public void InitDailyData(string dataTime)
	{
		string path = globalVal.getSavePath() + dataTime;
		MonoBehaviour.print("------------------ init data -------------------------");
		try
		{
			FileStream fileStream = new FileStream(path, FileMode.Create);
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			d_totalTime = 0uL;
			d_loginCount = 0;
			d_gameGold = 0;
			d_IapGold = 0;
			d_UsedGold = 0;
			binaryWriter.Write(d_totalTime);
			binaryWriter.Write(d_loginCount);
			binaryWriter.Write(d_gameGold);
			binaryWriter.Write(d_IapGold);
			binaryWriter.Write(d_UsedGold);
			for (int i = 0; i < d_iapCount.Length; i++)
			{
				d_iapCount[i] = 0;
				binaryWriter.Write(d_iapCount[i]);
			}
			for (int j = 0; j < d_itemBuyCount.Count; j++)
			{
				d_itemBuyCount[j] = 0;
				binaryWriter.Write((int)d_itemBuyCount[j]);
			}
			for (int k = 0; k < d_itemUsedCount.Count; k++)
			{
				d_itemUsedCount[k] = 0;
				binaryWriter.Write((int)d_itemUsedCount[k]);
			}
			for (int l = 0; l < 5; l++)
			{
				d_UITapCount[l] = 0;
				binaryWriter.Write(d_UITapCount[l]);
			}
			d_UiLastTap = 0;
			d_playCount = 0;
			d_gameLongTime = 0;
			d_gameShotTime = 100000000;
			d_gameNormalTime = 0;
			d_gameLongDis = 0;
			d_gameShotDis = 100000000;
			d_gameNormalDis = 0;
			binaryWriter.Write(d_UiLastTap);
			binaryWriter.Write(d_playCount);
			binaryWriter.Write(d_gameLongTime);
			binaryWriter.Write(d_gameShotTime);
			binaryWriter.Write(d_gameNormalTime);
			binaryWriter.Write(d_gameLongDis);
			binaryWriter.Write(d_gameShotDis);
			binaryWriter.Write(d_gameNormalDis);
			binaryWriter.Close();
			fileStream.Close();
		}
		catch (Exception)
		{
		}
		UpdateDataManager(dataTime);
	}

	public bool SendStatisticsDataDaily()
	{
		string empty = string.Empty;
		empty = ((!((string)data_saved_file[0] == GetSystemDay()) || data_saved_file.Count <= 1) ? ((string)data_saved_file[0]) : ((string)data_saved_file[1]));
		if ((string)data_saved_file[0] == GetSystemDay() && data_saved_file.Count == 1)
		{
			return false;
		}
		ReadDailyData(empty);
		Hashtable hashtable = new Hashtable();
		int num = 0;
		if (Utils.IsJailbreak())
		{
			num = 1;
		}
		hashtable.Add("isCrack", num);
		hashtable.Add("firstDay", globalVal.GetFirstDay());
		hashtable.Add("totalPlayTime", globalVal.g_totalPlayTime);
		hashtable.Add("dayTotalTime", d_totalTime);
		hashtable.Add("dayLoginCount", d_loginCount);
		hashtable.Add("totalGold", globalVal.g_totalGold);
		hashtable.Add("curGold", globalVal.g_gold);
		hashtable.Add("dayGameGold", d_gameGold);
		hashtable.Add("dayIapGold", d_IapGold);
		hashtable.Add("dayUsedGold", d_UsedGold);
		for (int i = 0; i < 6; i++)
		{
			hashtable.Add("dayIap" + (i + 1) + "Count", d_iapCount[i]);
		}
		for (int j = 0; j < globalVal.g_avatar_isbuy.Count; j++)
		{
			hashtable.Add("playerA" + j + "Active", (int)globalVal.g_avatar_isbuy[j]);
		}
		MonoBehaviour.print("----------send statistics data daily----------------------");
		for (int k = 0; k < ItemManagerClass.body.attributeArray.Count; k++)
		{
			ItemAttribute itemAttribute = ItemManagerClass.body.attributeArray[k] as ItemAttribute;
			if (itemAttribute.inshop)
			{
				MonoBehaviour.print("item upgradeB" + (k + 1) + "   " + itemAttribute.type);
				int num2 = (int)globalVal.g_itemlevel[itemAttribute.index];
				hashtable.Add("upgradeB" + (k + 1) + "Level", num2);
			}
		}
		for (int l = 0; l < globalVal.g_item_once_count.Count; l++)
		{
			hashtable.Add("itemCountC" + (l + 1), (int)globalVal.g_item_once_count[l]);
		}
		for (int m = 0; m < d_itemBuyCount.Count; m++)
		{
			hashtable.Add("dayItemBuyCountC" + (m + 1), (int)d_itemBuyCount[m]);
		}
		for (int n = 0; n < d_itemUsedCount.Count; n++)
		{
			hashtable.Add("dayItemUsedCountC" + (n + 1), (int)d_itemUsedCount[n]);
		}
		for (int num3 = 0; num3 < 5; num3++)
		{
			hashtable.Add("dayPage" + (num3 + 1) + "TapCount", d_UITapCount[num3]);
		}
		hashtable.Add("dayUiLastTap", d_UiLastTap);
		hashtable.Add("dayPlayCount", d_playCount);
		hashtable.Add("dayGameLongTime", d_gameLongTime);
		hashtable.Add("dayGameShotTime", d_gameShotTime);
		hashtable.Add("dayGameNormalTime", d_gameNormalTime);
		hashtable.Add("dayGameLongDis", d_gameLongDis);
		hashtable.Add("dayGameShotDis", d_gameShotDis);
		hashtable.Add("dayGameNormalDis", d_gameNormalDis);
		string text = JsonMapper.ToJson(hashtable);
		Debug.Log(text);
		hashtable.Clear();
		Hashtable hashtable2 = new Hashtable();
		hashtable2.Add("gamename", "miniglider");
		hashtable2.Add("deviceid", Utils.GetMacAddr());
		hashtable2.Add("whatday", empty);
		hashtable2.Add("data", text);
		string request = JsonMapper.ToJson(hashtable2);
		hashtable2.Clear();
		HttpManager.Instance().SendRequest(statistics_url, request, string.Empty, 15f, OnResponse, OnRequestTimeout);
		return false;
	}

	private void Start()
	{
		StatisticsData.data = this;
		t_cur_sys_time = GetSystemTime_Second();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public string GetSystemDay()
	{
		string empty = string.Empty;
		int iOSYear = Utils.GetIOSYear();
		int iOSMonth = Utils.GetIOSMonth();
		int iOSDay = Utils.GetIOSDay();
		return string.Empty + iOSYear + "-" + iOSMonth + "-" + iOSDay;
	}

	public ulong GetSystemTime_Second()
	{
		ulong num = 0uL;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		num2 = Utils.GetIOSYear();
		num3 = Utils.GetIOSMonth();
		num4 = Utils.GetIOSDay();
		num5 = Utils.GetIOSHour();
		num6 = Utils.GetIOSMin();
		num7 = Utils.GetIOSSec();
		num = (ulong)num7;
		num += (ulong)(num6 * 60);
		num += (ulong)(num5 * 60 * 60);
		num += (ulong)((num4 - 1) * 60 * 60 * 24);
		int num8 = 0;
		for (int i = 0; i < num3; i++)
		{
			switch (num3)
			{
			case 0:
			case 2:
			case 4:
			case 6:
			case 7:
			case 9:
			case 11:
				num8 = 31;
				break;
			case 3:
			case 5:
			case 8:
			case 10:
				num8 = 30;
				break;
			case 1:
				num8 = 28;
				break;
			}
			num += (ulong)(num3 * num8 * (num4 - 1) * 60 * 60 * 24);
		}
		int num9 = ((num2 - 1900) * 365 + (num2 - 1900) / 4) * 60 * 60 * 24;
		num += (ulong)num9;
		Debug.Log("second ---- : " + num);
		return num;
	}

	private void OnResponse(int task_id, string param, int code, string response)
	{
		if (code > -1)
		{
			if (data_saved_file.Count > 1 && (string)data_saved_file[0] == GetSystemDay())
			{
				data_saved_file.RemoveAt(1);
			}
			else
			{
				data_saved_file.RemoveAt(0);
			}
			dataCount = data_saved_file.Count;
			if (dataCount > 0)
			{
				SendStatisticsDataDaily();
			}
			SaveDataManager();
			UpdateDataManager(GetSystemDay());
		}
	}

	private void OnRequestTimeout(int task_id, string param)
	{
		if (sendCount > 0)
		{
			StartCoroutine(OnTimeSendDailyData(60f));
			sendCount--;
		}
	}

	private IEnumerator OnTimeSendDailyData(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		SendStatisticsDataDaily();
	}

	public void ReadDataManager()
	{
		string path = globalVal.getSavePath() + "datafile.sav";
		if (!File.Exists(path))
		{
			return;
		}
		try
		{
			data_saved_file.Clear();
			FileStream fileStream = new FileStream(path, FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			string text = binaryReader.ReadString();
			MonoBehaviour.print("day data : " + text);
			string empty = string.Empty;
			int num = text.IndexOf('|');
			int num2 = 0;
			while (num > 0)
			{
				empty = text.Substring(0, num);
				text = text.Substring(num + 1);
				num = text.IndexOf('|');
				data_saved_file.Add(empty);
				num2++;
			}
			binaryReader.Close();
			fileStream.Close();
		}
		catch (Exception)
		{
		}
	}

	public bool CheckDataManager(string dataname)
	{
		for (int i = 0; i < data_saved_file.Count; i++)
		{
			if ((string)data_saved_file[i] == dataname)
			{
				return true;
			}
		}
		return false;
	}

	public void SaveDataManager()
	{
		string path = globalVal.getSavePath() + "datafile.sav";
		string text = string.Empty;
		for (int i = 0; i < data_saved_file.Count; i++)
		{
			text = text + (string)data_saved_file[i] + '|';
		}
		if (!(text == string.Empty))
		{
			MonoBehaviour.print(text);
			FileStream fileStream = new FileStream(path, FileMode.Create);
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(text);
			binaryWriter.Close();
			fileStream.Close();
		}
	}

	public void UpdateDataManager(string dataname)
	{
		string path = globalVal.getSavePath() + "datafile.sav";
		if (File.Exists(path))
		{
			ReadDataManager();
			if (!CheckDataManager(dataname))
			{
				data_saved_file.Add(dataname);
				SaveDataManager();
			}
		}
		else
		{
			data_saved_file.Add(dataname);
			SaveDataManager();
		}
	}
}
