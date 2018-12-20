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
    private int wormCount = 0;
    private int clearProgress = 0;
    private Animator anim;

    public List<GameObject> consumablesPrefabs;
    public GameObject wormPrefab;
    public GameObject normalCookPrefab1;
    public GameObject normalCookPrefab2;
    public GameObject normalCookPrefab3;
    public GameObject vegetableCookPrefab;
    public GameObject fruitCookPrefab;
    public float ticksPerSecond;
    public Slider rottenSlider;
    public GameObject canvas;
    public GameObject gameOverOverlay;
    public GameObject unlockOverlay;
    public GameObject wormOverlay;
    public Slider clearSlider;
    public GameObject clearPanel;
    public GameObject clearButton;
    public Text scoreTextEnd;
    public Text scoreTextCorner;

    // conf
    public float rottenConsumablesForLose = 8f;
    public float cookedConsumablesForClear = 8f;
    public GameObject hud;

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

        SpawnNewCook(normalCookPrefab1);

        anim = hud.GetComponent<Animator>();
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

        if (rottenSlider.value > (rottenConsumables / rottenConsumablesForLose))
        {
            rottenSlider.value = rottenSlider.value - 0.01f;
        }

        if (clearSlider.value < (clearProgress / cookedConsumablesForClear))
        {
            clearSlider.value = clearSlider.value + 0.01f;
        }

        if ((clearProgress == 0) && (clearSlider.value > (clearProgress / cookedConsumablesForClear)))
        {
            clearSlider.value = clearSlider.value - 0.04f;
        }

    }

    

    private void tick()
    {
        currentTime++;
        bool[] busyPositions = {false, false, false, false, false};
        int[] possiblePositions = { 0, 1, 2, 3, 4 };
        possiblePositions = Shuffle<int>(possiblePositions);
        if (itsSpawnTime())
        {
            

            for (int i = 0; i < getConsumableNbToSpawn(); i++)
            {
                int position = possiblePositions[i];
                busyPositions[position] = true;
                spawnConsumable(position);
            }

        }

        if (ItsWormTime())
        {
            int position = 0;
            int j = 0;
            do
            {
                position = possiblePositions[j];
                j++;
            } while (busyPositions[position] == true);
            SpawnWorm(position);
            
        }

    }

    public bool itsSpawnTime()
    {
        if (currentTime == 1)
        {
            return true;
        }

        if (currentTime < 15 && currentTime % 4 == 0)
            return true;

        if (currentTime > 15 && currentTime % 2 ==0)
            return true;

        return false;
    }

    public bool ItsWormTime()
    {

        if (currentTime == 20 || (currentTime > 40 && currentTime < 95 && currentTime % 10 == 0) || (currentTime > 95 && currentTime % 6 == 0))
            return true;

        return false;
    }

    public int getConsumableNbToSpawn()
    {
        if (currentTime < 30)
            return 1;

        if (currentTime < 95)
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
                clearProgress++;
                scoreTextCorner.text = playerScore.ToString();
                //scoreText.text = playerScore.ToString();
                cookManager.StartCooking(consumable);
                destroyConsumable(consumable);
                CheckNewCook();
                if (clearProgress > cookedConsumablesForClear)
                {
                    clearPanel.SetActive(false);
                    clearButton.SetActive(true);
                }
            }

        }
        
        if (consumable.transform.position.y < 0)
        {
            ConsumableManager consumableManager = (ConsumableManager)consumable.GetComponent(typeof(ConsumableManager));
            if (consumableManager.type == ConsumableManager.ConsumableTypeEnum.Worm)
            {
                rottenConsumables = 0;
            }
            else
            {
                anim.SetTrigger("TriggerTrash");
                rottenConsumables++;
                if (rottenConsumables > rottenConsumablesForLose)
                {
                    gameInProgress = false;
                    gameOverOverlay.SetActive(true);
                    scoreTextEnd.text = playerScore.ToString();
                }
                
            }
            destroyConsumable(consumable);
        }
    }

    IEnumerator ShowMessage(float delay)
    {
        yield return new WaitForSeconds(delay);
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

    public void SpawnWorm(int spawnRandom)
    {
        float canvasWidth = canvasRectTransform.rect.width;
        float canvasHeight = canvasRectTransform.rect.height;
        float canvasLeft = -1 * (canvasWidth / 2);
        float spawnX = (canvasLeft + (canvasWidth / 5) * spawnRandom) + (canvasWidth / 5) / 2;

        GameObject wormCreated = Instantiate(wormPrefab, new Vector3(spawnX, spawnPosition, 0), Quaternion.identity) as GameObject;
        RectTransform wormTransform = wormCreated.GetComponent<RectTransform>();
        BoxCollider2D wormCollider = wormCreated.GetComponent<BoxCollider2D>();
        RectTransform wormImageTransform = wormTransform.GetChild(0).GetComponent<RectTransform>();
        wormImageTransform.sizeDelta = new Vector2((canvasWidth / 5), (canvasWidth / 5));
        wormCollider.size = new Vector2((canvasWidth / 5), (canvasWidth / 5));
        wormCreated.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        consumablesList.Add(wormCreated);
        wormCount++;

        if (wormCount == 1)
        {
            wormOverlay.SetActive(true);
            gameInProgress = false;
        }
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
            SpawnNewCook(normalCookPrefab2);
            Image unlockedImage = unlockOverlay.transform.Find("UnlockedImage").GetComponent<Image>();
            Text unlockedSpecialization = unlockOverlay.transform.Find("UnlockedSpecialization").GetComponent<Text>();
            unlockedImage.sprite = normalCookPrefab2.transform.Find("CookImage").GetComponent<Image>().sprite;
            unlockedSpecialization.text = "";
            unlockOverlay.SetActive(true);
            gameInProgress = false;
            currentLevel++;
        }

        if (currentLevel == 1 && playerScore > 9)
        {
            SpawnNewCook(normalCookPrefab3);
            Image unlockedImage = unlockOverlay.transform.Find("UnlockedImage").GetComponent<Image>();
            Text unlockedSpecialization = unlockOverlay.transform.Find("UnlockedSpecialization").GetComponent<Text>();
            unlockedImage.sprite = normalCookPrefab3.transform.Find("CookImage").GetComponent<Image>().sprite;
            unlockedSpecialization.text = "";
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

        GameObject cookCreated = Instantiate(cookPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        RectTransform cookTransform = cookCreated.GetComponent<RectTransform>();
        Vector2 size = cookTransform.sizeDelta;
        float ratio = size.y / size.x;
        cookTransform.sizeDelta = new Vector2((canvasWidth / 5), (canvasWidth / 5) * ratio);
        cookCreated.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        CookManager cookManager = (CookManager)cookCreated.GetComponent(typeof(CookManager));

        cooksList.Add(cookCreated);
    }

    public void Restart()
    {
        currentLevel = 0;
        rottenConsumables = 0;
        wormCount = 0;
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
        SpawnNewCook(normalCookPrefab1);

    }

    public void ContinueGame()
    {
        gameInProgress = true;
    }

    public void Clear()
    {
        foreach (GameObject consumable in consumablesList)
        {
            Destroy(consumable);
            //consumablesList.Remove(consumable);
        }
        consumablesList.RemoveAll(delegate (GameObject o) { return o == null; });

        clearPanel.SetActive(true);
        clearButton.SetActive(false);
        clearProgress = 0;
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
