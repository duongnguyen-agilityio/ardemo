using UnityEngine;

public class AddIndicatorToCustom : MonoBehaviour
{

    public Renderer rendererIndicator;

    // Use this for initialization
    void Start()
    {
        CustomPlaneIndicator.SetIndicator( transform, rendererIndicator);
    }
}
