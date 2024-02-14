using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class GameCenterPlugin
{
	[DllImport("__Internal")]
	protected static extern void GMInitialize();

	[DllImport("__Internal")]
	protected static extern void GMUninitialize();

	[DllImport("__Internal")]
	protected static extern bool GMIsSupported();

	[DllImport("__Internal")]
	protected static extern bool GMIsLogin();

	[DllImport("__Internal")]
	protected static extern bool GMLogin();

	[DllImport("__Internal")]
	protected static extern int GMLoginStatus();

	[DllImport("__Internal")]
	protected static extern bool GMGetAccount([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder account);

	[DllImport("__Internal")]
	protected static extern bool GMGetName([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder name);

	[DllImport("__Internal")]
	protected static extern bool GMSubmitScore(string category, int score);

	[DllImport("__Internal")]
	protected static extern int GMSubmitScoreStatus(string category, int score);

	[DllImport("__Internal")]
	protected static extern bool GMSubmitAchievement(string category, int percent);

	[DllImport("__Internal")]
	protected static extern int GMSubmitAchievementStatus(string category, int percent);

	[DllImport("__Internal")]
	protected static extern bool GMOpenLeaderboard();

	[DllImport("__Internal")]
	protected static extern bool GMOpenLeaderboardForCategory(string category);

	[DllImport("__Internal")]
	protected static extern bool GMOpenAchievement();

	public static void Initialize()
	{
		if (!Application.isEditor)
		{
			GMInitialize();
		}
	}

	public static void Uninitialize()
	{
		if (!Application.isEditor)
		{
			GMUninitialize();
		}
	}

	public static bool IsSupported()
	{
		bool result = true;
		if (!Application.isEditor)
		{
			result = GMIsSupported();
		}
		return result;
	}

	public static bool IsLogin()
	{
		bool result = false;
		if (!Application.isEditor)
		{
			result = GMIsLogin();
		}
		return result;
	}

	public static bool Login()
	{
		bool result = false;
		if (!Application.isEditor)
		{
			result = GMLogin();
		}
		return result;
	}

	public static int LoginStatus()
	{
		int result = 0;
		if (!Application.isEditor)
		{
			result = GMLoginStatus();
		}
		return result;
	}

	public static string GetAccount()
	{
		string result = string.Empty;
		if (!Application.isEditor)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			if (GMGetAccount(stringBuilder))
			{
				result = stringBuilder.ToString();
			}
		}
		return result;
	}

	public static string GetName()
	{
		string result = string.Empty;
		if (!Application.isEditor)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			if (GMGetName(stringBuilder))
			{
				result = stringBuilder.ToString();
			}
		}
		return result;
	}

	public static bool SubmitScore(string category, int score)
	{
		bool result = false;
		if (!Application.isEditor)
		{
			result = GMSubmitScore(category, score);
		}
		return result;
	}

	public static int SubmitScoreStatus(string category, int score)
	{
		int result = 0;
		if (!Application.isEditor)
		{
			result = GMSubmitScoreStatus(category, score);
		}
		return result;
	}

	public static bool SubmitAchievement(string category, int percent)
	{
		bool result = false;
		if (!Application.isEditor)
		{
			result = GMSubmitAchievement(category, percent);
		}
		return result;
	}

	public static int SubmitAchievementStatus(string category, int percent)
	{
		int result = 0;
		if (!Application.isEditor)
		{
			result = GMSubmitAchievementStatus(category, percent);
		}
		return result;
	}

	public static bool OpenLeaderboard()
	{
		bool result = false;
		if (!Application.isEditor)
		{
			result = GMOpenLeaderboard();
		}
		return result;
	}

	public static bool OpenLeaderboard(string category)
	{
		bool result = false;
		if (!Application.isEditor)
		{
			result = GMOpenLeaderboardForCategory(category);
		}
		return result;
	}

	public static bool OpenAchievement()
	{
		bool result = false;
		if (!Application.isEditor)
		{
			result = GMOpenAchievement();
		}
		return result;
	}
}
