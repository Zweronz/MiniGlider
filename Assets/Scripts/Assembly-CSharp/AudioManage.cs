using System.Collections;
using UnityEngine;

public class AudioManage : MonoBehaviour
{
	private GameObject audioobj;

	private void Start()
	{
		if (!GameObject.Find("audios"))
		{
			audioobj = new GameObject("audios");
			audioobj.AddComponent<AudioListener>();
			Object.DontDestroyOnLoad(audioobj);
		}
		else
		{
			audioobj = GameObject.Find("audios");
		}
	}

	private void Update()
	{
	}

	public void playmusic(string musicname, bool isloop)
	{
		GameObject gameObject = Resources.Load(musicname) as GameObject;
		AudioClip clip = gameObject.GetComponent<AudioSource>().clip;
		GameObject gameObject2;
		if (!GameObject.Find("musics"))
		{
			gameObject2 = new GameObject("musics");
			gameObject2.transform.parent = audioobj.transform;
			AudioSource audioSource = gameObject2.AddComponent<AudioSource>() as AudioSource;
			audioSource.playOnAwake = false;
		}
		else
		{
			gameObject2 = GameObject.Find("musics");
		}
		gameObject2.GetComponent<AudioSource>().clip = clip;
		gameObject2.GetComponent<AudioSource>().loop = isloop;
		gameObject2.GetComponent<AudioSource>().Play();
	}

	public void playsound(string soundname, bool isloop)
	{
		GameObject gameObject = Resources.Load(soundname) as GameObject;
		AudioClip clip = gameObject.GetComponent<AudioSource>().clip;
		GameObject gameObject2;
		if (!GameObject.Find("sounds"))
		{
			gameObject2 = new GameObject("sounds");
			gameObject2.transform.parent = audioobj.transform;
			gameObject2.AddComponent<AudioSource>();
		}
		else
		{
			gameObject2 = GameObject.Find("sounds");
		}
		gameObject2.GetComponent<AudioSource>().loop = isloop;
		gameObject2.GetComponent<AudioSource>().clip = clip;
		gameObject2.GetComponent<AudioSource>().PlayOneShot(clip);
	}

	public void pausemusic()
	{
		GameObject gameObject = GameObject.Find("musics");
		gameObject.GetComponent<AudioSource>().Pause();
	}

	public void repausemusic()
	{
		GameObject gameObject = GameObject.Find("musics");
		gameObject.GetComponent<AudioSource>().Play();
	}

	private IEnumerator destoryaudio(float waitTime, GameObject audios)
	{
		yield return new WaitForSeconds(waitTime);
		audios.GetComponent<AudioSource>().Stop();
		Object.Destroy(audios);
	}

	public void destoryaudio()
	{
		GameObject gameObject = GameObject.Find("musics");
		GameObject gameObject2 = GameObject.Find("sounds");
		GameObject obj = GameObject.Find("audios");
		gameObject.GetComponent<AudioSource>().Stop();
		gameObject2.GetComponent<AudioSource>().Stop();
		Object.Destroy(gameObject);
		Object.Destroy(gameObject2);
		Object.Destroy(obj);
	}
}
