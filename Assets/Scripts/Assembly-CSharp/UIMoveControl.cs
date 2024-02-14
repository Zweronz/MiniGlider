using UnityEngine;

public class UIMoveControl : MonoBehaviour
{
	private UIMoveData startData;

	private UIMoveData moveData;

	private UIMoveData endData;

	private float startTime;

	private float endTime;

	private int state;

	private float percent;

	private float totalTime;

	private bool canMove;

	private float begainTime;

	private float canMoveTime;

	private float curRealTime;

	private float lastRealTime;

	private Transform call_trans;

	private string call_methodName = string.Empty;

	private bool canCallBack;

	private object call_value;

	public Vector3 endPos = Vector3.zero;

	private void Start()
	{
		lastRealTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		curRealTime = Time.realtimeSinceStartup - lastRealTime;
		lastRealTime = Time.realtimeSinceStartup;
		canMoveTime += curRealTime;
		if (begainTime > canMoveTime || !canMove)
		{
			return;
		}
		totalTime += curRealTime;
		switch (state)
		{
		case 1:
			percent = totalTime / startTime;
			if (percent >= 1f)
			{
				percent = 1f;
				state = 2;
				totalTime -= startTime;
			}
			base.transform.localPosition = Vector3.Lerp(startData.pos, moveData.pos, percent);
			base.transform.localScale = Vector3.Lerp(startData.size, moveData.size, percent);
			base.transform.localRotation = Quaternion.Lerp(startData.rotate, moveData.rotate, percent);
			break;
		case 2:
			percent = totalTime / endTime;
			if (percent >= 1f)
			{
				canMove = false;
				percent = 1f;
				if (canCallBack)
				{
					canCallBack = false;
					base.transform.localPosition = Vector3.Lerp(moveData.pos, endData.pos, percent);
					base.transform.localScale = Vector3.Lerp(moveData.size, endData.size, percent);
					base.transform.localRotation = Quaternion.Lerp(moveData.rotate, endData.rotate, percent);
					if (call_value != null)
					{
						call_trans.SendMessage(call_methodName, call_value);
					}
					else
					{
						call_trans.SendMessage(call_methodName);
					}
					break;
				}
			}
			base.transform.localPosition = Vector3.Lerp(moveData.pos, endData.pos, percent);
			base.transform.localScale = Vector3.Lerp(moveData.size, endData.size, percent);
			base.transform.localRotation = Quaternion.Lerp(moveData.rotate, endData.rotate, percent);
			break;
		}
	}

	public void SetCallBack(Transform parent, string methodName)
	{
		canCallBack = true;
		call_trans = parent;
		call_methodName = methodName;
		call_value = null;
	}

	public void SetCallBack(Transform parent, string methodName, object value)
	{
		canCallBack = true;
		call_trans = parent;
		call_methodName = methodName;
		call_value = value;
	}

	public void SetEndPos(Vector3 pos)
	{
		endPos = pos;
		base.transform.localPosition = endPos + new Vector3(400f, 0f, 0f);
	}

	public void SetEndPos(float x, float y, float z)
	{
		endPos.x = x;
		endPos.y = y;
		endPos.z = z;
		Vector3 zero = Vector3.zero;
		zero.x = x;
		zero.y = y + 1000f;
		zero.z = z;
		base.transform.localPosition = zero;
	}

	public void SetTargetPos(float x, float y, float z)
	{
		endPos.x = x;
		endPos.y = y;
		endPos.z = z;
	}

	public void SetPos(Vector3 pos)
	{
		base.transform.localPosition = pos;
	}

	public void SetPos(float x, float y, float z)
	{
		Vector3 zero = Vector3.zero;
		zero.x = x;
		zero.y = y;
		zero.z = z;
		base.transform.localPosition = zero;
	}

