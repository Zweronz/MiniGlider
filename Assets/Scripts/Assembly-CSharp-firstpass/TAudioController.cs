using System.Collections;
using UnityEngine;

public class TAudioController : MonoBehaviour
{
	public delegate void OnAudioEventPlay(ref string eventName);

	public bool useAuidoEvent = true;

	public bool m_music = true;

	public bool m_sound = true;

	private OnAudioEventPlay onAudioEventPlay;

	private string musicName = string.Empty;

	private ArrayList audioNameArray = new ArrayList();

	private ArrayList audioTransArray = new ArrayList();

	private ArrayList audioSourceArray = new ArrayList();

	private void Start()
	{
		audioNameArray.Clear();
		audioTransArray.Clear();
		audioSourceArray.Clear();
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void ResetFollowData()
	{
		audioNameArray.Clear();
		audioTransArray.Clear();
		audioSourceArray.Clear();
	}

	public string GetMusicName()
	{
		return musicName;
	}

	private void Update()
	{
		for (int i = 0; i < audioNameArray.Count; i++)
		{
			AudioSource audioSource = audioSourceArray[i] as AudioSource;
			Transform transform = audioTransArray[i] as Transform;
			if (transform == null)
			{
				audioNameArray.RemoveAt(i);
				audioTransArray.RemoveAt(i);
				audioSourceArray.RemoveAt(i);
			}
			else
			{
				audioSource.transform.localPosition = transform.position;
			}
		}
	}

	public void ReplayMusic()
	{
		string text = musicName;
		musicName = string.Empty;
		PlayMusic(text);
		MonoBehaviour.print("replay : " + text);
	}

	public void PlayMusic(string objName)
	{
		MonoBehaviour.print("music : " + objName);
		if (musicName == objName || !useAuidoEvent)
		{
			return;
		}
		if (onAudioEventPlay != null)
		{
			onAudioEventPlay(ref objName);
		}
		Transform transform = base.transform.Find("Audio");
		if (null == transform)
		{
			GameObject gameObject = new GameObject("Audio");
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			transform = gameObject.transform;
		}
		int num = objName.LastIndexOf('/');
		num++;
		string text = objName.Substring(num);
		if (musicName != string.Empty)
		{
			StopAudio(musicName);
		}
		GameObject gameObject2 = null;
		Transform transform2 = base.transform.Find("Audio/" + text);
		if (null == transform2)
		{
			gameObject2 = Resources.Load("SoundEvent/" + objName) as GameObject;
			if (null == gameObject2)
			{
				Debug.LogWarning(objName + " is null");
				return;
			}
			gameObject2 = Object.Instantiate(gameObject2) as GameObject;
			if (null == gameObject2)
			{
				Debug.LogWarning(objName + " is null");
				return;
			}
			gameObject2.name = text;
			gameObject2.transform.parent = transform;
			gameObject2.transform.localPosition = Vector3.zero;
		}
		else
		{
			gameObject2 = transform2.gameObject;
		}
		MonoBehaviour.print("audio name : " + objName);
		ITAudioEvent iTAudioEvent = (ITAudioEvent)gameObject2.GetComponent(typeof(ITAudioEvent));
		if (iTAudioEvent != null && m_music)
		{
			iTAudioEvent.Trigger();
		}
		musicName = objName;
	}

	public void PlayAudio(string objName, Transform targetTrans)
	{
		if (!useAuidoEvent)
		{
			return;
		}
		if (onAudioEventPlay != null)
		{
			onAudioEventPlay(ref objName);
		}
		Transform transform = base.transform.Find("Audio");
		if (null == transform)
		{
			GameObject gameObject = new GameObject("Audio");
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			transform = gameObject.transform;
		}
		int num = objName.LastIndexOf('/');
		num++;
		string text = objName.Substring(num);
		GameObject gameObject2 = null;
		Transform transform2 = base.transform.Find("Audio/" + text);
		if (null == transform2)
		{
			gameObject2 = Resources.Load("SoundEvent/" + objName) as GameObject;
			if (null == gameObject2)
			{
				Debug.LogWarning(objName + " is null");
				return;
			}
			gameObject2 = Object.Instantiate(gameObject2) as GameObject;
			if (null == gameObject2)
			{
				Debug.LogWarning(objName + " is null");
				return;
			}
			gameObject2.name = text;
			gameObject2.transform.parent = transform;
			gameObject2.transform.localPosition = Vector3.zero;
		}
		else
		{
			gameObject2 = transform2.gameObject;
		}
		AudioSource value = (AudioSource)gameObject2.GetComponent(typeof(AudioSource));
		ITAudioEvent iTAudioEvent = (ITAudioEvent)gameObject2.GetComponent(typeof(ITAudioEvent));
		TAudioLimits tAudioLimits = gameObject2.GetComponent(typeof(TAudioLimits)) as TAudioLimits;
		if (tAudioLimits != null && CheckLimits(tAudioLimits))
		{
			return;
		}
		TAudioLimits_Fadeout tAudioLimits_Fadeout = gameObject2.GetComponent(typeof(TAudioLimits_Fadeout)) as TAudioLimits_Fadeout;
		if (tAudioLimits_Fadeout != null)
		{
			CheckFadeLimits(tAudioLimits_Fadeout);
		}
		if (iTAudioEvent != null && m_sound)
		{
			iTAudioEvent.Trigger();
		}
		for (int i = 0; i < audioNameArray.Count; i++)
		{
			string text2 = audioNameArray[i] as string;
			if (text2 == objName)
			{
				audioTransArray[i] = targetTrans;
				audioSourceArray[i] = value;
				return;
			}
		}
		audioTransArray.Add(targetTrans);
		audioNameArray.Add(objName);
		audioSourceArray.Add(value);
	}

	private bool CheckLimits(TAudioLimits limits)
	{
		bool result = false;
		for (int i = 0; i < limits.LimitAudios.Length; i++)
		{
			string empty = string.Empty;
			empty = limits.LimitAudios[i];
			Transform transform = base.transform.Find("Audio/" + empty);
			if (!(null == transform))
			{
				ITAudioEvent iTAudioEvent = transform.GetComponent(typeof(ITAudioEvent)) as ITAudioEvent;
				if (iTAudioEvent != null && iTAudioEvent.isPlaying)
				{
					return true;
				}
			}
		}
		return result;
	}

	private void CheckFadeLimits(TAudioLimits_Fadeout limits)
	{
		for (int i = 0; i < limits.LimitAudios_Fadeout.Length; i++)
		{
			string empty = string.Empty;
			empty = limits.LimitAudios_Fadeout[i];
			Transform transform = base.transform.Find("Audio/" + empty);
			if (null == transform)
			{
				continue;
			}
			ITAudioEvent iTAudioEvent = transform.GetComponent(typeof(ITAudioEvent)) as ITAudioEvent;
			if (iTAudioEvent != null && iTAudioEvent.isPlaying)
			{
				MonoBehaviour.print("fade: " + iTAudioEvent.isPlaying);
				TAudioAuxFade tAudioAuxFade = transform.GetComponent(typeof(TAudioAuxFade)) as TAudioAuxFade;
				if (tAudioAuxFade != null)
				{
					MonoBehaviour.print("audio " + empty + " fadeout ");
					tAudioAuxFade.StartCoroutine(tAudioAuxFade.FadeOut(null));
				}
				else
				{
					iTAudioEvent.Stop();
				}
			}
		}
	}

	public void PlayAudio(string objName)
	{
		if (!useAuidoEvent)
		{
			return;
		}
		if (onAudioEventPlay != null)
		{
			onAudioEventPlay(ref objName);
		}
		Transform transform = base.transform.Find("Audio");
		if (null == transform)
		{
			GameObject gameObject = new GameObject("Audio");
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			transform = gameObject.transform;
		}
		int num = objName.LastIndexOf('/');
		num++;
		string text = objName.Substring(num);
		GameObject gameObject2 = null;
		Transform transform2 = base.transform.Find("Audio/" + text);
		if (null == transform2)
		{
			gameObject2 = Resources.Load("SoundEvent/" + objName) as GameObject;
			if (null == gameObject2)
			{
				Debug.LogWarning(objName + " is null");
				return;
			}
			gameObject2 = Object.Instantiate(gameObject2) as GameObject;
			if (null == gameObject2)
			{
				Debug.LogWarning(objName + " is null");
				return;
			}
			gameObject2.name = text;
			gameObject2.transform.parent = transform;
			gameObject2.transform.localPosition = Vector3.zero;
		}
		else
		{
			gameObject2 = transform2.gameObject;
		}
		TAudioLimits tAudioLimits = gameObject2.GetComponent(typeof(TAudioLimits)) as TAudioLimits;
		if (!(tAudioLimits != null) || !CheckLimits(tAudioLimits))
		{
			ITAudioEvent iTAudioEvent = (ITAudioEvent)gameObject2.GetComponent(typeof(ITAudioEvent));
			if (iTAudioEvent != null && m_sound)
			{
				iTAudioEvent.Trigger();
			}
		}
	}

	public void SetAudioEventPlayDelegate(OnAudioEventPlay onAudioEventDelegate)
	{
		onAudioEventPlay = onAudioEventDelegate;
	}

	public void StopAllMusic()
	{
		StopAudio(musicName);
	}

	public void StopAllAudio()
	{
		Transform transform = base.transform.Find("Audio");
		foreach (Transform item in transform)
		{
			if (item != null)
			{
				ITAudioEvent iTAudioEvent = item.GetComponent(typeof(ITAudioEvent)) as ITAudioEvent;
				if (iTAudioEvent != null)
				{
					iTAudioEvent.Stop();
				}
			}
		}
	}

	public void StopAudio(string audioName)
	{
		Transform transform = base.transform.Find("Audio/" + audioName);
		if (transform != null)
		{
			ITAudioEvent iTAudioEvent = transform.GetComponent(typeof(ITAudioEvent)) as ITAudioEvent;
			if (iTAudioEvent != null)
			{
				iTAudioEvent.Stop();
			}
		}
	}
}
