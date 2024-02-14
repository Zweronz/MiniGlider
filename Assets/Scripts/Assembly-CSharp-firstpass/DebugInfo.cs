using UnityEngine;

public class DebugInfo : MonoBehaviour
{
	public static bool Enable = true;

	public void OnGUI()
	{
		GUILayout.Label("FPS " + 1f / Time.deltaTime);
	}
}
