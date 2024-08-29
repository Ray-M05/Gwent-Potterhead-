using System.Collections;
using System.Collections.Generic;
using LogicalSide;
using TMPro;
using Compiler;
using UnityEngine;
using Unity.VisualScripting;

public class Compilation : MonoBehaviour
{
    public GameObject LittleErrors;
    public TextMeshProUGUI Console;
    [SerializeField] public List<UnityCard> CardsPlayer1;
    [SerializeField] public List<UnityCard> CardsPlayer2;

    public string FilePath= "C:/Pro/Gwent-Potterhead-/Gwent++/Assets/Scripts/Compilation/Tools/Example.txt";

    private void Start()
    {
        SavedData Data= GameObject.Find("SoundManager").GetComponent<SavedData>();
        Data.Compi = this;
    }


    public void PathUpdate(GameObject s)
    {
        FilePath = s.GetComponent<TMPro.TMP_InputField>().text;
    }

    public void CloseConsole()
    {
        LittleErrors.SetActive(false);
        Console.text = "";
    }

    public int Player;

    public void SetPlayer(int player)
    {
        Player = player;
    }
    public void CompilerPortal()
    {
        List<UnityCard> cards = new List<UnityCard>();
        List<Card> cardscomp = new List<Card>();

        try
        {
            cardscomp = Compilator.GetCards(FilePath);
            if (Errors.List.Count > 0)
            {
                foreach (var item in Errors.List)
                {
                    Console.text += item.ToString() + "\n";
                }
                LittleErrors.SetActive(true);
            }
            if (Errors.List.Count == 0)
            {
                Console.text = "Compilacion exitosa" + "\n" + "Las cartas compiladas se mostraran de color azul dentro del juego";
                LittleErrors.SetActive(true);
            }
            Errors.List.Clear();
            bool b;
            if (Player == 1)
            {
                b = true;
                CardsPlayer1 = cards;
            }
            else
            {
                b = false;
                CardsPlayer2 = cards;
            }
            int cantLiders = 0;

            foreach (var card in cardscomp)
            {
                if (card.Type == "Lider")
                {
                    cantLiders++;
                    if (cantLiders > 1)
                        throw new System.Exception("Has declarado al menos dos cartas lider");
                }
                cards.Add(GenerateCard(card, b));
            }
        }
        catch (System.Exception e)
        {
            LittleErrors.SetActive(true);
            Console.text = e.Message;
        }


        Errors.List.Clear();
        Processor.ParamsRequiered.Clear();
        Processor.Effects.Clear();
    }

    private UnityCard GenerateCard(Card card, bool DownBoard)
    {
        KindofCard unit = KindofCard.None;
        Effect eff = Effect.None;
        string image = null;
        string Type="";

        switch (card.Type)
        {
            case "Clima":
                Type = "C";
                eff = Effect.Weather;
                image = "Weather";
                if (card.Range.Length > 1)
                    throw new System.Exception("Los climas solo pueden afectar un rango");
                break;
            case "Plata":
                unit = KindofCard.Silver;
                Type = "U";
                image = "Unit";
                break;
            case "Oro":
                unit = KindofCard.Golden;
                Type = "U";
                image = "Gold unit";
                break;
            case "Aumento":
                unit = KindofCard.None;
                if(card.Range != null || card.Range != "")
                foreach(char c in card.Range)
                {
                    Type += "A"+c;
                }
                eff = Effect.Raise;
                image = "Increasement";
                if (card.Range.Length > 1)
                    throw new System.Exception("Los aumentos solo pueden afectar un rango");
                break;
            case "Lider":
                Type = "L";
                image = "Ravenclaw leader";
                break;
            case "Despeje":
                Type = "C";
                eff = Effect.Cleaner;
                image = "Clearance";
                break;
            case "Senuelo":
                Type = "D";
                eff = Effect.Decoy;
                image = "Clearance";
                break;
            default:
                throw new System.Exception($"Tipo no valido: {card.Type}.");
        }

        string range = "";
        if( card.Type =="Lider" || card.Type == "Despeje" || card.Type == "Senuelo")
        {
            if(!(card.Range == null || card.Range== ""))
            throw new System.Exception("No se puede crear una carta lider/despeje/senuelo que tenga Rangos");
        }
        else
        {
            if (card.Range == null || card.Range == "")
            throw new System.Exception("No se puede crear una carta de unidad o especial sin rango");
            else
            {
                range = card.Range;
            }
        }
        if( card.Type == "Lider" || card.Type == "Clima" || card.Type == "Aumento" || card.Type == "Despeje" || card.Type == "Senuelo")
        {
            if (card.Power != null && card.Power != 0 )
                throw new System.Exception("Las cartas especiales no pueden tener poder");
            else
                card.Power = 0;
        }


        UnityCard UnityCard = new(DownBoard, card.Name, card.Type, card.Power, null, unit, Type, eff, range, Resources.Load<Sprite>(image), $"Carta de tipo {card.Type} Compilada");
        UnityCard.Effects= card.Effects;
        UnityCard.OnConstruction = true;
        UnityCard.Faction= card.Faction;
        UnityCard.OnConstruction = false;
        return UnityCard;
    }

}
