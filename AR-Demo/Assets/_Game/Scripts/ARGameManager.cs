using UnityEngine;
using Vuforia;
using System;

public class ARGameManager : Singleton<ARGameManager>
{
	public GameObject groundPlaneObject;
	public GameObject midAirObject;
	public Transform castShadow;

	public ItemType currentItemType = ItemType.None;

	public Transform wallTransform;


	public ItemBase currentItemSelected { get; private set; }

	private GameObject _itemPrefab = null;

	private event Action<ItemBase> _onSelectedItem;
	private event Action _onResetSpawner;

	public bool IsHitTest { get; private set; }

	private bool initGroud = false;

	private StateManager _stateManager;

	private SmartTerrain _smartTerrain;

	private PositionalDeviceTracker _positionalDeviceTracker;

	private int m_AnchorCounter;


	private void Start()
	{
		VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
		VuforiaARController.Instance.RegisterOnPauseCallback(OnVuforiaPaused);
		DeviceTrackerARController.Instance.RegisterTrackerStartedCallback(OnTrackerStarted);
	}

	/// <summary>
	/// select a item
	/// </summary>
	/// <param name="item"></param>
	public void OnSetSelectedItem(ItemBase item)
	{
		if (currentItemSelected == item) return;
		currentItemSelected = item;

		if (item != null)
		{
			if (currentItemSelected.itemType == ItemType.MidAir)
			{
				var itemTrans = currentItemSelected.transform;

				wallTransform.gameObject.SetActive(true);
				wallTransform.position = itemTrans.position;
				wallTransform.rotation = itemTrans.rotation;
			}
			else
			{
				wallTransform.gameObject.SetActive(false);
			}
		}
		else
		{
			wallTransform.gameObject.SetActive(false);
		}
		if (_onSelectedItem != null)
			_onSelectedItem.Invoke(currentItemSelected);
	}

	/// <summary>
	/// Reset spawner
	/// </summary>
	public void OnResetSpanner()
	{
		if (groundPlaneObject != null)
			groundPlaneObject.SetActive(false);

		if (midAirObject != null)
			midAirObject.SetActive(false);

		currentItemType = ItemType.None;

		if (_onResetSpawner != null)
			_onResetSpawner.Invoke();
	}

	/// <summary>
	/// Delete current item
	/// </summary>
	public void OnDeleteSelectedItem()
	{
		GameObject currentItem = currentItemSelected.gameObject;
		OnSetSelectedItem(null);

		if (currentItem != null)
		{
			Destroy(currentItem);
		}

		OnResetSpanner();
	}

	/// <summary>
	/// Put and unselect current item 
	/// </summary>
	public void OnPutItem()
	{
		OnResetSpanner();
		OnSetSelectedItem(null);
	}

	/// <summary>
	/// Listening event form toggle to switch spawner
	/// </summary>
	/// <param name="contentPositioning"></param>
	/// <param name="itemType"></param>
	public void OnSetSelectedPlanFilter(GameObject itemPrefab, ItemType itemType)
	{
		// Un-select current item 
		OnSetSelectedItem(null);

		currentItemType = itemType;
		_itemPrefab = itemPrefab;

		if (currentItemType == ItemType.Plane)
		{
			groundPlaneObject.SetActive(true);
			midAirObject.SetActive(false);
		}
		else
		{
			groundPlaneObject.SetActive(false);
			midAirObject.SetActive(true);
		}
	}

	/// <summary>
	/// Vuforia event
	/// Hit to ground plane
	/// </summary>
	/// <param name="hitResult"></param>
	public void OnInteractiveHit(HitTestResult hitResult)
	{
		if (hitResult == null || currentItemType != ItemType.Plane || _itemPrefab == null) return;

		var planeAnchor = _positionalDeviceTracker.CreatePlaneAnchor(_itemPrefab.name + "PlaneAnchor" + m_AnchorCounter++, hitResult);

		var item = Instantiate(_itemPrefab, planeAnchor.transform).transform;

		if (CustomPlaneIndicator.isCastedToItem)
			item.position = CustomPlaneIndicator.positionCast;
		else
			item.localPosition = Vector3.zero;

		item.localRotation = Quaternion.identity;

		Vector3 pos = planeAnchor.transform.position;
		OnSetGroundCastShadow(pos);
		//floorTransform.position = pos;

		OnResetSpanner();
	}



	/// <summary>
	/// Vuforia event
	/// Hit to ground plane
	/// </summary>
	/// <param name="hitResult"></param>
	public void OnAnchorPositionComfirmed(Transform target)
	{
		if (target == null || currentItemType != ItemType.MidAir || _itemPrefab == null) return;

		var planeAnchor = _positionalDeviceTracker.CreateMidAirAnchor(_itemPrefab.name + "PlaneAnchor" + m_AnchorCounter++, target);

		var item = Instantiate(_itemPrefab, planeAnchor.transform).transform;
		item.localPosition = Vector3.zero;
		item.localRotation = Quaternion.identity;

		Vector3 pos = planeAnchor.transform.position;
		OnSetGroundCastShadow(pos);

		OnResetSpanner();
	}


