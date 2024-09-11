using LogicalSide;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using ListExtensions;
using Compiler;

namespace LogicalSide
{

    public class Efectos : MonoBehaviour, IDeckContext
    {
        public bool Turn{ get; set; }
        public bool DecksInverted = false;

        /// <summary>
        /// Gets the deck of the triggering player. Resets the `DecksInverted` flag to ensure the deck is retrieved in the correct order.
        /// </summary>
        public List<UnityCard> Deck 
        {
            get
            {
                DecksInverted= false;
                return DeckOfPlayer(TriggerPlayer);
            }
        }
        /// <summary>
        /// Gets the deck of the opponent
        /// </summary>
        public List<UnityCard> OtherDeck 
        {
            get
            {
                DecksInverted = true;
                List<UnityCard> l = DeckOfPlayer(TriggerPlayer);
                DecksInverted = false;
                return l;
            }
        }

        /// <summary>
        /// Retrieves the deck of a specified player, determining which deck (player's or opponent's) to fetch based on the `Turn` and `DecksInverted` flags. 
        /// Updates the deck owner and ID based on whether it is the player's or opponent's turn.
        /// </summary>
        public List<UnityCard> DeckOfPlayer(Compiler.Player player)
        {
            PlayerDeck deck;
            if ((player.Turn && !DecksInverted)||(!player.Turn && DecksInverted))
            {
                deck = GameObject.Find("Deck").GetComponent<PlayerDeck>();
                deck.deck.UpdateOwner(true, true);
                deck.deck.UpdateId("Deck");
            }
            else
            {
                deck = GameObject.Find("DeckEnemy").GetComponent<PlayerDeck>();
                deck.deck.UpdateOwner(false, true);
                deck.deck.UpdateId("OtherDeck");
            }
            return deck.deck;
        }

        /// <summary>
        /// Retrieves the graveyard by temporarily inverting the `DecksInverted` flag.
        /// </summary>
        public List<UnityCard> GraveYard 
        {
            get
            {
                DecksInverted = false;
                return GraveYardOfPlayer(TriggerPlayer);
            }
        }

        /// <summary>
        /// Retrieves the opponent's graveyard by temporarily inverting the `DecksInverted` flag.
        /// </summary>
        public List<UnityCard> OtherGraveYard 
        {
            get
            {
                DecksInverted = true;
                List<UnityCard> l = GraveYardOfPlayer(TriggerPlayer);
                DecksInverted = false;
                return l;
            }
        }

        /// <summary>
        /// Retrieves the graveyard of the specified player. Uses the player's turn and `DecksInverted` status to determine which graveyard (player's or opponent's) to access.
        /// Updates the graveyard's owner and ID accordingly.
        /// </summary>
        public List<UnityCard> GraveYardOfPlayer(Compiler.Player player)
        {
            PlayerDeck deck;
            if ((player.Turn && !DecksInverted) || (!player.Turn && DecksInverted))
            {
                
                deck = GameObject.Find("Deck").GetComponent<PlayerDeck>();
                deck.cement.UpdateOwner(true, true);
                deck.cement.UpdateId("GraveYard");
            }
            else
            {
                deck = GameObject.Find("DeckEnemy").GetComponent<PlayerDeck>();
                deck.cement.UpdateOwner(false, true);
                deck.cement.UpdateId("OtherGraveYard");
            }
            return deck.cement;
        }


        /// <summary>
        /// Retrieves the field of the triggering player. Ensures `DecksInverted` is reset before fetching the correct field data.
        /// </summary
        public List<UnityCard> Field 
        {
            get
            {
                DecksInverted = false;
                return FieldOfPlayer(TriggerPlayer);
            }
        }

        /// <summary>
        /// Temporarily inverts the `DecksInverted` flag to access the opponent's field, ensuring the proper field is retrieved.
        /// </summary>
        public List<UnityCard> OtherField 
        {
            get
            {
                DecksInverted = true;
                List<UnityCard> l = FieldOfPlayer(TriggerPlayer);
                DecksInverted = false;
                return l;
            }
        }

