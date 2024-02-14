using UnityEngine;

public class loading : MonoBehaviour
{
	private float curRealTime;

	private float lastRealTime;

	private float _time;

	private float _endTime;

	private bool _key;

	private float startTime;

	private float endTime;

	private bool _update = true;

	private string _scene = string.Empty;

	private TUIButtonPush _loading;

	private void Start()
	{
		_loading = base.transform.Find("loading").GetComponent(typeof(TUIButtonPush)) as TUIButtonPush;
		lastRealTime = Time.realtimeSinceStartup;
		OnTimeChangeScene(3f, globalVal.loadingSceneName);
		globalVal.SetOpenClickShow(true);
	}

	private void Update()
	{
		curRealTime = Time.realtimeSinceStartup - lastRealTime;
		lastRealTime = Time.realtimeSinceStartup;
		_time += curRealTime;
		if (_time > _endTime)
		{
			_endTime += 1f;
			_loading.SetPressed(_key);
			_key = !_key;
		}
		if (_update)
		{
			if (startTime > endTime)
			{
				_update = false;
				Application.LoadLevel(_scene);
			}
			else
			{
				startTime += curRealTime;
			}
		}
	}

	private void OnTimeChangeScene(float waitTime, string sceneName)
	{
		startTime = 0f;
		endTime = waitTime;
		_update = true;
		_scene = sceneName;
	}
}
