using UnityEngine.UI;
using UnityEngine;
using Vuforia;

public class SpawItem : MonoBehaviour
{
    public ContentPositioningBehaviour contentPositioningBehaviour;

	public GameObject itemPrefab;

	[SerializeField]
    private ItemType _itemType;

    private Toggle _ownerToggle;

    // Use this for initialization
    void Start()
    {
        _ownerToggle = GetComponent<Toggle>();
        _ownerToggle.onValueChanged.AddListener(OnButtonSpawClick);
        var itemBase = contentPositioningBehaviour.AnchorStage.GetComponent<ItemBase>();
        if (itemBase != null)
            _itemType = itemBase.itemType;
    }

    public void OnButtonSpawClick(bool isOn)
    {
        if (isOn)
        {
            ARGameManager.Instance.OnSetSelectedPlanFilter(itemPrefab, _itemType);
        }
    }

#if UNITY_EDITOR

    ItemBase _itemBase = null;
    private void OnValidate()
    {
        if (Application.isPlaying || _itemBase != null) return;

        _itemBase = contentPositioningBehaviour.AnchorStage.GetComponent<ItemBase>();
        if (_itemBase != null)
            _itemType = _itemBase.itemType;
    }
#endif
}
