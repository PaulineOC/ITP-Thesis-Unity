using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.Rendering;
 
public class Manager : MonoBehaviour
{

    //Master Lists:

    public Dictionary<string, ArtInformation> TemplateArtInformationList = new Dictionary<string, ArtInformation>();

    public Dictionary<ART_TITLES, MuseumObject> MuseumObjectMasterList = new Dictionary<ART_TITLES, MuseumObject>();

    public Dictionary<ROOM_SIZE, RoomMeasurements> RoomSizeToMeasurements =
        new Dictionary<ROOM_SIZE, RoomMeasurements>
        {
            {
                ROOM_SIZE.Small,
                new RoomMeasurements(
                    15,
                    10,
                    8,
                    1f,
                    0.5f
                )
            },
            {
                ROOM_SIZE.Medium,
                new RoomMeasurements(
                    18,
                    18,
                    8,
                    1f,
                    0.5f
                )
            },
            {
                ROOM_SIZE.Large,
                new RoomMeasurements(
                    25,
                    20,
                    8,
                    1f,
                    0.5f
                )
            },
        };

    private UIManager UIManager;

    public Dictionary<OBJECT_PLACEMENT, Grid> GridManager = new Dictionary<OBJECT_PLACEMENT, Grid>();

    public MAIN_SCREEN CurrentMainScreen = MAIN_SCREEN.VIEW_SCREEN;


    public Grid CurrActiveGrid;
    public OBJECT_PLACEMENT CurrentActiveWall = OBJECT_PLACEMENT.INVENTORY;
    public ROOM_EDITING_STATE RoomState = ROOM_EDITING_STATE.TRANSFORMATION;

    private float timer = -1f;
    private const float TOUCH_THRESHOLD = 0.15f;

    public MuseumObject SelectedObj = null;

    public int xOffset;
    public int yOffset;

    //Camera
    public GameObject MainCamera;

    public float CameraXOffSet;
    public float CameraYOffset;
    public Vector3 PrevMouseWorldPosition;

    public GameObject Room;

    private void Awake()
    {

        UIManager = GetComponent<UIManager>();

        CreateTemplateArtStructs();


        //OLD
        CurrentActiveWall = OBJECT_PLACEMENT.BACK_WALL;
        RoomState = ROOM_EDITING_STATE.IN_UI;

    }

    // Start is called before the first frame update
    void Start()
    {
        FakeReceiveDataFromFrontEnd();

        #if (!UNITY_EDITOR)
        //TODO: Remove this for testing
        OnStartForFrontend();
        #endif


        RoomState = ROOM_EDITING_STATE.TRANSFORMATION;

        PopulateMasterMuseumObjectsList();

        //AddArtToWallAndGrid(ART_TITLES.FLOWER_GIRL, new Vector3(6.5f, 1.0f, -0.5f));

        //AddArtToWallAndGrid(ART_TITLES.MADAME_X, new Vector3(3.5f, -1.5f, -0.5f));

   
        //Move Camera to Center of Current Room:
        RoomMeasurements rmMeasurements = RoomSizeToMeasurements[CurrRoomSize];

        float newX = rmMeasurements.backFrontWallLength / 2;
        float testZ = (float)(-rmMeasurements.wallHeight / 2 + 0.5) + 1.6256f;
        float newY = -3f + 1.6256f;
        float newZ = -rmMeasurements.leftRightWallLength / 2;
        MainCamera.transform.position = new Vector3(newX, -0.75f, newZ);

        CurrActiveGrid = GridManager[CurrentActiveWall];

    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 MouseWorldPosition = GetMouseWorldPosition();

        switch (RoomState)
        {
            case ROOM_EDITING_STATE.TRANSFORMATION:
                TransformationPhase(MouseWorldPosition, PrevMouseWorldPosition);
                break;
            case ROOM_EDITING_STATE.REMOVING:
                RemovePhase(MouseWorldPosition);
                break;
            default:
                break;
        }

        PrevMouseWorldPosition = MouseWorldPosition;
    }

