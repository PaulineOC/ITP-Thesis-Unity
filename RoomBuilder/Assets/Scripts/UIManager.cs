using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HSVPicker;

public class UIManager : MonoBehaviour
{

    private Manager GameManager;
    private EDIT_UI_MODE Mode;
    private MAIN_MENU_SETTING? MainMenuMode = null;

    private Color32 SelectedButtonColor = new Color32(89, 0, 17, 255);
    private Color32 DefaultSelectedButtonColor = new Color32(245, 245, 245, 255);

    private Color32 DisabledInventoryImageColor = new Color32(128,128,128,125);


    [SerializeField]
    private GameObject FullScreenPanel;

    private void Awake()
    {

        GameManager = this.GetComponent<Manager>();

        WelcomeSetup();

        ViewRoomSetup();

        EditRoomSetup();

        // To move into Edit Room Setup

        SaveRoomSetup();

        SetupRoomSettingsMenu();

        RotateCameraSetup();

        ZoomCameraSetup();

        MyArtSetup();

        RoomStructuresSetup();
        BuildNewRoom();
    }


    // Start is called before the first frame update
    void Start()
    {
        // TODO: Call this after the list is received by the UI
        //CreateVerticalInventory();

    }

    // Update is called once per frame
    void Update()
    {

         
    }

    // Room Structures:
    [SerializeField]
    private GameObject Room;
    private GameObject BLColumn;
    [HideInInspector]
    public GameObject BackWall;
    private GameObject BRColumn;
    [HideInInspector]
    public GameObject RightWall;
    private GameObject FRColumn;
    [HideInInspector]
    public GameObject FrontWall;
    private GameObject FLColumn;
    [HideInInspector]
    public GameObject LeftWall;
    [HideInInspector]
    public GameObject Ground;
    [HideInInspector]
    public GameObject Ceiling;

    private void RoomStructuresSetup()
    {
        Debug.Log("UI: Room Structure Setup");
        BLColumn = FindGameObject(Room,"Column-BL");
        BackWall = FindGameObject(Room, "BackWall");
        BRColumn = FindGameObject(Room, "Column-BR");
        RightWall = FindGameObject(Room, "RightWall");
        FRColumn = FindGameObject(Room, "Column-FR");
        FrontWall = FindGameObject(Room, "FrontWall");
        FLColumn = FindGameObject(Room, "Column-FL");
        LeftWall = FindGameObject(Room, "LeftWall");
        Ground = FindGameObject(Room, "Ground");
        Ceiling = FindGameObject(Room,"Ceiling");
    }

    public void BuildNewRoom()
    {

        //Debug.Log("Building New Room");
        RoomMeasurements rmMeasurements = GameManager.RoomSizeToMeasurements[GameManager.CurrRoomSize];

        BLColumn.transform.localScale = new Vector3(rmMeasurements.poleXZ, rmMeasurements.wallHeight, rmMeasurements.poleXZ);
        BLColumn.transform.position = new Vector3(0f, 0f, 0f);

        BackWall.tag = "Walls";
        BackWall.transform.localScale = new Vector3(rmMeasurements.backFrontWallLength, rmMeasurements.wallHeight, 1f);
        BackWall.transform.position = new Vector3(
            BLColumn.transform.position.x + ((rmMeasurements.backFrontWallLength + rmMeasurements.poleXZ) / 2),
            0f,
            0f
        );

        BRColumn.transform.localScale = new Vector3(rmMeasurements.poleXZ, rmMeasurements.wallHeight, rmMeasurements.poleXZ);
        BRColumn.transform.position = new Vector3(
            BackWall.transform.position.x + ((rmMeasurements.backFrontWallLength + rmMeasurements.poleXZ) / 2),
            0f,
            0f
            );

        RightWall.tag = "Walls";
        RightWall.transform.localScale = new Vector3(1f, rmMeasurements.wallHeight, rmMeasurements.leftRightWallLength);
        RightWall.transform.position = new Vector3(
          rmMeasurements.poleXZ + rmMeasurements.backFrontWallLength,
          0f,
          -(rmMeasurements.leftRightWallLength + rmMeasurements.poleXZ) / 2
          );

        FRColumn.transform.localScale = new Vector3(rmMeasurements.poleXZ, rmMeasurements.wallHeight, rmMeasurements.poleXZ);
        FRColumn.transform.position = new Vector3(
            BackWall.transform.position.x + ((rmMeasurements.backFrontWallLength + rmMeasurements.poleXZ) / 2),
            0f,
            -(rmMeasurements.poleXZ + rmMeasurements.leftRightWallLength)
            );

        FrontWall.tag = "Walls";
        FrontWall.transform.localScale = new Vector3(rmMeasurements.backFrontWallLength, rmMeasurements.wallHeight, 1f);
        FrontWall.transform.position = new Vector3(
            BLColumn.transform.position.x + ((rmMeasurements.backFrontWallLength + rmMeasurements.poleXZ) / 2),
            0f,
            -(rmMeasurements.poleXZ + rmMeasurements.leftRightWallLength)
            );

        FLColumn.transform.localScale = new Vector3(rmMeasurements.poleXZ, rmMeasurements.wallHeight, rmMeasurements.poleXZ);
        FLColumn.transform.position = new Vector3(
            0f,
            0f,
            -(rmMeasurements.poleXZ + rmMeasurements.leftRightWallLength)
            );

        LeftWall.tag = "Walls";
        LeftWall.transform.localScale = new Vector3(1f, rmMeasurements.wallHeight, rmMeasurements.leftRightWallLength);
        LeftWall.transform.position = new Vector3(
          0f,
          0f,
          -(rmMeasurements.leftRightWallLength + rmMeasurements.poleXZ) / 2
          );

        Ground.tag = "Walls";
        Ground.transform.localScale = new Vector3(
            rmMeasurements.backFrontWallLength,
            rmMeasurements.structureDepth,
            rmMeasurements.leftRightWallLength);
        Ground.transform.position = new Vector3(
          BLColumn.transform.position.x + ((rmMeasurements.backFrontWallLength + rmMeasurements.poleXZ) / 2),
          (float)(-rmMeasurements.wallHeight / 2 + 0.5),
          -(rmMeasurements.leftRightWallLength + rmMeasurements.poleXZ) / 2
          );

        GameManager.RebuildAllGrids();
    }

