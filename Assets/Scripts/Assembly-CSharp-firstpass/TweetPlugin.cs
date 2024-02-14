using System.Runtime.InteropServices;

public class TweetPlugin
{
	[DllImport("__Internal")]
	protected static extern bool Twitter_canTweetStatus();

	[DllImport("__Internal")]
	protected static extern void Twitter_sendMsg(string msg);

	[DllImport("__Internal")]
	protected static extern void Twitter_followUser(string name);

	public static bool CanTweet()
	{
		return Twitter_canTweetStatus();
	}

	public static void SendMsg(string msg)
	{
		Twitter_sendMsg(msg);
	}

	public static void FollowUser(string name)
	{
		Twitter_followUser(name);
	}
}
