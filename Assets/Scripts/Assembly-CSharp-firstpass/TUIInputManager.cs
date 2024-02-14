using UnityEngine;

public class TUIInputManager
{
	private static int m_lastFrameCount = -1;

	public static TUIInput[] GetInput()
	{
		if (Time.frameCount != m_lastFrameCount)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				TUIInputManageriOS.UpdateInput();
			}
			else
			{
				TUIInputManagerWindows.UpdateInput();
			}
		}
		m_lastFrameCount = Time.frameCount;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return TUIInputManageriOS.GetInput();
		}
		return TUIInputManagerWindows.GetInput();
	}
}