    #region WelcomeScreen
    [SerializeField]
    private GameObject WelcomeCanvas;

    private void WelcomeSetup() {
        WelcomeCanvas.SetActive(true);
        GameObject BeginButtonGameObject = FindCanvasGameObject(WelcomeCanvas,"BeginButton");
        Button BeginButton = BeginButtonGameObject.GetComponent<Button>();
        BeginButton.onClick.AddListener(OnWelcomeButtonClick);
    }

    public void OnWelcomeButtonClick() {
        WelcomeCanvas.SetActive(false);
        ViewRoomCanvas.SetActive(true);
    }

    #endregion

    #region ViewRoom
    [SerializeField]
    private GameObject ViewRoomCanvas;

    private void ViewRoomSetup() {
        ViewRoomCanvas.SetActive(false);
        HamburgerSetupOnAwake();

        GameObject ViewRoomRotateCamera = FindCanvasGameObject(ViewRoomCanvas, "RotateCamera");
        Button RotateCameraButton = ViewRoomRotateCamera.GetComponent<Button>();
        RotateCameraButton.onClick.AddListener(OnRotateCameraClick);

        GameObject ViewRoomZoomCamera = FindCanvasGameObject(ViewRoomCanvas, "ZoomCamera");
        Button ZoomCameraButton = ViewRoomZoomCamera.GetComponent<Button>();
        ZoomCameraButton.onClick.AddListener(OnZoomClick);
    }

    #region HamburgerMenu

    private GameObject HamburgerButton;

    private GameObject HamburgerOriginalPosition;
    private GameObject HamburgerActivePosition;

    private GameObject HamburgerMenu;

    [SerializeField]
    private Sprite OpenHamburgerSprite;
    [SerializeField]
    private Sprite CloseHamburgerSprite;

    private bool isHamburgerOpen = false;

    private void HamburgerSetupOnAwake()
    {
        HamburgerButton = FindCanvasGameObject(ViewRoomCanvas, "Hamburger-Button");
        Button HamburgerOpenButton = HamburgerButton.GetComponent<Button>();
        HamburgerOpenButton.onClick.AddListener(OnHamburgerClick);

        HamburgerOriginalPosition = FindCanvasGameObject(ViewRoomCanvas, "Hamburger-OriginalPosition");
        HamburgerActivePosition = FindCanvasGameObject(ViewRoomCanvas, "Hamburger-ActivePosition");
  
        HamburgerMenu = FindCanvasGameObject(ViewRoomCanvas, "HamburgerMenu");
        HamburgerMenu.transform.position = HamburgerOriginalPosition.transform.position;

        GameObject EditRoomMode = FindCanvasGameObject(ViewRoomCanvas,"EditRoom");
        Button EditRoomButton = EditRoomMode.GetComponent<Button>();
        EditRoomButton.onClick.AddListener(OnEditRoomClick);

        GameObject ShareRoom = FindCanvasGameObject(ViewRoomCanvas, "ShareRoom");
        Button ShareRoomButton = ShareRoom.GetComponent<Button>();
    }

    private void CloseHamburgerMenu()
    {
        Button HamburgerOpenButton = HamburgerButton.GetComponent<Button>();
        Image ButtonImage = HamburgerOpenButton.GetComponent<Image>();

        StartCoroutine(MoveHamburger(HamburgerOriginalPosition.transform.position, 0.75f));
        ButtonImage.sprite = OpenHamburgerSprite;
        isHamburgerOpen = false;
        return;
    }

    private void OnHamburgerClick() {
        // Closed --> Open
        if (!isHamburgerOpen)
        {
            Button HamburgerOpenButton = HamburgerButton.GetComponent<Button>();
            Image ButtonImage = HamburgerOpenButton.GetComponent<Image>();
            StartCoroutine(MoveHamburger(HamburgerActivePosition.transform.position, 0.75f));
            ButtonImage.sprite = CloseHamburgerSprite;

            isHamburgerOpen = true;
            return;

        }
        // Open --> Closed
        else
        {
            CloseHamburgerMenu();
            return;
        }
    }

