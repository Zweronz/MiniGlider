using System;
using System.Collections;
using UnityEngine;

public class DrawCircleManager : MonoBehaviour
{
	private class DrawPoint
	{
		public Vector3 pos0;

		public Vector3 pos1;

		public Vector3 pos2;

		public Vector3 pos3;

		public Vector4 rct;

		public Color color;
	}

	public bool _asMgrShadow;

	public Material _Material;

	public Texture _Texture;

	protected float _texFacX;

	protected float _texFacY;

	protected int _Size;

	protected ArrayList _Buffer;

	protected MeshFilter _MeshFilter;

	protected MeshRenderer _MeshRenderer;

	protected Mesh _Mesh;

	public Vector3 GetPos()
	{
		return base.transform.position;
	}

	private void Awake()
	{
		if (_asMgrShadow)
		{
			DrawCircleClass.circle = this;
		}
		base.gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 1f);
		base.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		base.gameObject.AddComponent<MeshFilter>();
		base.gameObject.AddComponent<MeshRenderer>();
		_MeshFilter = (MeshFilter)GetComponent(typeof(MeshFilter));
		_MeshRenderer = (MeshRenderer)GetComponent(typeof(MeshRenderer));
		_MeshRenderer.castShadows = false;
		_MeshRenderer.receiveShadows = false;
		_MeshRenderer.GetComponent<Renderer>().material = _Material;
		_Mesh = _MeshFilter.mesh;
		_Material.SetTexture("texBase", _Texture);
		_texFacX = 1f / (float)_Texture.width;
		_texFacY = 1f / (float)_Texture.height;
		_Buffer = new ArrayList();
		_Size = 0;
	}

	public void ClearMesh()
	{
		_Mesh.Clear();
		_Buffer.Clear();
		_Size = 0;
	}

	public void InitCircleProgress(float radius, float width, Color clr)
	{
		ClearMesh();
		for (int i = 0; i <= 100; i++)
		{
			if (i > 0)
			{
				Vector3 zero = Vector3.zero;
				zero.x = Mathf.Sin((float)(i - 1) * 3.6f * ((float)Math.PI / 180f)) * (radius + width * 0.5f);
				zero.y = Mathf.Cos((float)(i - 1) * 3.6f * ((float)Math.PI / 180f)) * (radius + width * 0.5f);
				Vector3 zero2 = Vector3.zero;
				zero2.x = Mathf.Sin((float)(i - 1) * 3.6f * ((float)Math.PI / 180f)) * (radius - width * 0.5f);
				zero2.y = Mathf.Cos((float)(i - 1) * 3.6f * ((float)Math.PI / 180f)) * (radius - width * 0.5f);
				int num = i;
				if (num == 100)
				{
					num = 0;
				}
				Vector3 zero3 = Vector3.zero;
				zero3.x = Mathf.Sin((float)num * 3.6f * ((float)Math.PI / 180f)) * (radius + width * 0.5f);
				zero3.y = Mathf.Cos((float)num * 3.6f * ((float)Math.PI / 180f)) * (radius + width * 0.5f);
				Vector3 zero4 = Vector3.zero;
				zero4.x = Mathf.Sin((float)num * 3.6f * ((float)Math.PI / 180f)) * (radius - width * 0.5f);
				zero4.y = Mathf.Cos((float)num * 3.6f * ((float)Math.PI / 180f)) * (radius - width * 0.5f);
				AddCirclePoint(zero, zero2, zero3, zero4, clr);
			}
		}
		SetCircleMesh();
	}

	public void SetProgress(int pro)
	{
		_Size = pro;
		SetProgressMesh();
	}

	public int GetProgress()
	{
		return _Size;
	}

	public void AddCirclePoint(Vector3 p, Vector3 p1, Vector3 p2, Vector3 p3, Color clr)
	{
		DrawPoint drawPoint = new DrawPoint();
		drawPoint.pos0 = p;
		drawPoint.pos1 = p1;
		drawPoint.pos2 = p3;
		drawPoint.pos3 = p2;
		drawPoint.rct = new Vector4(0f, 0f, 1f, 1f);
		drawPoint.color = clr;
		_Buffer.Add(drawPoint);
	}

	private void SetProgressMesh()
	{
		if (_Buffer.Count > 0)
		{
			Vector3[] array = new Vector3[_Size * 4];
			Vector2[] array2 = new Vector2[_Size * 4];
			Color[] array3 = new Color[_Size * 4];
			int[] array4 = new int[_Size * 6];
			for (int i = 0; i < _Size; i++)
			{
				DrawPoint drawPoint = _Buffer[i] as DrawPoint;
				int num = i * 4;
				array[num] = drawPoint.pos0;
				array[num + 1] = drawPoint.pos1;
				array[num + 2] = drawPoint.pos2;
				array[num + 3] = drawPoint.pos3;
				array2[num] = new Vector2(drawPoint.rct.x, drawPoint.rct.w);
				array2[num + 1] = new Vector2(drawPoint.rct.x, drawPoint.rct.y);
				array2[num + 2] = new Vector2(drawPoint.rct.z, drawPoint.rct.y);
				array2[num + 3] = new Vector2(drawPoint.rct.z, drawPoint.rct.w);
				array3[num] = drawPoint.color;
				array3[num + 1] = drawPoint.color;
				array3[num + 2] = drawPoint.color;
				array3[num + 3] = drawPoint.color;
				int num2 = i * 6;
				array4[num2] = num;
				array4[num2 + 1] = num + 1;
				array4[num2 + 2] = num + 3;
				array4[num2 + 3] = num + 3;
				array4[num2 + 4] = num + 1;
				array4[num2 + 5] = num + 2;
			}
			_Mesh.vertices = array;
			_Mesh.triangles = array4;
			_Mesh.uv = array2;
			_Mesh.colors = array3;
		}
	}

	private void SetCircleMesh()
	{
		_Mesh.Clear();
		if (_Buffer.Count > 0)
		{
			_Size = _Buffer.Count;
			Vector3[] array = new Vector3[_Size * 4];
			Vector2[] array2 = new Vector2[_Size * 4];
			Color[] array3 = new Color[_Size * 4];
			int[] array4 = new int[_Size * 6];
			for (int i = 0; i < _Size; i++)
			{
				DrawPoint drawPoint = _Buffer[i] as DrawPoint;
				int num = i * 4;
				array[num] = drawPoint.pos0;
				array[num + 1] = drawPoint.pos1;
				array[num + 2] = drawPoint.pos2;
				array[num + 3] = drawPoint.pos3;
				array2[num] = new Vector2(drawPoint.rct.x, drawPoint.rct.w);
				array2[num + 1] = new Vector2(drawPoint.rct.x, drawPoint.rct.y);
				array2[num + 2] = new Vector2(drawPoint.rct.z, drawPoint.rct.y);
				array2[num + 3] = new Vector2(drawPoint.rct.z, drawPoint.rct.w);
				array3[num] = drawPoint.color;
				array3[num + 1] = drawPoint.color;
				array3[num + 2] = drawPoint.color;
				array3[num + 3] = drawPoint.color;
				int num2 = i * 6;
				array4[num2] = num;
				array4[num2 + 1] = num + 1;
				array4[num2 + 2] = num + 3;
				array4[num2 + 3] = num + 3;
				array4[num2 + 4] = num + 1;
				array4[num2 + 5] = num + 2;
			}
			_Mesh.vertices = array;
			_Mesh.uv = array2;
			_Mesh.colors = array3;
			_Mesh.triangles = array4;
		}
	}
}
