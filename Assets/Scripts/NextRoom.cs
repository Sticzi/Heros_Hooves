﻿using UnityEngine;
using Cinemachine;
using System.Threading.Tasks;
using DG.Tweening;

public class NextRoom : MonoBehaviour
{
    private GameMaster gameMaster;
    private Transform mainCamera;
    private CinemachineConfiner horseCameraConfinerComponent;
    private CinemachineConfiner knightCameraConfinerComponent;    
    private Transform background;
    private GameObject horse;
    public GameObject knight;

    public Transform[] playerSpawn;   

    public float tossForce;
    public float transitionDuration = 2;
    public float damping = 1;
    public float UnfreezeDelay;
    public float push;

    public bool respawnPoint;
    public bool isFrozen;

    public LayerMask roomLayerMask;
    public float roomCheckRadius = 1f;

    private Vector2 horseVelocityContainer;
    private Vector2 knightVelocityContainer;  

    public void Awake()
    {


        gameMaster = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();

        horseCameraConfinerComponent = GameObject.FindGameObjectWithTag("VirtualCameraHorse").GetComponent<CinemachineConfiner>();        
        knightCameraConfinerComponent = GameObject.FindGameObjectWithTag("VirtualCameraKnight").GetComponent<CinemachineConfiner>();
        background = GameObject.FindGameObjectWithTag("Background").GetComponent<Transform>(); //paralax              
    }


    public void Start()
    {
        horse = GameObject.FindGameObjectWithTag("Horse");
        knight = GameObject.FindGameObjectWithTag("Knight");

        CheckPlayerRoom(horse.transform.position, horseCameraConfinerComponent);
        if (knight != null)
        {
            CheckPlayerRoom(knight.transform.position, knightCameraConfinerComponent);
        }
        else
        {
            knightCameraConfinerComponent.m_BoundingShape2D = horseCameraConfinerComponent.m_BoundingShape2D;

        }


        //i'm not sure what's that for
        //background.GetChild(0).GetComponent<Paralax>().startPos = gameMaster.savedBackgroundPos1;
        //background.GetChild(1).GetComponent<Paralax>().startPos = gameMaster.savedBackgroundPos2;
        //background.GetChild(2).GetComponent<Paralax>().startPos = gameMaster.savedBackgroundPos3;
        //background.GetChild(3).GetComponent<Paralax>().startPos = gameMaster.savedBackgroundPos4;
        //background.GetChild(4).GetComponent<Paralax>().startPos = gameMaster.savedBackgroundPos5;
    }


    public void OnTriggerExit2D(Collider2D player)
    {
        if (player.CompareTag("HorseCameraCollider"))
        {            
            Vector2 playerPosition = player.transform.position;
            CheckPlayerRoom(playerPosition, horseCameraConfinerComponent);                     
        }

        if (player.CompareTag("KnightCameraCollider"))
        {            
            //here may be something wrong with the transform parent
            Vector2 playerPosition = player.transform.parent.position;
            CheckPlayerRoom(playerPosition, knightCameraConfinerComponent);            
        }
    }

    private void OnTriggerEnter2D(Collider2D playerCollider2D)
    {
        if (respawnPoint && playerCollider2D.CompareTag("KnightCameraCollider") || playerCollider2D.CompareTag("HorseCameraCollider"))
        {            
            SaveBackgroundParalaxPos(background);
            SavePlayerSpawnPoint(playerCollider2D);            
        }
    }


    public void SavePlayerSpawnPoint(Collider2D playerCollider2D)
    {        
        if(playerCollider2D.CompareTag("KnightCameraCollider"))
        {
            gameMaster.savedKnightPosition = playerSpawn[0].position;
        }

        if(playerCollider2D.CompareTag("HorseCameraCollider"))
        {
            gameMaster.savedHorsePosition = playerSpawn[0].position;
            if(playerCollider2D.transform.parent.GetComponent<HorseController2D>().KnightPickedUp)
            {
                gameMaster.savedKnightPosition = playerSpawn[0].position;
            }
        }
        
    }

    //saves the position of the background so that after the respawn, background doesn't fly all over the screen
    public void SaveBackgroundParalaxPos(Transform background)
    {     
        int childCount = background.childCount;
        gameMaster.savedBackgroundPos = new Vector2[childCount];

        for (int i = 0; i < childCount; i++)
        {
            gameMaster.savedBackgroundPos[i] = background.GetChild(i).position;
        }
    }

    private void CheckPlayerRoom(Vector2 playerPosition, CinemachineConfiner currentConfinerComponent)
    {
        Collider2D checkedRoomCameraBound = Physics2D.OverlapCircle(playerPosition, roomCheckRadius, roomLayerMask);       

        if (checkedRoomCameraBound != null && currentConfinerComponent.m_BoundingShape2D != checkedRoomCameraBound)
        {
            // Player is in another room
            CameraTransition(checkedRoomCameraBound, currentConfinerComponent);
        }        
    }  

    public async void CameraTransition(Collider2D roomCameraBound, CinemachineConfiner confiner)
    {
        //freeze the player in place for the duration of camera transition. Might need some updating Knight-wise, not sure have to check
        horse.GetComponent<HorseController2D>().PlayerFreeze();

        //tu było przedtem save paralaxy ale chyba niepotrzebnie



        //jeżeli wchodzi od dołu
        //if (tossForce > 0)
        //{
        //    horse.GetComponent<BetterJump>().isTossed = true;
        //}

        confiner.m_Damping = damping;
        confiner.m_BoundingShape2D = roomCameraBound;

        //if the knight is picked up then set his bounding shape to the same as the horse (because he also goes to the next screen)
        if (horse.GetComponent<HorseController2D>().KnightPickedUp == true)
        {
            knightCameraConfinerComponent.m_BoundingShape2D = roomCameraBound;
        }

        await DOVirtual.Float(damping, 0.2f, transitionDuration, v =>
        {
            confiner.m_Damping = v;

        }).AsyncWaitForCompletion();

        horse.GetComponent<HorseController2D>().KnightAndHorseFreeze();

        
        confiner.m_Damping = 0;
        await Task.Yield();
    }
}
