using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MuseumObject
{
    #nullable enable
    public GameObject PrefabObj;
    public string OfficialTitle = "";
    public ART_TITLES Title;
    public OBJECT_PLACEMENT CurrentWall;
    public int width;
    public int height;
    public Vector2 origin;
    

    public MuseumObject(
        ART_TITLES Art_Title,
        string officialTitle,
        int width,
        int height,
        GameObject thisPrefab,
        OBJECT_PLACEMENT currWall = OBJECT_PLACEMENT.INVENTORY
        )
    {
        this.Title = Art_Title;
        this.OfficialTitle = officialTitle;
        this.width = width;
        this.height = height;
        this.PrefabObj = thisPrefab;
        this.CurrentWall = currWall;
        this.origin = new Vector2(-1f, -1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override string ToString()
    {
        return Title.ToString();
    }

    public void SetOriginalMaterial() {
        if (PrefabObj == null)
        {
            return;
        }
        Renderer rend = PrefabObj.GetComponentsInChildren<Renderer>()[0];
        Material spriteMaterial = PrefabObj.GetComponentsInChildren<MeshRenderer>()[0].material;
        rend.enabled = true;
        spriteMaterial.SetColor("_Color", Color.white);
    }

    public void SetMovingPosMaterial()
    {
        if (PrefabObj == null)
        {
            return;
        }
        Renderer rend = PrefabObj.GetComponentsInChildren<Renderer>()[0];
        Material spriteMaterial = PrefabObj.GetComponentsInChildren<MeshRenderer>()[0].material;
        rend.enabled = true;
        spriteMaterial.SetColor("_Color", Color.green);
    }

    public void SetIncorrectPosMaterial()
    {
        if (PrefabObj == null)
        {
            return;
        }
        Renderer rend = PrefabObj.GetComponentsInChildren<Renderer>()[0];
        Material spriteMaterial = PrefabObj.GetComponentsInChildren<MeshRenderer>()[0].material;
        rend.enabled = true;
        spriteMaterial.SetColor("_Color", Color.red);
    }

    public void SetDisabledPosMaterial()
    {
        if (PrefabObj == null)
        {
            return;
        }
        Renderer rend = PrefabObj.GetComponentsInChildren<Renderer>()[0];
        Material spriteMaterial = PrefabObj.GetComponentsInChildren<MeshRenderer>()[0].material;
        rend.enabled = true;
        spriteMaterial.SetColor("_Color", Color.gray);
    }


}
