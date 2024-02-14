using System.Collections;
using UnityEngine;

public class GoldEffectScript : MonoBehaviour
{
	private float canMoveTime;

	private UIMoveData startData;

	private UIMoveData endData;

	private float startTime;

	private bool canMove;

	private Transform camTrans;

	private int m_value;

	private GameObject UIs;

	private TUIMeshText label_gold;

	private TUIMeshText gameover_gold;

	private bool m_call_key;

	private string m_call_name = string.Empty;

	private Transform m_call_trans;

	private int m_call_value;

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
		UIs = GameObject.Find("TUI/TUIControl");
		label_gold = UIs.transform.Find("title/label_glod").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
		gameover_gold = UIs.transform.Find("gameover_page/title/label_glod").GetComponent(typeof(TUIMeshText)) as TUIMeshText;
		audios = gameObject.GetComponent(typeof(TAudioController)) as TAudioController;
	}

	private void Update()
	{
		if (!canMove)
		{
			return;
		}
		canMoveTime += Time.deltaTime;
		float num = canMoveTime / startTime;
		if (num >= 1f)
		{
			num = 1f;
			canMove = false;
			RecoverGoldFly();
			if (globalVal.UIState == UILayer.GAMEOVER)
			{
				AddTaskGold(m_value);
			}
			else
			{
				AddPlayerGold(m_value);
			}
			if (m_call_key)
			{
				m_call_trans.SendMessage(m_call_name, m_call_value);
				m_call_key = false;
			}
		}
		base.transform.position = Vector3.Lerp(startData.pos, endData.pos, num);
		base.transform.localScale = Vector3.Lerp(startData.size, endData.size, num);
	}

	private void StartMove(UIMoveData startdata, UIMoveData enddata, float time)
	{
		startData = startdata;
		endData = enddata;
		startTime = time;
		canMoveTime = 0f;
		canMove = true;
	}

	private void RecoverGoldFly()
	{
		ArrayList arrayList = null;
		if (base.gameObject.name == "effect_gold_mesh")
		{
			arrayList = EffectManagerClass.body.effGold;
		}
		else if (base.gameObject.name == "effect_gold_red")
		{
			arrayList = EffectManagerClass.body.effRedGold;
		}
		base.gameObject.active = false;
		arrayList.Add(base.gameObject);
	}

	private void AddPlayerGold(int value)
	{
		if (globalVal.g_gold_isdoubel)
		{
			value *= 2;
		}
		globalVal.g_gold += value;
		StatisticsData.data.d_gameGold += value;
		globalVal.g_totalGold += (ulong)value;
		PlayerScriptClass.playerInfo.gold += value;
		PlayerScriptClass.playerInfo.goldCount++;
		if (PlayerScriptClass.playerInfo.huojianKey)
		{
			PlayerScriptClass.playerInfo.huojianAddAgain++;
			UpdateHuojianPower();
		}
		label_gold.text = string.Empty + globalVal.g_gold;
	}

	private void UpdateHuojianPower()
	{
		int num = 0;
		int huojianAddMax = PlayerScriptClass.playerInfo.GetHuojianAddMax();
		int num2 = PlayerScriptClass.playerInfo.huojianAddAgain;
		if (num2 >= huojianAddMax)
		{
			PlayerScriptClass.playerInfo.huojianKey = false;
			PlayerScriptClass.playerInfo.leftUseTime++;
			num2 = huojianAddMax;
			Transform itemTrans = GameObject.Find("TUI/TUIControl/control/circle").transform;
			EffectManagerClass.body.PlayEffect("effect_ui_02", itemTrans);
		}
		float num3 = num2 * 100 / huojianAddMax;
		num = (int)num3;
		DrawCircleClass.circle.SetProgress(num);
	}

	private void AddTaskGold(int value)
	{
		globalVal.g_gold += value;
		globalVal.g_totalGold += (ulong)value;
		StatisticsData.data.d_gameGold += value;
		PlayerScriptClass.playerInfo.gold += value;
		gameover_gold.text = string.Empty + globalVal.g_gold;
		audios.PlayAudio("UIcoin_count", base.transform);
	}

	public void GoldFly(Vector3 viewPoint, int value)
	{
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		camTrans = GameObject.Find("TUI/TUICamera").transform;
		m_value = value;
		Vector3 vector = camTrans.position + new Vector3(-240f, -160f, 0f);
		vector.x += viewPoint.x * 480f;
		vector.y += viewPoint.y * 320f;
		vector.z = 0f;
		Vector3 vector2 = camTrans.position + new Vector3(-240f, -160f, 0f);
		vector2.x += 465f;
		vector2.y += 302f;
		uIMoveData.pos = new Vector3(vector.x, vector.y, 0f);
		uIMoveData2.pos = new Vector3(vector2.x, vector2.y, 0f);
		uIMoveData.size = new Vector3(25f, 25f, 25f);
		uIMoveData2.size = new Vector3(18f, 18f, 18f);
		StartMove(uIMoveData, uIMoveData2, 0.5f);
	}

	public void GoldFly_big(Vector3 viewPoint, int value)
	{
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		camTrans = GameObject.Find("TUI/TUICamera").transform;
		m_value = value;
		Vector3 vector = camTrans.position + new Vector3(-240f, -160f, 0f);
		vector.x += viewPoint.x * 480f;
		vector.y += viewPoint.y * 320f;
		vector.z = 0f;
		Vector3 vector2 = camTrans.position + new Vector3(-240f, -160f, 0f);
		vector2.x += 465f;
		vector2.y += 302f;
		uIMoveData.pos = new Vector3(vector.x, vector.y, 0f);
		uIMoveData2.pos = new Vector3(vector2.x, vector2.y, 0f);
		uIMoveData.size = new Vector3(65f, 65f, 65f);
		uIMoveData2.size = new Vector3(18f, 18f, 18f);
		StartMove(uIMoveData, uIMoveData2, 0.5f);
	}

	public void GoldFly_UI(Vector3 viewPoint, int value)
	{
		UIMoveData uIMoveData = new UIMoveData();
		UIMoveData uIMoveData2 = new UIMoveData();
		camTrans = GameObject.Find("TUI/TUICamera").transform;
		m_value = value;
		Vector3 vector = camTrans.position + new Vector3(-240f, -160f, 0f);
		vector.x += viewPoint.x * 480f;
		vector.y += viewPoint.y * 320f;
		vector.z = 0f;
		Vector3 vector2 = camTrans.position + new Vector3(-240f, -160f, 0f);
		vector2.x += 465f;
		vector2.y += 302f;
		uIMoveData.pos = new Vector3(vector.x, vector.y, -9f);
		uIMoveData2.pos = new Vector3(vector2.x, vector2.y, -9f);
		uIMoveData.size = new Vector3(18f, 18f, 18f);
		uIMoveData2.size = new Vector3(18f, 18f, 18f);
		StartMove(uIMoveData, uIMoveData2, 1f);
	}

	public void SetCallBack(Transform parent, string callback, int value)
	{
		m_call_key = true;
		m_call_name = callback;
		m_call_trans = parent;
		m_call_value = value;
	}
}