    IEnumerator MoveHamburger(Vector3 Dest, float inTime)
    {
        for (var t = 0f; t < 1; t += Time.deltaTime / inTime)
        {
            HamburgerMenu.transform.position = Vector3.Lerp(HamburgerMenu.transform.position, Dest, t);
            yield return null;
        }
        HamburgerMenu.transform.position = Dest;

    }

    private void OnEditRoomClick() {
        CloseHamburgerMenu();
        ViewRoomCanvas.SetActive(false);
        GameManager.ChangeMainScreen(MAIN_SCREEN.EDIT_SCREEN);
        EditRoomCanvas.SetActive(true);
    }

    #endregion

    #endregion

    #region EditRoom

    [SerializeField]
    private GameObject EditRoomCanvas;

    private void EditRoomSetup()
    {

        RoomSettingsSetup();
        EditRoomCanvas.SetActive(false);
    }


    public void ToggleMainUIButtons(bool turnOn)
    {
        Button DoneButton = Done.GetComponent<Button>();
        DoneButton.interactable = turnOn;

        Button RoomSettingsBttn = RoomSettingsButton.GetComponent<Button>();
        RoomSettingsBttn.interactable = turnOn;

        Button RotateCameraButton = RotateCamera.GetComponent<Button>();
        RotateCameraButton.interactable = turnOn;

        Button ZoomCameraButton = ZoomCamera.GetComponent<Button>();
        ZoomCameraButton.interactable = turnOn;

        Button MyArtButton = MyArtButtonObject.GetComponent<Button>();
        MyArtButton.interactable = turnOn;

    }

    #region Settings

    [SerializeField]
    private GameObject RoomSettingsButton;

    private GameObject RoomSettingsCanvas;
    private GameObject RoomSettingsBackground;
    private GameObject MainSettingsCanvas;

    private void RoomSettingsSetup() {

        Button OpenRoomSettingsButton = RoomSettingsButton.GetComponent<Button>();
        OpenRoomSettingsButton.onClick.AddListener(OnRoomSettingsClick);

        RoomSettingsCanvas = FindCanvasGameObject(EditRoomCanvas,"RoomSettings");
        RoomSettingsBackground = FindCanvasGameObject(EditRoomCanvas, "RoomSettingsBackground");

        MainSettingsCanvas = FindCanvasGameObject(RoomSettingsCanvas, "MainSettings");

        OnWallFloorColorCeilingSetup();

        Mode = EDIT_UI_MODE.NULL;

        //Default:
        MainSettingsCanvas.SetActive(true);
        RoomSettingsCanvas.SetActive(false);
    }

    private void OnRoomSettingsClick() {
        if (Mode == EDIT_UI_MODE.NULL)
        {
            RoomSettingsCanvas.SetActive(true);
            MainSettingsCanvas.SetActive(true);
            ToggleMainUIButtons(false);
            Button SettingsButton = RoomSettingsButton.GetComponent<Button>();
            SettingsButton.interactable = true;
            Mode = EDIT_UI_MODE.SETTINGS;
            GameManager.RoomState = ROOM_EDITING_STATE.IN_UI;
            return;
        }
        else {
            WallFloorCeilingColorCanvas.SetActive(false);
            RoomSettingsCanvas.SetActive(false);
            ToggleMainUIButtons(true);
            Mode = EDIT_UI_MODE.NULL;
            GameManager.RoomState = ROOM_EDITING_STATE.TRANSFORMATION;
            return;
        }
       
    }

    #region WallFloorColor

    private GameObject ChangeWallColor;
    private GameObject ChangeFloorColor;
    private GameObject ChangeCeilingColor;

    private GameObject WallFloorCeilingColorCanvas;

    private Color? TempColor;
    private Color WallColor;
    private Color FloorColor;
    private Color CeilingColor;


    private void OnWallFloorColorCeilingSetup() {
        ChangeWallColor = FindCanvasGameObject(MainSettingsCanvas, "ChangeWallColor");
        ChangeFloorColor = FindCanvasGameObject(MainSettingsCanvas, "ChangeFloorColor");
        ChangeCeilingColor = FindCanvasGameObject(MainSettingsCanvas, "ChangeCeilingColor");


        WallFloorCeilingColorCanvas = FindCanvasGameObject(RoomSettingsCanvas,"WallFloorColor");

        Button ChangeWallColorButton = ChangeWallColor.GetComponent<Button>();
        ChangeWallColorButton.onClick.AddListener(delegate {
            OnWallFloorCeilingColorClick(MAIN_MENU_SETTING.WALL_COLOR);
        });

        Button ChangeFloorColorButton = ChangeFloorColor.GetComponent<Button>();
        ChangeFloorColorButton.onClick.AddListener(delegate
        {
            OnWallFloorCeilingColorClick(MAIN_MENU_SETTING.FLOOR_COLOR);
        });

        Button ChangeCeilingColorButton = ChangeCeilingColor.GetComponent<Button>();
        ChangeCeilingColorButton.onClick.AddListener(delegate
        {
            OnWallFloorCeilingColorClick(MAIN_MENU_SETTING.CEILING_COLOR);
        });


        GameObject ColorPicker = FindCanvasGameObject(WallFloorCeilingColorCanvas, "ColorPicker");
        ColorPicker ColorPickerScript = ColorPicker.GetComponent<ColorPicker>();
        ColorPickerScript.onValueChanged.AddListener(OnWallColorPickerChange);

        GameObject CancelColor = FindCanvasGameObject(WallFloorCeilingColorCanvas, "Cancel");
        GameObject SaveColor = FindCanvasGameObject(WallFloorCeilingColorCanvas, "Save");

        Button CancelColorButton = CancelColor.GetComponent<Button>();
        CancelColorButton.onClick.AddListener(OnWallFloorCeilingColorCancelClick);

        Button SaveColorButton = SaveColor.GetComponent<Button>();
        SaveColorButton.onClick.AddListener(delegate {
            OnWallFloorColorConfirmClick();
        });

        //Default
        WallFloorCeilingColorCanvas.SetActive(false);

    }

