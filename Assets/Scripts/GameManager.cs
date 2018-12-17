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
    public GameObject fruit;
    public GameObject cook;
    private bool gameInProgress = true;
    private int enemyOverFence = 0;
    public GameObject defeatPanel;
    public float ticksPerSecond;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Start");

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
            RectTransform fruitTransform = cookCreated.GetComponent<RectTransform>();
            fruitTransform.sizeDelta = new Vector2((canvasWidth / 5), (canvasWidth / 5));
            cookCreated.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
            cooksList.Add(cookCreated);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameInProgress)
        {
            Debug.Log("progress");
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
            for (int i = 0; i < getEnemyNbToSpawn(); i++)
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
        if (enemyOverFence > 0)
        {
            gameInProgress = false;
            defeatPanel.SetActive(true);
        }
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

    public int getEnemyNbToSpawn()
    {
        if (currentTime < 30)
            return 1;

        if (currentTime < 40)
            return 2;

        return 3;
    }

    public void moveFruit(GameObject fruit)
    {
        fruit.transform.Translate(new Vector3(0, -.01f, 0));
        List<GameObject> overlappingCooks = getOverlappingCooks(fruit);
        if (overlappingCooks.Count > 0)
        {
            destroyFruit(fruit);
            playerScore++;
            //scoreText.text = playerScore.ToString();
            //destroyFruit(overlappingSoldiers[0]);
        }
        //if (fruit.transform.position.y < -7)
        //{
        //    enemyOverFence++;
        //}
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
        Debug.Log(spawnRandom);
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRectTransform.rect.width;
        float canvasHeight = canvasRectTransform.rect.height;

        float canvasLeft = -1 * (canvasWidth / 2);

        float spawnX = (canvasLeft + (canvasWidth / 5) * spawnRandom) + (canvasWidth / 5)/2;
        
        GameObject fruitCreated = Instantiate(fruit, new Vector3(spawnX, 1 * (canvasHeight / 2), 0), Quaternion.identity) as GameObject;
        RectTransform fruitTransform = fruitCreated.GetComponent<RectTransform>();
        fruitTransform.sizeDelta = new Vector2((canvasWidth / 5), (canvasWidth / 5));
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
                Debug.Log("Overlapping object : " + obj.name);
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
