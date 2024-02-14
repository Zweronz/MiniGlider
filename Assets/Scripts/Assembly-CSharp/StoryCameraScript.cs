using System.Collections;
using UnityEngine;

public class StoryCameraScript : MonoBehaviour
{
	private GameObject player;

	private StoryCameraState state;

	private Transform baseTrans;

	private Transform endTrans;

	private float startTime;

	private float endTime;

	private void Start()
	{
		player = GameObject.Find("player2/ani");
		base.transform.position = new Vector3(-22.28292f, 7.771163f, -20.14292f);
		baseTrans = GameObject.Find("CameraBaseTrans").transform;
	}

	private void FixedUpdate()
	{
		StoryCameraState storyCameraState = state;
		if ((storyCameraState != StoryCameraState.MOVEUP && storyCameraState != StoryCameraState.CHANGETOREADY) || !(baseTrans != null))
		{
			return;
		}
		startTime += Time.fixedDeltaTime;
		float num = startTime / endTime;
		if (num > 1f)
		{
			num = 1f;
			if (state == StoryCameraState.MOVEUP)
			{
				state = StoryCameraState.NONE;
				StartCoroutine(changeToReady(2f));
			}
			else if (state == StoryCameraState.CHANGETOREADY)
			{
				SlingshotScript slingshotScript = GameObject.Find("Slingshot").GetComponent(typeof(SlingshotScript)) as SlingshotScript;
				slingshotScript.ReStart();
				state = StoryCameraState.NONE;
			}
		}
		Vector3 position = Vector3.Lerp(baseTrans.position, endTrans.position, num);
		Quaternion rotation = Quaternion.Lerp(baseTrans.rotation, endTrans.rotation, num);
		base.transform.position = position;
		base.transform.rotation = rotation;
	}

	private void LateUpdate()
	{
		StoryCameraState storyCameraState = state;
		if (storyCameraState == StoryCameraState.FOLLOW && player != null && player.transform.position.x >= base.transform.position.x)
		{
			Vector3 position = base.transform.position;
			position.x = player.transform.position.x;
			base.transform.position = position;
		}
	}

	private IEnumerator playStoryAni(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		GameObject.Find("equip_apartment_ani").GetComponent<Animation>().Play("cover");
		player.transform.localPosition = new Vector3(2.12f, -6f, -5.287f);
		player.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
		player.GetComponent<Animation>().Play("storyclimb_copy");
	}

	private IEnumerator changeToReady(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		SetCameraState(StoryCameraState.CHANGETOREADY);
	}

	public void SetCameraState(StoryCameraState cameraState)
	{
		switch (cameraState)
		{
		case StoryCameraState.MOVEUP:
			baseTrans.position = base.transform.position;
			baseTrans.rotation = base.transform.rotation;
			endTrans = GameObject.Find("CameraEndTrans").transform;
			startTime = 0f;
			endTime = 4f;
			GameObject.Find("equip_apartment_ani").GetComponent<Animation>().Play("closedoor");
			StartCoroutine(playStoryAni(2f));
			break;
		case StoryCameraState.CHANGETOREADY:
			baseTrans.position = base.transform.position;
			baseTrans.rotation = base.transform.rotation;
			endTrans = GameObject.Find("CameraReadyTrans").transform;
			startTime = 0f;
			endTime = 5f;
			break;
		case StoryCameraState.NONE:
			StopAllCoroutines();
			break;
		}
		state = cameraState;
	}
}
