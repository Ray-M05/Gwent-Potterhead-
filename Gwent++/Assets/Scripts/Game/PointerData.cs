using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The PointerData class manages the interaction logic when a card is selected and played in the game.
/// It keeps track of the currently selected card and handles dropping it into a specified dropzone.
/// </summary>
public class PointerData : MonoBehaviour
{
    public GameObject CardSelected= null;
    public bool IsOnDecoy = false;

    /// <summary>
    /// Handles the logic for playing a selected card into the provided dropzone.
    /// If a card is selected, it calls the card's click-and-drop logic to move it to the new dropzone.
    /// </summary>
    /// <param name="dropzone">The GameObject representing the area where the card is to be dropped.</param>
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
}