    private void OnWallFloorCeilingColorClick(MAIN_MENU_SETTING SettingType) {
        MainMenuMode = SettingType;

        MainSettingsCanvas.SetActive(false);
        WallFloorCeilingColorCanvas.SetActive(true);

        // Reset color in the color picker:
        GameObject ColorPicker = FindCanvasGameObject(WallFloorCeilingColorCanvas, "ColorPicker");
        ColorPicker ColorPickerScript = ColorPicker.GetComponent<ColorPicker>();

        if (SettingType == MAIN_MENU_SETTING.WALL_COLOR) {
            Material spriteMaterial = BackWall.GetComponentsInChildren<MeshRenderer>()[0].material;
            TempColor = spriteMaterial.color;
            Debug.Log(spriteMaterial.color.ToString());
        }
        else if (SettingType == MAIN_MENU_SETTING.FLOOR_COLOR) {
            Material spriteMaterial = Ground.GetComponentsInChildren<MeshRenderer>()[0].material;
            TempColor = spriteMaterial.color;
        }
        else if (SettingType == MAIN_MENU_SETTING.CEILING_COLOR)
        {
            Material spriteMaterial = Ceiling.GetComponentsInChildren<MeshRenderer>()[0].material;
            TempColor = spriteMaterial.color;
        }
        ColorPickerScript.CurrentColor = (Color)TempColor;
    }

    public void OnWallColorPickerChange(Color NewColor)
    {
        TempColor = NewColor;
    }

    private void OnWallFloorColorConfirmClick(){

        if (TempColor != null ) {

            if (MainMenuMode == MAIN_MENU_SETTING.WALL_COLOR)
            {
                WallColor = (Color)TempColor;

                ChangeStructureColor(BLColumn, WallColor);
                ChangeStructureColor(BackWall, WallColor);
                ChangeStructureColor(BRColumn, WallColor);
                ChangeStructureColor(RightWall, WallColor);
                ChangeStructureColor(FRColumn, WallColor);
                ChangeStructureColor(FrontWall, WallColor);
                ChangeStructureColor(FLColumn, WallColor);
                ChangeStructureColor(LeftWall, WallColor);
                ChangeStructureColor(LeftWall, WallColor);

            }
            else if (MainMenuMode == MAIN_MENU_SETTING.FLOOR_COLOR)
            {
                FloorColor = (Color)TempColor;
                ChangeStructureColor(Ground, FloorColor);
            }
            else if (MainMenuMode == MAIN_MENU_SETTING.CEILING_COLOR)
            {
                CeilingColor = (Color)TempColor;
                ChangeStructureColor(Ceiling, CeilingColor);
            }
        }
        WallFloorCeilingColorCanvas.SetActive(false);
        MainSettingsCanvas.SetActive(true);
        MainMenuMode = MAIN_MENU_SETTING.NULL;
    }

    private void OnWallFloorCeilingColorCancelClick()
    {
        MainSettingsCanvas.SetActive(true);
        WallFloorCeilingColorCanvas.SetActive(false);
        MainMenuMode = MAIN_MENU_SETTING.NULL;
    }

    private void ChangeStructureColor(GameObject Structure, Color ToChangeColor) {
        Renderer rend = Structure.GetComponentsInChildren<Renderer>()[0];
        Material spriteMaterial = Structure.GetComponentsInChildren<MeshRenderer>()[0].material;
        rend.enabled = true;
        spriteMaterial.SetColor("_Color", ToChangeColor);
    }
    #endregion


    #endregion

    #region Rotate Camera
    [SerializeField]
    private GameObject RotateCamera;

    public void RotateCameraSetup()
    {
        Button RotateCameraButton = RotateCamera.GetComponent<Button>();
        RotateCameraButton.onClick.AddListener(OnRotateCameraClick);
    }