    public void ChangeMainScreen(MAIN_SCREEN newScreen) {
        CurrentMainScreen = newScreen;
        Debug.Log("New Main Screen: " + CurrentMainScreen);
    }

#region View Mode

#endregion

#region Edit Mode

    #region Rotate Camera

    IEnumerator RotatingCamera(Vector3 byAngles, float inTime, GameObject toRotate)
    {
        var fromAngle = MainCamera.transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
        for (var t = 0f; t < 1; t += Time.deltaTime / inTime)
        {
            toRotate.transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            yield return null;
        }
        // Hard-set angle if Quaternion slerp makes weird angles
        toRotate.transform.rotation = toAngle;

    }

    public void RotateCamera(Vector3 newPos, float duration)
    {
        Room.transform.rotation = Quaternion.Euler(newPos);
        ResetCameraPositionToRoomCenter();

        StartCoroutine(RotatingCamera(Vector3.up * 0, duration, MainCamera));

        MainCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void ResetCameraPositionToRoomCenter()
    {

        RoomMeasurements rmMeasurements = RoomSizeToMeasurements[CurrRoomSize];

        float x;
        float y = -0.75f;
        float z;


        //float testZ = (float)(-rmMeasurements.wallHeight / 2 + 0.5) + 1.6256f;
        //float newY = -3f + 1.6256f;
        //float newZ = -rmMeasurements.leftRightWallLength / 2;

        switch (CurrentActiveWall)
        {
            case OBJECT_PLACEMENT.BACK_WALL:

                //Camera -> right wall
                x = rmMeasurements.leftRightWallLength / 2;
                z = rmMeasurements.backFrontWallLength / 2;
                break;
            case OBJECT_PLACEMENT.RIGHT_WALL:
                //Camera -> front wall
                x = -rmMeasurements.backFrontWallLength / 2;
                z = rmMeasurements.leftRightWallLength / 2;
                break;
            case OBJECT_PLACEMENT.FRONT_WALL:
                // Camera -> left wall
                x = -rmMeasurements.leftRightWallLength / 2;
                z = -rmMeasurements.backFrontWallLength / 2;
                break;
            case OBJECT_PLACEMENT.LEFT_WALL:
                // Camera -> backwall
                x = rmMeasurements.backFrontWallLength / 2;
                z = -rmMeasurements.leftRightWallLength / 2;
                break;
            default:
                // Camera -> backwall
                x = rmMeasurements.backFrontWallLength / 2;
                z = -rmMeasurements.leftRightWallLength / 2;
                break;
        }

        MainCamera.transform.position = new Vector3(x, y, z);
    }

    public void MoveCamera(Vector3 MouseWorldPosition, Vector3 PrevMousePosition, float dt)
    {
        Vector3 movement = MouseWorldPosition - PrevMouseWorldPosition;
        movement.z = 0f;

        float dragSpeed = 0.75f;
        MainCamera.transform.Translate(movement * dragSpeed);

        //MainCamera.transform.position = new Vector3(MouseWorldPosition.x+xOffset, MouseWorldPosition.y+YOffset, MainCamera.transform.position.z);
    }

    public void HideAllGridObjects() {
        foreach (MuseumObject obj in MuseumObjectMasterList.Values) {
            if (obj.CurrentWall != OBJECT_PLACEMENT.INVENTORY) {
                obj.PrefabObj.SetActive(false);
            }
        }
    }

    public void ShowCurrentGridObjects()
    {
        foreach (MuseumObject obj in MuseumObjectMasterList.Values)
        {
            if (obj.CurrentWall == CurrentActiveWall)
            {
                obj.PrefabObj.SetActive(true);
            }
        }

    }

    #endregion

    #region ZoomCamera

    IEnumerator ZoomingCamera(float toFOV, float InTime, Vector3 NewPos, bool IsZoomIn)
    {
        float difference = Mathf.Abs(Camera.main.fieldOfView - toFOV);
        while (difference > 0.05) {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, toFOV, InTime);
            difference = Mathf.Abs(Camera.main.fieldOfView - toFOV);
            yield return null;
        }

        Camera.main.fieldOfView = toFOV;
        if (IsZoomIn) {
            Camera.main.transform.localPosition = NewPos;
        }
    }

