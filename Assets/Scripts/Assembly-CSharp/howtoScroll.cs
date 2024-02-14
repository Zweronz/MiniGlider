using UnityEngine;

public class howtoScroll : MonoBehaviour
{
	public int pageCount = 5;

	public float pageWidth = 335f;

	public float pageHeight;

	private int curPage;

	private void Start()
	{
		SetPageButtom(curPage);
	}

	private void Update()
	{
	}

	private void SetPageButtom(int index)
	{
		Transform transform = base.transform.parent.Find("listgroundbuttom");
		for (int i = 0; i < pageCount; i++)
		{
			Transform child = transform.GetChild(i);
			TUIMeshSprite tUIMeshSprite = child.GetComponent(typeof(TUIMeshSprite)) as TUIMeshSprite;
			if (index == i)
			{
				tUIMeshSprite.frameName = "point1";
			}
			else
			{
				tUIMeshSprite.frameName = "point2";
			}
		}
	}

	public void OnScrollBegin()
	{
		MonoBehaviour.print("scroll begin pos : " + base.transform.localPosition);
	}

	public void OnScrollMove()
	{
		int num = (int)((base.transform.localPosition.x * -1f + pageWidth * 0.5f) / pageWidth);
		if (num != curPage)
		{
			curPage = num;
			SetPageButtom(curPage);
		}
	}

	public void OnScrollEnd()
	{
		MonoBehaviour.print("scroll end pos : " + base.transform.localPosition);
	}
}
