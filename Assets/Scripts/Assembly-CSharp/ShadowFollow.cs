using UnityEngine;

public class ShadowFollow : MonoBehaviour
{
	private int floorLayer;

	private Transform playerTrans;

	private Transform aniTrans;

	private RaycastHit ray_hit;

	private SlingshotScript slinshot;

	private Vector3 t_point = Vector3.zero;

	private void Start()
	{
		floorLayer = 1 << LayerMask.NameToLayer("floor");
		playerTrans = base.transform.parent.Find("Bip01");
		aniTrans = base.transform.parent.Find("ani/Bip01");
		slinshot = GameObject.Find("Slingshot").GetComponent(typeof(SlingshotScript)) as SlingshotScript;
	}

	private void Update()
	{
		if (slinshot.isAniBody)
		{
			t_point = aniTrans.position;
		}
		else
		{
			t_point = playerTrans.position;
		}
		if (Physics.Raycast(t_point, Vector3.down, out ray_hit, 100f, floorLayer))
		{
			Vector3 position = playerTrans.position;
			position.x = ray_hit.point.x;
			position.y = ray_hit.point.y + 0.1f;
			position.z = ray_hit.point.z;
			base.transform.position = position;
			float x = Vector3.Angle(Vector3.left, ray_hit.normal);
			base.transform.localEulerAngles = new Vector3(x, 0f, 0f);
		}
	}
}
