using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableObject : MonoBehaviour
{

    float distFromCamera = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDrag()
    {
        if (distFromCamera == 0)
        {
            distFromCamera = Vector3.Distance(Camera.main.transform.position, transform.position);
        }

        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distFromCamera);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //transform.position = objPosition;
    }
}
