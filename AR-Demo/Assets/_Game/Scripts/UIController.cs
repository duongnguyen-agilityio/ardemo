using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject scroll;

    public GameObject itemEditor;

    public ToggleGroup toggleGroup;
    
    public Button putItem;
    public Button deleteItem;

    public Transform arCamera;

    private void Start()
    {
        ARGameManager.Instance.AddResetItemListener(OnReset);
        ARGameManager.Instance.RegiterSelectionItemListener(OnSelectionItem);
        
        putItem.onClick.AddListener(OnButtonPut);
        deleteItem.onClick.AddListener(OnButtonDelete);

        scroll.SetActive(false);
        Invoke("SetActiveScroll", 3);

    }

    void SetActiveScroll()
    {
        scroll.SetActive(true);
    }

    void OnSelectionItem(ItemBase item)
    {
        itemEditor.SetActive(item != null);
    }

    /// <summary>
    /// 
    /// </summary>
    void OnReset()
    {
        toggleGroup.SetAllTogglesOff();
    }

    /// <summary>
    /// 
    /// </summary>
    void OnButtonPut()
    {
        ARGameManager.Instance.OnPutItem();
    }

    /// <summary>
    /// 
    /// </summary>
    void OnButtonDelete()
    {
        ARGameManager.Instance.OnDeleteSelectedItem();
    }
}

