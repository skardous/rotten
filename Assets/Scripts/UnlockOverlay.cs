using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockOverlay : MonoBehaviour
{

    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Continue()
    {
        gameObject.SetActive(false);
        gameManager.ContinueGame();
    }
}
