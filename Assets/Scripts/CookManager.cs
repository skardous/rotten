using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CookManager : MonoBehaviour, IDragHandler
{
    private bool cooking = false;
    private float cookStart = 0;
    private ConsumableManager.ConsumableTypeEnum cookingConsumableType;
    private Image myImage;

    public enum CookTypeEnum { VegetableCook, FruitCook, NormalCook };
    public CookTypeEnum cookType;

    public Sprite idleImage;
    //public Sprite normalIdleImage;
    //public Sprite vegIdleImage;
    //public Sprite fruitIdleImage;
    public Sprite busyImage;

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
        //cookType = CookTypeEnum.NormalCook;
        myImage = gameObject.transform.Find("CookImage").GetComponent<Image>();
        //myImage.sprite = normalIdleImage;

    }

    // Update is called once per frame
    void Update()
    {
        Slider slider = gameObject.transform.Find("Slider").GetComponent<Slider>();
        float cookTime = (Time.time - cookStart);

        if (cooking == true)
        {
            int cookingDuration = GetCookingDuration();
            if (cookTime > cookingDuration)
            {
                cooking = false;
                SetIdle();
                slider.normalizedValue = 0;
                gameObject.transform.Find("Slider").gameObject.SetActive(false);
            }
        }

        if (cooking == true)
        {
            gameObject.transform.Find("Slider").gameObject.SetActive(true);
            slider.normalizedValue = cookTime / GetCookingDuration();
        }
    }

    public void StartCooking(GameObject consumable)
    {
        cooking = true;
        cookStart = Time.time;
        //myImage.sprite = busyImage;
        ConsumableManager consumableManager = (ConsumableManager)consumable.GetComponent(typeof(ConsumableManager));
        cookingConsumableType = consumableManager.type;
    }

    public bool IsCooking()
    {
        return cooking;
    }

    public void SetCookType(CookTypeEnum type)
    {
        Debug.Log(type);
        cookType = type;
        SetIdle();
    }

    public int GetCookingDuration()
    {
        if (cookType == CookTypeEnum.NormalCook)
        {
            return 3;
        }

        if (cookType == CookTypeEnum.VegetableCook )
        {
            if (cookingConsumableType == ConsumableManager.ConsumableTypeEnum.Vegetable)
            {
                return 1;
            } else
            {
                return 4;
            }
        }

        if (cookType == CookTypeEnum.FruitCook)
        {
            if (cookingConsumableType == ConsumableManager.ConsumableTypeEnum.Fruit)
            {
                return 1;
            }
            else
            {
                return 4;
            }
        }

        return 3;
    }

    public void SetIdle()
    {
        myImage = gameObject.transform.Find("CookImage").GetComponent<Image>();
        myImage.sprite = idleImage;
        //switch (cookType)
        //{
        //    case CookTypeEnum.NormalCook:
        //        myImage.sprite = normalIdleImage;
        //        Debug.Log("normal");
        //        break;
        //    case CookTypeEnum.FruitCook:
        //        myImage.sprite = fruitIdleImage;
        //        Debug.Log("fruit");
        //        break;
        //    case CookTypeEnum.VegetableCook:
        //        myImage.sprite = vegIdleImage;
        //        Debug.Log("veg");
        //        break;
        //    default:
        //        myImage.sprite = normalIdleImage;
        //        Debug.Log("default");
        //        break;
        //}
    }
}
