using UnityEngine;
using Vuforia;

[RequireComponent(typeof(Camera))]
public class CustomPlaneIndicator : MonoBehaviour
{
    public static Vector3 positionCast = Vector3.zero;

    public static bool isCastedToItem = false;

    private static Transform arIndicator;

    private static Renderer arIndicatorRenderer;

    public PlaneFinderBehaviour planeFinderBehaviour;

    public LayerMask layerSelectItem;

	public LayerMask layerCastIndicator;

	public Transform targetImage;

    private Camera _camera;

    private PositionalDeviceTracker positionalDeviceTracker;

    // Use this for initialization
    void Start()
    {
        DeviceTrackerARController.Instance.RegisterTrackerStartedCallback(OnTrackerStarted);
        
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && ARGameManager.Instance.currentItemType == ItemType.None && ARGameManager.Instance.currentItemSelected == null)
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hitInfo, layerSelectItem.value);
            if (hit)
            {
                var obj = hitInfo.transform.gameObject;
                var item = obj.GetComponent<ItemBase>();
                if (item != null)
                {
                    ARGameManager.Instance.OnSetSelectedItem(item);
                }
            }
        }

        bool isTrackGround = arIndicatorRenderer.enabled && ARGameManager.Instance.currentItemType == ItemType.Plane;

        targetImage.gameObject.SetActive(isTrackGround);
        if (isTrackGround)
        {
            RaycastHit hitItem = new RaycastHit();
            if (Physics.Raycast(transform.position, transform.forward, out hitItem, 50, layerCastIndicator.value))
            {
                targetImage.position = hitItem.point;
                positionCast = hitItem.point;
				isCastedToItem = true;
                if (arIndicator != null)
                {
                    targetImage.rotation = arIndicator.rotation;
                }
            }
            else
            {
                targetImage.rotation = arIndicator.rotation;
				positionCast  = targetImage.position = arIndicator.position;
                isCastedToItem = false;
            }
        }
    }

    void OnTrackerStarted()
    {
        Debug.Log("OnTrackerStarted() called.");

        positionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();

        if (positionalDeviceTracker != null)
        {
            if (!positionalDeviceTracker.IsActive)
                positionalDeviceTracker.Start();

            Debug.Log("PositionalDeviceTracker is Active?: " + positionalDeviceTracker.IsActive);
        }
    }

    public static void SetIndicator(Transform indicatorTransform, Renderer indicatorRenderer)
    {
        arIndicator = indicatorTransform;
        arIndicatorRenderer = indicatorRenderer;
    }
}
