using LogicalSide;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.XR;

public class PlayerDeck : MonoBehaviour
{
    public GameObject prefabCarta; 
    public GameObject prefabLeader;
    public Transform playerZone; 
    public GameObject PlayerHand;
    public Transform Leaderzone;
    [SerializeField]public List<UnityCard> deck; 
    [SerializeField]public List<UnityCard> cement;
    public Image Back;

    public void Start()
    {
        cement= new();
    }

    /// <summary>
    /// Sets the sprite of the card back image.
    /// </summary>
    public void SetSprite(Sprite sprite)
    {
        gameObject.GetComponent<Image>().sprite=  sprite;
    }


        /// <summary>
    /// Instantiates a card object from a prefab, associates it with a UnityCard object, and places it in a specified zone.
    /// </summary>
    /// <param name="card">The UnityCard object to represent.</param>
    /// <param name="zone">The transform of the zone where the card will be placed.</param>
    /// <param name="prefab">The prefab to instantiate the card from.</param>
    /// <returns>The instantiated card GameObject.</returns>
    public GameObject GetInstance(UnityCard card, Transform zone, GameObject prefab)
    {
            GameObject instanciaCarta = Instantiate(prefab, zone);
            CardDisplay disp = instanciaCarta.GetComponent<CardDisplay>();
            disp.cardTemplate = card;
            disp.ArtworkImg = instanciaCarta.transform.GetChild(0).GetComponent<Image>();
            disp.Back = instanciaCarta.transform.GetChild(3).GetComponent<Image>();
            disp.DescriptionText = instanciaCarta.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            disp.PwrTxt = instanciaCarta.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            if (prefab == prefabCarta && PlayerHand.tag.IndexOf("DE") != -1)
            {
                instanciaCarta.transform.Rotate(0, 0, 180);
            }
            return instanciaCarta;
    }

    /// <summary>
    /// Draws and places the last card from the deck into the player's hand. 
    /// </summary>
    /// <param name="n">The number of cards to draw.</param>
    public void GetLastInstance( int n, bool exception)
    {
        if (deck.Count > 0 && n>0)
        {
            UnityCard card = deck[deck.Count - 1];
            if ((playerZone.childCount <= 9 || exception))
            {
                GameObject instanciateCard = Instantiate(prefabCarta, playerZone);
                CardDisplay disp = instanciateCard.GetComponent<CardDisplay>();
                disp.cardTemplate = card;
                disp.ArtworkImg = instanciateCard.transform.GetChild(0).GetComponent<Image>();
                disp.DescriptionText = instanciateCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                disp.PwrTxt = instanciateCard.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                disp.Back = instanciateCard.transform.GetChild(3).GetComponent<Image>();
                deck.RemoveAt(deck.Count-1);
                if (PlayerHand.tag.IndexOf("DE") != -1)
                {
                    playerZone.GetChild(playerZone.childCount - 1).Rotate(0, 0, 180);
                }
            }
            else
            {
                deck.Remove(card);
                cement.Add(card);
            }
            // Recursively call to draw multiple cards
            GetLastInstance(n - 1, exception);
        }
    }

    /// <summary>
    /// Adds a card to the player's graveyard (cement).
    /// </summary>
    public void AddToGraveYard(UnityCard card)
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(0).GetComponent<Image>().sprite= gameObject.GetComponent<Image>().sprite;
        cement.Add(card);
    }

    /// <summary>
    /// Retrieves a card from the graveyard.
    /// </summary>
    public void GetFromGraveYard()
    {
        if (cement.Count > 0)
        {
            GetInstance(cement[cement.Count-1],PlayerHand.transform,prefabCarta);
        }
        if(cement.Count==0)
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    /// <summary>
    /// Handles the event of clicking the deck to draw a card.
    /// </summary>
    public void OnClick()
    {
        if(deck.Count > 0)
        GetLastInstance(1,false);
    }

    /// <summary>
    /// Shuffles the deck and places a specified number of cards from the compilation on top.
    /// Also instantiates a leader card.
    /// </summary>
    /// <param name="deck">The deck to shuffle.</param>
    /// <param name="compiler">The number of cards from the compilation to remain on top.</param>
    public void ShuffleCompilationOnTop(List<UnityCard> deck, int compiler=0)
    {
        System.Random random = new System.Random();
        GetInstance(deck[0],Leaderzone, prefabLeader);
        deck.RemoveAt(0);
        if(Leaderzone.name == "LeaderplaceEnemy")
            Leaderzone.transform.GetChild(0).Rotate(0, 0, 180);
        int n = deck.Count - compiler;
        while (n > 0)
        {
            n--;
            int k = random.Next(n + 1);
            (deck[n], deck[k]) = (deck[k], deck[n]);
        }
        GetLastInstance(10,false);
    }

}
