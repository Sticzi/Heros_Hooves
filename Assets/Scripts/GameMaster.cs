using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    private static GameMaster instance;

    public Vector2 savedKnightPosition;
    public Vector2 savedHorsePosition;    

    //public Vector2 savedCameraPosition;
    public Vector2[] savedBackgroundPos;

    public Transform playerPositionTesting;
    public Transform firstSpawnPoint;

    void Awake()
    {       
        if(playerPositionTesting != null)
        {
            savedKnightPosition = playerPositionTesting.position;
            savedHorsePosition = playerPositionTesting.position;
        }

        else
        {
            savedKnightPosition = firstSpawnPoint.position;
            savedHorsePosition = firstSpawnPoint.position;
        }

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }    
}