	/// <summary>
	/// Vuforia event
	/// Hit to ground plane
	/// </summary>
	/// <param name="hitResult"></param>
	/// 
	public void OnInteractiveAutoHit(HitTestResult hitResult)
	{
		OnSetGroundCastShadow(hitResult.Position);

		//Debug.Log("Auto hit:" + hitResult);


		//if (Time.frameCount % 10 == 0)
		//{
		//	var hit = HitOnStartTerrain(hitResult.Position);
		//	Instantiate(test, hit.Position, Quaternion.identity);
		//}

		return;
		//		if (hitResult == null || currentItemType != ItemType.Plane || _itemPrefab == null) return;

		//#if UNITY_EDITOR
		//		Instantiate(_itemPrefab.AnchorStage, hitResult.Position, Quaternion.identity, null);
		//		floorTransform.position = hitResult.Position;
		//#else
		//		_currentContentPositioning.PositionContentAtPlaneAnchor(hitResult);

		//		Vector3 pos = hitResult.Position;
		//		//Instantiate(_currentContentPositioning.AnchorStage, pos, hitResult.Rotation, null);
		//		OnSetGroundCastShadow(pos);
		//		floorTransform.position = pos;
		//#endif

		//OnResetSpanner();
	}


	/// <summary>
	/// Add select item event listeners
	/// </summary>
	/// <param name="onSelectedItem"></param>
	public void RegiterSelectionItemListener(Action<ItemBase> onSelectedItem)
	{
		_onSelectedItem += onSelectedItem;
	}

	public void UnRegiterSelectionItemListener(Action<ItemBase> onSelectedItem)
	{
		_onSelectedItem -= onSelectedItem;
	}

	/// <summary>
	/// add reset event listeners
	/// </summary>
	/// <param name="onReset"></param>
	public void AddResetItemListener(Action onReset)
	{
		_onResetSpawner += onReset;
	}

	public void OnSetGroundCastShadow(Vector3 position)
	{
		if (initGroud) return;
		initGroud = true;

		castShadow.position = position;
	}

	#region VUFORIA_CALLBACKS

	void OnTrackerStarted()
	{
		Debug.Log("OnTrackerStarted() called.");

		_positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
		_smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

		if (_positionalDeviceTracker != null)
		{
			if (!_positionalDeviceTracker.IsActive)
				_positionalDeviceTracker.Start();

			Debug.Log("PositionalDeviceTracker is Active?: " + _positionalDeviceTracker.IsActive +
					  "\nSmartTerrain Tracker is Active?: " + _smartTerrain.IsActive);
		}
	}

	private void OnVuforiaStarted()
	{
		initGroud = false;
		_stateManager = TrackerManager.Instance.GetStateManager();

		// Check trackers to see if started and start if necessary
		_positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
		_smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

		if (_positionalDeviceTracker != null && _smartTerrain != null)
		{
			if (!_positionalDeviceTracker.IsActive)
				_positionalDeviceTracker.Start();
			if (_positionalDeviceTracker.IsActive && !_smartTerrain.IsActive)
				_smartTerrain.Start();
		}
		else
		{
			if (_positionalDeviceTracker == null)
				Debug.Log("PositionalDeviceTracker returned null. GroundPlane not supported on this device.");
			if (_smartTerrain == null)
				Debug.Log("SmartTerrain returned null. GroundPlane not supported on this device.");
		}
	}

	public void ResetTrackers()
	{
		_smartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();
		_positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();

		// Stop and restart trackers
		_smartTerrain.Stop(); // stop SmartTerrain tracker before PositionalDeviceTracker
		_positionalDeviceTracker.Stop();
		_positionalDeviceTracker.Start();
		_smartTerrain.Start(); // start SmartTerrain tracker after PositionalDeviceTracker
	}


	void OnVuforiaPaused(bool paused)
	{
		Debug.Log("OnVuforiaPaused(" + paused.ToString() + ") called.");

		if (paused)
			ResetScene();
	}

	public void ResetScene()
	{
		Debug.Log("ResetScene() called.");

		OnResetSpanner();

		OnSetSelectedItem(null);
	}
	#endregion Vuforia callback


	public HitTestResult HitOnStartTerrain(Vector2 pos)
	{
		HitTestResult hit = null;
		//Vector2 viewPos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
		_smartTerrain.HitTest(pos, 10f, out hit);
		return hit;
	}
}
