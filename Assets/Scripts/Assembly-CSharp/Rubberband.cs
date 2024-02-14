using UnityEngine;

public class Rubberband : MonoBehaviour
{
	public struct Rubber
	{
		public Vector3 pos1;

		public Vector3 pos2;

		public Vector3 up;

		public Vector3 side;

		public Vector4 rct;

		public Color color;
	}

	public Material _Material;

	public Texture _Texture;

	public int m_capacity = 128;

	protected float _texFacX;

	protected float _texFacY;

	protected int _Size;

	protected Rubber[] _Buffer;

	protected MeshFilter _MeshFilter;

	protected MeshRenderer _MeshRenderer;

	protected Mesh _Mesh;

	public void AddRubber(Vector3 point1, Vector3 point2, Vector3 up, Vector3 side, Color clr)
	{
		if (_Size < m_capacity)
		{
			Rubber rubber = default(Rubber);
			rubber.pos1 = point1;
			rubber.pos2 = point2;
			rubber.up = up * 0.1f;
			rubber.side = side * 0f;
			rubber.rct = new Vector4(0f, 0f, 1f, 1f);
			rubber.color = clr;
			_Buffer[_Size] = rubber;
			_Size++;
		}
	}

	private void Awake()
	{
		RubberbandClass.rubber = this;
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
		_Material.SetTexture("_MainTex", _Texture);
		_texFacX = 1f / (float)_Texture.width;
		_texFacY = 1f / (float)_Texture.height;
		_Buffer = new Rubber[m_capacity];
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
				Rubber rubber = _Buffer[i];
				int num = i * 4;
				array[num] = rubber.pos1 - rubber.side + rubber.up;
				array[num + 1] = rubber.pos1 - rubber.side - rubber.up;
				array[num + 2] = rubber.pos2 + rubber.side - rubber.up;
				array[num + 3] = rubber.pos2 + rubber.side + rubber.up;
				array2[num] = new Vector2(rubber.rct.x, rubber.rct.w);
				array2[num + 1] = new Vector2(rubber.rct.x, rubber.rct.y);
				array2[num + 2] = new Vector2(rubber.rct.z, rubber.rct.y);
				array2[num + 3] = new Vector2(rubber.rct.z, rubber.rct.w);
				array3[num] = rubber.color;
				array3[num + 1] = rubber.color;
				array3[num + 2] = rubber.color;
				array3[num + 3] = rubber.color;
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
