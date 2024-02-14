using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class IAPPlugin
{
	public enum Status
	{
		kUserCancel = -2,
		kError = -1,
		kBuying = 0,
		kSuccess = 1
	}

	[DllImport("__Internal")]
	protected static extern void PurchaseProduct(string productId, string productCount);

	[DllImport("__Internal")]
	protected static extern int PurchaseStatus();

	[DllImport("__Internal")]
	protected static extern void RestoreProduct();

	[DllImport("__Internal")]
	protected static extern int RestoreStatus();

	[DllImport("__Internal")]
	protected static extern void RestoreGetProductId([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder productId);

	public static void NowPurchaseProduct(string productId, string productCount)
	{
		if (Utils.IsIAPCrack())
		{
			Debug.Log("IsIAPCrack!!!!!!");
		}
		else
		{
			PurchaseProduct(productId, productCount);
		}
	}

	public static int GetPurchaseStatus()
	{
		if (Utils.IsIAPCrack())
		{
			return -3;
		}
		return PurchaseStatus();
	}

	public static void DoRestoreProduct()
	{
		RestoreProduct();
	}

	public static int DoRestoreStatus()
	{
		int num = 1;
		return RestoreStatus();
	}

	public static string[] DoRestoreGetProductId()
	{
		string empty = string.Empty;
		StringBuilder stringBuilder = new StringBuilder(1024);
		RestoreGetProductId(stringBuilder);
		empty = stringBuilder.ToString();
		return empty.Split('|');
	}
}
