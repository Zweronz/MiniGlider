using System;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
	public GameObject playerBip01;

	public IdleState state;

	public TUIRect uiRect;

	private GameObject aniTrans;

	private Rect viewOffset = new Rect(-2f, 0f, -4f, 0f);

	private float baseFloorY = 30f;

	private float baseCameraZ = -10f;

	public AnimList playerAnim;

	private Vector2 lastPoint = Vector2.zero;

	private Transform playerPoint;

	private bool powerClipKey;

	private TAudioController audios;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("TAudioController");
		if (gameObject == null)
		{
			gameObject = UnityEngine.Object.Instantiate(Resources.Load("TAudioController")) as GameObject;
			gameObject.name = "TAudioController";
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		audios = gameObject.GetComponent(typeof(TAudioController)) as TAudioController;
		CannonClass.body = this;
		playerBip01.transform.parent.position = base.transform.position;
		playerBip01.transform.parent.eulerAngles = new Vector3(0f, 90f, 0f);
		playerBip01.GetComponent<Rigidbody>().velocity = Vector3.zero;
		playerBip01.GetComponent<Rigidbody>().isKinematic = true;
		playerBip01.GetComponent<Rigidbody>().useGravity = false;
		playerPoint = GameObject.Find("cannon/cannon/playerPos").transform;
		playerBip01.transform.position = playerPoint.position;
		playerBip01.transform.localRotation = playerPoint.rotation;
		playerBip01.transform.localEulerAngles = new Vector3(270f, 90f, 0f);
		Physics.gravity = new Vector3(-0.5f, -9.8f, 0f);
		aniTrans = playerBip01.transform.parent.Find("ani").gameObject;
		ShowAnimation(true);
		Camera.main.transform.position = new Vector3(6f, 4.65795f, -10f);
		aniTrans.transform.position = playerBip01.transform.position;
		aniTrans.transform.localEulerAngles = Vector3.zero;
		aniTrans.GetComponent<Animation>().Play("flyupsupperman");
		state = IdleState.STAND;
	}

	private void FixedUpdate()
	{
	}

	private void Update()
	{
		switch (state)
		{
		case IdleState.STAND:
			UpdateInput();
			if (playerPoint != null)
			{
				playerBip01.transform.position = playerPoint.position;
				playerBip01.transform.localEulerAngles = new Vector3(270f, 90f, 0f);
				aniTrans.transform.position = playerBip01.transform.position;
				aniTrans.transform.rotation = playerPoint.rotation;
				Rect rect = uiRect.rect;
				if (rect.height <= 1f)
				{
					powerClipKey = false;
				}
				else if (rect.height >= 112f)
				{
					powerClipKey = true;
				}
				rect.height = ((!powerClipKey) ? (rect.height + 1f * Time.deltaTime * 100f) : (rect.height - 1f * Time.deltaTime * 100f));
				uiRect.rect = rect;
			}
			break;
		case IdleState.SHOT_FLYING:
		{
			Vector3 zero = Vector3.zero;
			switch (playerAnim)
			{
			case AnimList.ANIM1:
				zero = playerBip01.transform.position;
				if (!aniTrans.GetComponent<Animation>().isPlaying)
				{
					aniTrans.GetComponent<Rigidbody>().velocity = Vector3.zero;
					playerAnim = AnimList.NONE;
				}
				break;
			case AnimList.ANIM2:
			{
				zero = playerBip01.transform.position;
				float num2 = Vector3.Angle(Vector3.up, playerBip01.GetComponent<Rigidbody>().velocity);
				Vector3 zero3 = Vector3.zero;
				zero3.x = num2 - 90f;
				aniTrans.transform.localEulerAngles = zero3;
				if (!aniTrans.GetComponent<Animation>().isPlaying)
				{
					aniTrans.GetComponent<Rigidbody>().velocity = Vector3.zero;
					playerAnim = AnimList.NONE;
				}
				break;
			}
			default:
			{
				float num = Vector3.Angle(Vector3.up, playerBip01.GetComponent<Rigidbody>().velocity);
				Vector3 zero2 = Vector3.zero;
				zero2.x = num - 90f;
				aniTrans.transform.localEulerAngles = zero2;
				zero = playerBip01.transform.position;
				break;
			}
			}
			aniTrans.transform.position = playerBip01.transform.position;
			zero.z = 0f;
			Vector3 position = Camera.main.transform.position;
			if (zero.x > position.x + viewOffset.x)
			{
				position.x = zero.x + viewOffset.x * -1f;
			}
			if (zero.y > position.y + viewOffset.width)
			{
				position.y = zero.y + viewOffset.width * -1f;
			}
			if (zero.y < position.y + viewOffset.height)
			{
				position.y = zero.y + viewOffset.height * -1f;
			}
			if (zero.y < baseFloorY)
			{
				position.z = baseCameraZ + zero.y * -0.25f;
			}
			if (position.y < 4f)
			{
				position.y = 4f;
			}
			if (position.z > -10f)
			{
				position.z = -10f;
			}
			Camera.main.transform.position = position;
			break;
		}
		case IdleState.SHOTING:
			break;
		}
	}

	public void ShowAnimation(bool iscan)
	{
		ShowRagdoll(!iscan);
		ShowAniBody(iscan);
	}

	private void ShowRagdoll(bool iscan)
	{
		Transform transform = playerBip01.transform.parent.Find("Cowboy");
		if ((bool)transform)
		{
			transform.GetComponent<Renderer>().enabled = iscan;
		}
	}

	private void ShowAniBody(bool iscan)
	{
		Transform transform = playerBip01.transform.parent.Find("ani/Cowboy");
		if ((bool)transform)
		{
			transform.GetComponent<Renderer>().enabled = iscan;
		}
	}

	private void UpdateInput()
	{
		UITouchInner[] array = iPhoneInputMgr.MockTouches();
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].phase == TouchPhase.Began)
			{
				OnMouseDown(array[i].position);
			}
			if (array[i].phase == TouchPhase.Moved)
			{
				OnMouseMove(array[i].position);
			}
			if (array[i].phase == TouchPhase.Ended)
			{
				OnMouseUp(array[i].position);
			}
		}
	}

	private void OnMouseDown(Vector2 mousePosition)
	{
		lastPoint = mousePosition;
	}

	private void OnMouseMove(Vector2 mousePosition)
	{
		float num = mousePosition.y - lastPoint.y;
		Vector3 eulerAngles = playerPoint.parent.eulerAngles;
		eulerAngles.x += num * Time.fixedTime * 0.02f;
		eulerAngles.y = 270f;
		eulerAngles.z = 180f;
		if (eulerAngles.x < 270f)
		{
			eulerAngles.x = 270f;
		}
		if (eulerAngles.x > 359f)
		{
			eulerAngles.x = 359f;
		}
		playerPoint.parent.eulerAngles = eulerAngles;
		lastPoint = mousePosition;
	}

	private void OnMouseUp(Vector2 mousePosition)
	{
	}

	public void CannonFire()
	{
		if (state == IdleState.STAND)
		{
			playerBip01.GetComponent<Rigidbody>().useGravity = true;
			playerBip01.GetComponent<Rigidbody>().isKinematic = false;
			float num = 360f - playerPoint.parent.eulerAngles.x % 360f;
			float num2 = 400f + uiRect.rect.height * 5f;
			Vector3 zero = Vector3.zero;
			zero.x = Mathf.Sin(num * ((float)Math.PI / 180f)) * num2;
			zero.y = Mathf.Cos(num * ((float)Math.PI / 180f)) * num2;
			MonoBehaviour.print(zero);
			Transform itemTrans = base.transform.Find("cannon/cannonfire");
			EffectManagerClass.body.PlayEffect("effect_cannonfire", itemTrans);
			playerBip01.GetComponent<Rigidbody>().AddForce(zero, ForceMode.Impulse);
			state = IdleState.SHOT_FLYING;
			ShowAnimation(false);
			audios.PlayAudio("FXcannon");
			MonoBehaviour.print("mouse up");
		}
	}

	public void ReStart()
	{
		Start();
	}

	public void GameOver()
	{
		state = IdleState.GAMEOVER;
		GameObject gameObject = GameObject.Find("TUI/TUIControl");
		gameUI gameUI2 = gameObject.GetComponent(typeof(gameUI)) as gameUI;
		gameUI2.InitGameOver();
	}
}