        /// <summary>
        /// Retrieves the field for the specified player, dynamically fetching cards from the correct part of the game board based on whether it's the player's or opponent's turn.
        /// The field's owner and ID are also updated accordingly.
        /// </summary>
        public List<UnityCard> FieldOfPlayer(Compiler.Player player)
        {
            List<UnityCard> l= new();
            int count;
            if((player.Turn && !DecksInverted)|| (!player.Turn && DecksInverted))
            {
                count= 0;
                l.UpdateOwner(true, true);
                l.UpdateId("Field");
            }
            else
            {
                count= 6;
                l.UpdateOwner(false, true);
                l.UpdateId("OtherField");
            }
            for(int i = count; i<6+count ; i++)
            {
                for(int j =0; j < BoardOfGameObject[i].transform.childCount; j++)
                {
                    GameObject card = BoardOfGameObject[i].transform.GetChild(j).gameObject;
                    CardDisplay disp= card.GetComponent<CardDisplay>();
                    l.Add(disp.cardTemplate);
                }
            }
            return l;
        }

        /// <summary>
        /// Gets the hand of the triggering player by resetting the `DecksInverted` flag and returning the correct hand.
        /// </summary>
        public List<UnityCard> Hand 
        {
            get
            {
                DecksInverted = false;
                return HandOfPlayer(TriggerPlayer);
            }
        }

        /// <summary>
        /// Gets the hand of the opponent by temporarily inverting the `DecksInverted` flag and returning the correct hand.
        ///  </summary>
        public List<UnityCard> OtherHand 
        {
            get
            {
                DecksInverted = true;
                List<UnityCard> l = HandOfPlayer(TriggerPlayer);
                DecksInverted = false;
                return l;
            }
        }

        /// <summary>
        /// Retrieves the hand for the specified player, determining whether it's the player's or opponent's hand based on the player's turn and `DecksInverted` flag.
        /// </summary>
        public List<UnityCard> HandOfPlayer(Compiler.Player player)
        {
            GameObject Hand;
            List<UnityCard> l = new();
            if((player.Turn && !DecksInverted) || (!player.Turn && DecksInverted))
            {
                Hand= GameObject.Find("Player Hand");
                l.UpdateOwner(true,true);
                l.UpdateId("Hand");
            }
            else
            {
                Hand = GameObject.Find("Enemy Hand");
                l.UpdateOwner(false, true);
                l.UpdateId("OtherHand");
            }
            GameObject obj = null;
            for (int i = 0; i< Hand.transform.childCount; i++)
            {
                obj = Hand.transform.GetChild(i).gameObject;
                CardDisplay disp = obj.GetComponent<CardDisplay>();
                l.Add(disp.cardTemplate);
            }
            return l;
        }

        /// <summary>
        /// Retrieves all the cards currently on the board. Does not differentiate between players; retrieves all cards from all zones on the board.
        /// </summary>
        public List<UnityCard> Board 
        {
            get
            {
                List<UnityCard> l = new();
                l.UpdateOwner(null, true);
                l.UpdateId("Board");
                foreach(GameObject zone in BoardOfGameObject)
                {
                    GameObject obj;
                    for (int i = 0; i < zone.transform.childCount; i++)
                    {
                        obj = zone.transform.GetChild(i).gameObject;
                        CardDisplay disp = obj.GetComponent<CardDisplay>();
                        l.Add(disp.cardTemplate);
                    }
                }
                return l;
            }
        }

        /// <summary>
        /// Determines the player whose turn it is, retrieving the appropriate player from the `GameManager`.
        /// </summary>
        public Compiler.Player TriggerPlayer 
        {
            get
            {
                GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
                return GM.GetPlayer(GM.Turn);
            }
        }

