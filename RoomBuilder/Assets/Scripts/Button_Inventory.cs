using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Inventory : MonoBehaviour
{

    public ART_TITLES AffiliatedArtTitle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetArtTitle(ART_TITLES title) {
        this.AffiliatedArtTitle = title;
    }
}
