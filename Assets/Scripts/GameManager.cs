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
    public GameObject fruit;
    private bool gameInProgress = true;
    private int enemyOverFence = 0;
    public GameObject defeatPanel;
    public float ticksPerSecond;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Start");
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
        }


        checkLoss();
    }

    private void tick()
    {
        currentTime++;
        Debug.Log(currentTime.ToString());
        //timeText.text = currentTime.ToString();
        if (itsSpawnTime())
        {
            for (int i = 0; i < getEnemyNbToSpawn(); i++)
            {
                spawnFruit();
            }
        }

        List<GameObject> enemiesToMove = new List<GameObject>();
        foreach (GameObject element in getElements())
        {
            if (element.name.Contains("Enemy"))
            {
                enemiesToMove.Add(element);
            }
        }

        foreach (GameObject enemy in enemiesToMove)
        {
            moveEnemy(enemy);
        }

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
        Debug.Log("currentTime : " + currentTime);
        //if (currentTime < 10 && (currentTime % 5 == 0 || currentTime == 2))
        //    return true;

        //if (currentTime > 10 && currentTime < 20 && currentTime % 3 == 0)
        //    return true;

        //if (currentTime > 20 && currentTime < 30 && currentTime % 1 == 0)
        //    return true;

        //if (currentTime > 30 && currentTime % 1 == 0)
        //    return true;
        
        if(currentTime % 5 == 0)
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

    public void moveEnemy(GameObject enemy)
    {
        enemy.transform.Translate(new Vector2(0, -1));
        List<GameObject> overlappingSoldiers = getOverlappingSoldiers(enemy);
        if (overlappingSoldiers.Count > 0)
        {
            destroyUnit(enemy);
            playerScore++;
            //scoreText.text = playerScore.ToString();
            destroyUnit(overlappingSoldiers[0]);
        }
        if (enemy.transform.position.y < -7)
        {
            enemyOverFence++;
        }
    }

    public List<GameObject> getElements()
    {
        return boardElements;
    }

    public void spawnFruit()
    {
        int spawnX = Random.Range(-5, 5);
        GameObject fruitCreated = Instantiate(fruit, new Vector2(spawnX, 10), gameObject.transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform) as GameObject;
        //fruitCreated.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        boardElements.Add(fruitCreated);
    }

    public void destroyUnit(GameObject unit)
    {
        Destroy(unit);
        boardElements.Remove(unit);
    }

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
                Debug.Log("Overlapping object : " + c.gameObject.name);

                if (getElements().Contains(c.gameObject))
                {
                    overlappingElements.Add(c.gameObject);
                }
            }
        }

        return overlappingElements;
    }

    public List<GameObject> getOverlappingSoldiers(GameObject me)
    {
        List<GameObject> overlappingElements = getOverlappingElements(me);
        List<GameObject> overlappingSoldiers = new List<GameObject>();
        foreach (GameObject obj in overlappingElements)
        {
            if (obj.name.Contains("Soldier"))
            {
                overlappingSoldiers.Add(obj);
            }
        }
        return overlappingSoldiers;
    }

}
