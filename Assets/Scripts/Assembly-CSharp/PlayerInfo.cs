using UnityEngine;

public class PlayerInfo
{
	public float height;

	public float speed;

	public float distance;

	public int gold;

	public int goldCount;

	public int heightUp10Distance;

	public int firstTouchFloorDistance;

	public int score;

	public ulong breakZombiesCount;

	public int leftUseTime;

	public int rightUseTime;

	public float playerFlyDis;

	public float heightUp1500Time;

	public float heightDown50Distance;

	public int huojianAddAgain;

	public bool huojianKey;

	public int huojianAddLv;

	public int GetHuojianAddMax()
	{
		return (int)Mathf.Pow(2f, huojianAddLv) * 50;
	}

	public void Reset()
	{
		height = 0f;
		speed = 0f;
		distance = 0f;
		gold = 0;
		goldCount = 0;
		heightUp10Distance = 0;
		firstTouchFloorDistance = 0;
		score = 0;
		breakZombiesCount = 0uL;
		playerFlyDis = 0f;
		leftUseTime = 0;
		rightUseTime = 0;
		heightUp1500Time = 0f;
		heightDown50Distance = 0f;
		huojianAddAgain = 0;
		huojianKey = false;
		huojianAddLv = 0;
	}
}
