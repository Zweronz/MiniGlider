using System.Runtime.InteropServices;
using UnityEngine;

public class OpenClickPlugin
{
	[DllImport("__Internal")]
	protected static extern void OpenClik_Initialize(string key);

	[DllImport("__Internal")]
	protected static extern void OpenClik_Show(bool show_full);

	[DllImport("__Internal")]
	protected static extern void OpenClik_Hide();

	public static void Initialize(string key)
	{
		OpenClik_Initialize(key);
		Debug.Log("OpenClickPlugin Initialize key : " + key);
	}

	public static void Show(bool show_full)
	{
		OpenClik_Show(show_full);
		Debug.Log("OpenClickPlugin Show : " + show_full);
	}

	public static void Hide()
	{
		OpenClik_Hide();
		Debug.Log("OpenClickPlugin Hide");
	}
}
