using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ArtInformation {
    public ART_TITLES ArtTitle;
    public string OfficialTitle;
    public int Width;
    public int Height;
    public string ResourcesPath;

    public ArtInformation(ART_TITLES Title, string OfficialTitle, int Width, int Height) {
        this.ArtTitle = Title;
        this.OfficialTitle = OfficialTitle;
        this.Width = Width;
        this.Height = Height;
        this.ResourcesPath = "Prefabs/"+this.ArtTitle;
    }
}

public struct RoomMeasurements {
    public int backFrontWallLength;
    public int leftRightWallLength;
    public int wallHeight;
    public float poleXZ;
    public int structureDepth;
    public float groundHeight;

    public RoomMeasurements(int backFrontWallLength, int leftRightWallLength, int wallHeight, float poleXZ, float groundHeight)
    {
        this.backFrontWallLength = backFrontWallLength;
        this.leftRightWallLength = leftRightWallLength;
        this.wallHeight = wallHeight;
        this.poleXZ = poleXZ;
        this.groundHeight = groundHeight;
        this.structureDepth = 1;
    }

}

public enum ROOM_SIZE
{
    Null,
    Small,
    Medium,
    Large,
    XLarge,
};

public enum MAIN_MENU_SETTING
{
    NULL,
    ROOM_SIZE,
    WALL_COLOR,
    FLOOR_COLOR,
    CEILING_COLOR,
    ToggleWindows,
}

public enum OBJECT_PLACEMENT
{
    INVENTORY,
    BACK_WALL,
    RIGHT_WALL,
    FRONT_WALL,
    LEFT_WALL,
}

public enum GRID_ORIENTATION
{
    XY,
    YZ,
    XZ
}

public enum ROOM_RESIZE_TYPE
{
    SmallerToBigger,
    LargerToSmaller,
    NewRoomNoPrevSize,
    Null
}

public enum ART_TITLES
{
    NULL,
    FLOWER_GIRL,
    MADAME_X,
    SEASIDE,
    DRAGONS_FLOWERS,
    NIGHT_RAIN,
    VIRGIN_FACING_RIGHT,
    RELIEF_MENTUHOTEP,
    WOMAN_MAN_CASEMENT,
    WHEAT_FIELD,
    HARVESTERS,
    MUSICIANS,
    WAVY_VINE,
    PORT_BOULOGNE
}

public enum MAIN_SCREEN {
    VIEW_SCREEN,
    EDIT_SCREEN
}

public enum EDIT_UI_MODE {
    NULL,
    SAVING,
    SETTINGS,
    PLACING,
    REMOVING
}

public enum ROOM_EDITING_STATE {
    TRANSFORMATION,
    PLACING,
    REMOVING,
    CAMERA_CHANGING,
    IN_UI,
    NULL,
    ROTATING
}