    public void ZoomCameraOut()
    {
        float newY = -0.75f;
        Vector3 NewPos = new Vector3(0f, 0f, 0f);
        switch (CurrentActiveWall)
        {
            case OBJECT_PLACEMENT.BACK_WALL:
                NewPos = new Vector3(8f, newY, -5f);
                Camera.main.transform.localPosition = NewPos;
                StartCoroutine(ZoomingCamera(140f, 0.5f, NewPos, false));
                break;
            case OBJECT_PLACEMENT.RIGHT_WALL:
                NewPos = new Vector3(7.25f, newY, -5.5f);
                Camera.main.transform.localPosition = NewPos;
                StartCoroutine(ZoomingCamera(100, 0.5f, NewPos, false));
                break;
            case OBJECT_PLACEMENT.FRONT_WALL:
                NewPos = new Vector3(8f, newY, -5f);
                Camera.main.transform.localPosition = NewPos;
                StartCoroutine(ZoomingCamera(140f, 0.5f, NewPos, false));
                break;
             case OBJECT_PLACEMENT.LEFT_WALL:
                NewPos = new Vector3(9f, newY, -5.5f);
                Camera.main.transform.localPosition = NewPos;
                StartCoroutine(ZoomingCamera(100, 0.5f, NewPos, false));
                break;
        }
    }

    public void ZoomCameraIn()
    {
        float newY = -0.75f;
        Vector3 NewPos = new Vector3(0f,0f,0f);

        switch (CurrentActiveWall)
        {
            case OBJECT_PLACEMENT.BACK_WALL:
                NewPos = new Vector3(8f, newY, -5f);
                StartCoroutine(ZoomingCamera(50f, 0.5f, NewPos , true));
                break;
            case OBJECT_PLACEMENT.RIGHT_WALL:
                NewPos = new Vector3(7.25f, newY, -5.5f);
                StartCoroutine(ZoomingCamera(50f, 0.5f, NewPos, true));
                break;
            case OBJECT_PLACEMENT.FRONT_WALL:
                NewPos = new Vector3(8f, newY, -5f);
                StartCoroutine(ZoomingCamera(50f, 0.5f, NewPos, true));
                break;
            case OBJECT_PLACEMENT.LEFT_WALL:
                NewPos = new Vector3(9f, newY, -5.5f);
                StartCoroutine(ZoomingCamera(50f, 0.5f, NewPos, true));
                break;
        }
    }

    #endregion

    #endregion

    private void TransformationPhase(Vector3 MouseWorldPosition, Vector3 PrevMouseWorldPosition)
    {

        if (timer >= 0.0f)
        {
            timer += Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0))
        {
            SelectedObj = SelectedObj == null ? CurrActiveGrid.GetMuseumObjectFromWorldPosition(MouseWorldPosition) : SelectedObj;
            if (SelectedObj != null)
            {
                int x, y;
                CurrActiveGrid.GetGridXYFromWorldPosition(MouseWorldPosition, out x, out y);
                xOffset = (int)(x - SelectedObj.origin.x);
                yOffset = (int)(y - SelectedObj.origin.y);
            }
            else
            {
                CameraXOffSet = MainCamera.transform.position.x - MouseWorldPosition.x;
                CameraYOffset = MainCamera.transform.position.y - MouseWorldPosition.y;
            }

            timer = 0.0f;
        }

        //Moving an Obj
        if (SelectedObj != null && timer >= TOUCH_THRESHOLD)
        {
            MoveCurrentSelectedObj(SelectedObj, MouseWorldPosition, CurrActiveGrid, xOffset, yOffset);
        }
        //Moving Main Camera
        else if (SelectedObj == null && timer >= TOUCH_THRESHOLD)
        {
            MoveCamera(MouseWorldPosition, PrevMouseWorldPosition, Time.deltaTime);
        }

