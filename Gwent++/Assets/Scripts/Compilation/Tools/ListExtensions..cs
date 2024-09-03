using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;
using Compiler;
using LogicalSide;
using UnityEngine;

namespace ListExtensions
{
    /// <summary>
    /// Provides a set of extension methods for the <see cref="List{T}"/> class to support various operations used in the game.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Finds and returns a list of items that satisfy the given predicate within the list.
        /// </summary>
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


        /// <summary>
        /// Shuffles the items in the list using the Fisher-Yates shuffle algorithm, if the list is of a shufflable type.
        /// </summary>
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


        /// <summary>
        /// Removes and returns the last item from the list.
        /// </summary>
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


        /// <summary>
        /// Moves the specified item to the bottom of the list.
        /// </summary>
        public static void SendBottom<T>(this List<T> list, T item)
        {
            list.Add(item);
            list.RemoveAt(list.Count - 1);
            list.Insert(0, item);
        }


        /// <summary>
        /// Adds a card to the list and handles its placement depending on the list's ID and context.
        /// </summary>
        public static void AddCard<T>(this List<T> list, T item)
        {
            if (item is UnityCard Card)
            {
                GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
                bool allfine = true;
                UnityCard card = (UnityCard)Card.GetCopy();
                PlayerDeck Deck;
                if (Id == "Hand")
                {
                    Deck = GameObject.Find("Deck").GetComponent<PlayerDeck>();
                    Deck.GetInstance(card, Deck.playerZone, Deck.prefabCarta);
                }
                else if (Id == "OtherHand")
                {
                    Deck = GameObject.Find("DeckEnemy").GetComponent<PlayerDeck>();
                    Deck.GetInstance(card, Deck.playerZone, Deck.prefabCarta);
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
                {
                    bool[] luck = new bool[3] { card.LocationBoard, card.LocationBoard, !(card.LocationBoard) };
                    System.Random random = new System.Random();
                    bool result = luck[random.Next(luck.Length)];
                    Efectos efectos = GameObject.Find("Effects").GetComponent<Efectos>();

                    if (!efectos.AddInField(card, result))
                    {
                        if (card.TypeOfCard != "C" && !efectos.AddInField(card, !result))
                        {
                            GM.SendMessage($"Imposible a�adir la carta {card.Name}");
                            allfine = false;
                        }
                    }
                }
                if (allfine && PlayerOwner != null && Id != "Board" && Id != "OtherField" && Id != "Field")
                {
                    card.LocationBoard = (bool)PlayerOwner;
                    card.OnConstruction = true;
                    card.Owner = GM.GetPlayer((bool)PlayerOwner);
                    card.OnConstruction = false;
                }
                if (allfine)
                    list.Add(item);
            }
            else
            {
                GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
                GM.SendMessage(Id + " list, is not available to Add elements, due to its Ambiguity");
                throw new Exception(Id + " list, is not available to Add elements, due to its Ambiguity");
            }
        }

         
        /// <summary>
        /// Removes the specified card from the list and marks it for destruction if it is a UnityCard.
        /// </summary>
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

        /// <summary>
        /// Updates the owner of the list and the possibility of adding new items.
        /// </summary>
        public static void UpdateOwner<T>(this List<T> list, bool? newOwner, bool? IstherePosibility)
        {
            PlayerOwner = newOwner; 
            AddPosibility = IstherePosibility;
        }
        public static bool? AddPosibility = null;
        public static string Id = "";

        /// <summary>
        /// Updates the ID of the list.
        /// </summary>
        public static void UpdateId<T>(this List<T> list, string newId)
        {
            Id = newId; 
        }
    }
}

