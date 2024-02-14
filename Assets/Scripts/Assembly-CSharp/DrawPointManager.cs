using System.Collections;
using UnityEngine;

public class DrawPointManager : MonoBehaviour
{
	private class DrawPoint
	{
		public Vector3 pos;

		public Vector3 up;

		public Vector3 side;

		public Vector3 pos2;

		public Vector4 rct;

		public Color color;
	}

	public bool _asMgrShadow = true;

	public Material _Material;

	public Texture _Texture;

	public int _Capacity = 128;

	protected float _texFacX;

	protected float _texFacY;

	protected int _Size;

	protected ArrayList _Buffer;

	protected MeshFilter _MeshFilter;

	protected MeshRenderer _MeshRenderer;

	protected Mesh _Mesh;

	public void AddPoint(Vector3 p, Vector3 up, Vector3 side, Color clr)
	{
		DrawPoint drawPoint = new DrawPoint();
		drawPoint.pos = base.transform.position + p;
		drawPoint.up = up * 0.5f;
		drawPoint.side = side * 0.5f;
		drawPoint.rct = new Vector4(0f, 0f, 1f, 1f);
		drawPoint.color = clr;
		_Buffer.Add(drawPoint);
		SetMesh();
	}

	public void AddPoint(Vector3 p, Vector3 up, Vector3 side, float sc, Color clr)
	{
		DrawPoint drawPoint = new DrawPoint();
		drawPoint.pos = p;
		drawPoint.up = up * sc;
		drawPoint.side = side * sc;
		drawPoint.rct = new Vector4(0f, 0f, 1f, 1f);
		drawPoint.color = clr;
		_Buffer.Add(drawPoint);
		SetMesh();
	}

	public void AddPoint(Vector3 p, Vector3 up, Vector3 side, Vector4 rct, Color clr)
	{
		DrawPoint drawPoint = new DrawPoint();
		drawPoint.pos = p;
		drawPoint.up = up;
		drawPoint.side = side;
		drawPoint.rct = rct;
		drawPoint.rct.z += drawPoint.rct.x;
		drawPoint.rct.w += drawPoint.rct.y;
		drawPoint.rct.x *= _texFacX;
		drawPoint.rct.z *= _texFacX;
		drawPoint.rct.y *= _texFacY;
		drawPoint.rct.w *= _texFacY;
		drawPoint.color = clr;
		_Buffer.Add(drawPoint);
		SetMesh();
	}

	public void AddLine(Vector3 p, Vector3 p2, Vector3 up, Vector3 side, float sc, Color clr)
	{
		DrawPoint drawPoint = new DrawPoint();
		drawPoint.pos = p;
		drawPoint.pos2 = p2;
		drawPoint.up = up * sc;
		drawPoint.side = side * sc;
		drawPoint.rct = new Vector4(0f, 0f, 1f, 1f);
		drawPoint.color = clr;
		_Buffer.Add(drawPoint);
		SetLineMesh();
	}

	public void AddLine2(Vector3 p, Vector3 p2, Vector3 up, Vector3 side, float sc, Color clr)
	{
		DrawPoint drawPoint = new DrawPoint();
		drawPoint.pos = p;
		drawPoint.pos2 = p2;
		drawPoint.up = up * sc;
		drawPoint.side = side * sc;
		drawPoint.rct = new Vector4(0f, 0f, 1f, 1f);
		drawPoint.color = clr;
		_Buffer.Add(drawPoint);
	}

	public Vector3 GetPos()
	{
		return base.transform.position;
	}

	private void Awake()
	{
		if (_asMgrShadow)
		{
			DrawPointClass.point = this;
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

	private void SetMesh()
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
				array[num] = drawPoint.pos - drawPoint.side + drawPoint.up;
				array[num + 1] = drawPoint.pos - drawPoint.side - drawPoint.up;
				array[num + 2] = drawPoint.pos + drawPoint.side - drawPoint.up;
				array[num + 3] = drawPoint.pos + drawPoint.side + drawPoint.up;
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
			_Size = 0;
		}
	}

	public void SetLineMesh()
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
				array[num] = drawPoint.pos2 + drawPoint.up;
				array[num + 1] = drawPoint.pos2 - drawPoint.up;
				array[num + 2] = drawPoint.pos - drawPoint.up;
				array[num + 3] = drawPoint.pos + drawPoint.up;
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
			_Size = 0;
		}
	}
}