    public void OnRotateCameraClick()
    {
        switch (GameManager.CurrentActiveWall)
        {
            case OBJECT_PLACEMENT.BACK_WALL:
                GameManager.RotateCamera(Vector3.up * 270, 0.5f);
                GameManager.CurrentActiveWall = OBJECT_PLACEMENT.RIGHT_WALL;
                break;
            case OBJECT_PLACEMENT.RIGHT_WALL:
                GameManager.RotateCamera(Vector3.up * 180, 0.5f);
                GameManager.CurrentActiveWall = OBJECT_PLACEMENT.FRONT_WALL;
                break;
            case OBJECT_PLACEMENT.FRONT_WALL:
                GameManager.RotateCamera(Vector3.up * 90, 0.5f);
                GameManager.CurrentActiveWall = OBJECT_PLACEMENT.LEFT_WALL;
                break;
            case OBJECT_PLACEMENT.LEFT_WALL:
                GameManager.RotateCamera(Vector3.up * 360, 0.5f);
                GameManager.CurrentActiveWall = OBJECT_PLACEMENT.BACK_WALL;
                break;
        }
        GameManager.CurrActiveGrid = GameManager.GridManager[GameManager.CurrentActiveWall];
        //GameManager.ShowCurrentGridObjects();
    }

    #endregion

    #region Zoom Camera
    [SerializeField]
    private GameObject ZoomCamera;

    private bool IsZoomedIn;

    public void ZoomCameraSetup()
    {
        IsZoomedIn = true;
        Button ZoomCameraButton = ZoomCamera.GetComponent<Button>();
        ZoomCameraButton.onClick.AddListener(OnZoomClick);
    }

    private void OnZoomClick() {
        if (IsZoomedIn)
        {
            GameManager.ZoomCameraOut();
            IsZoomedIn = false;
        }
        else {
            GameManager.ZoomCameraIn();
            IsZoomedIn = true;
        }
    }

    #endregion

    #region My Art



    [SerializeField]
    private GameObject MyArtButtonObject;

    private GameObject MyArtCanvas;

    private GameObject InventoryContainer;
    private GameObject InventoryContent;

    [SerializeField]
    private GameObject ObjectTogglePrefab;

    [SerializeField]
    private Sprite PlaceObjectSprite;
    [SerializeField]
    private Sprite RemoveObjectSprite;
    [SerializeField]
    private Sprite RemoveObjectDisabledSprite;

    private void MyArtSetup() {

        MyArtCanvas = FindCanvasGameObject(EditRoomCanvas,"MyArt");

        Button MyArtButton = MyArtButtonObject.GetComponent<Button>();
        MyArtButton.onClick.AddListener(OnMyArtClick);

        InventoryContainer = FindCanvasGameObject(MyArtCanvas,"Inventory-Vertical");
        //InventoryContent = FindCanvasGameObject(InventoryContainer, "Inventory-Content");

        //Default
        MyArtCanvas.SetActive(false);
    }

    public void CreateVerticalInventory()
    {
        MyArtCanvas = FindCanvasGameObject(EditRoomCanvas, "MyArt");

        InventoryContent = FindCanvasGameObject(MyArtCanvas, "Inventory-Content");
        RectTransform ScrollContentTransfrom = InventoryContent.GetComponent<RectTransform>();
        int NumRows = Mathf.CeilToInt((float)GameManager.MuseumObjectMasterList.Count / 3f);
        ScrollContentTransfrom.sizeDelta = new Vector2(6.8f, 160 * NumRows);

        GameObject Row = new GameObject("Row");
        int i = 0;
        foreach (var obj in GameManager.MuseumObjectMasterList.Values)
        {


            Debug.Log(ObjectTogglePrefab);
            GameObject ButtonContainerObj = Instantiate(ObjectTogglePrefab) as GameObject;
            ButtonContainerObj.tag = "Inventory-Button";
            ButtonContainerObj.AddComponent<Button_Inventory>();
            ButtonContainerObj.GetComponent<Button_Inventory>().SetArtTitle(obj.Title);
            RectTransform ButtonContainerRect = ButtonContainerObj.GetComponent<RectTransform>();
            ButtonContainerRect.sizeDelta = new Vector2(100f, 100f);

            GameObject ButtonObj = FindCanvasGameObject(ButtonContainerObj, "Inventory-Button");
            RectTransform ButtonRect = ButtonObj.GetComponent<RectTransform>();

            GameObject ButtonImageObj = FindCanvasGameObject(ButtonContainerObj, "Inventory-Image");
            RectTransform ButtonImageRect = ButtonImageObj.GetComponent<RectTransform>();

            Image InventoryImage = ButtonImageObj.GetComponent<Image>();
           
            //If the image is in portrait mode
            if (obj.height > obj.width)
            {
                ButtonRect.sizeDelta = new Vector2(75f, 100);
                ButtonImageRect.sizeDelta = new Vector2(75f, 100);
                // Aligns indicator to be correct position for portrait mode images
                ButtonRect.anchoredPosition = new Vector2(37.5f, -50f);
            }
            //If the image is in landscape mode
            else if (obj.width > obj.height)
            {
                ButtonRect.sizeDelta = new Vector2(100, 75f);
                ButtonImageRect.sizeDelta = new Vector2(100, 75f);
            }
            // Square image
            else
            {
                ButtonRect.sizeDelta = new Vector2(100, 100);
                ButtonImageRect.sizeDelta = new Vector2(100, 100);
            }

            Sprite img = Resources.Load<Sprite>("Inventory/" + obj.Title);
            InventoryImage.sprite = img;

            // Set button onclick + isEnabled
            Button NewButton = ButtonObj.GetComponent<Button>();
            NewButton.name = obj.Title.ToString() + "-button";
            NewButton.onClick.AddListener(delegate
            {
                OnInventoryObjectClick(obj);
            });

            if (i % 3 == 0)
            {
                if (i != 0)
                {
                    Row = new GameObject("Row");
                }
                HorizontalLayoutGroup HorizontalLayout = Row.AddComponent(typeof(HorizontalLayoutGroup)) as HorizontalLayoutGroup;
                HorizontalLayout.childAlignment = TextAnchor.UpperLeft;
                HorizontalLayout.childForceExpandWidth = true;
                HorizontalLayout.childForceExpandHeight = true;

                RectTransform RowRect = Row.GetComponent<RectTransform>();
                RowRect.sizeDelta = new Vector2(300, 100);
                RowRect.position = new Vector2(150, -50);

                Row.transform.SetParent(InventoryContent.transform);
            }

            ButtonContainerObj.transform.SetParent(Row.transform);
            i++;
        }
    }

