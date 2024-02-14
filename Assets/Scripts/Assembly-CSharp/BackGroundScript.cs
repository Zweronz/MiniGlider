using UnityEngine;

public class BackGroundScript : MonoBehaviour
{
	public float totalLength = 640f;

	public SceneForwardState state;

	private float m_changeDis = 1000f;

	private float m_changeDis2 = 800f;

	private float m_changeDis3 = 800f;

	private string[] sceneName = new string[5];

	private void Start()
	{
		sceneName[0] = "scene_suburb";
		sceneName[1] = "scene_suburbturncity";
		sceneName[2] = "scene_city1";
		sceneName[3] = "scene_cityturnhighway";
		sceneName[4] = "scene_highway1";
	}

	private void Update()
	{
		if (state == SceneForwardState.NONE || state == SceneForwardState.SUBURBTOCITY || state == SceneForwardState.CITYTONEXT)
		{
			return;
		}
		Vector3 position = Camera.main.transform.position;
		if (!(base.transform.localPosition.x < position.x - totalLength / 2f))
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x += totalLength;
		if (state == SceneForwardState.CLOUD)
		{
			base.transform.localPosition = localPosition;
			return;
		}
		Transform sceneLastTrans = globalVal.sceneLastTrans;
		BackGroundScript backGroundScript = sceneLastTrans.GetComponent(typeof(BackGroundScript)) as BackGroundScript;
		SceneForwardState stateByDis = GetStateByDis(localPosition.x);
		if (stateByDis == backGroundScript.state)
		{
			if (globalVal.secondKey)
			{
				globalVal.secondKey = false;
				localPosition = sceneLastTrans.localPosition;
				localPosition.x += totalLength * 0.5f;
				if (state == SceneForwardState.SUBURB)
				{
					InitScene(localPosition, sceneName[2]);
				}
				if (state == SceneForwardState.CITY)
				{
					InitScene(localPosition, sceneName[4]);
				}
				if (state == SceneForwardState.NEXT)
				{
					InitScene(localPosition, sceneName[4]);
				}
				base.transform.localPosition = new Vector3(-1000f, 0f, 0f);
				state = SceneForwardState.NONE;
			}
			else
			{
				base.transform.localPosition = localPosition;
			}
		}
		else
		{
			CheckLastAndLoad();
			base.transform.localPosition = new Vector3(-1000f, 0f, 0f);
			if (state == SceneForwardState.SUBURB)
			{
				globalVal.changeDis += m_changeDis2;
			}
			else if (state == SceneForwardState.CITY)
			{
				globalVal.changeDis += m_changeDis3;
			}
			else if (state == SceneForwardState.NEXT)
			{
				globalVal.changeDis += m_changeDis3;
			}
			state = SceneForwardState.NONE;
			globalVal.secondKey = true;
		}
	}

	private SceneForwardState GetStateByDis(float dis)
	{
		SceneForwardState result = SceneForwardState.NONE;
		if (dis > 0f && dis <= m_changeDis)
		{
			result = SceneForwardState.SUBURB;
		}
		else if (dis > m_changeDis && dis <= m_changeDis + m_changeDis2)
		{
			result = SceneForwardState.CITY;
		}
		else if (dis > m_changeDis + m_changeDis2 && dis <= m_changeDis + m_changeDis2 + m_changeDis3)
		{
			result = SceneForwardState.NEXT;
		}
		else if (dis > m_changeDis + m_changeDis2 + m_changeDis3)
		{
			result = SceneForwardState.NEXT;
		}
		return result;
	}

	private void CheckLastAndLoad()
	{
		Transform sceneLastTrans = globalVal.sceneLastTrans;
		BackGroundScript backGroundScript = sceneLastTrans.GetComponent(typeof(BackGroundScript)) as BackGroundScript;
		Vector3 localPosition = globalVal.sceneLastTrans.localPosition;
		if (backGroundScript.state == SceneForwardState.SUBURB)
		{
			Transform transform = base.transform.parent.Find(sceneName[1]);
			transform.localScale = new Vector3(1f, 1f, 1f);
			transform.localPosition = localPosition + new Vector3(totalLength * 0.25f, 0f, 0f) + new Vector3(25f, 0f, 0f);
			Vector3 pos = transform.localPosition + (new Vector3(25f, 0f, 0f) + new Vector3(totalLength * 0.25f, 0f, 0f));
			InitScene(pos, sceneName[2]);
		}
		if (backGroundScript.state == SceneForwardState.CITY)
		{
			Transform transform2 = base.transform.parent.Find(sceneName[3]);
			transform2.localScale = new Vector3(1f, 1f, 1f);
			transform2.localPosition = localPosition + new Vector3(totalLength * 0.25f, 0f, 0f) + new Vector3(60f, 0f, 0f);
			Vector3 pos2 = transform2.localPosition + (new Vector3(60f, 0f, 0f) + new Vector3(totalLength * 0.25f, 0f, 0f));
			InitScene(pos2, sceneName[4]);
		}
	}

	private SceneForwardState GetIdByName(string name)
	{
		SceneForwardState result = SceneForwardState.NONE;
		switch (name)
		{
		case "scene_suburb":
			result = SceneForwardState.SUBURB;
			break;
		case "scene_suburbturncity":
			result = SceneForwardState.SUBURBTOCITY;
			break;
		case "scene_city1":
			result = SceneForwardState.CITY;
			break;
		case "scene_cityturnhighway":
			result = SceneForwardState.CITYTONEXT;
			break;
		case "scene_highway1":
			result = SceneForwardState.NEXT;
			break;
		}
		return result;
	}

	private void InitScene(Vector3 pos, string scene)
	{
		GameObject gameObject = GameObject.Find("scene");
		if (!(gameObject != null))
		{
			return;
		}
		int num = 0;
		foreach (Transform item in gameObject.transform)
		{
			BackGroundScript backGroundScript = item.GetComponent(typeof(BackGroundScript)) as BackGroundScript;
			if (item.name == scene && backGroundScript.state == SceneForwardState.NONE)
			{
				item.localPosition = pos + new Vector3(320 * num, 0f, 0f);
				backGroundScript.state = GetIdByName(scene);
				globalVal.sceneLastTrans = item;
				break;
			}
		}
	}
}
