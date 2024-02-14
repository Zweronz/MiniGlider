using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class Utils
{
	private static string m_SavePath;

	static Utils()
	{
		string text = Application.dataPath;
		if (!Application.isEditor)
		{
			text = text.Substring(0, text.LastIndexOf('/'));
			text = text.Substring(0, text.LastIndexOf('/'));
		}
		text += "/Documents";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		m_SavePath = text;
	}

	public static bool CreateDocumentSubDir(string dirname)
	{
		string path = m_SavePath + "/" + dirname;
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
			return true;
		}
		return false;
	}

	public static void DeleteDocumentDir(string dirname)
	{
		string path = m_SavePath + "/" + dirname;
		if (Directory.Exists(path))
		{
			Directory.Delete(path, true);
		}
	}

	public static string SavePath()
	{
		return m_SavePath;
	}

	public static string GetTextAsset(string txt_name)
	{
		TextAsset textAsset = Resources.Load(txt_name) as TextAsset;
		if (null != textAsset)
		{
			return textAsset.text;
		}
		return string.Empty;
	}

	public static void FileSaveString(string name, string content)
	{
		string text = SavePath() + "/" + name;
		try
		{
			FileStream fileStream = new FileStream(text, FileMode.Create);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(content);
			streamWriter.Close();
			fileStream.Close();
		}
		catch
		{
			Debug.Log("Save" + text + " error");
		}
	}

	public static void FileGetString(string name, ref string content)
	{
		string text = SavePath() + "/" + name;
		if (!File.Exists(text))
		{
			return;
		}
		try
		{
			FileStream fileStream = new FileStream(text, FileMode.Open);
			StreamReader streamReader = new StreamReader(fileStream);
			content = streamReader.ReadToEnd();
			streamReader.Close();
			fileStream.Close();
		}
		catch
		{
			Debug.Log("Load" + text + " error");
		}
	}

	public static string SaveBinaryData(string name, byte[] data)
	{
		string text = SavePath() + "/" + name;
		FileStream fileStream = new FileStream(text, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		binaryWriter.Write(data);
		binaryWriter.Close();
		fileStream.Close();
		return text;
	}

	public static bool IsChineseLetter(string input)
	{
		for (int i = 0; i < input.Length; i++)
		{
			int num = Convert.ToInt32(Convert.ToChar(input.Substring(i, 1)));
			if (num >= 128)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsIpad()
	{
		if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPad1Gen || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPad2Gen)
		{
			return true;
		}
		return false;
	}

	public static int ScreenType()
	{
		int result = -1;
		if (Screen.width > 1024)
		{
			result = 2;
		}
		else if (Screen.width > 960 && Screen.width <= 1024)
		{
			result = 1;
		}
		else if (Screen.width <= 960)
		{
			result = 0;
		}
		return result;
	}

	public static bool IsRetina()
	{
		if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone4 || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen)
		{
			return true;
		}
		return false;
	}

	[DllImport("__Internal")]
	protected static extern void OSGetMAC([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder output);

	public static string GetMacAddr()
	{
		StringBuilder stringBuilder = new StringBuilder(256);
		OSGetMAC(stringBuilder);
		return stringBuilder.ToString();
	}

	[DllImport("__Internal")]
	protected static extern void SendMail(string address, string subject, string content);

	public static void ToSendMail(string address, string subject, string content)
	{
		SendMail(address, subject, content);
	}

	[DllImport("__Internal")]
	protected static extern int MessgeBox1(string title, string message, string button);

	[DllImport("__Internal")]
	protected static extern int MessgeBox2(string title, string message, string button1, string button2);

	public static int ShowMessageBox1(string title, string message, string button)
	{
		return MessgeBox1(title, message, button);
	}

	public static int ShowMessageBox2(string title, string message, string button1, string button2)
	{
		return MessgeBox2(title, message, button1, button2);
	}

	[DllImport("__Internal")]
	protected static extern void SavePhotoToCameraRoll(byte[] bytes, int width, int height);

	[DllImport("__Internal")]
	protected static extern void SavePhotoToCameraRollFile(string filename);

	public static void SavePhoto(int photo_index, int width, int height)
	{
		string path = null;
		switch (photo_index)
		{
		case 0:
			path = "Model1";
			break;
		case 1:
			path = "Model2";
			break;
		case 2:
			path = "Model3";
			break;
		case 3:
			path = "Model4";
			break;
		}
		TextAsset textAsset = Resources.Load(path, typeof(TextAsset)) as TextAsset;
		byte[] bytes = textAsset.bytes;
		string filename = SaveBinaryData("tem.png", bytes);
		SavePhotoToCameraRollFile(filename);
	}

	[DllImport("__Internal")]
	protected static extern int CheckPhotoSaveStatus();

	public static int OnCheckPhotoSaveStatus()
	{
		return CheckPhotoSaveStatus();
	}

	[DllImport("__Internal")]
	protected static extern void ResetPhotoSaveStatus();

	public static void OnResetPhotoSaveStatus()
	{
		ResetPhotoSaveStatus();
	}

	[DllImport("__Internal")]
	protected static extern void OpenCameraRoll();

	public static void OpenLocalCameraRoll()
	{
		OpenCameraRoll();
	}

	[DllImport("__Internal")]
	protected static extern void ShowIndicator(int style, bool iPad, float r, float g, float b, float a);

	public static void ShowIndicatorSystem(int style, bool iPad, float r, float g, float b, float a)
	{
		ShowIndicator(style, iPad, r, g, b, a);
	}

	[DllImport("__Internal")]
	protected static extern void ShowIndicator_int(int style, int iPad, float r, float g, float b, float a);

	public static void ShowIndicatorSystem_int(int style, int iPad, float r, float g, float b, float a)
	{
		ShowIndicator_int(style, iPad, r, g, b, a);
	}

	[DllImport("__Internal")]
	protected static extern void HideIndicator();

	public static void HideIndicatorSystem()
	{
		HideIndicator();
	}

	[DllImport("__Internal")]
	protected static extern int GetIOSYearVal();

	public static int GetIOSYear()
	{
		return 0;
//		return GetIOSYearVal();
	}

	[DllImport("__Internal")]
	protected static extern int GetIOSMonthVal();

	public static int GetIOSMonth()
	{
		return 0;
	//	return GetIOSMonthVal();
	}

	[DllImport("__Internal")]
	protected static extern int GetIOSDayVal();

	public static int GetIOSDay()
	{
		return 0;
		//return GetIOSDayVal();
	}

	[DllImport("__Internal")]
	protected static extern int GetIOSHourVal();

	public static int GetIOSHour()
	{
		return 0;
		//return GetIOSHourVal();
	}

	[DllImport("__Internal")]
	protected static extern int GetIOSMinVal();

	public static int GetIOSMin()
	{
		return 0;
		//return GetIOSMinVal();
	}

	[DllImport("__Internal")]
	protected static extern int GetIOSSecVal();

	public static int GetIOSSec()
	{
		return 0;
		//return GetIOSSecVal();
	}

	public static long GetSystemSecond()
	{
		TimeSpan timeSpan = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
		return (long)timeSpan.TotalMilliseconds;
	}

	[DllImport("__Internal")]
	protected static extern bool OSIsJailbreak();

	public static bool IsJailbreak()
	{
		return false;
		//return OSIsJailbreak();
	}

	[DllImport("__Internal")]
	protected static extern bool OSIsIAPCrack();

	public static bool IsIAPCrack()
	{
		return false;
		//return OSIsIAPCrack();
	}
}
