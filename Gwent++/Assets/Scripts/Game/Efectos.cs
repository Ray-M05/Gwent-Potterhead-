using LogicalSide;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

namespace LogicalSide
{
    public class Efectos : MonoBehaviour
    {

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

        public List<GameObject>[] Board;
        public Dictionary<(bool, string), GameObject> RangeMap;
        public Dictionary<Effect, Action<Card>> ListEffects;
        public Dictionary<(bool, string), GameObject> RaiseMap;
        private void Start()
        {
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
            ListEffects = new Dictionary<Effect, Action<Card>>()
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
        public void None(Card card)
        {
            return;
        }

        public void Weather(Card card)
        {
            GameObject C = RangeMap[(card.LocationBoard, card.CurrentPlace)];
            C.GetComponent<DropProp>().DropStatus(-1);
            C = RangeMap[(!card.LocationBoard, card.CurrentPlace)];
            C.GetComponent<DropProp>().DropStatus(-1);
            
        }
        public void Raise(Card card)
        {
            GameObject C = RangeMap[(card.LocationBoard, card.CurrentPlace)];
            C.GetComponent<DropProp>().DropStatus(+1);
        }
        public void PlayCard(Card card)
        {
            string rg = card.CurrentPlace;
            int increase = 0;
            GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
            GM.AddScore(card.LocationBoard, card.Points);
            GameObject C = RangeMap[(card.LocationBoard, card.CurrentPlace)];
            increase = C.GetComponent<DropProp>().weather + C.GetComponent<DropProp>().raised;
            if(card.unit!= KindofCard.Golden)
            card.Points = card.Points + increase;
            
        }
        public void Decoy(Card card)
        {//Este efecto realmente establece las dropzones
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
        public void MostPwr(Card card)
        {
            GameObject Bigger = null;
            GameObject Var = null;
            Card disp = null;
            Card dispvar = null;
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
                        if ((dispvar.type == "U" || dispvar.type == "D") && dispvar.unit == KindofCard.Silver )
                        {
                            Bigger = Gamezone.transform.GetChild(i).gameObject;
                            disp = Bigger.GetComponent<CardDisplay>().cardTemplate;
                        }
                    }
                    else
                    {
                        Var = Gamezone.transform.GetChild(i).gameObject;
                        dispvar = Var.GetComponent<CardDisplay>().cardTemplate;
                        if ((dispvar.type == "U" || dispvar.type == "D") && dispvar.unit == KindofCard.Silver)
                        {
                            if (dispvar.Points > disp.Points)
                            {
                                disp = dispvar;
                                Bigger = Var;
                            }
                            else if (dispvar.Points == disp.Points)
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
                Current.AddToCement(disp);
                Destroy(Bigger);
            }
        } 
        public void LessPwr(Card card)
        {
            GameObject Bigger = null;
            GameObject Var = null;
            Card disp = null;
            Card dispvar = null;
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
                        if ((dispvar.type == "U" || dispvar.type == "D") && dispvar.unit == KindofCard.Silver)
                        {
                            Bigger = Gamezone.transform.GetChild(i).gameObject;
                            disp = Bigger.GetComponent<CardDisplay>().cardTemplate;
                        }
                    }
                    else
                    {
                        Var = Gamezone.transform.GetChild(i).gameObject;
                        dispvar = Var.GetComponent<CardDisplay>().cardTemplate;
                        if ((dispvar.type == "U" || dispvar.type == "D") && dispvar.unit == KindofCard.Silver)
                        {
                            if (dispvar.Points < disp.Points)
                            {
                                disp = dispvar;
                                Bigger = Var;
                            }
                            else if (dispvar.Points == disp.Points)
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
                Current.AddToCement(disp);
                Destroy(Bigger);
            }
        }
        public void Average(Card card)
        {
            int totalPwr = 0;
            int cant = 0;
            Card dispvar;
            foreach (GameObject Gamezone in RangeMap.Values)
            {
                if(Gamezone.tag.IndexOf("A") == -1)
                for (int i = 0; i < Gamezone.transform.childCount; i++)
                {
                    dispvar = Gamezone.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().cardTemplate;
                    if (dispvar != null&& (dispvar.type=="U" || dispvar.type == "D"))
                    {
                        totalPwr += dispvar.Points;
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
                    if (dispvar != null && (dispvar.type == "U" || dispvar.type == "D") && dispvar.unit== KindofCard.Silver)
                    {
                        dispvar.Points = media;
                    }
                }
            }
        }
        public void Stealer(Card card)
        {
            Decking(card).InstanciateLastOnDeck(1,false);
        }
        public void Multpwr(Card card)
        {
            int increase = 0;
            Card dispvar;
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
            card.Points += increase;
        }
        public void ZoneCleanerMax(Card card)
        {//Este efecto es la misma idea del expuesto en el pdf, solo me parecio mejor eliminar la zona mas poblada, en caso de que quieran probar su funcionamiento para el otro caso basta cambiar el signo > por <
            GameObject Me;
            int childs = 0;
            GameObject Target = null;
            Card dispvar = null;

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
                        Current.AddToCement(dispvar);
                        Destroy(Me);
                    }
                }

            }
        }
        public void ZoneCleaner(Card card)
        {
            GameObject Me;
            int childs = int.MaxValue;
            GameObject Target = null;
            Card dispvar = null;

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
                        Current.AddToCement(dispvar);
                        Destroy(Me);
                    }
                }

            }
        }
        public void Cleaner(Card card)
        {
            foreach (GameObject Gamezone in RangeMap.Values)
            {
                DropProp props = Gamezone.GetComponent<DropProp>();
                if (props != null)
                {
                    for (int i = 0; i < Gamezone.transform.childCount; i++)
                    {
                        Card disp = Gamezone.transform.GetChild(i).GetComponent<CardDisplay>().cardTemplate;
                        if (disp.unit == KindofCard.Silver&& disp.type!="D")
                            disp.Points -= props.weather;
                    }
                    props.weather = 0;
                }
            }
            for (int i = 0; i < C.transform.childCount; i++)
            {
                GameObject weather = C.transform.GetChild(i).gameObject;
                Card disp = C.transform.GetChild(i).GetComponent<CardDisplay>().cardTemplate;
                PlayerDeck Current = Decking(weather);
                Current.AddToCement(disp);
                Destroy(weather);
            }
        }

        public void RestartCard(GameObject Card, GameObject Place, bool home)
        {
            Card card = Card.GetComponent<CardDisplay>().cardTemplate;
            Restart(card);
            if (home)
            {
                Card.GetComponent<CardDrag>().Played = false;
                GameObject Hand;
                if (card.LocationBoard)
                    Hand = GameObject.FindWithTag("P");
                else
                    Hand = GameObject.FindWithTag("E");
                Card.transform.SetParent(Hand.transform, false);
            }
        }
        public PlayerDeck Decking(bool Location)
        {
            if (Location)
                return GameObject.Find("Deck").GetComponent<PlayerDeck>();
            else
                return GameObject.Find("DeckEnemy").GetComponent<PlayerDeck>();
        }
        private void Restart(Card card)
        {
            card.Points = 0;
            card.CurrentPlace = "";
            card.Points = card.OriginalPoints;
        }
        public void ToCementery()
        {
            PlayerDeck DeckP = GameObject.Find("Deck").GetComponent<PlayerDeck>();
            PlayerDeck DeckE = GameObject.Find("DeckEnemy").GetComponent<PlayerDeck>();
            PlayerDeck Current;
            GameObject card;
            List<Card> Permanents = new List<Card>();
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
                        if (disp.cardTemplate.type == "U")
                            Restart(disp.cardTemplate);
                        
                            Current.AddToCement(disp.cardTemplate);
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
                    Current.AddToCement(disp.cardTemplate);
                    Destroy(card);
                }
            }
            foreach (Card disp in Permanents)
            {
                //Primero verifico que en el otro terreno no haya quedado en juego un clima casualmente
                int increase = 0;
                disp.Points= disp.OriginalPoints;
                GameObject C = RangeMap[(disp.LocationBoard, disp.CurrentPlace)];
                increase = C.GetComponent<DropProp>().weather + C.GetComponent<DropProp>().raised;
                if (disp.unit != KindofCard.Golden)
                    disp.Points = disp.Points + increase;
                if(disp.SuperPower== Effect.Weather|| disp.SuperPower== Effect.Raise|| disp.SuperPower == Effect.Multpwr)
                    ListEffects[disp.SuperPower](disp);
            }
        }
        
        public bool RandomizedRem(Player Player)
        {
            int cant = 0;
            Card dispvar;
            System.Random r= new();
            foreach (GameObject Gamezone in RangeMap.Values)
            {
                for (int i = 0; i < Gamezone.transform.childCount; i++)
                {
                    dispvar = Gamezone.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().cardTemplate;
                    if (dispvar != null && (dispvar.type == "U"|| dispvar.type== "D" )&& dispvar.Faction== Player)
                    {
                        cant++;
                    }
                }
            }
            if (cant > 0)
            {
                int cant2 = r.Next(1, cant);
                cant = 1;
                foreach (GameObject Gamezone in RangeMap.Values)
                {
                    for (int i = 0; i < Gamezone.transform.childCount; i++)
                    {
                        dispvar = Gamezone.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().cardTemplate;
                        if (dispvar != null && (dispvar.type == "U" || dispvar.type == "D") && dispvar.Faction == Player)
                        {
                            if (cant2 == cant)
                            {
                                return true;
                            }
                            else
                                cant++;
                        }
                    }
                }
            }
            return false;
        }
    }
}
