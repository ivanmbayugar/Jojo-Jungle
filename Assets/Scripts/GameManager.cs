using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool isPlayerDead;
    float restartHoldTimer;
    public float restartHoldTime = 3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    void CheckInput()
    {
        if(Input.GetKey(KeyCode.R)) restartHoldTimer += Time.deltaTime;
        else restartHoldTimer =0f;
        if (restartHoldTimer >= restartHoldTime) RestartLevel();
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CheckPlayerDeath()
    {
        isPlayerDead = true;
    }
}
