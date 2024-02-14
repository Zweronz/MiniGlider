using System.Collections;
using UnityEngine;

public class ShowGoldGround : MonoBehaviour
{
	private int index;

	private ArrayList array = new ArrayList();

	private bool canUpdate;

	private Vector3 offsetPoint = Vector3.zero;

	private void Start()
	{
	}

	private void Update()
	{
		if (canUpdate)
		{
			for (int i = 0; i < 5; i++)
			{
				UpdateOnce();
			}
		}
	}

	private void UpdateOnce()
	{
		index = array.Count;
		if (index < 1)
		{
			canUpdate = false;
			Object.Destroy(this);
			Object.Destroy(base.gameObject);
			return;
		}
		PointInfo pointInfo = array[index - 1] as PointInfo;
		if (pointInfo != null)
		{
			GameObject item_new = ItemManagerClass.body.GetItem_new(pointInfo.name);
			if (item_new != null)
			{
				if (item_new.GetComponent(typeof(GoldRotateScript)) == null)
				{
					item_new.AddComponent(typeof(GoldRotateScript));
				}
				item_new.name = pointInfo.name;
				item_new.GetComponent<Collider>().enabled = true;
				item_new.active = true;
				Vector3 position = offsetPoint + pointInfo.pos;
				position.y -= 100f;
				position.z = 0f;
				position.x -= 8f;
				item_new.transform.position = position;
				ItemManagerClass.body.SetItemAttr(item_new);
			}
		}
		index--;
		array.RemoveAt(index);
	}

	public void StartShow(ArrayList groundArray, Vector3 offset)
	{
		array.Clear();
		array = (ArrayList)groundArray.Clone();
		offsetPoint = offset;
		index = array.Count;
		canUpdate = true;
	}
}