        //End Mouse Drag
        if (Input.GetMouseButtonUp(0))
        {

            //Rotating Object
            if (SelectedObj != null && timer < TOUCH_THRESHOLD)
            {
                Debug.Log("ROTATING OBJECT");
            }
            else if (SelectedObj != null && timer >= TOUCH_THRESHOLD)
            {
                Vector2 gridOrigin = SelectedObj.origin;

                // Since we're moving via an offset from the original bottom left anchor,
                // need to correctly set the obj and its origin when placing

                CurrActiveGrid.GetGridXYFromWorldPosition(MouseWorldPosition, out int rawX, out int rawY);
                int x = rawX - xOffset;
                int y = rawY - yOffset;

                //Check for collisions
                if (CurrActiveGrid.IsColliding(SelectedObj, x, y))
                {

                    SelectedObj.PrefabObj.transform.position = CurrActiveGrid.GetWorldPositionFromGridCoords((int)SelectedObj.origin.x,
                                                                                                             (int)SelectedObj.origin.y);
                    SelectedObj.SetOriginalMaterial();
                }
                else
                {
                    CurrActiveGrid.RemoveMuseumObjectFromGridCoords((int)gridOrigin.x, (int)gridOrigin.y);
                    CurrActiveGrid.SetCellMuseumObject(x, y, SelectedObj);
                    SelectedObj.SetOriginalMaterial();
                }

            }
            SelectedObj = null;
            timer = -1f;
        }

    }

    private void MoveCurrentSelectedObj(MuseumObject obj, Vector3 MouseWorldPos, Grid CurrGrid, int offX, int offY) {

        if (obj == null || MouseWorldPos == null || CurrGrid == null) {
            return;
        };

        obj.SetMovingPosMaterial();

        int rawX, rawY;

        CurrGrid.GetGridXYFromWorldPosition(MouseWorldPos, out rawX, out rawY);
        int x = rawX - offX;
        int y = rawY - offY;
        //Debug.Log("x,y: "+x+" "+y);

        //Check if valid new position (within bounds of grid)
        if (!CurrGrid.IsNewValidPosition(obj, x, y)) {
            return;
        };

        //Check for collisions
        if (CurrGrid.IsColliding(obj, x, y))
        {
            obj.SetIncorrectPosMaterial();
        }

        Vector3 mousePosFromGridCoords = CurrGrid.GetWorldPositionFromGridCoords(x, y);
        obj.PrefabObj.transform.position = mousePosFromGridCoords;
    }

    public bool HasPlacedObject(MuseumObject obj)
    {
        //Can fit object
        int x, y;
        CurrActiveGrid.FindEmptySpace(obj, out x, out y);

        //Valid position found
        if (x != -1 && y != -1) {

            obj.CurrentWall = CurrentActiveWall;
            Debug.Log("Museum obj set to: " + MuseumObjectMasterList[obj.Title].CurrentWall+" at coords: "+x+" "+y);
            obj.PrefabObj = Instantiate(obj.PrefabObj, new Vector3(0f, 0f, 0f), Quaternion.identity);
            obj.PrefabObj.name = obj.Title.ToString();
            CurrActiveGrid.SetCellMuseumObject(x, y, obj);

            GameObject parent;
            switch (CurrentActiveWall)
            {
                case OBJECT_PLACEMENT.BACK_WALL:
                    //Debug.Log("Placing on Back Wall");
                    obj.CurrentWall = OBJECT_PLACEMENT.BACK_WALL;
                    parent = UIManager.FindGameObject(Room, "BackWall");
                    break;
                case OBJECT_PLACEMENT.RIGHT_WALL:
                    //Debug.Log("Placing on Right Wall");
                    obj.CurrentWall = OBJECT_PLACEMENT.RIGHT_WALL;
                    parent = UIManager.FindGameObject(Room, "RightWall");
                    break;
                case OBJECT_PLACEMENT.FRONT_WALL:
                    //Debug.Log("Placing on Front Wall");
                    obj.CurrentWall = OBJECT_PLACEMENT.FRONT_WALL;
                    parent = UIManager.FindGameObject(Room, "FrontWall");
                    break;
                case OBJECT_PLACEMENT.LEFT_WALL:
                    //Debug.Log("Placing on left Wall");
                    obj.CurrentWall = OBJECT_PLACEMENT.LEFT_WALL;
                    parent = UIManager.FindGameObject(Room, "LeftWall");
                    break;
                default:
                    parent = UIManager.FindGameObject(Room, "BackWall");
                    break;
            }

            obj.PrefabObj.transform.SetParent(parent.transform);

            return true;
        }
        else
        {
            return false;
        }
    }

    #region Screenshot Room
    [DllImport("__Internal")]
    private static extern void  SendScreenshot(string imgData);

    [SerializeField]
    private RenderTexture BackwallRenderTexture;

    [SerializeField]
    private RenderTexture RightwallRenderTexture;

    [SerializeField]
    private RenderTexture FrontwallRenderTexture;

    [SerializeField]
    private RenderTexture LeftwallRenderTexture;

    [SerializeField]
    private RenderTexture OverheadBackRightTexture;

    [SerializeField]
    private RenderTexture OverheadFrontLeftTexture;

    public List<string> AllScreenshotAsStrings = new List<string>();

    private string ShareableImageData;

    public void TakeScreenshots()
    {
        Camera.onPostRender += ScreenShotCamerasOnPostRender;
    }

    public void ScreenShotCamerasOnPostRender(Camera cam) {

        Debug.Log("In Screenshot Post Render!");

        CaptureImage("Screenshot-Backwall.png", BackwallRenderTexture);
        CaptureImage("Screenshot-Rightwall.png", RightwallRenderTexture);
        CaptureImage("Screenshot-Frontwall.png", FrontwallRenderTexture);
        CaptureImage("Screenshot-Leftwall.png", LeftwallRenderTexture);

        CaptureImage("Screenshot-Overhead-Back-Right.png", OverheadBackRightTexture);
        CaptureImage("Screenshot-Overhead-Front-Left.png", OverheadFrontLeftTexture);

        #if !UNITY_EDITOR
            var AllScreenShotsAsString = string.Join("**BREAK**", AllScreenshotAsStrings.ToArray());
            SendScreenshot(AllScreenShotsAsString);
        #endif

        Camera.onPostRender -= ScreenShotCamerasOnPostRender;

        UIManager.OnSaveDone();
    }

    public void CaptureImage(string filename,RenderTexture ToSet, bool IsShareableImage = false) {

        RenderTexture PreviousRenderTexture = RenderTexture.active;
        RenderTexture.active = ToSet;

        Texture2D tex = new Texture2D(ToSet.width, ToSet.height);
        //Texture2D tex = new Texture2D(ScreenshotRenderTexture.width, ScreenshotRenderTexture.height, TextureFormat.RGB24, false);

        //float UIHeight = 120f;
        //Rect RectWithoutUI = new Rect(0,0 + UIHeight,tex.width, tex.height-UIHeight);
        //tex.ReadPixels(RectWithoutUI, 0, 0);

        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);

        tex.Apply();
        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        //string ToBase64String byte[]
        string encodedText = System.Convert.ToBase64String(bytes);
        string image_url = "data:image/png;base64," + encodedText;
        //Debug.Log(image_url);

        AllScreenshotAsStrings.Add(image_url);
        if (IsShareableImage) {
            ShareableImageData = image_url;
        }

        #if UNITY_EDITOR
            File.WriteAllBytes(filename, bytes);
        #endif

        RenderTexture.active = PreviousRenderTexture;
    }

    #endregion


    #region Share Room

    //[DllImport("__Internal")]
    //private static extern void SendScreenshot(string imgData);


    private void OnShareSetup() { }

    private void OnShareClick() { }


    private void SendShareableImage() {
        if (string.IsNullOrEmpty(ShareableImageData)) {
            return;
        }
        Debug.Log("Sharing Image");

    }

    #endregion

    #region Room Settings
    public ROOM_SIZE CurrRoomSize = ROOM_SIZE.Small;
