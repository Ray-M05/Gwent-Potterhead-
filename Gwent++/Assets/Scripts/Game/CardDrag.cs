using LogicalSide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class CardDrag : MonoBehaviour
{
    
    public bool Played= false;
    private Vector2 startPos;
    public GameObject dropzone;
    private Efectos efectos;
    public Card AssociatedCard;
    private GameManager GM;
    PointerData pointer;
    void Start()
    // Start is called before the first frame update
    {
        efectos = GameObject.Find("Effects").GetComponent<Efectos>();

        Visualizer = GameObject.Find("Visualizer");
        AssociatedCard = gameObject.GetComponent<CardDisplay>().cardTemplate;
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        Description = GameObject.Find("Description").GetComponent<TextMeshProUGUI>();
        ZoneDescription = GameObject.Find("ZoneDescription").GetComponent<TextMeshProUGUI>();
        pointer = GameObject.Find("GameManager").GetComponent<PointerData>();
    }

    public void Selected()
    {
            startPos = transform.position;
            if (GM.WhichPlayer(AssociatedCard.LocationBoard).SetedUp)
            {
                if (AssociatedCard.LocationBoard == GM.Turn)
                {
                    if (pointer.CardSelected != null)
                    {
                        Card card = pointer.CardSelected.GetComponent<CardDrag>().AssociatedCard;
                        if (card.type.IndexOf("D")!= -1)
                        {
                            pointer.PlayCard(this.gameObject);                                
                        }
                    }
                    if(!Played){
                        GM.Sounds.PlaySoundButton();
                        pointer.CardSelected=this.gameObject;
                        }
                    
                }
            }
    }
    public void EndClicked()
    {
        if (!Played)
        {
            dropzone = IsPossible();
            if (dropzone != null)
            {
                if (AssociatedCard.type != "D")
                {
                    if (AssociatedCard.SuperPower != Effect.Cleaner)
                        transform.SetParent(dropzone.transform, false);
                    if (AssociatedCard.type.IndexOf("C") == -1 && AssociatedCard.type.IndexOf("A") == -1)
                        AssociatedCard.CurrentPlace = dropzone.tag;
                    else
                    {
                        AssociatedCard.CurrentPlace = AssociatedCard.AttackPlace;
                    }
                }
                else
                {
                    //Es un Decoy, regreso la carta a la mano
                    CardDisplay exchange = dropzone.GetComponent<CardDisplay>();
                    AssociatedCard.CurrentPlace = exchange.cardTemplate.CurrentPlace;
                    Transform drop = dropzone.transform.parent;
                    transform.SetParent(drop.transform, false);
                    efectos.Decoy(exchange.cardTemplate);
                    efectos.RestartCard(dropzone, null, true);
                }
                Played = true;
                if (GM.Turn)
                    GM.P1.Surrender = false;
                else
                    GM.P2.Surrender = false;
                if (AssociatedCard.type == "U")
                    efectos.PlayCard(AssociatedCard);
                GM.Sounds.PlaySoundButton();
                if(AssociatedCard.type!="D")
                    efectos.ListEffects[AssociatedCard.SuperPower].Invoke(AssociatedCard);

                GM.Turn = !GM.Turn;
                if (AssociatedCard.SuperPower == Effect.Cleaner)
                {
                    PlayerDeck deck = efectos.Decking(AssociatedCard.LocationBoard);
                    deck.AddToCement(AssociatedCard);
                    Destroy(gameObject);
                }
                
            }
        }
        if (!Played)
        {
            transform.position = startPos;
            dropzone = null;
            GM.Sounds.PlayError();
        }
    }
    private GameObject IsPossible()
    {
        if (AssociatedCard.type.IndexOf("C") == -1)
            if (AssociatedCard.type.IndexOf("A") == -1)
            {
                if (AssociatedCard.type.IndexOf('D') == -1)
                {
                    if (dropzone.transform.childCount < 6 && AssociatedCard.AttackPlace.IndexOf(dropzone.tag) != -1 && efectos.RangeMap[(AssociatedCard.LocationBoard, dropzone.tag)] == dropzone)
                    {
                        return dropzone;
                    }
                }
                else
                {
                    if (dropzone.tag == "Card"&& dropzone.transform.parent.tag!="P"&& dropzone.transform.parent.tag != "E")
                        {
                            if(dropzone.GetComponent<CardDisplay>().cardTemplate.LocationBoard== AssociatedCard.LocationBoard)
                                return dropzone;

                        }
                    }
            }
            else
            {
                if (dropzone.tag == AssociatedCard.type && efectos.RangeMap[(AssociatedCard.LocationBoard, dropzone.tag)] == dropzone&& dropzone.transform.childCount<1)
                    return dropzone;
            }
        else
        {
            if ((dropzone.transform.childCount < 3 && dropzone.tag == "C") || (dropzone.tag != "P" && AssociatedCard.SuperPower == Effect.Cleaner))
                return dropzone;
        }
        return null;
    }

    public GameObject BigCardPrefab;
    GameObject Big;
    public GameObject Visualizer;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI ZoneDescription;

    public Vector3 zoneBig= new Vector3(1800, 300);
    public void BigCardProduce() 
    {
        if(Big!=null)
            BigCardDestroy();
        if (( gameObject.tag=="LeaderCard"||(gameObject.tag=="Card" && !gameObject.transform.GetChild(3).gameObject.activeSelf)))
        {
            CardDisplay card = gameObject.GetComponent<CardDisplay>();
            Big = Instantiate(BigCardPrefab, zoneBig, Quaternion.identity);
            Big.transform.SetParent(Visualizer.transform, worldPositionStays: true);
            Big.transform.position = zoneBig;
            CardDisplay disp = Big.GetComponent<CardDisplay>();
            disp.cardTemplate = card.cardTemplate;
            disp.ArtworkImg = Big.transform.GetChild(0).GetComponent<Image>();
            if (disp.ArtworkImg != null)
                disp.DescriptionText = Big.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            disp.PwrTxt = Big.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            Description.text = card.cardTemplate.description;
            ZoneDescription.text = card.cardTemplate.AttackPlace;
        }
    }
    public void BigCardDestroy()
    {
        Destroy(Big);
        Description.text = "";
        ZoneDescription.text = "";
    }
    public void CardExchange()
    {
        Player P = GM.WhichPlayer(AssociatedCard.LocationBoard);
        if (GM.Turn == AssociatedCard.LocationBoard) 
        {
            if (!P.SetedUp)
            {
                BigCardDestroy();
                PlayerDeck Deck = efectos.Decking(AssociatedCard.LocationBoard);
                Deck.deck.Insert(0,AssociatedCard);
                Deck.InstanciateLastOnDeck(1, true);
                P.cardsExchanged++;
                Destroy(gameObject);
                
            }
        }
    }
    
}
