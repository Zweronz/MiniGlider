using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TUICamera : MonoBehaviour
{
	public void Initialize(bool landscape, int layer, int depth)
	{
		float width;
		float height;
		bool hd;
		GetScreenInfo(out width, out height, out hd);
		if (landscape)
		{
			float num = width;
			width = height;
			height = num;
		}
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
		base.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
		base.GetComponent<Camera>().backgroundColor = Color.white;
		base.GetComponent<Camera>().nearClipPlane = -128f;
		base.GetComponent<Camera>().farClipPlane = 128f;
		base.GetComponent<Camera>().orthographic = true;
		base.GetComponent<Camera>().depth = depth;
		base.GetComponent<Camera>().cullingMask = 1 << layer;
		base.GetComponent<Camera>().aspect = width / height;
		base.GetComponent<Camera>().orthographicSize = height / ((!hd) ? 2f : 4f);
	}

	private void GetScreenInfo(out float width, out float height, out bool hd)
	{
		width = 0f;
		height = 0f;
		hd = false;
		if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone3G || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone3GS || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen)
		{
			width = 320f;
			height = 480f;
			hd = false;
		}
		else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone4 || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen)
		{
			width = 640f;
			height = 960f;
			hd = true;
		}
		else
		{
			width = 768f;
			height = 1024f;
			hd = true;
		}
	}
}