        /// <summary>
        /// Adds a specified card to the field for the given side (player or opponent). Handles special cases for different card types. Ensures the card is placed in a valid location based on game rules.
        /// </summary>
        public bool AddInField(UnityCard card, bool side)
        {
            PlayerDeck Deck = Decking(side);
            GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
            List<GameObject> Targets = new List<GameObject>();
            GameObject Target= null;
            System.Random random = new System.Random();
            //weather
            if (card.TypeOfCard == "C")
            {
                if (C.transform.childCount <= 2 || card.SuperPower == Effect.Cleaner)
                {
                    GameObject Card = Deck.GetInstance(card, null, Deck.prefabCarta);
                    ClickLogic drag = Card.GetComponent<ClickLogic>();
                    drag.Start2( side, C, card);
                    return true;
                }
                else
                {
                    GM.SendMessage("En ejecucion has tratado de agregar un clima al campo, pero ya exist�an 3");
                }
            }

            //Raise
            if (card.TypeOfCard.IndexOf("A") != -1)
            {
                if (RangeMap.ContainsKey((side, card.TypeOfCard)))
                {
                    GameObject zone = RangeMap[(side, card.TypeOfCard)];
                    if (zone.transform.childCount == 0)
                        Target=zone;
                }
                else
                    throw new Exception("Problemas a�adiendo un aumento no justificados");
                

                if (Target!= null)
                {
                    GameObject Card = Deck.GetInstance(card, null, Deck.prefabCarta);
                    ClickLogic drag = Card.GetComponent<ClickLogic>();
                    drag.Start2(side, Target, card);
                    return true;
                }
                else
                    GM.SendMessage($"No hay un lugar disponible para la carta: {card.Name} en el campo de {GM.GetPlayer(side).name}");

            }

            if (card.TypeOfCard == "U")
            {
                for (int i = 0; i < card.Range.Length; i++)
                {
                    string s = card.Range[i].ToString();
                    if (RangeMap.ContainsKey((side, s)))
                    {
                        GameObject zone = RangeMap[(side, s)];
                        if (zone.transform.childCount <= 6)
                            Targets.Add(zone);
                    }
                    else
                        throw new Exception("Problemas anadiendo una carta de unidad no justificados");
                }
                if (Targets.Count > 0)
                {
                    Target = Targets[random.Next(0, Targets.Count)];
                    GameObject Card = Deck.GetInstance(card, null, Deck.prefabCarta);
                    if (Card == null)
                    {
                        Debug.Log("Instancia nula inesperada");
                    }
                    ClickLogic drag = Card.GetComponent<ClickLogic>();
                    
                    drag.Start2(side, Target, card);
                    return true;
                }
                else
                    GM.SendMessage($"No hay un lugar disponible para la carta: {card.Name} en el campo de {GM.GetPlayer(side).name}");

            }
            return false;
        }

        public GameObject P1S;
        public GameObject P1R;
        public GameObject P1M;
        public GameObject P2S;
        public GameObject P2R;
        public GameObject P2M;
        public GameObject P1AM;
        public GameObject P1AR;
        public GameObject P1AS;
        public GameObject P2AM;
        public GameObject P2AR;
        public GameObject P2AS;
        public GameObject C;
        
