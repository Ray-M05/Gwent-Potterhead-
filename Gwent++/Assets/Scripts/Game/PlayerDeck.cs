using LogicalSide;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class PlayerDeck : MonoBehaviour
{
    public GameObject prefabCarta; // El prefab gen�rico de la carta
    public GameObject prefabLeader;
    public Transform playerZone; // El lugar donde se colocar� la carta del jugador
    public GameObject PlayerHand;
    public Transform Leaderzone;
    public List<Card> deck; // Tu lista de cartas
    public List<Card> cement;
    public Image Back;
    
    // M�todo para instanciar la �ltima carta del mazo
    public void Instanciate(Card card, Transform zone, GameObject prefab)
    {
        Sprite sp;
        if (card.Faction.faction == 1)
        {
            sp= Resources.Load<Sprite>("gryffreverse");
            Back.sprite = Resources.Load<Sprite>("gryffreverse");
            gameObject.GetComponent<Image>().sprite = sp;
        }
        else
        {
            sp = Resources.Load<Sprite>("slythreverse");
            Back.sprite = Resources.Load<Sprite>("slythreverse");
            gameObject.GetComponent<Image>().sprite = sp;
        }
        if (deck.Count > 0 )
        {
            if ((playerZone.childCount <= 9))
            {
                GameObject instanciaCarta = Instantiate(prefab, zone);
                CardDisplay disp = instanciaCarta.GetComponent<CardDisplay>();
                disp.cardTemplate = card;
                disp.ArtworkImg = instanciaCarta.transform.GetChild(0).GetComponent<Image>();
                disp.Back = instanciaCarta.transform.GetChild(3).GetComponent<Image>();
                disp.DescriptionText = instanciaCarta.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                disp.PwrTxt = instanciaCarta.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                deck.Remove(card);
            }
            
        }
    }
    public void InstanciateLastOnDeck( int n, bool exception)
    {
        if (deck.Count > 0 && n>0)
        {
            Card card = deck[deck.Count - 1];
            if ((playerZone.childCount <= 9 || exception))
            {
                GameObject instanciaCarta = Instantiate(prefabCarta, playerZone);
                CardDisplay disp = instanciaCarta.GetComponent<CardDisplay>();
                disp.cardTemplate = card;
                disp.ArtworkImg = instanciaCarta.transform.GetChild(0).GetComponent<Image>();
                disp.DescriptionText = instanciaCarta.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                disp.PwrTxt = instanciaCarta.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                disp.Back = instanciaCarta.transform.GetChild(3).GetComponent<Image>();
                deck.Remove(card);
                GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
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
            InstanciateLastOnDeck(n - 1, exception);
        }
    }
    public void AddToCement(Card card)
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        cement.Add(card);
    }
    public void GetFromCement()
    {
        if (cement.Count > 0)
        {
            Instanciate(cement[cement.Count-1],PlayerHand.transform,prefabCarta);
        }
        if(cement.Count==0)
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    public void OnClick()
    {
        if(deck.Count > 0)
        InstanciateLastOnDeck(1,false);
    }
    public void Shuffle(List<Card> deck)
    {
        System.Random random = new System.Random();
        Instanciate(deck[0],Leaderzone, prefabLeader);
        if(Leaderzone.name == "LeaderplaceEnemy")
            Leaderzone.transform.GetChild(0).Rotate(0, 0, 180);
        int n = deck.Count;
        while (n > 0)
        {
            n--;
            int k = random.Next(n + 1);
            (deck[n], deck[k]) = (deck[k], deck[n]);
        }
        InstanciateLastOnDeck(10,false);
    }

}
