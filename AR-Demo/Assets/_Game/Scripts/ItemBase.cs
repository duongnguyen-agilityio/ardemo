using UnityEngine;
using System.Collections;

public class ItemBase : MonoBehaviour
{
	public ItemType itemType;

	[SerializeField]
	protected GameObject _movingIndicator;

	[SerializeField]
	protected GameObject _rotatingIndicator;

	[SerializeField]
	protected GameObject _selectedIndicator;

	[SerializeField]
	protected bool _canRotation;

	[SerializeField]
	protected bool _canScale;

	[SerializeField]
	protected Collider[] _colliders;

	protected bool _isSelected = false;

#if UNITY_EDITOR
	protected void OnValidate()
	{
		_colliders = GetComponentsInChildren<Collider>();
	}
#endif

	public void Start()
	{
		ARGameManager.Instance.RegiterSelectionItemListener(OnSelectedItem);
		if (Input.touchCount == 0)
			ARGameManager.Instance.OnSetSelectedItem(this);
		else
			StartCoroutine(WatingForReleaseTouch());

	}

	public virtual void OnRotation(Vector3 cache, float rotation)
	{

	}

	public virtual void SetActiveIndicator(bool isRotation, bool isTranslate, bool scale)
	{
		if (_movingIndicator != null)
			_movingIndicator.SetActive(isTranslate);
		if (_rotatingIndicator != null)
			_rotatingIndicator.SetActive(isRotation && _canRotation);
	}


	public virtual void OnScale(float scale)
	{
		if (_canScale)
			transform.localScale = new Vector3(scale, scale, scale);
	}

	IEnumerator WatingForReleaseTouch()
	{
		yield return new WaitUntil(() => Input.touchCount == 0);
		ARGameManager.Instance.OnSetSelectedItem(this);
	}


	protected void OnSelectedItem(ItemBase selectedItem)
	{
		bool selected = selectedItem == this;
		if (_isSelected != selected)
		{
			_isSelected = selected;
			foreach (var col in _colliders)
			{
				col.enabled = !_isSelected;
			}

			_selectedIndicator.SetActive(_isSelected);
		}
	}

	protected void OnDestroy()
	{
		ARGameManager.Instance.UnRegiterSelectionItemListener(OnSelectedItem);
	}

}

public enum ItemType
{
	None,
	MidAir,
	Plane
}