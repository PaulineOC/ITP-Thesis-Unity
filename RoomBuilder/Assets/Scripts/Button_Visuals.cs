using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button_Visuals : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Color32 DefaultTextColor = new Color32(50, 50, 50, 255);

    Button Button;
    Text ButtonText;
    // Start is called before the first frame update
    void Start()
    {

        Button = GetComponent<Button>();
        ButtonText = Button.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData) {

        if (Button.IsInteractable()) {
            //Debug.Log("Pressing Button");

            if (ButtonText)
            {
                ButtonText.color = Color.white;
            }
        }


    }

    public void OnPointerUp(PointerEventData eventData)
    {

        if (Button.IsInteractable() && ButtonText)
        {
            //Debug.Log("Releasing Button");
            ButtonText.color = DefaultTextColor;

        }


    }

}
