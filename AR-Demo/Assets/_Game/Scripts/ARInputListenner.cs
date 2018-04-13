using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ARInputListenner : MonoBehaviour
{
    public UnityAction<Vector2> inputHit;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            OnDrag(Input.touches[0].position);
        }
    }

    public void OnDrag(Vector2 position)
    {
        Debug.Log("On drag" + position);
        if (inputHit != null)
            inputHit.Invoke(position);
    }

}
