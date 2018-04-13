/*============================================================================== 
Copyright (c) 2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.   
==============================================================================*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vuforia;

public class TouchHandler : MonoBehaviour
{
    #region PUBLIC_MEMBERS

    public bool enableRotation;

    public bool enablePinchScaling;

    public LayerMask floorLayer;

    public LayerMask wallLayer;

    public Camera mainCamera;

    private Transform selectedItemTransform;

    private ItemBase _currentItem;

    private Ray cameraToPlaneRay;

    private RaycastHit cameraToPlaneHit;

    private int _currentLayerIndex = 0;

    public static bool DoubleTap
    {
        get { return (Input.touchSupported) && Input.touches[0].tapCount == 2; }
    }

    #endregion // PUBLIC MEMBERS


    #region PRIVATE_MEMBERS
    const float scaleRangeMin = 0.1f;
    const float scaleRangeMax = 2.0f;

    Touch[] touches;
    static int lastTouchCount;
    bool isFirstFrameWithTwoTouches;
    float cachedTouchAngle;
    float cachedTouchDistance;
    float cachedAugmentationScale;
    Vector3 cachedAugmentationRotation;
    GraphicRaycaster m_GraphicRayCaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    #endregion // PRIVATE_MEMBERS

    #region MONOBEHAVIOUR_METHODS

    void Start()
    {
        ARGameManager.Instance.RegiterSelectionItemListener(OnSelectItem);
        m_EventSystem = FindObjectOfType<EventSystem>();
        m_GraphicRayCaster = FindObjectOfType<GraphicRaycaster>();
        mainCamera = Camera.main;
    }

    void OnSelectItem(ItemBase item)
    {
        // reset indicators of last item
        if (_currentItem != null)
            _currentItem.SetActiveIndicator(false, false, false);

        if (item == null)
        {
            selectedItemTransform = null;
            _currentItem = null;
            _currentLayerIndex = 0;
        }
        else
        {
            _currentItem = item;
            selectedItemTransform = _currentItem.transform;
            cachedAugmentationScale = selectedItemTransform.localScale.x;
            cachedAugmentationRotation = selectedItemTransform.localEulerAngles;

            _currentLayerIndex = _currentItem.itemType == ItemType.MidAir ? wallLayer.value : floorLayer.value;
        }
    }


    void Update()
    {
        if (selectedItemTransform == null) return;

        if (!IsCanvasButtonPressed())
        {
            touches = Input.touches;

            if (Input.touchCount == 2)
            {
                float currentTouchDistance = Vector2.Distance(touches[0].position, touches[1].position);
                float diff_y = touches[0].position.y - touches[1].position.y;
                float diff_x = touches[0].position.x - touches[1].position.x;
                float currentTouchAngle = Mathf.Atan2(diff_y, diff_x) * Mathf.Rad2Deg;

                if (isFirstFrameWithTwoTouches)
                {
                    cachedTouchDistance = currentTouchDistance;
                    cachedTouchAngle = currentTouchAngle;
                    isFirstFrameWithTwoTouches = false;
                }

                float angleDelta = currentTouchAngle - cachedTouchAngle;
                float scaleMultiplier = (currentTouchDistance / cachedTouchDistance);
                float scaleAmount = cachedAugmentationScale * scaleMultiplier;
                float scaleAmountClamped = Mathf.Clamp(scaleAmount, scaleRangeMin, scaleRangeMax);

                if (enableRotation)
                {
                    _currentItem.OnRotation(cachedAugmentationRotation, angleDelta);
                }
                if (enableRotation && enablePinchScaling)
                {
                    _currentItem.OnScale(scaleAmountClamped);
                }

            }
            else if (Input.touchCount < 2)
            {
                cachedAugmentationScale = selectedItemTransform.localScale.x;
                cachedAugmentationRotation = selectedItemTransform.localEulerAngles;
                isFirstFrameWithTwoTouches = true;
            }
            else if (Input.touchCount == 6)
            {
                // enable runtime testing of pinch scaling
                enablePinchScaling = true;
            }
            else if (Input.touchCount == 5)
            {
                // disable runtime testing of pinch scaling
                enablePinchScaling = false;
            }
        }

        _currentItem.SetActiveIndicator(Input.touchCount == 2, IsSingleFingerDragging(), enableRotation && enablePinchScaling);
        DragToMoveItem();
    }

    #endregion // MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS

    public static bool IsSingleFingerDragging()
    {
        if (Input.touchCount == 0 || Input.touchCount >= 2)
            lastTouchCount = Input.touchCount;

        return (
            Input.touchCount == 1 &&
            Input.touches[0].fingerId == 0 &&
            (Input.touches[0].phase == TouchPhase.Moved) &&
            lastTouchCount == 0);
    }


    bool IsCanvasButtonPressed()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        m_GraphicRayCaster.Raycast(m_PointerEventData, results);

        bool resultIsButton = false;
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponentInParent<Toggle>() ||
                result.gameObject.GetComponent<Button>())
            {
                resultIsButton = true;
                break;
            }
        }
        return resultIsButton;
    }


    void DragToMoveItem()
    {
        if (IsSingleFingerDragging() || (VuforiaRuntimeUtilities.IsPlayMode() && Input.GetMouseButton(0)))
        {
            if (!IsCanvasButtonPressed())
            {
                cameraToPlaneRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(cameraToPlaneRay, out cameraToPlaneHit, 100, _currentLayerIndex))
                {
                    selectedItemTransform.gameObject.PositionAt(cameraToPlaneHit.point);
                }
            }
        }
    }
    #endregion // PUBLIC_METHODS
}