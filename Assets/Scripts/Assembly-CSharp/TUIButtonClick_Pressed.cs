using UnityEngine;

public class TUIButtonClick_Pressed : TUIButtonBase
{
	public const int CommandDown = 1;

	public const int CommandUp = 2;

	public const int CommandClick = 3;

	public GameObject framePressed2;

	public new void Start()
	{
		Reset();
	}

	public void Reset()
	{
		pressed = false;
		fingerId = -1;
		UpdateFrame();
		UpdateFrame2();
	}

	protected void UpdateFrame2()
	{
		if (!(framePressed2 == null))
		{
			framePressed2.active = false;
			if (!disabled && pressed)
			{
				framePressed2.active = true;
			}
		}
	}

	public void SetDisabled_Pressed(bool disabled)
	{
		SetDisabled(disabled);
		UpdateFrame2();
		if (disabled)
		{
			pressed = false;
			fingerId = -1;
		}
	}

	public override bool HandleInput(TUIInput input)
	{
		if (disabled)
		{
			return false;
		}
		if (input.inputType == TUIInputType.Began)
		{
			if (PtInControl(input.position))
			{
				pressed = true;
				fingerId = input.fingerId;
				UpdateFrame();
				UpdateFrame2();
				PostEvent(this, 1, 0f, 0f, null);
				return true;
			}
			return false;
		}
		if (input.fingerId == fingerId)
		{
			if (input.inputType == TUIInputType.Moved)
			{
				if (PtInControl(input.position))
				{
					if (!pressed)
					{
						pressed = true;
						UpdateFrame();
						UpdateFrame2();
						PostEvent(this, 1, 0f, 0f, null);
					}
				}
				else if (pressed)
				{
					pressed = false;
					UpdateFrame();
					UpdateFrame2();
					PostEvent(this, 2, 0f, 0f, null);
				}
			}
			else if (input.inputType == TUIInputType.Ended)
			{
				pressed = false;
				fingerId = -1;
				if (PtInControl(input.position))
				{
					UpdateFrame();
					UpdateFrame2();
					PostEvent(this, 2, 0f, 0f, null);
					PostEvent(this, 3, 0f, 0f, null);
				}
				else
				{
					UpdateFrame();
					UpdateFrame2();
					PostEvent(this, 2, 0f, 0f, null);
				}
			}
			return true;
		}
		return false;
	}
}
