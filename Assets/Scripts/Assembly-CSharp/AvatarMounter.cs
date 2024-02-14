using UnityEngine;

public class AvatarMounter
{
	public static void Mount(GameObject avatarObj, GameObject partObj, Vector3 offset)
	{
		partObj.transform.parent = avatarObj.transform;
		partObj.transform.localPosition = offset;
	}

	public static void MountSkinnedMesh(GameObject avatarObj, GameObject partModel, string partName)
	{
		SkinnedMeshRenderer skinnedMeshRenderer = CheckAvatarMounted(avatarObj);
		SkinnedMeshRenderer skinnedMeshRenderer2 = null;
		if (skinnedMeshRenderer == null)
		{
			GameObject gameObject = new GameObject(partName);
			gameObject.layer = partModel.layer;
			Mount(avatarObj, gameObject, Vector3.zero);
			skinnedMeshRenderer2 = gameObject.AddComponent<SkinnedMeshRenderer>();
		}
		else
		{
			skinnedMeshRenderer.gameObject.name = partName;
			skinnedMeshRenderer2 = skinnedMeshRenderer;
		}
		SkinnedMeshRenderer componentInChildren = partModel.GetComponentInChildren<SkinnedMeshRenderer>();
		skinnedMeshRenderer2.sharedMesh = componentInChildren.sharedMesh;
		skinnedMeshRenderer2.sharedMaterials = componentInChildren.sharedMaterials;
		Transform[] array = new Transform[componentInChildren.bones.Length];
		for (int i = 0; i < componentInChildren.bones.Length; i++)
		{
			Transform transform = null;
			Transform[] componentsInChildren = avatarObj.GetComponentsInChildren<Transform>();
			foreach (Transform transform2 in componentsInChildren)
			{
				if (transform2.name == componentInChildren.bones[i].name)
				{
					transform = transform2;
					break;
				}
			}
			if (null == transform)
			{
				Debug.LogError("MountSkinnedMesh : bones do not match ( " + partModel.name + " )");
				break;
			}
			array[i] = transform;
		}
		skinnedMeshRenderer2.bones = array;
		skinnedMeshRenderer2.updateWhenOffscreen = true;
	}

	private static SkinnedMeshRenderer CheckAvatarMounted(GameObject avatarObj)
	{
		SkinnedMeshRenderer skinnedMeshRenderer = null;
		foreach (Transform item in avatarObj.transform)
		{
			skinnedMeshRenderer = item.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
			if (skinnedMeshRenderer != null)
			{
				return skinnedMeshRenderer;
			}
		}
		return skinnedMeshRenderer;
	}
}