        public Dictionary<(bool, string), GameObject> RangeMap;
        public Dictionary<Effect, Action<UnityCard>> ListEffects;
        public Dictionary<(bool, string), GameObject> RaiseMap;
        public List<GameObject> BoardOfGameObject;
        private void Start()
        {
            // GameObject references for player zones and central zone
            BoardOfGameObject = new List<GameObject>()
            {
                P1M,
                P1R,
                P1S,
                P1AS,
                P1AR,
                P1AM,
                P2M,
                P2R,
                P2S,
                P2AS,
                P2AR,
                P2AM ,
                C,
            };

            // Mapping zones by player and zone type
            RaiseMap = new Dictionary<(bool, string), GameObject>
            {
                [(true, "S")] = P1AS,
                [(true, "R")] = P1AR,
                [(true, "M")] = P1AM,
                [(false, "S")] = P2AS,
                [(false, "R")] = P2AR,
                [(false, "M")] = P2AM,
            };
            RangeMap = new Dictionary<(bool, string), GameObject>()
            {
                [(true, "M")] = P1M,
                [(true, "R")] = P1R,
                [(true, "S")] = P1S,
                [(true, "AS")] = P1AS,
                [(true, "AR")] = P1AR,
                [(true, "AM")] = P1AM,
                [(false, "M")] = P2M,
                [(false, "R")] = P2R,
                [(false, "S")] = P2S,
                [(false, "AS")] = P2AS,
                [(false, "AR")] = P2AR,
                [(false, "AM")] = P2AM,
            };

        // Dictionary to map effects enums to the corresponding actions
        ListEffects = new Dictionary<Effect, Action<UnityCard>>()
        {
            {Effect.Weather, Weather },
            {Effect.Raise, Raise },
            {Effect.None, None },
            {Effect.MostPwr, MostPwr},
            {Effect.LessPwr, LessPwr},
            {Effect.Multpwr, Multpwr},
            {Effect.ZoneCleaner, ZoneCleaner},
            {Effect.Stealer, Stealer},
            {Effect.Cleaner, Cleaner},
            {Effect.Average, Average},
            {Effect.Decoy, Decoy},
        };
        }
        public void None(UnityCard card)
        {
            return;
        }

