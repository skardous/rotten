using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CookManager : MonoBehaviour, IDragHandler
{
    private bool cooking = false;
    private float cookStart = 0;
    public Sprite idleImage;
    public Sprite busyImage;
    Image myImage;

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
        Slider slider = gameObject.transform.Find("Slider").GetComponent<Slider>();
        float cookTime = (Time.time - cookStart);
        if (cooking == true && cookTime > 3)
        {
            cooking = false;
            myImage = gameObject.transform.Find("CookImage").GetComponent<Image>();
            myImage.sprite = idleImage;
            slider.normalizedValue = 0;
            gameObject.transform.Find("Slider").gameObject.SetActive(false);
        }

        Debug.Log(slider.value);
        if (cooking == true)
        {
            gameObject.transform.Find("Slider").gameObject.SetActive(true);
            slider.normalizedValue = (cookTime * 1) / 3;
        }
    }

    public void StartCooking(GameObject fruit)
    {
        cooking = true;
        cookStart = Time.time;
        myImage = gameObject.transform.Find("CookImage").GetComponent<Image>();
        myImage.sprite = busyImage;
    }

    public bool IsCooking()
    {
        return cooking;
    }
}
