using System.Collections;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
	private bool taskPop;

	private GameObject UIs;

	private TUIMeshText label_gold;

	private TAudioController audios;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("TAudioController");
		if (gameObject == null)
		{
			gameObject = Object.Instantiate(Resources.Load("TAudioController")) as GameObject;
			gameObject.name = "TAudioController";
			Object.DontDestroyOnLoad(gameObject);
		}
		audios = gameObject.GetComponent(typeof(TAudioController)) as TAudioController;
	}

	private void Update()
	{
	}

	public void InitTaskList_pause()
	{
		Transform transform = null;
		transform = base.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			int num = 0;
			switch (child.name)
			{
			case "task_list1":
				num = 0;
				SetTaskInfo(num, child);
				break;
			case "task_list2":
				num = 1;
				SetTaskInfo(num, child);
				break;
			case "task_list3":
				num = 2;
				SetTaskInfo(num, child);
				break;
			}
		}
	}

	public void InitTaskList()
	{
		taskPop = false;
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform;
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		float y = -40f;
		if (globalVal.UIState == UILayer.GAMEOVER)
		{
			y = 0f;
		}
		uIMoveControl.SetEndPos(-265f, y, 0f);
		uIMoveControl.LeftToRight(0.2f, transform);
		if (globalVal.UIState == UILayer.GAMEOVER)
		{
			GameObject gameObject = GameObject.Find("TUI/TUIControl");
			uIMoveControl.SetCallBack(gameObject.transform, "CheckTaskCompleteState");
		}
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			int num = 0;
			switch (child.name)
			{
			case "task_list1":
				num = 0;
				SetTaskInfo(num, child);
				break;
			case "task_list2":
				num = 1;
				SetTaskInfo(num, child);
				break;
			case "task_list3":
				num = 2;
				SetTaskInfo(num, child);
				break;
			}
		}
	}

	public void BackTask(int index)
	{
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		string text = "task_list" + index;
		transform = base.transform.Find(text);
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetEndPos(0f, -45 * (index - 1), 0f);
		uIMoveControl.RightToLeft_taskback(0f, transform);
		uIMoveControl.SetCallBack(base.transform, "ChangeTaskInfo", index);
		globalVal.cur_task_id[index - 1]++;
		globalVal.g_best_taskcomplete++;
		globalVal.SaveFile("saveData.txt");
	}

	public void ChangeTaskInfo(int index)
	{
		MonoBehaviour.print("call back " + index);
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		string text = "task_list" + index;
		transform = base.transform.Find(text);
		ArrayList arrayList = ItemManagerClass.body.taskListArray[index - 1] as ArrayList;
		if (globalVal.cur_task_id[index - 1] > arrayList.Count - 1)
		{
			globalVal.cur_task_id[index - 1] = arrayList.Count;
			Transform transform2 = transform.Find("label_task_text");
			if (transform2 != null)
			{
				TUIMeshText tUIMeshText = transform2.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				tUIMeshText.fontName = "TCCEB_16_img";
				tUIMeshText.characterSpacing = -1.5f;
				tUIMeshText.lineSpacing = 5f;
				tUIMeshText.text = "ALL COMPLETED.".ToUpper();
			}
			transform2 = transform.Find("label_glod");
			if (transform2 != null)
			{
				transform2.transform.localPosition = new Vector3(-1000f, -1000f, 0f);
			}
			transform2 = transform.Find("pic_complete");
			if (transform2 != null)
			{
				UIMoveControl uIMoveControl2 = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl2.SetPos(-1000f, -1000f, 0f);
			}
			transform2 = transform.Find("list_bg");
			if (transform2 != null)
			{
				TUIMeshSprite tUIMeshSprite = transform2.GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
				tUIMeshSprite.frameName = "task_list_bg";
			}
			transform2 = transform.Find("pic_gold");
			if (transform2 != null)
			{
				transform2.transform.localPosition = new Vector3(-1000f, -1000f, 0f);
			}
			transform2 = transform.Find("pic_gold_bg");
			if (transform2 != null)
			{
				transform2.transform.localPosition = new Vector3(-1000f, -1000f, 0f);
			}
			transform = base.transform.Find(text);
			uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
			uIMoveControl.RightToLeft_taskback_back(0f, transform);
			audios.PlayAudio("UImission_new");
		}
		else
		{
			string key = arrayList[globalVal.cur_task_id[index - 1]] as string;
			taskInfo taskInfo2 = ItemManagerClass.body.hashTask[key] as taskInfo;
			Transform transform3 = transform.Find("label_task_text");
			if (transform3 != null)
			{
				TUIMeshText tUIMeshText2 = transform3.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				if (taskInfo2.info.Length > 40)
				{
					tUIMeshText2.fontName = "TCCEB_14_img";
					tUIMeshText2.characterSpacing = -1.5f;
					tUIMeshText2.lineSpacing = 5f;
				}
				else
				{
					tUIMeshText2.fontName = "TCCEB_16_img";
					tUIMeshText2.characterSpacing = -1.5f;
					tUIMeshText2.lineSpacing = 5f;
				}
				tUIMeshText2.text = taskInfo2.info.ToUpper();
			}
			transform3 = transform.Find("label_glod");
			if (transform3 != null)
			{
				TUIMeshText tUIMeshText3 = transform3.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				tUIMeshText3.text = string.Empty + taskInfo2.golds;
			}
			transform3 = transform.Find("pic_complete");
			if (transform3 != null)
			{
				UIMoveControl uIMoveControl3 = transform3.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl3.SetPos(-1000f, -1000f, 0f);
			}
			transform3 = transform.Find("list_bg");
			if (transform3 != null)
			{
				TUIMeshSprite tUIMeshSprite2 = transform3.GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
				tUIMeshSprite2.frameName = "task_list_bg";
			}
			transform = base.transform.Find(text);
			uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
			uIMoveControl.RightToLeft_taskback_back(0f, transform);
			audios.PlayAudio("UImission_new");
		}
		globalVal.SaveFile("saveData.txt");
	}

	public void TaskComplete(int index)
	{
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		string text = "task_list" + index;
		transform = base.transform.Find(text);
		Transform transform2 = transform.Find("pic_complete");
		uIMoveControl = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		uIMoveControl.SetEndPos(-220f, 48f, -7.7f);
		uIMoveControl.ScaleRotate(0f, transform2);
		uIMoveControl.SetCallBack(base.transform, "GoldFly", index);
		audios.PlayAudio("UImission_complete");
	}

	public void GoldFly(int index)
	{
		Transform transform = null;
		string text = "task_list" + index;
		transform = base.transform.Find(text);
		Transform trans = transform.Find("pic_gold");
		int num = 0;
		Transform transform2 = transform.Find("label_glod");
		if (transform2 != null)
		{
			TUIMeshText tUIMeshText = transform2.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
			num = int.Parse(tUIMeshText.text);
		}
		transform2 = transform.Find("list_bg");
		if (transform2 != null)
		{
			TUIMeshSprite tUIMeshSprite = transform2.GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
			tUIMeshSprite.frameName = "task_list_bg_d";
		}
		for (int i = 0; i < 5; i++)
		{
			int num2 = 0;
			num2 = ((i == 4) ? num : (i + 1));
			StartCoroutine(GoldFlyOntime((float)i * 0.1f, trans, num2, index));
			num -= i + 1;
		}
	}

	private IEnumerator GoldFlyOntime(float waitTime, Transform trans, int value, int index)
	{
		yield return new WaitForSeconds(waitTime);
		GameObject t = EffectManagerClass.body.PlayGoldFlyEffect_UI(trans, value);
		if (value > 5)
		{
			GoldEffectScript ge = t.GetComponent(typeof(GoldEffectScript)) as GoldEffectScript;
			ge.SetCallBack(base.transform, "CallbackBackTask", index);
		}
	}

	public void CallbackBackTask(int index)
	{
		BackTask(index);
	}

	public void PopTaskList()
	{
		if (taskPop)
		{
			PopBackTaskList();
			taskPop = !taskPop;
			audios.PlayAudio("UImission_back");
			return;
		}
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform;
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		float y = -40f;
		if (globalVal.UIState == UILayer.GAMEOVER)
		{
			y = 0f;
		}
		uIMoveControl.SetEndPos(0f, y, 0f);
		uIMoveControl.LeftToRight_taskpop(0f, transform);
		if (globalVal.UIState == UILayer.GAMEOVER)
		{
			GameObject gameObject = GameObject.Find("TUI/TUIControl");
			uIMoveControl.SetCallBack(gameObject.transform, "CompleteTask");
		}
		taskPop = !taskPop;
		audios.PlayAudio("UImisson_draw");
	}

	public void PopBackTaskList()
	{
		Transform transform = null;
		UIMoveControl uIMoveControl = null;
		transform = base.transform;
		uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
		float y = -40f;
		if (globalVal.UIState == UILayer.GAMEOVER)
		{
			y = 0f;
		}
		uIMoveControl.SetEndPos(0f, y, 0f);
		uIMoveControl.LeftToRight_taskpop_back(0f, transform);
	}

	private void SetTaskInfo(int listId, Transform childs)
	{
		ArrayList arrayList = ItemManagerClass.body.taskListArray[listId] as ArrayList;
		if (globalVal.cur_task_id[listId] > arrayList.Count - 1)
		{
			Transform transform = childs.Find("label_task_text");
			if (transform != null)
			{
				TUIMeshText tUIMeshText = transform.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
				tUIMeshText.fontName = "TCCEB_16_img";
				tUIMeshText.characterSpacing = -1.5f;
				tUIMeshText.lineSpacing = 5f;
				tUIMeshText.text = "ALL COMPLETED.".ToUpper();
			}
			transform = childs.Find("label_glod");
			if (transform != null)
			{
				transform.transform.localPosition = new Vector3(-1000f, -1000f, 0f);
			}
			transform = childs.Find("pic_complete");
			if (transform != null)
			{
				UIMoveControl uIMoveControl = transform.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
				uIMoveControl.SetPos(-1000f, -1000f, 0f);
			}
			transform = childs.Find("list_bg");
			if (transform != null)
			{
				TUIMeshSprite tUIMeshSprite = transform.GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
				tUIMeshSprite.frameName = "task_list_bg";
			}
			transform = childs.Find("pic_gold");
			if (transform != null)
			{
				transform.transform.localPosition = new Vector3(-1000f, -1000f, 0f);
			}
			transform = childs.Find("pic_gold_bg");
			if (transform != null)
			{
				transform.transform.localPosition = new Vector3(-1000f, -1000f, 0f);
			}
			return;
		}
		string key = arrayList[globalVal.cur_task_id[listId]] as string;
		taskInfo taskInfo2 = ItemManagerClass.body.hashTask[key] as taskInfo;
		Transform transform2 = childs.Find("label_task_text");
		if (transform2 != null)
		{
			TUIMeshText tUIMeshText2 = transform2.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
			if (taskInfo2.info.Length > 40)
			{
				tUIMeshText2.fontName = "TCCEB_14_img";
				tUIMeshText2.characterSpacing = -1.5f;
				tUIMeshText2.lineSpacing = 5f;
			}
			else
			{
				tUIMeshText2.fontName = "TCCEB_16_img";
				tUIMeshText2.characterSpacing = -1.5f;
				tUIMeshText2.lineSpacing = 5f;
			}
			tUIMeshText2.text = taskInfo2.info.ToUpper();
		}
		transform2 = childs.Find("label_glod");
		if (transform2 != null)
		{
			TUIMeshText tUIMeshText3 = transform2.GetComponent(typeof(TUIMeshText)) as TUIMeshText;
			tUIMeshText3.text = string.Empty + taskInfo2.golds;
		}
		transform2 = childs.Find("pic_complete");
		if (transform2 != null)
		{
			UIMoveControl uIMoveControl2 = transform2.GetComponent(typeof(UIMoveControl)) as UIMoveControl;
			uIMoveControl2.SetPos(-1000f, -1000f, 0f);
		}
		transform2 = childs.Find("list_bg");
		if (transform2 != null)
		{
			TUIMeshSprite tUIMeshSprite2 = transform2.GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
			tUIMeshSprite2.frameName = "task_list_bg";
		}
	}
}