        public void Weather(UnityCard card)
        {
            GameObject C = RangeMap[(card.LocationBoard, card.CurrentPlace)];
            C.GetComponent<DropProp>().DropStatus(-1);
            C = RangeMap[(!card.LocationBoard, card.CurrentPlace)];
            C.GetComponent<DropProp>().DropStatus(-1);
            
        }
        public void Raise(UnityCard card)
        {
            GameObject C = RangeMap[(card.LocationBoard, card.CurrentPlace)];
            C.GetComponent<DropProp>().DropStatus(+1);
        }
        public void PlayCard(UnityCard card)
        {
            string rg = card.CurrentPlace;
            int increase = 0;
            GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
            GM.ActualScore(card.LocationBoard, card.Power);
            GameObject C = RangeMap[(card.LocationBoard, card.CurrentPlace)];
            increase = C.GetComponent<DropProp>().weather + C.GetComponent<DropProp>().raised;
            if(card.unit!= KindofCard.Golden)
            card.Power = card.Power + increase;
            
        }
        public void Decoy(UnityCard card)
        {
            if(card.SuperPower == Effect.Weather)
            {
                GameObject C = RangeMap[(card.LocationBoard, card.CurrentPlace)];
                C.GetComponent<DropProp>().DropOnReset(+1);
                C = RangeMap[(!card.LocationBoard, card.CurrentPlace)];
                C.GetComponent<DropProp>().DropOnReset(+1);
            }
            if (card.SuperPower == Effect.Raise)
            {
                GameObject C = RangeMap[(card.LocationBoard, card.CurrentPlace)];
                C.GetComponent<DropProp>().DropOnReset(-1);
                C = RangeMap[(!card.LocationBoard, card.CurrentPlace)];
                C.GetComponent<DropProp>().DropOnReset(-1);
            }
            
        }
        public void MostPwr(UnityCard card)
        {
            GameObject Bigger = null;
            GameObject Var = null;
            UnityCard disp = null;
            UnityCard dispvar = null;
            System.Random random = new();
            foreach (GameObject Gamezone in RangeMap.Values)
            {
                if(Gamezone.tag.IndexOf("A") == -1)
                for (int i = 0; i < Gamezone.transform.childCount; i++)
                {
                    if (Bigger == null)
                    {
                        Var = Gamezone.transform.GetChild(i).gameObject;
                        dispvar = Var.GetComponent<CardDisplay>().cardTemplate;
                        if ((dispvar.TypeOfCard == "U" || dispvar.TypeOfCard == "D") && dispvar.unit == KindofCard.Silver )
                        {
                            Bigger = Gamezone.transform.GetChild(i).gameObject;
                            disp = Bigger.GetComponent<CardDisplay>().cardTemplate;
                        }
                    }
                    else
                    {
                        Var = Gamezone.transform.GetChild(i).gameObject;
                        dispvar = Var.GetComponent<CardDisplay>().cardTemplate;
                        if ((dispvar.TypeOfCard == "U" || dispvar.TypeOfCard == "D") && dispvar.unit == KindofCard.Silver)
                        {
                            if (dispvar.Power > disp.Power)
                            {
                                disp = dispvar;
                                Bigger = Var;
                            }
                            else if (dispvar.Power == disp.Power)
                            {
                                int var = random.Next(0, 1);
                                if (var == 0)
                                {
                                    Bigger = Var;
                                    disp = dispvar;
                                }
                            }
                        }
                    }
                }
            }
            if (Bigger != null && disp != null)
            {
                PlayerDeck Current = Decking(disp.LocationBoard);
                Decoy(disp);
                Restart(disp);
                Current.AddToGraveYard(disp);
                Destroy(Bigger);
            }
        } 
        public void LessPwr(UnityCard card)
        {
            GameObject Bigger = null;
            GameObject Var = null;
            UnityCard disp = null;
            UnityCard dispvar = null;
            System.Random random = new();
            foreach (GameObject Gamezone in RangeMap.Values)
            {
                if(Gamezone.tag.IndexOf("A") == -1)
                for (int i = 0; i < Gamezone.transform.childCount; i++)
                {
                    if (Bigger == null)
                    {
                        Var = Gamezone.transform.GetChild(i).gameObject;
                        dispvar = Var.GetComponent<CardDisplay>().cardTemplate;
                        if ((dispvar.TypeOfCard == "U" || dispvar.TypeOfCard == "D") && dispvar.unit == KindofCard.Silver)
                        {
                            Bigger = Gamezone.transform.GetChild(i).gameObject;
                            disp = Bigger.GetComponent<CardDisplay>().cardTemplate;
                        }
                    }
                    else
                    {
                        Var = Gamezone.transform.GetChild(i).gameObject;
                        dispvar = Var.GetComponent<CardDisplay>().cardTemplate;
                        if ((dispvar.TypeOfCard == "U" || dispvar.TypeOfCard == "D") && dispvar.unit == KindofCard.Silver)
                        {
                            if (dispvar.Power < disp.Power)
                            {
                                disp = dispvar;
                                Bigger = Var;
                            }
                            else if (dispvar.Power == disp.Power)
                            {
                                int var = random.Next(0, 1);
                                if (var == 0)
                                {
                                    Bigger = Var;
                                    disp = dispvar;
                                }
                            }
                        }
                    }
                }
            }
            if (Bigger != null && disp != null)
            {
                PlayerDeck Current = Decking(disp.LocationBoard);
                Decoy(disp);
                Restart(disp);
                Current.AddToGraveYard(disp);
                Destroy(Bigger);
            }
        }
        public void Average(UnityCard card)
        {
            int totalPwr = 0;
            int cant = 0;
            UnityCard dispvar;
            foreach (GameObject Gamezone in RangeMap.Values)
            {
                if(Gamezone.tag.IndexOf("A") == -1)
                for (int i = 0; i < Gamezone.transform.childCount; i++)
                {
                    dispvar = Gamezone.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().cardTemplate;
                    if (dispvar != null&& (dispvar.TypeOfCard=="U" || dispvar.TypeOfCard == "D"))
                    {
                        totalPwr += dispvar.Power;
                        cant++;
                    }
                }
            }
            int media= totalPwr/cant;
            foreach (GameObject Gamezone in RangeMap.Values)
            {
                if(Gamezone.tag.IndexOf("A") == -1)
                for (int i = 0; i < Gamezone.transform.childCount; i++)
                {
                    dispvar = Gamezone.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().cardTemplate;
                    if (dispvar != null && (dispvar.TypeOfCard == "U" || dispvar.TypeOfCard == "D") && dispvar.unit== KindofCard.Silver)
                    {
                        dispvar.Power = media;
                    }
                }
            }
        }
        public void Stealer(UnityCard card)
        {
            Decking(card.Owner.Turn).GetLastInstance(1,false);
        }
        public void Multpwr(UnityCard card)
        {
            int increase = 0;
            UnityCard dispvar;
            foreach (GameObject Gamezone in RangeMap.Values)
            {
                for (int i = 0; i < Gamezone.transform.childCount; i++)
                {
                    dispvar = Gamezone.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().cardTemplate;
                    if (dispvar != null && dispvar.Name == card.Name)
                    {
                        increase++;
                    }
                }
            }
            card.Power += increase;
        }
        public void ZoneCleanerMax(UnityCard card)
        {
            GameObject Me;
            int childs = 0;
            GameObject Target = null;
            UnityCard dispvar = null;

            System.Random random = new();
            foreach (GameObject Gamezone in RangeMap.Values)
            {
                if (Gamezone.transform.childCount > childs && Gamezone.tag.IndexOf("A")==-1)
                {
                    childs = Gamezone.transform.childCount;
                    Target = Gamezone;
                }

            }
            if (Target != null && childs > 0)
            {
                for (int i = 0; i < Target.transform.childCount; i++)
                {
                    Me = Target.transform.GetChild(i).gameObject;
                    dispvar = Me.GetComponent<CardDisplay>().cardTemplate;
                    if ( dispvar.unit!= KindofCard.Golden)
                    {
                        PlayerDeck Current = Decking(dispvar.LocationBoard);
                        Decoy(dispvar);
                        Restart(dispvar);
                        Current.AddToGraveYard(dispvar);
                        Destroy(Me);
                    }
                }

            }
        }
        public void ZoneCleaner(UnityCard card)
        {
            GameObject Me;
            int childs = int.MaxValue;
            GameObject Target = null;
            UnityCard dispvar = null;

            System.Random random = new();
            foreach (GameObject Gamezone in RangeMap.Values)
            {
                if (Gamezone.transform.childCount!=0 && Gamezone.transform.childCount < childs && Gamezone.tag.IndexOf("A") == -1)
                {
                    childs = Gamezone.transform.childCount;
                    Target = Gamezone;
                }
            }
            if (Target != null && childs > 0)
            {
                for (int i = 0; i < Target.transform.childCount; i++)
                {
                    Me = Target.transform.GetChild(i).gameObject;
                    dispvar = Me.GetComponent<CardDisplay>().cardTemplate;
                    if (dispvar.unit != KindofCard.Golden)
                    {
                        PlayerDeck Current = Decking(dispvar.LocationBoard);
                        Decoy(dispvar);
                        Restart(dispvar);
                        Current.AddToGraveYard(dispvar);
                        Destroy(Me);
                    }
                }

            }
        }
        public void Cleaner(UnityCard card)
        {
            foreach (GameObject Gamezone in RangeMap.Values)
            {
                DropProp props = Gamezone.GetComponent<DropProp>();
                if (props != null)
                {
                    for (int i = 0; i < Gamezone.transform.childCount; i++)
                    {
                        UnityCard disp = Gamezone.transform.GetChild(i).GetComponent<CardDisplay>().cardTemplate;
                        if (disp.unit == KindofCard.Silver&& disp.TypeOfCard!="D")
                            disp.Power -= props.weather;
                    }
                    props.weather = 0;
                }
            }
            for (int i = 0; i < C.transform.childCount; i++)
            {
                GameObject weather = C.transform.GetChild(i).gameObject;
                UnityCard disp = C.transform.GetChild(i).GetComponent<CardDisplay>().cardTemplate;
                PlayerDeck Current = Decking(weather);
                Current.AddToGraveYard(disp);
                Destroy(weather);
            }
        }

