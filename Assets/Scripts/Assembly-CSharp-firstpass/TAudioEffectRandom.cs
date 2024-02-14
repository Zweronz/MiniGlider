using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TAudioEffectRandom : ITAudioEvent
{
	public enum LoopMode
	{
		Default = 0,
		SingleLoop = 1,
		MultiLoop = 2
	}

	public bool isSfx = true;

	public AudioClip[] audioClips;

	public float[] probability;

	public float volumOffset;

	public float pitchOffset;

	public LoopMode loopMode;

	public bool cutoff;

	private static Dictionary<string, int> s_random_index = new Dictionary<string, int>();

	private ITAudioLimit[] m_audioLimits;

	private int m_lastRandomIndex = -1;

	private float m_volumBase;

	private float m_pitchBase;

	private float nullProbability = -1f;

	private bool m_isPlaying;

	public int currentPlayIndex
	{
		get
		{
			return m_lastRandomIndex;
		}
	}

	public override bool isPlaying
	{
		get
		{
			return m_isPlaying;
		}
	}

	public override bool isLoop
	{
		get
		{
			if (loopMode == LoopMode.Default)
			{
				return base.GetComponent<AudioSource>().loop;
			}
			return true;
		}
	}

	private void Awake()
	{
		m_audioLimits = GetComponents<ITAudioLimit>();
		m_volumBase = base.GetComponent<AudioSource>().volume;
		m_pitchBase = base.GetComponent<AudioSource>().pitch;
		if (probability.Length > 0)
		{
			float num = 0f;
			float[] array = probability;
			foreach (float num2 in array)
			{
				num += num2;
			}
			if (num < 0.999f)
			{
				nullProbability = 1f - num;
			}
		}
		if (audioClips.Length == 1 && loopMode == LoopMode.MultiLoop)
		{
			loopMode = LoopMode.Default;
		}
	}

	private void OnDestroy()
	{
		Stop();
	}

	private IEnumerator TriggerDelay(float time)
	{
		float time_trigger = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - time_trigger < time)
		{
			yield return 0;
		}
		Trigger();
	}

	private IEnumerator PlayOver(float time)
	{
		float time_over = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - time_over < time)
		{
			yield return 0;
		}
		m_isPlaying = false;
	}

	private void SendTriggerEvent(AudioClip clip)
	{
		ITAudioLimit[] audioLimits = m_audioLimits;
		foreach (ITAudioLimit iTAudioLimit in audioLimits)
		{
			iTAudioLimit.OnAudioTrigger(clip);
		}
	}

	public override void Trigger()
	{
		if (audioClips.Length == 0)
		{
			return;
		}
		ITAudioLimit[] audioLimits = m_audioLimits;
		foreach (ITAudioLimit iTAudioLimit in audioLimits)
		{
			if (!iTAudioLimit.isCanPlay)
			{
				return;
			}
		}
		string key = "AudioRandomIndex_" + base.name;
		bool flag = false;
		if (probability.Length == 0)
		{
			if (audioClips.Length == 1)
			{
				m_lastRandomIndex = 0;
				flag = true;
			}
			else
			{
				int num = Random.Range(0, 1000);
				if (s_random_index.ContainsKey(key))
				{
					m_lastRandomIndex = s_random_index[key];
				}
				if (m_lastRandomIndex == -1)
				{
					num %= audioClips.Length;
					m_lastRandomIndex = num;
				}
				else
				{
					num %= audioClips.Length - 1;
					m_lastRandomIndex = (m_lastRandomIndex + num + 1) % audioClips.Length;
				}
				flag = true;
			}
		}
		else
		{
			if (s_random_index.ContainsKey(key))
			{
				m_lastRandomIndex = s_random_index[key];
			}
			float num2 = 0f;
			for (int j = 0; j < probability.Length; j++)
			{
				if (j != m_lastRandomIndex)
				{
					num2 += probability[j];
				}
			}
			if (nullProbability > 0.001f)
			{
				num2 += nullProbability;
			}
			float num3 = Random.Range(0f, num2);
			if (nullProbability > 0.001f)
			{
				num2 -= nullProbability;
				if (num3 > num2)
				{
					m_lastRandomIndex = -1;
					flag = true;
				}
			}
			if (!flag)
			{
				for (int num4 = probability.Length - 1; num4 >= 0; num4--)
				{
					if (num4 != m_lastRandomIndex)
					{
						num2 -= probability[num4];
						if (num3 > num2)
						{
							m_lastRandomIndex = num4;
							flag = true;
							break;
						}
					}
				}
			}
		}
		if (flag)
		{
			if (s_random_index.ContainsKey(key))
			{
				s_random_index[key] = m_lastRandomIndex;
			}
			else
			{
				s_random_index.Add(key, m_lastRandomIndex);
			}
		}
		if (m_lastRandomIndex == -1)
		{
			return;
		}
		AudioClip audioClip = audioClips[m_lastRandomIndex];
		if (!(null != audioClip))
		{
			return;
		}
		base.GetComponent<AudioSource>().volume = Mathf.Clamp01(Random.Range(m_volumBase - volumOffset, m_volumBase + volumOffset));
		base.GetComponent<AudioSource>().pitch = Mathf.Clamp(Random.Range(m_pitchBase / (1f + pitchOffset), m_pitchBase * (1f + pitchOffset)), 0.01f, 3f);
		m_isPlaying = true;
		if (loopMode == LoopMode.Default)
		{
			if (isSfx)
			{
				TAudioManager.instance.PlaySound(base.GetComponent<AudioSource>(), audioClip, base.GetComponent<AudioSource>().loop, cutoff);
			}
			else
			{
				TAudioManager.instance.PlayMusic(base.GetComponent<AudioSource>(), audioClip, base.GetComponent<AudioSource>().loop, cutoff);
			}
			if (!base.GetComponent<AudioSource>().loop)
			{
				StopAllCoroutines();
				StartCoroutine(PlayOver(audioClip.length / base.GetComponent<AudioSource>().pitch));
			}
			SendTriggerEvent(audioClip);
		}
		else if (loopMode == LoopMode.MultiLoop)
		{
			if (!base.GetComponent<AudioSource>().isPlaying)
			{
				if (isSfx)
				{
					TAudioManager.instance.AddLoopSoundEvt(this);
				}
				else
				{
					TAudioManager.instance.AddLoopMusicEvt(this);
				}
			}
			if (isSfx)
			{
				TAudioManager.instance.PlaySound(base.GetComponent<AudioSource>(), audioClip, false, true);
			}
			else
			{
				TAudioManager.instance.PlayMusic(base.GetComponent<AudioSource>(), audioClip, false, true);
			}
			StopAllCoroutines();
			StartCoroutine(TriggerDelay(audioClip.length / base.GetComponent<AudioSource>().pitch));
			SendTriggerEvent(audioClip);
		}
		else if (loopMode == LoopMode.SingleLoop)
		{
			if (isSfx)
			{
				TAudioManager.instance.PlaySound(base.GetComponent<AudioSource>(), audioClip, true);
			}
			else
			{
				TAudioManager.instance.PlayMusic(base.GetComponent<AudioSource>(), audioClip, true);
			}
			SendTriggerEvent(audioClip);
		}
	}

	public override void Stop()
	{
		m_isPlaying = false;
		if (loopMode == LoopMode.MultiLoop)
		{
			StopAllCoroutines();
		}
		else if (loopMode == LoopMode.Default && !base.GetComponent<AudioSource>().loop)
		{
			StopAllCoroutines();
		}
		if (isSfx)
		{
			TAudioManager.instance.StopSound(base.GetComponent<AudioSource>());
		}
		else
		{
			TAudioManager.instance.StopMusic(base.GetComponent<AudioSource>());
		}
	}
}
