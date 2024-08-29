using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerData : MonoBehaviour
{
    public GameObject CardSelected= null;
    public bool IsOnDecoy = false;
    public void PlayCard(GameObject dropzone)
    {
        if(CardSelected != null)
        {
            ClickLogic drag= CardSelected.GetComponent<ClickLogic>();
            drag.dropzone = dropzone;
            drag.EndClicked();
            CardSelected=null;
        }
        else
        Debug.Log("No hay Carta seleccionada");
    }
    // Start is called before the first frame update
}
