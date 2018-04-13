using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPlaneItem : ItemBase
{

#if UNITY_EDITOR
	protected new void OnValidate()
	{
		base.OnValidate();
		itemType = ItemType.Plane;
	}
#endif


	public override void OnRotation(Vector3 cache, float rotation)
	{
		if (_canRotation)
			transform.localEulerAngles = cache - new Vector3(0, rotation * 3f, 0);
	}

}