    private void OnMyArtClick()
    {
        //Null --> Placing
        if (Mode == EDIT_UI_MODE.NULL)
        {
            Debug.Log("Opening My Art");
            MyArtCanvas.SetActive(true);


            InventoryContainer.SetActive(true);
            ToggleMainUIButtons(false);

            ResetInventoryButtons();

            //Set original button as "Back" button
            Button MyArtButton = MyArtButtonObject.GetComponent<Button>();
            MyArtButton.interactable = true;


            // Change Camera FOV
            GameManager.ZoomCameraOut();

            GameManager.RoomState = ROOM_EDITING_STATE.IN_UI;
            Mode = EDIT_UI_MODE.PLACING;
        }
        //Placing --> Null
        else
        {
            Debug.Log("Closing My Art");
            InventoryContainer.SetActive(false);
            ToggleMainUIButtons(true);

            //GameManager.RestoreCameraZoom();
            GameManager.RoomState = ROOM_EDITING_STATE.TRANSFORMATION;
            Mode = EDIT_UI_MODE.NULL;
        }

    }

    private void ResetInventoryButtons()
    {
        GameObject[] AllInventoryButtons = GameObject.FindGameObjectsWithTag("Inventory-Button");
        foreach (GameObject ButtonContainer in AllInventoryButtons)
        {

            ART_TITLES title = ButtonContainer.GetComponent<Button_Inventory>().AffiliatedArtTitle;
            GameObject Indicator = FindCanvasGameObject(ButtonContainer, "Indicator");
            Image IndicatorImage = Indicator.GetComponent<Image>();

            GameObject ButtonImageObj = FindCanvasGameObject(ButtonContainer, "Inventory-Image");
            Image ButtonImage = ButtonImageObj.GetComponent<Image>();

            // Can place obj
            if (GameManager.MuseumObjectMasterList[title].CurrentWall == OBJECT_PLACEMENT.INVENTORY)
            {
                IndicatorImage.sprite = PlaceObjectSprite;
            }
            else
            {
                IndicatorImage.sprite = RemoveObjectSprite;

                //TODO: Delete if user should be able to remove from any wall
                if (GameManager.MuseumObjectMasterList[title].CurrentWall != GameManager.CurrentActiveWall)
                {
                    Button button = ButtonContainer.GetComponentInChildren<Button>();
                    IndicatorImage.sprite = RemoveObjectDisabledSprite;
                    button.interactable = false;

                    ButtonImage.color = DisabledInventoryImageColor;
                }

            }       
        }
    }

    private void OnInventoryObjectClick(MuseumObject obj) {
        //Place object
        if (obj.CurrentWall == OBJECT_PLACEMENT.INVENTORY)
        {
            if (GameManager.HasPlacedObject(obj))
            {
                Debug.Log("Placed obj");
            }
            else {
                Debug.Log("Cannot Place Object");
            }

        }
        // Remove and object
        else if(obj.CurrentWall == GameManager.CurrentActiveWall){
            Debug.Log("Removing obj");
            GameManager.CurrActiveGrid.RemoveMuseumObjectFromGridCoords((int)obj.origin.x, (int)obj.origin.y);
            obj.CurrentWall = OBJECT_PLACEMENT.INVENTORY;
        }

        ResetInventoryButtons();
    }


    #endregion

    //End of Edit Room Settings
    #endregion


    #region Done Room
    [SerializeField]
    private GameObject DoneRoomCanvas;

    [SerializeField]
    private GameObject Done;

    private GameObject ShareWrapper;
    private GameObject LoaderContainer;

    public void SaveRoomSetup()
    {
        Button SaveButton = Done.GetComponent<Button>();
        SaveButton.onClick.AddListener(OnSaveClick);

        LoaderContainer = FindCanvasGameObject(DoneRoomCanvas, "LoaderContainer");

        ShareWrapper = FindCanvasGameObject(DoneRoomCanvas, "ShareWrapper");

        GameObject ShareGameObject = FindCanvasGameObject(DoneRoomCanvas, "ShareOnSocialButton");
        Button ShareButton = ShareGameObject.GetComponent<Button>();
        //ShareButton.onClick.AddListener()

        GameObject CloseGameObject = FindCanvasGameObject(DoneRoomCanvas, "CloseButton");
        Button CloseButton = CloseGameObject.GetComponent<Button>();
        CloseButton.onClick.AddListener(OnShareCloseClick);

        //Default
        DoneRoomCanvas.SetActive(false);
    }

