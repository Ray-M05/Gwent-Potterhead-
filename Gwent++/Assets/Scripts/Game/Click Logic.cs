using LogicalSide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using Compiler;

public class ClickLogic : MonoBehaviour
{
    public bool InARow=false;
    public bool Played= false;
    private Vector2 startPos;
    public GameObject dropzone;
    private Efectos efectos;
    public UnityCard AssociatedCard;
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

    /// <summary>
    /// This method initializes the card with gameplay information, such as which side (player) it belongs to and its initial zone.
    /// </summary>
    /// <param name="side">Boolean representing which player's side the card belongs to.</param>
    /// <param name="zone">The zone (game board area) where the card will be placed.</param>
    /// <param name="card">The card to be played.</param>
    public void Start2(bool side,GameObject zone, UnityCard card)
    {
        Start();
        InARow = true;
        AssociatedCard.OnConstruction = true;
        dropzone = zone;
        Played = false;
        AssociatedCard = card;
        card.LocationBoard = side;
        card.Owner = GM.GetPlayer(card.LocationBoard);
        card.OnConstruction = false;
        EndClicked();
        InARow = false;
    }

    /// <summary>
    /// This method checks if the card is marked for destruction (i.e., it should be removed from play).
    /// </summary>
    private void Update()
    {
        if (AssociatedCard.Destroy)
        {
            PlayerDeck Current = efectos.Decking(AssociatedCard.LocationBoard);
            efectos.Decoy(AssociatedCard);
            efectos.Restart(AssociatedCard);
            Current.AddToGraveYard(AssociatedCard);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when a card is selected by the player, it checks if the player can interact with the card (based on game rules) and selects it for dragging or playing.
    /// </summary
    public void Selected()
    {
            startPos = transform.position;
            if (GM.GetPlayer(AssociatedCard.LocationBoard).SetedUp)
            {
                if (AssociatedCard.LocationBoard == GM.Turn &&  AssociatedCard.TypeOfCard!= "L")
                {
                    if (pointer.CardSelected != null)
                    {
                        UnityCard card = pointer.CardSelected.GetComponent<ClickLogic>().AssociatedCard;
                        if (card.TypeOfCard.IndexOf("D")!= -1)
                        {
                            pointer.PlayCard(this.gameObject);                                
                        }
                    }
                    if(!Played){
                        GM.Sounds.PlaySoundButton();
                    if (pointer.IsOnDecoy)
                    {
                        pointer.IsOnDecoy = false;
                    }
                    else
                        pointer.CardSelected=this.gameObject;
                }
                    
                }
            }
    }

    /// <summary>
    /// Called when the card is placed (dropped) on the game board. This method verifies the drop location, 
    /// applies relevant effects, and switches the turn if necessary.
    /// </summary>
    public void EndClicked()
    {
        if (!Played)
        {
            dropzone = IsPossible();
            if (dropzone != null)
            {
                if (AssociatedCard.TypeOfCard != "D")
                {
                    if (AssociatedCard.SuperPower != Effect.Cleaner)
                        transform.SetParent(dropzone.transform, false);
                    if (AssociatedCard.TypeOfCard.IndexOf("C") == -1 && AssociatedCard.TypeOfCard.IndexOf("A") == -1)
                        AssociatedCard.CurrentPlace = dropzone.tag;
                    else
                    {
                        AssociatedCard.CurrentPlace = AssociatedCard.Range;
                    }
                }
                else
                {
                    //Its a decoy, returns to hand
                    pointer.IsOnDecoy=true;
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
                if (AssociatedCard.TypeOfCard == "U")
                    efectos.PlayCard(AssociatedCard);
                GM.Sounds.PlaySoundButton();
                if(AssociatedCard.TypeOfCard!="D")
                    efectos.ListEffects[AssociatedCard.SuperPower].Invoke(AssociatedCard);

                if (!(AssociatedCard.Effects == null || AssociatedCard.Effects.Count == 0))
                {
                    try
                    {
                        AssociatedCard.Execute(efectos);
                    }
                    catch (System.Exception ex)
                    {
                        GM.SendMessage("Error en la ejecucion del efecto:");
                        GM.SendMessage(ex.Message);
                    }

                    foreach(CompilingError  error in Errors.List)
                    {
                        GM.SendMessage(error.ToString());
                    }
                }
                if(!InARow)
                GM.Turn = !GM.Turn;
                if (AssociatedCard.SuperPower == Effect.Cleaner)
                {
                    PlayerDeck deck = efectos.Decking(AssociatedCard.LocationBoard);
                    deck.AddToGraveYard(AssociatedCard);
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
        pointer.CardSelected = null;
    }

    /// <summary>
    /// Verifies if the card's drop location is valid based on game rules.
    /// </summary>
    private GameObject IsPossible()
    {
        if (AssociatedCard.TypeOfCard.IndexOf("C") == -1)
            if (AssociatedCard.TypeOfCard.IndexOf("A") == -1)
            {
                if (AssociatedCard.TypeOfCard.IndexOf('D') == -1)
                {
                    if (dropzone.transform.childCount < 6 && AssociatedCard.Range.IndexOf(dropzone.tag) != -1 && efectos.RangeMap[(AssociatedCard.LocationBoard, dropzone.tag)] == dropzone)
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
                if (dropzone.tag == AssociatedCard.TypeOfCard && efectos.RangeMap[(AssociatedCard.LocationBoard, dropzone.tag)] == dropzone&& dropzone.transform.childCount<1)
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

    /// <summary>
    /// Creates a larger, detailed view of the card when hovered or clicked.
    /// </summary>
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
            ZoneDescription.text = card.cardTemplate.Range;
        }
    }

    /// <summary>
    /// Destroys the larger card view.
    /// </summary>
    public void BigCardDestroy()
    {
        Destroy(Big);
        Description.text = "";
        ZoneDescription.text = "";
    }

    /// <summary>
    /// Handles the logic of changing two cards from the deck at start.
    /// </summary>
    public void CardExchange()
    {
        Player P = GM.GetPlayer(AssociatedCard.LocationBoard);
        if (GM.Turn == AssociatedCard.LocationBoard && AssociatedCard.TypeOfCard != "L") 
        {
            if (!P.SetedUp)
            {
                BigCardDestroy();
                PlayerDeck Deck = efectos.Decking(AssociatedCard.LocationBoard);
                Deck.deck.Insert(0,AssociatedCard);
                Deck.GetLastInstance(1, true);
                P.cardsExchanged++;
                Destroy(gameObject);
                
            }
        }
    }
    
}
