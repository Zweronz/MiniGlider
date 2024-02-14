using UnityEngine;

public class footShadow : MonoBehaviour
{
	public struct Shadow
	{
		public Vector3 pos;

		public Vector3 up;

		public Vector3 side;

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

	protected Shadow[] _Buffer;

	protected MeshFilter _MeshFilter;

	protected MeshRenderer _MeshRenderer;

	protected Mesh _Mesh;

	public void AddShadow(Vector3 p, Vector3 up, Vector3 side, Color clr)
	{
		if (_Size < _Capacity)
		{
			Shadow shadow = default(Shadow);
			shadow.pos = p;
			shadow.up = up * 0.5f;
			shadow.side = side * 0.5f;
			shadow.rct = new Vector4(0f, 0f, 1f, 1f);
			shadow.color = clr;
			_Buffer[_Size] = shadow;
			_Size++;
		}
	}

	public void AddShadow(Vector3 p, Vector3 up, Vector3 side, float sc, Color clr)
	{
		if (_Size < _Capacity)
		{
			Shadow shadow = default(Shadow);
			shadow.pos = p;
			shadow.up = up * sc;
			shadow.side = side * sc;
			shadow.rct = new Vector4(0f, 0f, 1f, 1f);
			shadow.color = clr;
			_Buffer[_Size] = shadow;
			_Size++;
		}
	}

	public void AddShadow(Vector3 p, Vector3 up, Vector3 side, Vector4 rct, Color clr)
	{
		if (_Size < _Capacity)
		{
			Shadow shadow = default(Shadow);
			shadow.pos = p;
			shadow.up = up;
			shadow.side = side;
			shadow.rct = rct;
			shadow.rct.z += shadow.rct.x;
			shadow.rct.w += shadow.rct.y;
			shadow.rct.x *= _texFacX;
			shadow.rct.z *= _texFacX;
			shadow.rct.y *= _texFacY;
			shadow.rct.w *= _texFacY;
			shadow.color = clr;
			_Buffer[_Size] = shadow;
			_Size++;
		}
	}

	private void Awake()
	{
		if (_asMgrShadow)
		{
			ShadowManager.shadow = this;
		}
		base.gameObject.transform.position = new Vector3(0f, 0.018f, 0f);
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
		_Buffer = new Shadow[_Capacity];
		_Size = 0;
	}

	public void LateUpdate()
	{
		_Mesh.Clear();
		if (_Size > 0)
		{
			Vector3[] array = new Vector3[_Size * 4];
			Vector2[] array2 = new Vector2[_Size * 4];
			Color[] array3 = new Color[_Size * 4];
			int[] array4 = new int[_Size * 6];
			for (int i = 0; i < _Size; i++)
			{
				Shadow shadow = _Buffer[i];
				int num = i * 4;
				array[num] = shadow.pos - shadow.side + shadow.up;
				array[num + 1] = shadow.pos - shadow.side - shadow.up;
				array[num + 2] = shadow.pos + shadow.side - shadow.up;
				array[num + 3] = shadow.pos + shadow.side + shadow.up;
				array2[num] = new Vector2(shadow.rct.x, shadow.rct.w);
				array2[num + 1] = new Vector2(shadow.rct.x, shadow.rct.y);
				array2[num + 2] = new Vector2(shadow.rct.z, shadow.rct.y);
				array2[num + 3] = new Vector2(shadow.rct.z, shadow.rct.w);
				array3[num] = shadow.color;
				array3[num + 1] = shadow.color;
				array3[num + 2] = shadow.color;
				array3[num + 3] = shadow.color;
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
