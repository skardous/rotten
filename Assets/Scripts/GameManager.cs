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
    private List<GameObject> boardElements = new List<GameObject>();
    private List<GameObject> fruitsList = new List<GameObject>();
    private List<GameObject> cooksList = new List<GameObject>();

    public GameObject[] fruitsPrefabs;
    public GameObject fruit;
    public GameObject cook;
    private bool gameInProgress = true;
    private int rottenFruits = 0;
    public GameObject defeatPanel;
    public float ticksPerSecond;
    public GameObject sproutchText;

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

            foreach (GameObject fruit in fruitsList)
            {
                moveFruit(fruit);
            }
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
            for (int i = 0; i < getFruitNbToSpawn(); i++)
            {
                spawnFruit();
            }
        }

        //List<GameObject> enemiesToMove = new List<GameObject>();
        //foreach (GameObject fruit in fruitsList)
        //{
        //    fruitsTo.Add(element);
        //}

        //foreach (GameObject enemy in enemiesToMove)
        //{
        //    moveEnemy(enemy);
        //}

    }



    public void checkLoss()
    {
        //if (rottenFruits > 0)
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

    public int getFruitNbToSpawn()
    {
        if (currentTime < 30)
            return 1;

        if (currentTime < 40)
            return 2;

        return 3;
    }

    public void moveFruit(GameObject fruit)
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        float canvasHeight = canvasRectTransform.rect.height;
        float moveY = -1 * (canvasHeight / 300);
        fruit.transform.Translate(new Vector3(0, moveY, 0));
        List<GameObject> overlappingCooks = getOverlappingCooks(fruit);
        if (overlappingCooks.Count > 0)
        {
            GameObject cook = overlappingCooks[0];
            CookManager cookManager = (CookManager)cook.GetComponent(typeof(CookManager));
            if (cookManager.IsCooking() == true)
            {

            } else
            {
                destroyFruit(fruit);
                playerScore++;
                //scoreText.text = playerScore.ToString();
                cookManager.StartCooking(fruit);
            }
           
        }
        
        if (fruit.transform.position.y < 0)
        {
            rottenFruits++;
            destroyFruit(fruit);
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

    public List<GameObject> getFruits()
    {
        return fruitsList;
    }

    public void spawnFruit()
    {
        int spawnRandom = Random.Range(0, 5);
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRectTransform.rect.width;
        float canvasHeight = canvasRectTransform.rect.height;

        float canvasLeft = -1 * (canvasWidth / 2);

        float spawnX = (canvasLeft + (canvasWidth / 5) * spawnRandom) + (canvasWidth / 5)/2;
        
        GameObject fruitCreated = Instantiate(fruit, new Vector3(spawnX, 1 * (canvasHeight / 2), 0), Quaternion.identity) as GameObject;
        RectTransform fruitTransform = fruitCreated.GetComponent<RectTransform>();
        BoxCollider2D fruitCollider = fruitCreated.GetComponent<BoxCollider2D>();
        RectTransform fruitImageTransform = fruitTransform.GetChild(0).GetComponent<RectTransform>();
        fruitImageTransform.sizeDelta = new Vector2((canvasWidth / 5), (canvasWidth / 5));
        fruitCollider.size = new Vector2((canvasWidth / 5), (canvasWidth / 5));
        fruitCreated.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        fruitsList.Add(fruitCreated);
    }

    public void destroyFruit(GameObject fruit)
    {
        Destroy(fruit);
        fruitsList.Remove(fruit);
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
                //Debug.Log("Overlapping object : " + c.gameObject.name);

                if (getCooks().Contains(c.gameObject))
                {
                    overlappingElements.Add(c.gameObject);
                }
            }
        }

        return overlappingElements;
    }

}