    private void OnSaveClick()
    {
        ToggleMainUIButtons(false);
        GameManager.RoomState = ROOM_EDITING_STATE.IN_UI;

        DoneRoomCanvas.SetActive(true);
        LoaderContainer.SetActive(true);

        GameManager.TakeScreenshots();
    }

    public void OnSaveDone()
    {
        LoaderContainer.SetActive(false);
    }

    public void OnShareCloseClick() {

        EditRoomCanvas.SetActive(false);
        ToggleMainUIButtons(true);

        ViewRoomCanvas.SetActive(true);
        DoneRoomCanvas.SetActive(false);
        GameManager.RoomState = ROOM_EDITING_STATE.TRANSFORMATION;
    }


    #endregion

    #region OLD

    #region Place Object

    //sliding area = 10, 10
    //width 13, bottom 0.1

    private void CreateHorizontalInventory()
    {
        RectTransform ScrollContentTransform = InventoryContent.GetComponent<RectTransform>();
        int NumObjects = GameManager.MuseumObjectMasterList.Count;
        ScrollContentTransform.sizeDelta = new Vector2(NumObjects * 100, 100);
        //ScrollContentTransform.position = new Vector2((NumObjects*100)/2, -54f);

        foreach (var obj in GameManager.MuseumObjectMasterList.Values)
        {
            GameObject ButtonObj = Instantiate(ObjectTogglePrefab) as GameObject;
            ButtonObj.tag = "Inventory-Button";
            ButtonObj.AddComponent<Button_Inventory>();
            ButtonObj.GetComponent<Button_Inventory>().SetArtTitle(obj.Title);

            // Set image of button
            Image ButtonImage = ButtonObj.GetComponentInChildren<Image>();
            Sprite img = Resources.Load<Sprite>("Inventory/" + "madame-x");
            ButtonImage.sprite = img;

            // Set button onclick + isEnabled
            Button NewButton = ButtonObj.GetComponentInChildren<Button>();
            NewButton.onClick.AddListener(delegate
            {
                OnInventoryObjectClick(obj);
            });
            NewButton.interactable = obj.CurrentWall == OBJECT_PLACEMENT.INVENTORY ? true : false;

            ButtonObj.transform.SetParent(InventoryContent.transform);
        }
    }


    #endregion

    #region Room Settings Menu

    [SerializeField]
    private GameObject RoomSettingsContainer;

    [SerializeField]
    private GameObject SetRoomSize;

    [SerializeField]
    private GameObject ConfirmationContainer;


    #region Main Functions
    private void SetupRoomSettingsMenu()
    {

        Button confirmationPanelCancel = ConfirmationPanelCancel.GetComponent<Button>();
        confirmationPanelCancel.onClick.AddListener(VerifyPanelCancelOnClick);

        Button confirmationPanelConfirm = ConfirmationPanelConfirm.GetComponent<Button>();
        confirmationPanelConfirm.onClick.AddListener(VerifyPanelConfirmOnClick);

        //Room Size
        Button setRoomSizeButton = SetRoomSize.GetComponent<Button>();
        setRoomSizeButton.onClick.AddListener(OnSetRoomSizeClick);
        SetRoomSizeButtonListeners();
    }

    
    private void CloseRoomSettingsMenu()
    {
        //Room Size
        tmpRoomSize = ROOM_SIZE.Null;

        // Confirmation Panel
        VerifyPanelCancelOnClick();

        // Main Room Settings
        RoomSettingsContainer.SetActive(false);
        FullScreenPanel.SetActive(false);
        RoomSettingsButton.SetActive(true);
    }
    #endregion


    #region Confirmation Panel
    [SerializeField]
    private Text primaryPromptText;

    [SerializeField]
    private GameObject ConfirmationPanelCancel;

    [SerializeField]
    private GameObject ConfirmationPanelConfirm;

    private void ShowConfirmationPanel()
    {
        ConfirmationContainer.SetActive(true);

        switch (MainMenuMode)
        {
            case MAIN_MENU_SETTING.ROOM_SIZE:
                primaryPromptText.text = "Are sure you want this new room size? \n All your objects will be returne to your inventory!";


                break;
            default:
                break;
        }
    }

    private void VerifyPanelCancelOnClick()
    {

        switch (MainMenuMode)
        {
            case MAIN_MENU_SETTING.ROOM_SIZE:
                ResetRoomSizeSetting();
                break;
            default:
                break;
        }

        ConfirmationContainer.SetActive(false);
        MainMenuMode = null;
    }

    private void VerifyPanelConfirmOnClick()
    {
        switch (MainMenuMode)
        {
            case MAIN_MENU_SETTING.ROOM_SIZE:

                ResizeRoomConfirm();
                ConfirmationContainer.SetActive(false);
                CloseRoomSettingsMenu();
                break;
            default:
                break;
        }
    }
    #endregion

