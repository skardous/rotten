using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    private bool gameInProgress = true;
    private int rottenConsumables = 0;
    float lastTick = 0;
    float currentTime = 0;
    float playerScore = 0;
    private List<GameObject> consumablesList = new List<GameObject>();
    private List<GameObject> vegetablesPrefabsList = new List<GameObject>();
    private List<GameObject> fruitsPrefabsList = new List<GameObject>();
    private List<GameObject> cooksList = new List<GameObject>();
    private RectTransform canvasRectTransform;
    private float spawnPosition = 100f;
    private int currentLevel = 0;

    public List<GameObject> consumablesPrefabs;
    public GameObject normalCookPrefab;
    public GameObject vegetableCookPrefab;
    public GameObject fruitCookPrefab;
    public float ticksPerSecond;
    public GameObject sproutchText;
    public Slider rottenSlider;
    public GameObject canvas;
    public GameObject gameOverOverlay;
    public GameObject unlockOverlay;
    public Text scoreTextEnd;
    public Text scoreTextCorner;
    public float rottenConsumablesForLose = 8f;

    public static System.Random rnd = new System.Random();

    // Use this for initialization
    void Start()
    {
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        spawnPosition = canvasRectTransform.rect.height;

        foreach (GameObject consumablePrefab in consumablesPrefabs)
        {
            ConsumableManager consManager = consumablePrefab.GetComponent<ConsumableManager>();
            if (consManager.type == ConsumableManager.ConsumableTypeEnum.Vegetable)
            {
                vegetablesPrefabsList.Add(consumablePrefab);
            }
            else if (consManager.type == ConsumableManager.ConsumableTypeEnum.Fruit)
            {
                fruitsPrefabsList.Add(consumablePrefab);
            }
        }

        SpawnNewCook(normalCookPrefab);
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

        if (rottenSlider.value < (rottenConsumables/ rottenConsumablesForLose))
        {
            rottenSlider.value = rottenSlider.value + 0.01f;
        }

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

    public bool itsSpawnTime()
    {
        if (currentTime == 1)
        {
            return true;
        }
        //Debug.Log("currentTime : " + currentTime);
        if (currentTime < 15 && currentTime % 4 == 0)
            return true;

        if (currentTime > 15 && currentTime % 2 ==0)
            return true;

        //if (currentTime > 20 && currentTime < 30 && currentTime % 1 == 0)
        //    return true;

        //if (currentTime > 30 && currentTime % 1 == 0)
        //    return true;

        //if (currentTime % 2 == 0)
        //{
        //    return true;
        //}

        return false;
    }

    public int getConsumableNbToSpawn()
    {
        if (currentTime < 30)
            return 1;

        if (currentTime < 120)
            return 2;

        return 3;
    }

    public void moveConsumable(GameObject consumable)
    {
        if (consumable == null)
        {
            return;
        }
        float canvasHeight = canvasRectTransform.rect.height;
        float moveY = -1 * (canvasHeight / 300);
        consumable.transform.Translate(new Vector3(0, moveY, 0));
        List<GameObject> overlappingCooks = getOverlappingCooks(consumable);
        if (overlappingCooks.Count > 0)
        {
            //Debug.Log("overlapping!");
            //Debug.Log(consumable.transform.position.y);
            GameObject cook = overlappingCooks[0];
            CookManager cookManager = (CookManager)cook.GetComponent(typeof(CookManager));
            if (cookManager.IsCooking() == false)
            { 
                playerScore++;
                scoreTextCorner.text = playerScore.ToString();
                //scoreText.text = playerScore.ToString();
                cookManager.StartCooking(consumable);
                destroyConsumable(consumable);
                CheckNewCook();
            }

        }
        
        if (consumable.transform.position.y < 0)
        {
            rottenConsumables++;
            destroyConsumable(consumable);
            StartCoroutine(ShowMessage(1));
            if (rottenConsumables > rottenConsumablesForLose)
            {
                gameInProgress = false;
                gameOverOverlay.SetActive(true);
                scoreTextEnd.text = playerScore.ToString();
            }
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

        float canvasWidth = canvasRectTransform.rect.width;
        float canvasHeight = canvasRectTransform.rect.height;

        float canvasLeft = -1 * (canvasWidth / 2);

        float spawnX = (canvasLeft + (canvasWidth / 5) * spawnRandom) + (canvasWidth / 5)/2;

        // veg or fruit ?
        GameObject consumable = vegetablesPrefabsList[0];
        if (spawnType == 0)
        {
            consumable = vegetablesPrefabsList[Random.Range(0, vegetablesPrefabsList.Count)];
        } else
        {
            consumable = fruitsPrefabsList[Random.Range(0, fruitsPrefabsList.Count)];
        }

        GameObject consumableCreated = Instantiate(consumable, new Vector3(spawnX, spawnPosition, 0), Quaternion.identity) as GameObject;
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
        //consumablesList.Remove(consumable);
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

    public void CheckNewCook()
    {
        if (currentLevel == 0 && playerScore > 2)
        {
            SpawnNewCook(vegetableCookPrefab);
            Image unlockedImage = unlockOverlay.transform.Find("UnlockedImage").GetComponent<Image>();
            Text unlockedSpecialization = unlockOverlay.transform.Find("UnlockedSpecialization").GetComponent<Text>();
            unlockedImage.sprite = vegetableCookPrefab.transform.Find("CookImage").GetComponent<Image>().sprite;
            unlockedSpecialization.text = "Vegetable expert !";
            unlockOverlay.SetActive(true);
            gameInProgress = false;
            currentLevel++;
        }

        if (currentLevel == 1 && playerScore > 9)
        {
            SpawnNewCook(fruitCookPrefab);
            Image unlockedImage = unlockOverlay.transform.Find("UnlockedImage").GetComponent<Image>();
            Text unlockedSpecialization = unlockOverlay.transform.Find("UnlockedSpecialization").GetComponent<Text>();
            unlockedImage.sprite = fruitCookPrefab.transform.Find("CookImage").GetComponent<Image>().sprite;
            unlockedSpecialization.text = "Fruit expert !";
            unlockOverlay.SetActive(true);
            gameInProgress = false;
            currentLevel++;
        }
    }

    public void SpawnNewCook(GameObject cookPrefab)
    {
        float canvasWidth = canvasRectTransform.rect.width;
        float canvasHeight = canvasRectTransform.rect.height;
        float canvasLeft = -1 * (canvasWidth / 2);
        float spawnX = (canvasLeft + (canvasWidth / 5) * (cooksList.Count + 1)) + (canvasWidth / 5) / 2;

        GameObject cookCreated = Instantiate(cookPrefab, new Vector3(spawnX, 0, 0), Quaternion.identity) as GameObject;
        RectTransform cookTransform = cookCreated.GetComponent<RectTransform>();
        Vector2 size = cookTransform.sizeDelta;
        float ratio = size.y / size.x;
        cookTransform.sizeDelta = new Vector2((canvasWidth / 5), (canvasWidth / 5) * ratio);
        cookCreated.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        CookManager cookManager = (CookManager)cookCreated.GetComponent(typeof(CookManager));

        cooksList.Add(cookCreated);
    }

    private void SpawnCooks()
    {
        //// spawn the cooks
        //int[] spawnCooks = new int[] { 1, 2, 3 };
        //for (int i = 0; i < 3; i++)
        //{
            
        //}
    }

    public void Restart()
    {
        currentLevel = 0;
        rottenConsumables = 0;
        foreach (GameObject consumable in consumablesList)
        {
            Destroy(consumable);
            //consumablesList.Remove(consumable);
        }
        foreach (GameObject cook in cooksList)
        {
            Destroy(cook);
        }
        cooksList.RemoveAll(delegate (GameObject o) { return o == null; });
        cooksList.Clear();

        rottenSlider.value = 0;
        gameInProgress = true;
        gameOverOverlay.SetActive(false);
        currentTime = 0;
        playerScore = 0;
        scoreTextCorner.text = playerScore.ToString();
        Debug.Log(cooksList.Count);
        SpawnNewCook(normalCookPrefab);

    }

    public void ContinueGame()
    {
        gameInProgress = true;
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