public ROOM_RESIZE_TYPE CurrRoomResizeType = ROOM_RESIZE_TYPE.NewRoomNoPrevSize;

public void ChangeRoomSize(ROOM_SIZE newRoomSize) {
    CurrRoomSize = newRoomSize;
}

public void RebuildAllGrids() {

    //Calculate the objects to remove
    //Debug.Log("Rebuilding Grids");
    RoomMeasurements rmMeasurements = RoomSizeToMeasurements[CurrRoomSize];

    float WallGridOriginY = (-rmMeasurements.wallHeight / 2) + 1f;
    //Use for sticking out of wall (maybe need for when paintings are on wall)
    //float WallGridOriginZ = -rmMeasurements.structureDepth;
    // Use for flat on wall
    float BackFrontWallGridOriginZ = -(rmMeasurements.structureDepth / 2f);

    float LeftRightWallGridOriginZ = -(rmMeasurements.leftRightWallLength + (float)rmMeasurements.structureDepth / 2);

    Grid NewBackWall = new Grid(
        rmMeasurements.backFrontWallLength,
        rmMeasurements.wallHeight - 1,
        (float)rmMeasurements.structureDepth,
        new Vector3(
            rmMeasurements.poleXZ / 2,
            WallGridOriginY,
            BackFrontWallGridOriginZ
        ),
        GRID_ORIENTATION.XY
        );

    Grid NewRightWall = new Grid(
        rmMeasurements.leftRightWallLength,
        rmMeasurements.wallHeight - 1,
        (float)rmMeasurements.structureDepth,
        new Vector3(
            0,
            WallGridOriginY,
            rmMeasurements.backFrontWallLength+rmMeasurements.poleXZ / 2
        ),
        GRID_ORIENTATION.YZ
        );

    Grid NewFrontWall = new Grid(
        rmMeasurements.backFrontWallLength,
        rmMeasurements.wallHeight - 1,
        (float)rmMeasurements.structureDepth,
        new Vector3(
            -rmMeasurements.backFrontWallLength - rmMeasurements.poleXZ / 2,
            WallGridOriginY,
            rmMeasurements.leftRightWallLength + rmMeasurements.poleXZ / 2
        ),
        GRID_ORIENTATION.XY
        );



    Grid NewLeftWall = new Grid(
        rmMeasurements.leftRightWallLength,
        rmMeasurements.wallHeight - 1,
        (float)rmMeasurements.structureDepth,
        new Vector3(
            -rmMeasurements.leftRightWallLength - rmMeasurements.poleXZ / 2,
            WallGridOriginY,
            -rmMeasurements.poleXZ / 2
        ),
        GRID_ORIENTATION.YZ
        );

    Grid NewGround = new Grid(
        rmMeasurements.backFrontWallLength,
        rmMeasurements.leftRightWallLength,
        (float)rmMeasurements.structureDepth,
        new Vector3(
            rmMeasurements.poleXZ / 2,
            WallGridOriginY,
            LeftRightWallGridOriginZ
        ),
        GRID_ORIENTATION.XZ

        );

    // TODO: Handle Objects Removal/Relocation

    if (CurrRoomResizeType == ROOM_RESIZE_TYPE.SmallerToBigger)
    {
        Debug.Log("Grow things from the center");
    }
    else if (CurrRoomResizeType == ROOM_RESIZE_TYPE.LargerToSmaller)
    {
        Debug.Log("Shrink things in the corner");

    }
    CurrRoomResizeType = ROOM_RESIZE_TYPE.Null;

    GridManager[OBJECT_PLACEMENT.BACK_WALL] = NewBackWall;
    GridManager[OBJECT_PLACEMENT.RIGHT_WALL] = NewRightWall;
    GridManager[OBJECT_PLACEMENT.FRONT_WALL] = NewFrontWall;
    GridManager[OBJECT_PLACEMENT.LEFT_WALL] = NewLeftWall;
}