    #region Room Size
    [SerializeField]
    private GameObject RoomSizeOptions;
    [SerializeField]
    private GameObject SmRoom;
    [SerializeField]
    private GameObject MdRoom;
    [SerializeField]
    private GameObject LgRoom;
    [SerializeField]
    private GameObject XLgRoom;

    private ROOM_SIZE tmpRoomSize = ROOM_SIZE.Null;

    private void OnSetRoomSizeClick()
    {
        SetRoomSize.GetComponent<Button>().interactable = false;
        RoomSizeOptions.SetActive(true);

        SetCurrentRoomSizeButton(GameManager.CurrRoomSize, false);
    }

    private void SetRoomSizeButtonListeners()
    {
        Button smRmBtn = SmRoom.GetComponent<Button>();
        smRmBtn.onClick.AddListener(delegate {
            OnNewRoomSizeClick(ROOM_SIZE.Small);
        });

        Button mdRmBtn = MdRoom.GetComponent<Button>();
        mdRmBtn.onClick.AddListener(delegate {
            OnNewRoomSizeClick(ROOM_SIZE.Medium);
        });

        Button lgRmBtn = LgRoom.GetComponent<Button>();
        lgRmBtn.onClick.AddListener(delegate {
            OnNewRoomSizeClick(ROOM_SIZE.Large);
        });

        Button xlgRmBtn = XLgRoom.GetComponent<Button>();
        xlgRmBtn.onClick.AddListener(delegate {
            OnNewRoomSizeClick(ROOM_SIZE.XLarge);
        });
    }

    private void OnNewRoomSizeClick(ROOM_SIZE newSize)
    {
        MainMenuMode = MAIN_MENU_SETTING.ROOM_SIZE;
        tmpRoomSize = newSize;

        SetCurrentRoomSizeButton(tmpRoomSize, true);
        ShowConfirmationPanel();
    }

    private void ResetSizeButtonAppearances(Button toChangeButton)
    {
        //toChangeButton.interactable = false;
        ColorBlock colorVar = toChangeButton.colors;
        colorVar.disabledColor = DefaultSelectedButtonColor;
        colorVar.selectedColor = DefaultSelectedButtonColor;
        colorVar.normalColor = DefaultSelectedButtonColor;
        colorVar.highlightedColor = DefaultSelectedButtonColor;
        toChangeButton.colors = colorVar;
    }

    private void SetCurrentRoomSizeButton(ROOM_SIZE? rmSize, bool shouldResetOtherButtons)
    {
        Button sm = SmRoom.GetComponent<Button>();
        Button md = MdRoom.GetComponent<Button>();
        Button lg = LgRoom.GetComponent<Button>();
        Button xlg = XLgRoom.GetComponent<Button>();

        if (shouldResetOtherButtons)
        {
            ResetSizeButtonAppearances(sm);
            ResetSizeButtonAppearances(md);
            ResetSizeButtonAppearances(lg);
            ResetSizeButtonAppearances(xlg);
        }

        Button currClickedButton;
        switch (rmSize)
        {
            case ROOM_SIZE.Small:
                currClickedButton = sm;
                break;
            case ROOM_SIZE.Medium:
                currClickedButton = md;
                break;
            case ROOM_SIZE.Large:
                currClickedButton = lg;
                break;
            case ROOM_SIZE.XLarge:
                currClickedButton = xlg;
                break;
            default:
                currClickedButton = sm;
                break;
        }

        ColorBlock colorVar = currClickedButton.colors;
        colorVar.selectedColor = SelectedButtonColor;
        colorVar.disabledColor = SelectedButtonColor;
        colorVar.highlightedColor = SelectedButtonColor;

        if (!shouldResetOtherButtons)
        {
            colorVar.normalColor = SelectedButtonColor;

        }

        currClickedButton.colors = colorVar;
    }

    private void ResetRoomSizeSetting()
    {
        SetCurrentRoomSizeButton(GameManager.CurrRoomSize, true);
        RoomSizeOptions.SetActive(false);
        SetRoomSize.GetComponent<Button>().interactable = true;
        tmpRoomSize = ROOM_SIZE.Null;
    }

    private void ResizeRoomConfirm()
    {
        if (GameManager.CurrRoomSize == tmpRoomSize)
        {
            return;
        }
        if (tmpRoomSize == ROOM_SIZE.Large || (tmpRoomSize == ROOM_SIZE.Medium && GameManager.CurrRoomSize == ROOM_SIZE.Small))
        {
            GameManager.CurrRoomResizeType = ROOM_RESIZE_TYPE.SmallerToBigger;
        }
        else if (tmpRoomSize == ROOM_SIZE.Small)
        {
            GameManager.CurrRoomResizeType = ROOM_RESIZE_TYPE.LargerToSmaller;
        }
        GameManager.CurrRoomSize = tmpRoomSize;
        BuildNewRoom();

    }


    #endregion

    #region Build Room


    #endregion

    #endregion

    #endregion


    #region Utils

    public static GameObject FindCanvasGameObject(GameObject parent, string name) {
        
        RectTransform[] trs = parent.GetComponentsInChildren<RectTransform>(true);
        foreach (RectTransform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    public static GameObject FindGameObject(GameObject parent, string name)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }


    #endregion



}
