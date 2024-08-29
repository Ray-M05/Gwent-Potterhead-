using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;
using Compiler;
using LogicalSide;
using UnityEditor.Build;
using UnityEngine;

namespace ListExtensions
{
    public static class Extensions
    {
        public static List<T> Find<T>(this List<T> list, Compiler.Expression pred, Scope Scope)
        {
            if (pred is Predicate predicate)
            {
                List<T> custom = new();
                foreach (var item in list)
                {
                    if ((bool)predicate.Evaluate(Scope, item))
                        custom.Add(item);
                }
                return custom;
            }
            else
                Errors.List.Add(new CompilingError("Find Argument must be a predicate", new Position()));
            return null;
        }

        public static void Shuffle<T>(this List<T> list)
        {
            if (Id == "Deck" || Id == "OtherDeck" || Id == "GraveYard" || Id == "OtherGraveYard")
            {
                int n = list.Count;
                System.Random random = new System.Random();
                while (n > 0)
                {
                    n--;
                    int k = random.Next(n + 1);
                    (list[n], list[k]) = (list[k], list[n]);
                }
            }
            else
                throw new Exception("You are trying to Shuffle a not shuffable list, please read the instructions and check the shuffable lists");
        }

        public static T Pop<T>(this List<T> list)
        {
            if (list.Count > 0)
            {
                T obj = list[list.Count - 1];
                list.RemoveCard(obj);
                return obj;
            }

            Errors.List.Add(new CompilingError("Trying to Pop from an empty List", new Position()));
            return default;
        }

        public static void SendBottom<T>(this List<T> list, T item)
        {
            list.Add(item);
            list.RemoveAt(list.Count - 1);
            list.Insert(0, item);
        }

        public static void AddCard<T>(this List<T> list, T item)
        {
            if (AddPosibility != null && (bool)AddPosibility && item is Card Card)
            {
                GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
                bool allfine = true;
                UnityCard card = (UnityCard)Card.GetCopy();
                PlayerDeck Deck;
                if (Id == "Hand")
                {
                    Deck = GameObject.Find("Deck").GetComponent<PlayerDeck>();
                    Deck.Instanciate(card, Deck.playerZone, Deck.prefabCarta);
                }
                else if (Id == "OtherHand")
                {
                    Deck = GameObject.Find("DeckEnemy").GetComponent<PlayerDeck>();
                    Deck.Instanciate(card, Deck.playerZone, Deck.prefabCarta);
                }
                else if (Id == "Field")
                {
                    Efectos efectos = GameObject.Find("Effects").GetComponent<Efectos>();
                    allfine = efectos.AddInField(card, true);
                }
                else if (Id == "OtherField")
                {
                    Efectos efectos = GameObject.Find("Effects").GetComponent<Efectos>();
                    allfine = efectos.AddInField(card, false);
                }
                else if (Id == "Board")
                {//En este caso hay probabilidad de que se juegue a ambos lados, luego hay q sortear, y si el lado 
                 //elegido al azar no se encuentra disponible, se añade al otro lado de ser posible(2 de cada 3 veces se elegirá el lado del propietario)

                    bool[] luck = new bool[3] { card.LocationBoard, card.LocationBoard, !(card.LocationBoard) };
                    System.Random random = new System.Random();
                    bool result = luck[random.Next(luck.Length)];
                    Efectos efectos = GameObject.Find("Effects").GetComponent<Efectos>();

                    if (!efectos.AddInField(card, result))
                    {
                        if (card.TypeOfCard != "C" && !efectos.AddInField(card, !result))
                        {
                            GM.SendPrincipal($"Imposible añadir la carta {card.Name}");
                            allfine = false;
                        }
                    }
                }
                if (allfine && PlayerOwner != null && Id != "Board" && Id != "OtherField" && Id != "Field")
                {
                    card.LocationBoard = (bool)PlayerOwner;
                    card.OnConstruction = true;
                    card.Owner = GM.WhichPlayer((bool)PlayerOwner);
                    card.OnConstruction = false;
                }
                if (allfine)
                    list.Add(item);
            }
            else
            {
                GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
                GM.SendPrincipal(Id + " list, is not available to Add elements, due to its Ambiguity");
                throw new Exception(Id + " list, is not available to Add elements, due to its Ambiguity");
            }
        }

        public static void RemoveCard<T>(this List<T> list, T item)
        {
            if(item is UnityCard card)
            {
                card.Destroy= true;
            }
            for(int i = 0; i< list.Count; i++)
            {
                if(list[i].Equals(item))
                {
                    list.RemoveAt(i);
                    break;
                }
            }
        }

        public static bool? PlayerOwner = null;
        public static void UpdateOwner<T>(this List<T> list, bool? newOwner, bool? IstherePosibility)
        {
            PlayerOwner = newOwner; 
            AddPosibility = IstherePosibility;
        }
        public static bool? AddPosibility = null;
        public static string Id = "";
        public static void UpdateId<T>(this List<T> list, string newId)
        {
            Id = newId; 
        }
    }
}