public void RemoveOrRelocateObjectsWhenResizing(Grid NewBackwall, Grid NewRightWall, Grid NewLeftWall, Grid NewFrontWall) {

    //If remo
}
#endregion

#region Raycasting

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

#endregion

#region Frontend Connections

    //    SEASIDE,
    //    DRAGONS_FLOWERS,
    //    NIGHT_RAIN,
    //    VIRGIN_FACING_RIGHT,
    //    RELIEF_MENTUHOTEP,
    //    WOMAN_MAN_CASEMENT,
    //    WHEAT_FIELD,
    //    HARVESTERS,
    //    MUSICIANS,
    //    WAVY_VINE,
    //    PORT_BOULOGNE

    [DllImport("__Internal")]
    private static extern void UnityHasAwoken(string hasAwoken);

    private void OnStartForFrontend(){
        UnityHasAwoken("AWAKE");
    }

    private void CreateTemplateArtStructs()
    {
        ArtInformation Seaside = new ArtInformation(ART_TITLES.SEASIDE, "At The Seaside", 3,2);
        TemplateArtInformationList.Add(Seaside.ArtTitle.ToString(), Seaside);
      
        ArtInformation DragonsFlowers = new ArtInformation(ART_TITLES.DRAGONS_FLOWERS, "Tapestry With Dragons and Flowers",1,2);
        TemplateArtInformationList.Add(DragonsFlowers.ArtTitle.ToString(), DragonsFlowers);

        ArtInformation NightRain = new ArtInformation(ART_TITLES.NIGHT_RAIN, "Night Rain at the Double-Shelf Stand, from the series Eight Parlor Views (Zashiki hakkei)", 1,1);
        TemplateArtInformationList.Add(NightRain.ArtTitle.ToString(), NightRain);

        ArtInformation VirginFacingRight = new ArtInformation(ART_TITLES.VIRGIN_FACING_RIGHT, "The Head of the Virgin in Three-Quarter View Facing Right",1,1);
        TemplateArtInformationList.Add(VirginFacingRight.ArtTitle.ToString(), VirginFacingRight);

        ArtInformation ReliefMentuhotep = new ArtInformation(ART_TITLES.RELIEF_MENTUHOTEP, "Relief of Nebhepetre Mentuhotep II and the Goddess Hathor",3,1);
        TemplateArtInformationList.Add(ReliefMentuhotep.ArtTitle.ToString(), ReliefMentuhotep);

        ArtInformation WomenManCasement = new ArtInformation(ART_TITLES.WOMAN_MAN_CASEMENT, "Portrait of a Woman with a Man at a Casement",1,2);
        TemplateArtInformationList.Add(WomenManCasement.ArtTitle.ToString(), WomenManCasement);

        ArtInformation WheatField = new ArtInformation(ART_TITLES.WHEAT_FIELD, "Wheat Field with Cypresses",3,3);
        TemplateArtInformationList.Add(WheatField.ArtTitle.ToString(), WheatField);

        ArtInformation Harvesters = new ArtInformation(ART_TITLES.HARVESTERS, "The Harvesters",5,4);
        TemplateArtInformationList.Add(Harvesters.ArtTitle.ToString(), Harvesters);

        ArtInformation Musicians = new ArtInformation(ART_TITLES.MUSICIANS,"The Musicians",4,3);
        TemplateArtInformationList.Add(Musicians.ArtTitle.ToString(), Musicians);

        ArtInformation WavyVine = new ArtInformation(ART_TITLES.WAVY_VINE, "Fragmentary Loom Width with Wavy-Vine Pattern",3,4);
        TemplateArtInformationList.Add(WavyVine.ArtTitle.ToString(), WavyVine);

        ArtInformation PortBoulogne = new ArtInformation(ART_TITLES.PORT_BOULOGNE,"",2,1);
        TemplateArtInformationList.Add(PortBoulogne.ArtTitle.ToString(), PortBoulogne);

    }

    public void ReceiveDataFromFrontEnd(string DataFromFrontend) {
        Debug.Log("UNITY: Received Data from Front-End ");
        Debug.Log(DataFromFrontend);

        string[] AllArt  = DataFromFrontend.Split(',');

        foreach (string CurrentArtEnum in AllArt) {

            // Find template struct with information via enum name
            ArtInformation RelatedTemplate = TemplateArtInformationList[CurrentArtEnum];
 
            //Use struct to populate MuseumObject
            MuseumObject CurrentMuseumObject = new MuseumObject(
                RelatedTemplate.ArtTitle,
                RelatedTemplate.OfficialTitle,
                RelatedTemplate.Width,
                RelatedTemplate.Height,
                Resources.Load<GameObject>(RelatedTemplate.ResourcesPath)
            );
             MuseumObjectMasterList.Add(CurrentMuseumObject.Title, CurrentMuseumObject);
        }
        UIManager.CreateVerticalInventory();
    }

