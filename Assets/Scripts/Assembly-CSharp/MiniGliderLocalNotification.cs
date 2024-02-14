using System;
using UnityEngine;

public class MiniGliderLocalNotification : MonoBehaviour
{
	public string m_alterAction = "MiniGlider";

	public string m_alterBody = "Monsters are everywhere! Come save the kingdom before it's too late!";

	private string[] thing = new string[2];

	public bool Add(string alterAction, string alterBody, DateTime fireDate)
	{
		//UnityEngine.iOS.LocalNotification localNotification = new UnityEngine.iOS.LocalNotification();
		//localNotification.alertBody = alterBody;
		//localNotification.fireDate = fireDate;
		//UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(localNotification);
		//Debug.Log("Add local notification, fire date: " + fireDate);
		return true;
	}

	public void Clear()
	{
		//UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
		//UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
	}

	private void Awake()
	{
		thing[0] = "The zombies have taken the city, but you can still claim the sky!";
		thing[1] = "It's time to blast past your old high score!";
		Clear();
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			Add(m_alterAction, thing[UnityEngine.Random.Range(0, 2)], DateTime.Now.AddDays(3.0));
			Add(m_alterAction, thing[UnityEngine.Random.Range(0, 2)], DateTime.Now.AddDays(7.0));
			Add(m_alterAction, thing[UnityEngine.Random.Range(0, 2)], DateTime.Now.AddDays(14.0));
			Add(m_alterAction, thing[UnityEngine.Random.Range(0, 2)], DateTime.Now.AddDays(21.0));
		}
		else
		{
			Clear();
		}
	}
}
