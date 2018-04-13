using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidAirItem : ItemBase
{

#if UNITY_EDITOR
	protected new void OnValidate()
	{
		base.OnValidate();
		itemType = ItemType.MidAir;
	}
#endif

	public override void OnRotation(Vector3 cache, float rotation)
	{
		//transform.localEulerAngles = cache - new Vector3(0, 0, -rotation * 3f);
	}

	public override void SetActiveIndicator(bool isRotation, bool isTranslate, bool isScale)
	{
		if (_movingIndicator != null)
			_movingIndicator.SetActive(isTranslate);
	}
}
