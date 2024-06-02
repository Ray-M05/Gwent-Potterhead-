using LogicalSide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class CardDrag : MonoBehaviour
{
    
    private bool IsDragging= false;
    public bool Played= false;
    private Vector2 startPos;
    private GameObject dropzone;
    private List<GameObject> dropzones = new List<GameObject>();
    private Efectos efectos;
    private Card AssociatedCard;
    private GameManager GM;
    void Start()
    // Start is called before the first frame update
    {
        efectos = GameObject.Find("Effects").GetComponent<Efectos>();

        Visualizer = GameObject.Find("Visualizer");
        AssociatedCard = gameObject.GetComponent<CardDisplay>().cardTemplate;
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    #region Drag
    public void StartDrag()
    {
        if (!IsDragging)
        {
            startPos = gameObject.transform.position;
            if (!Played && GM.WhichPlayer(AssociatedCard.DownBoard).SetedUp)
            {
                if (AssociatedCard.DownBoard == GM.Turn)
                {
                    IsDragging = true;
                    BigCardDestroy();
                }
            }
        }
    }
    public void EndDrag()
    {
        if (!Played)
        {
            IsDragging = false;
            dropzone = IsPosible();
            if (dropzone != null)
            {
                if (AssociatedCard.type != "D")
                {
                    if (AssociatedCard.Eff != "Light")
                        transform.SetParent(dropzone.transform, false);
                    if (AssociatedCard.type.IndexOf("C") == -1 && AssociatedCard.type.IndexOf("A") == -1)
                        AssociatedCard.current_Rg = dropzone.tag;
                    else
                    {
                        AssociatedCard.current_Rg = AssociatedCard.Atk_Rg;
                    }
                }
                else
                {
                    //Es un Decoy, regreso la carta a la mano
                    CardDisplay exchange = dropzone.GetComponent<CardDisplay>();
                    AssociatedCard.current_Rg = exchange.cardTemplate.current_Rg;
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
                    efectos.ListEffects[AssociatedCard.Eff].Invoke(AssociatedCard);

                GM.Turn = !GM.Turn;
                if (AssociatedCard.Eff == "Light")
                {
                    PlayerDeck deck = efectos.Decking(AssociatedCard.DownBoard);
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
    private GameObject IsPosible()
    {
        foreach(GameObject drop in dropzones)
        if (AssociatedCard.type.IndexOf("C") == -1)
            if (AssociatedCard.type.IndexOf("A") == -1)
            {
                if (AssociatedCard.type.IndexOf('D') == -1)
                {
                    if (drop.transform.childCount < 6 && AssociatedCard.Atk_Rg.IndexOf(drop.tag) != -1 && efectos.RangeMap[(AssociatedCard.DownBoard, drop.tag)] == drop)
                    {
                        return drop;
                    }
                }
                else
                {
                    if (drop.tag == "Card"&& drop.transform.parent.tag!="P"&& drop.transform.parent.tag != "E")
                        {
                            if(drop.GetComponent<CardDisplay>().cardTemplate.DownBoard== AssociatedCard.DownBoard)
                                return drop;

                        }
                    }
            }
            else
            {
                if (drop.tag == AssociatedCard.type && efectos.RangeMap[(AssociatedCard.DownBoard, drop.tag)] == drop&& drop.transform.childCount<1)
                    return drop;
            }
        else
        {
            if ((drop.transform.childCount < 3 && drop.tag == "C") || (drop.tag != "P" && AssociatedCard.Eff == "Light"))
                return drop;
        }
        return null;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        dropzones.Insert(0,collision.gameObject);
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        dropzones.Remove(collision.gameObject);
    }
    void Update()
    {
        if (IsDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }
    #endregion

    #region BigCard
    public GameObject BigCardPrefab;
    GameObject Big;
    public GameObject Visualizer;

    public Vector3 zoneBig= new Vector3(1800, 300);
    public void BigCardProduce() 
    {
        if(Big!=null)
            BigCardDestroy();
        if (!IsDragging&&( gameObject.tag=="LeaderCard"||(gameObject.tag=="Card" && !gameObject.transform.GetChild(3).gameObject.activeSelf)))
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
        }
    }
    public void BigCardDestroy()
    {
        Destroy(Big);
    }
    #endregion
    public void CardExchange()
    {
        Player P = GM.WhichPlayer(AssociatedCard.DownBoard);
        if (GM.Turn == AssociatedCard.DownBoard) 
        {
            if (!P.SetedUp)
            {
                BigCardDestroy();
                PlayerDeck Deck = efectos.Decking(AssociatedCard.DownBoard);
                Deck.deck.Insert(0,AssociatedCard);
                Deck.InstanciateLastOnDeck(1, true);
                P.cardsExchanged++;
                Destroy(gameObject);
                if (P.cardsExchanged == 2)
                {
                    GM.Teller.text="";
                }
            }
        }
    }
    
}