        /// <summary>
        /// Restarts the card's attributes to their original values and moves it to the appropriate hand if necessary.
        /// </summary>
        public void RestartCard(GameObject Card, GameObject Place, bool home)
        {
            UnityCard card = Card.GetComponent<CardDisplay>().cardTemplate;
            Restart(card);
            if (home)
            {
                Card.GetComponent<ClickLogic>().Played = false;
                GameObject Hand;
                if (card.LocationBoard)
                    Hand = GameObject.FindWithTag("P");
                else
                    Hand = GameObject.FindWithTag("E");
                Card.transform.SetParent(Hand.transform, false);
            }
        }

        /// <summary>
        /// Returns the appropriate deck based on the player.
        /// </summary>
        public PlayerDeck Decking(bool Location)
        {
            if (Location)
                return GameObject.Find("Deck").GetComponent<PlayerDeck>();
            else
                return GameObject.Find("DeckEnemy").GetComponent<PlayerDeck>();
        }

        /// <summary>
        /// Resets a card's power and current place to its original state.
        /// </summary>
        public void Restart(UnityCard card)
        {
            card.Power = 0;
            card.CurrentPlace = "";
            card.Power = card.OriginalPoints;
        }

        /// <summary>
        /// Moves all cards from the board to the players' graveyards and resets game zones.
        /// </summary>
        public void ToCementery()
        {
            PlayerDeck DeckP = GameObject.Find("Deck").GetComponent<PlayerDeck>();
            PlayerDeck DeckE = GameObject.Find("DeckEnemy").GetComponent<PlayerDeck>();
            PlayerDeck Current;
            GameObject card;
            List<UnityCard> Permanents = new List<UnityCard>();
            foreach (GameObject GameZone in RangeMap.Values)
            {
                DropProp drop = GameZone.GetComponent<DropProp>();
                if (drop != null)
                {
                    drop.weather = 0;
                    drop.raised = 0;
                }
            }
            foreach (GameObject C in RangeMap.Values)
            {
                if (C == P1M || C == P1R || C == P1S || C == P1AM || C == P1AR || C == P1AS)
                    Current = DeckP;
                else
                    Current = DeckE;

                for (int i = 0; i < C.transform.childCount; i++)
                {
                    card = C.transform.GetChild(i).gameObject;
                    CardDisplay disp = card.GetComponent<CardDisplay>();
                    if (disp != null)
                    {
                        if (disp.cardTemplate.TypeOfCard == "U")
                            Restart(disp.cardTemplate);
                        
                            Current.AddToGraveYard(disp.cardTemplate);
                            Destroy(card);
                        
                    }
                }
            }

            for (int i = 0; i < C.transform.childCount; i++)
            {
                card = C.transform.GetChild(i).gameObject;
                CardDisplay disp = card.GetComponent<CardDisplay>();
                Current = Decking(disp.cardTemplate.LocationBoard);
                if (disp != null)
                {
                    Restart(disp.cardTemplate);
                    Current.AddToGraveYard(disp.cardTemplate);
                    Destroy(card);
                }
            }
            foreach (UnityCard disp in Permanents)
            {
                int increase = 0;
                disp.Power= disp.OriginalPoints;
                GameObject C = RangeMap[(disp.LocationBoard, disp.CurrentPlace)];
                increase = C.GetComponent<DropProp>().weather + C.GetComponent<DropProp>().raised;
                if (disp.unit != KindofCard.Golden)
                    disp.Power = disp.Power + increase;
                if(disp.SuperPower== Effect.Weather|| disp.SuperPower== Effect.Raise|| disp.SuperPower == Effect.Multpwr)
                    ListEffects[disp.SuperPower](disp);
            }
        }
        
    }
}
