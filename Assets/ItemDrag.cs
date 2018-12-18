using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        transform.position = Input.mousePosition;

#elif UNITY_IOS || UNITY_ANDROID

        transform.position = Input.GetTouch(0).position;
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE


        if (Input.GetMouseButtonDown(0) == true)
        {
            Debug.Log(Input.mousePosition);
        }
#elif UNITY_IOS || UNITY_ANDROID
 
        if (Input.touchCount > 0)
        {
            Debug.Log(Input.GetTouch(0).position);
        }
#endif
    }
}
