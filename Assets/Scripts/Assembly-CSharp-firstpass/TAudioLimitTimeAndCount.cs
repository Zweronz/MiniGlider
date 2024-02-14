using System.Collections.Generic;
using UnityEngine;

public class TAudioLimitTimeAndCount : ITAudioLimit
{
	public int maxCount;

	public float deltaTime = 0.2f;

	private static Dictionary<string, int> s_records = new Dictionary<string, int>();

	private float m_triggerTime;

	private bool m_isTimeout = true;

	private Queue<float> m_timeBound = new Queue<float>();

	public override bool isCanPlay
	{
		get
		{
			return !Limit();
		}
	}

	private void Update()
	{
		if (!m_isTimeout && Time.realtimeSinceStartup - m_triggerTime > deltaTime)
		{
			m_isTimeout = true;
			string text = "TimeAndCountLimit_" + base.name;
			if (s_records.ContainsKey(text))
			{
				Dictionary<string, int> dictionary;
				Dictionary<string, int> dictionary2 = (dictionary = s_records);
				string key;
				string key2 = (key = text);
				int num = dictionary[key];
				dictionary2[key2] = num - 1;
				if (s_records[text] <= 0)
				{
					s_records.Remove(text);
				}
			}
		}
		while (m_timeBound.Count > 0 && m_timeBound.Peek() < Time.realtimeSinceStartup)
		{
			m_timeBound.Dequeue();
		}
	}

	private bool Limit()
	{
		if (m_timeBound.Count >= maxCount)
		{
			return true;
		}
		string key = "TimeAndCountLimit_" + base.name;
		if (s_records.ContainsKey(key) && s_records[key] >= maxCount)
		{
			return true;
		}
		return false;
	}

	public override void OnAudioTrigger(AudioClip clip)
	{
		if (m_timeBound.Count < maxCount)
		{
			m_timeBound.Enqueue(Time.realtimeSinceStartup + deltaTime);
		}
		if (m_isTimeout)
		{
			string text = "TimeAndCountLimit_" + base.name;
			if (s_records.ContainsKey(text))
			{
				Dictionary<string, int> dictionary;
				Dictionary<string, int> dictionary2 = (dictionary = s_records);
				string key;
				string key2 = (key = text);
				int num = dictionary[key];
				dictionary2[key2] = num + 1;
			}
			else
			{
				s_records.Add(text, 1);
			}
			m_triggerTime = Time.realtimeSinceStartup;
			m_isTimeout = false;
		}
	}
}
