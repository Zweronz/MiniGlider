using UnityEngine;

public class GameObjectMove : MonoBehaviour
{
	private GameObjectMoveData moveData;

	private bool canMove;

	private int posIndex;

	private void Start()
	{
	}

	private void Update()
	{
		if (!canMove || !(moveData != null))
		{
			return;
		}
		if (posIndex < moveData.trans.Length - 1)
		{
			Vector3 value = moveData.trans[posIndex + 1].position - base.transform.position;
			value = Vector3.Normalize(value);
			Vector3 vector = value * moveData.speed * Time.deltaTime;
			if (Vector3.Distance(moveData.trans[posIndex + 1].position, base.transform.position) <= 0.5f)
			{
				posIndex++;
				if (posIndex >= moveData.trans.Length - 1)
				{
					canMove = false;
					posIndex = 0;
					if (base.name == "ani")
					{
						Camera.main.SendMessage("SetCameraState", StoryCameraState.MOVEUP);
					}
					return;
				}
				value = moveData.trans[posIndex + 1].position - base.transform.position;
				value = Vector3.Normalize(value);
				vector = value * moveData.speed * Time.deltaTime;
			}
			base.transform.position = base.transform.position + vector;
			Vector3 position = moveData.trans[posIndex + 1].position;
			base.transform.LookAt(Vector3.Lerp(base.transform.position, position, 0.3f));
		}
		else
		{
			if (base.name == "ani")
			{
				Camera.main.SendMessage("SetCameraState", StoryCameraState.MOVEUP);
			}
			posIndex = 0;
			canMove = false;
		}
	}

	public void SetTransList(GameObjectMoveData transList)
	{
		moveData = transList;
	}

	public void StopMove()
	{
		canMove = false;
	}

	public void StartMove(GameObjectMoveData movedata)
	{
		moveData = movedata;
		posIndex = 0;
		canMove = true;
		base.transform.position = moveData.trans[posIndex].position;
	}
}