#endregion

#region Testing Code
    private void FakeReceiveDataFromFrontEnd() {

        string FakeData = "SEASIDE,DRAGONS_FLOWERS,NIGHT_RAIN,VIRGIN_FACING_RIGHT,RELIEF_MENTUHOTEP,WOMAN_MAN_CASEMENT,WHEAT_FIELD,HARVESTERS,MUSICIANS,WAVY_VINE,PORT_BOULOGNE";
        ReceiveDataFromFrontEnd(FakeData);
    }

    //Adds all sample collection items to user's collection
    private void PopulateMasterMuseumObjectsList() {

        //Debug.Log("Populate Master Museum Object List");

        MuseumObject FlowerGirl = new MuseumObject(
            ART_TITLES.FLOWER_GIRL,
            "Flower Girl",
            3,
            3,
            Resources.Load<GameObject>("Prefabs/FG2-Test")
        );
        MuseumObjectMasterList.Add(FlowerGirl.Title, FlowerGirl);


        MuseumObject MadameX = new MuseumObject(
            ART_TITLES.MADAME_X,
            "Madame X (Madame Pierre Gautreau)",
            2,
            4,
            Resources.Load<GameObject>("Prefabs/MadameX-Test")
        );
        MuseumObjectMasterList.Add(MadameX.Title, MadameX);

    }
    private void AddArtToWallAndGrid(ART_TITLES ArtTitle, Vector3 NewWorldPos)
    {

        //Debug.Log("In add art to wall");
        Grid currActiveGrid = GridManager[CurrentActiveWall];
        MuseumObject ToPlace = MuseumObjectMasterList[ArtTitle];
        ToPlace.CurrentWall = OBJECT_PLACEMENT.BACK_WALL;

        //Place in Grid
        int GridX, GridY;
        currActiveGrid.GetGridXYFromWorldPosition(NewWorldPos, out GridX, out GridY);


        //For testing: later instantation of prefab comes from after user has loaded colleciton from db
        ToPlace.PrefabObj = Instantiate(ToPlace.PrefabObj, new Vector3(0f,0f,0f), Quaternion.identity);

        ToPlace.PrefabObj.SetActive(false);

        currActiveGrid.SetCellMuseumObject(GridX, GridY, ToPlace);

        //Gets the current object from the grid
        //MuseumObject Test = currActiveGrid.GetMuseumObjectFromGridCoords(GridX, GridY);

        // Add to World


    }
#endregion

#region Unused/Old Code

    private void RemovePhase(Vector3 MouseWorldPosition)
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectedObj = SelectedObj == null ? CurrActiveGrid.GetMuseumObjectFromWorldPosition(MouseWorldPosition) : SelectedObj;
            if (SelectedObj != null)
            {
                int x, y;
                CurrActiveGrid.GetGridXYFromWorldPosition(MouseWorldPosition, out x, out y);
                xOffset = (int)(x - SelectedObj.origin.x);
                yOffset = (int)(y - SelectedObj.origin.y);
            }

        }
        if (Input.GetMouseButtonUp(0) && SelectedObj != null)
        {
            Vector2 gridOrigin = SelectedObj.origin;
            CurrActiveGrid.RemoveMuseumObjectFromGridCoords((int)gridOrigin.x, (int)gridOrigin.y);
            SelectedObj.CurrentWall = OBJECT_PLACEMENT.INVENTORY;
            SelectedObj = null;
        }
    }

    //TODO Rotate Art Object (later)
    private void RotateCurrentSelectedObject()
    {


    }


#endregion
}


