using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class References : MonoBehaviour
{
    private static References _instance;

    public static References Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public Camera mainCam;
    public Transform playerMovingTransform;
    public Rigidbody2D playerRigidBody;
    public UIControl uiControl;
    public InputControl playerController;
    //public Transform fishingLurePosition;
    public Transform fishingLure;
    public FishingHandler fishingHandler;
    public FishSpawnControl fishSpawnControl;
    public FishMoveControl fishMoveControl;
    public UnderWaterGenerator underwaterGenerator;
    public ExperienceControl experienceControl;
    public WaterTouchControl waterTouchControl;
    public BuilderControl builderControl;
    public ToolsControl toolsControl;
    public AquariumController aquariumController;

    public MapSpawner mapSpawner;
    public MapPlantSpawner mapPlantSpawner;

    public Vector3 mouseWorldPos;
    public Vector3 mouseScreenPosition;
    private void Update()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = -mainCam.transform.position.z; //The distance between the camera and object
        //objectPosition = Camera.main.WorldToScreenPoint(originPosition.position);
        mouseScreenPosition = mousePosition;

        mouseWorldPos = mainCam.ScreenToWorldPoint(mousePosition);
    }
}