	public void OnTimeCallBack(float waitTime, Transform parent, string methodName)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = base.transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = base.transform.localPosition;
		uIMoveData2.pos = base.transform.localPosition;
		uIMoveData3.pos = base.transform.localPosition;
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0f, 0f);
		SetCallBack(parent, methodName);
	}

	public void OnTimeCallBack(float waitTime, Transform parent, string methodName, object value)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = base.transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = base.transform.localPosition;
		uIMoveData2.pos = base.transform.localPosition;
		uIMoveData3.pos = base.transform.localPosition;
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0f, 0f);
		SetCallBack(parent, methodName, value);
	}

	public void StartMove(UIMoveData startdata, UIMoveData movedata, UIMoveData enddata, float starttime, float endtime)
	{
		startData = startdata;
		moveData = movedata;
		endData = enddata;
		startTime = starttime;
		endTime = endtime;
		state = 1;
		totalTime = 0f;
		canMove = true;
		base.transform.localPosition = startData.pos;
		base.transform.localScale = startData.size;
		base.transform.localRotation = startData.rotate;
	}

	public void StopMove()
	{
		startTime = 0f;
		endTime = 0f;
		totalTime = 0f;
		canMove = false;
		canCallBack = false;
	}

	public void CurToTargetMove(float waitTime, Transform trans, float moveTime)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = new Vector3(trans.localPosition.x, trans.localPosition.y, trans.localPosition.z);
		uIMoveData2.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData3.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, moveTime, 0f);
	}

	public void UpToDown(float waitTime, Transform trans)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y + 200f, uIMoveControl.endPos.z);
		uIMoveData2.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y - 5f, uIMoveControl.endPos.z);
		uIMoveData3.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0.3f, 0.05f);
	}

	public void UpToDown_back(float waitTime, Transform trans)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData2.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y - 5f, uIMoveControl.endPos.z);
		uIMoveData3.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y + 200f, uIMoveControl.endPos.z);
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0.05f, 0.3f);
	}

	public void LeftToRight(float waitTime, Transform trans)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = new Vector3(uIMoveControl.endPos.x - 500f, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData2.pos = new Vector3(uIMoveControl.endPos.x + 20f, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData3.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0.3f, 0.1f);
	}

	public void LeftToRight_back(float waitTime, Transform trans)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData2.pos = new Vector3(uIMoveControl.endPos.x + 20f, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData3.pos = new Vector3(uIMoveControl.endPos.x - 500f, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0.5f, 0.1f);
	}

	public void RightToLeft_taskback(float waitTime, Transform trans)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData2.pos = new Vector3(uIMoveControl.endPos.x + 10f, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData3.pos = new Vector3(uIMoveControl.endPos.x - 265f, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0.1f, 0.5f);
	}

	public void RightToLeft_taskback_back(float waitTime, Transform trans)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = new Vector3(uIMoveControl.endPos.x - 265f, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData2.pos = new Vector3(uIMoveControl.endPos.x + 10f, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData3.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0.5f, 0.1f);
	}

	public void LeftToRight_taskpop(float waitTime, Transform trans)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = new Vector3(uIMoveControl.endPos.x - 266f, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData2.pos = new Vector3(uIMoveControl.endPos.x + 10f, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData3.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0.5f, 0.1f);
	}

	public void LeftToRight_taskpop_back(float waitTime, Transform trans)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData2.pos = new Vector3(uIMoveControl.endPos.x + 10f, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData3.pos = new Vector3(uIMoveControl.endPos.x - 265f, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0.1f, 0.5f);
	}

	public void RightToLeft(float waitTime, Transform trans)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = new Vector3(uIMoveControl.endPos.x + 500f, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData2.pos = new Vector3(uIMoveControl.endPos.x - 20f, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData3.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0.7f, 0.2f);
	}

	public void RightToLeft_back(float waitTime, Transform trans)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData2.pos = new Vector3(uIMoveControl.endPos.x - 20f, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData3.pos = new Vector3(uIMoveControl.endPos.x + 500f, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0.2f, 0.7f);
	}

	public void DownToUp(float waitTime, Transform trans)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y - 300f, uIMoveControl.endPos.z);
		uIMoveData2.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y + 5f, uIMoveControl.endPos.z);
		uIMoveData3.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0.7f, 0.2f);
	}

	public void DownToUp_back(float waitTime, Transform trans)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y, uIMoveControl.endPos.z);
		uIMoveData2.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y + 5f, uIMoveControl.endPos.z);
		uIMoveData3.pos = new Vector3(uIMoveControl.endPos.x, uIMoveControl.endPos.y - 300f, uIMoveControl.endPos.z);
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0.2f, 0.7f);
	}

	public void ScaleNoMove(float waitTime, Transform trans)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = uIMoveControl.endPos;
		uIMoveData.size = Vector3.one * 0.1f;
		uIMoveData2.pos = uIMoveControl.endPos;
		uIMoveData2.size = Vector3.one * 1.1f;
		uIMoveData3.pos = uIMoveControl.endPos;
		uIMoveData3.size = Vector3.one;
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0.5f, 0.1f);
	}

	public void Rotate(float waitTime, Transform trans)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = uIMoveControl.endPos;
		uIMoveData.rotate = Quaternion.AngleAxis(-30f, Vector3.forward);
		uIMoveData2.pos = uIMoveControl.endPos;
		uIMoveData2.rotate = Quaternion.AngleAxis(30f, Vector3.forward);
		uIMoveData3.pos = uIMoveControl.endPos;
		uIMoveData3.rotate = Quaternion.AngleAxis(-30f, Vector3.forward);
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0.5f, 0.5f);
	}

	public void ScaleRotate(float waitTime, Transform trans)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = uIMoveControl.endPos;
		uIMoveData.size = Vector3.one * 3.5f;
		uIMoveData.rotate = Quaternion.AngleAxis(0f, Vector3.forward);
		uIMoveData2.pos = uIMoveControl.endPos;
		uIMoveData2.size = Vector3.one * 0.9f;
		uIMoveData2.rotate = Quaternion.AngleAxis(0f, Vector3.forward);
		uIMoveData3.pos = uIMoveControl.endPos;
		uIMoveData3.size = Vector3.one;
		uIMoveData3.rotate = Quaternion.AngleAxis(0f, Vector3.forward);
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0.7f, 0.2f);
	}

	public void ScaleRotate_newbest(float waitTime, Transform trans)
	{
		begainTime = waitTime;
		canMoveTime = 0f;
		UIMoveControl uIMoveControl = trans.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		UIMoveData uIMoveData3 = new UIMoveData();
		uIMoveData.pos = uIMoveControl.endPos;
		uIMoveData.size = Vector3.one * 2f;
		uIMoveData.rotate = Quaternion.AngleAxis(179f, Vector3.forward);
		uIMoveData2.pos = uIMoveControl.endPos;
		uIMoveData2.size = Vector3.one * 0.4f;
		uIMoveData2.rotate = Quaternion.AngleAxis(0f, Vector3.forward);
		uIMoveData3.pos = uIMoveControl.endPos;
		uIMoveData3.size = Vector3.one * 0.5f;
		uIMoveData3.rotate = Quaternion.AngleAxis(0f, Vector3.forward);
		uIMoveControl.StartMove(uIMoveData, uIMoveData2, uIMoveData3, 0.5f, 0.1f);
	}
}
