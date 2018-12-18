using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    float lastTick = 0;
    float currentTime = 0;
    float playerScore = 0;
    //public Text scoreText;
    //public Text timeText;
    private List<GameObject> consumablesList = new List<GameObject>();
    private List<GameObject> vegetablesPrefabsList = new List<GameObject>();
    private List<GameObject> fruitsPrefabsList = new List<GameObject>();
    private List<GameObject> cooksList = new List<GameObject>();

    public List<GameObject> consumablesPrefabs;
    public GameObject cook;
    private bool gameInProgress = true;
    private int rottenConsumables = 0;
    public GameObject defeatPanel;
    public float ticksPerSecond;
    public GameObject sproutchText;
    public Slider rottenSlider;
    public GameObject canvas;

    public static System.Random rnd = new System.Random();

    // Use this for initialization
    void Start()
    {
        // spawn the cooks
        int[] spawnCooks = new int[]{ 1, 2, 3 };
        for(int i = 0; i < 3; i++)
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
            float canvasWidth = canvasRectTransform.rect.width;
            float canvasHeight = canvasRectTransform.rect.height;

            float canvasLeft = -1 * (canvasWidth / 2);

            float spawnX = (canvasLeft + (canvasWidth / 5) * spawnCooks[i]) + (canvasWidth / 5) / 2;

            GameObject cookCreated = Instantiate(cook, new Vector3(spawnX, 0, 0), Quaternion.identity) as GameObject;
            RectTransform cookTransform = cookCreated.GetComponent<RectTransform>();
            Vector2 size = cookTransform.sizeDelta;
            float ratio = size.y / size.x;
            cookTransform.sizeDelta = new Vector2((canvasWidth / 5), (canvasWidth / 5) * ratio);
            cookCreated.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
            cooksList.Add(cookCreated);
        }

        foreach (GameObject consumablePrefab in consumablesPrefabs)
        {
            ConsumableManager consManager = consumablePrefab.GetComponent<ConsumableManager>();
            if (consManager.type == ConsumableManager.ConsumableTypeEnum.Vegetable)
            {
                vegetablesPrefabsList.Add(consumablePrefab);
            } else if (consManager.type == ConsumableManager.ConsumableTypeEnum.Fruit)
            {
                fruitsPrefabsList.Add(consumablePrefab);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameInProgress)
        {
            if (Time.time - lastTick >= (1 / ticksPerSecond))
            {
                lastTick = Time.time;
                tick();
            }

            foreach (GameObject consumable in consumablesList)
            {
                moveConsumable(consumable);
            }
        }

        if (rottenSlider.value < (rottenConsumables/3f))
        {
            rottenSlider.value = rottenSlider.value + 0.01f;
        }

        checkLoss();
    }

    private void tick()
    {
        currentTime++;
        //Debug.Log(currentTime.ToString());
        //timeText.text = currentTime.ToString();
        if (itsSpawnTime())
        {
            int[] possiblePositions = { 0, 1, 2, 3, 4 };
            possiblePositions = Shuffle<int>(possiblePositions);

            for (int i = 0; i < getConsumableNbToSpawn(); i++)
            {
                int position = possiblePositions[i];
                spawnConsumable(position);
            }
        }

        //List<GameObject> enemiesToMove = new List<GameObject>();
        //foreach (GameObject consumable in consumablesList)
        //{
        //    consumablesTo.Add(element);
        //}

        //foreach (GameObject enemy in enemiesToMove)
        //{
        //    moveEnemy(enemy);
        //}

    }



    public void checkLoss()
    {
        //if (rottenConsumables > 0)
        //{
        //    gameInProgress = false;
        //    defeatPanel.SetActive(true);
        //}
    }

    public bool itsSpawnTime()
    {
        //Debug.Log("currentTime : " + currentTime);
        //if (currentTime < 10 && (currentTime % 5 == 0 || currentTime == 2))
        //    return true;

        //if (currentTime > 10 && currentTime < 20 && currentTime % 3 == 0)
        //    return true;

        //if (currentTime > 20 && currentTime < 30 && currentTime % 1 == 0)
        //    return true;

        //if (currentTime > 30 && currentTime % 1 == 0)
        //    return true;
        
        if(currentTime % 2 == 0)
        {
            return true;
        }

        return false;
    }

    public int getConsumableNbToSpawn()
    {
        if (currentTime < 30)
            return 1;

        if (currentTime < 40)
            return 2;

        return 3;
    }

    public void moveConsumable(GameObject consumable)
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        float canvasHeight = canvasRectTransform.rect.height;
        float moveY = -1 * (canvasHeight / 300);
        consumable.transform.Translate(new Vector3(0, moveY, 0));
        List<GameObject> overlappingCooks = getOverlappingCooks(consumable);
        if (overlappingCooks.Count > 0)
        {
            GameObject cook = overlappingCooks[0];
            CookManager cookManager = (CookManager)cook.GetComponent(typeof(CookManager));
            if (cookManager.IsCooking() == false)
            { 
                destroyConsumable(consumable);
                playerScore++;
                //scoreText.text = playerScore.ToString();
                cookManager.StartCooking(consumable);
            }
           
        }
        
        if (consumable.transform.position.y < 0)
        {
            rottenConsumables++;
            destroyConsumable(consumable);
            StartCoroutine(ShowMessage(1));
        }
    }

    IEnumerator ShowMessage(float delay)
    {
        sproutchText.SetActive(true);
        yield return new WaitForSeconds(delay);
        sproutchText.SetActive(false);
    }

    public List<GameObject> getCooks()
    {
        return cooksList;
    }

    public List<GameObject> getConsumables()
    {
        return consumablesList;
    }

    public void spawnConsumable(int spawnRandom)
    {
        int spawnType = Random.Range(0, 2);

        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRectTransform.rect.width;
        float canvasHeight = canvasRectTransform.rect.height;

        float canvasLeft = -1 * (canvasWidth / 2);

        float spawnX = (canvasLeft + (canvasWidth / 5) * spawnRandom) + (canvasWidth / 5)/2;

        GameObject consumable = vegetablesPrefabsList[0];
        if (spawnType == 0)
        {
            consumable = vegetablesPrefabsList[Random.Range(0, vegetablesPrefabsList.Count)];
        } else
        {
            consumable = fruitsPrefabsList[Random.Range(0, fruitsPrefabsList.Count)];
        }

        GameObject consumableCreated = Instantiate(consumable, new Vector3(spawnX, 1 * (canvasHeight / 2), 0), Quaternion.identity) as GameObject;
        RectTransform consumableTransform = consumableCreated.GetComponent<RectTransform>();
        BoxCollider2D consumableCollider = consumableCreated.GetComponent<BoxCollider2D>();
        RectTransform consumableImageTransform = consumableTransform.GetChild(0).GetComponent<RectTransform>();
        consumableImageTransform.sizeDelta = new Vector2((canvasWidth / 5), (canvasWidth / 5));
        consumableCollider.size = new Vector2((canvasWidth / 5), (canvasWidth / 5));
        consumableCreated.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        consumablesList.Add(consumableCreated);
    }

    public void destroyConsumable(GameObject consumable)
    {
        Destroy(consumable);
        consumablesList.Remove(consumable);
    }

    

    public List<GameObject> getOverlappingCooks(GameObject me)
    {
        List<GameObject> overlappingElements = getOverlappingElements(me);
        List<GameObject> overlappingCooks = new List<GameObject>();
        foreach (GameObject obj in overlappingElements)
        {
            if (obj.name.Contains("Cook"))
            {
                overlappingCooks.Add(obj);
            }
        }
        return overlappingCooks;
    }

    /**
     * TOOLS
     *
     * **/
    public List<GameObject> getOverlappingElements(GameObject me)
    {
        BoxCollider2D boxCollider = me.GetComponent<BoxCollider2D>();
        Vector3 smallVector = new Vector3(0.5f, 0.5f, 0.5f);
        Collider2D[] overlap = Physics2D.OverlapAreaAll((boxCollider.bounds.min + smallVector), (boxCollider.bounds.max - smallVector));
        List<GameObject> overlappingElements = new List<GameObject>();
        if (overlap.Length > 1)
        {
            foreach (Collider2D c in overlap)
            {
                if (getCooks().Contains(c.gameObject))
                {
                    overlappingElements.Add(c.gameObject);
                }
            }
        }

        return overlappingElements;
    }

    public T[] Shuffle<T>(T[] array)
    {
        var random = rnd;
        for (int i = array.Length; i > 1; i--)
        {
            // Pick random element to swap.
            int j = random.Next(i); // 0 <= j <= i-1
                                    // Swap.
            T tmp = array[j];
            array[j] = array[i - 1];
            array[i - 1] = tmp;
        }
        return array;
    }

}
