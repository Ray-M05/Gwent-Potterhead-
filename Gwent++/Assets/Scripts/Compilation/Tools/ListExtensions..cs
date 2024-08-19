using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Compiler;
using LogicalSide;
using UnityEditor.Build;
using UnityEngine;

namespace ListExtensions
{
    public static class Extensions
    {
        public static List<T> Find<T>(this List<T> list, Compiler.Expression pred)
        {
            if (pred is Predicate predicate)
            {
                List<T> custom = new();
                foreach (var item in list)
                {
                    if ((bool)predicate.Evaluate(null, item))
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
            int n = list.Count;
            System.Random random = new System.Random();
            while (n > 0)
            {
                n--;
                int k = random.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        public static T Pop<T>(this List<T> list)
        {
            if (list.Count > 0)
            {
                T obj = list[list.Count - 1];
                list.Remove(obj);
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
            if(AddPosibility!= null && (bool)AddPosibility&& item is UnityCard card)
            {
                PlayerDeck Deck;
                if(Id== "Hand")
                {
                    Deck= GameObject.Find("Player Hand").GetComponent<PlayerDeck>();
                    Deck.Instanciate(card, Deck.playerZone, Deck.prefabCarta);
                }
                else if(Id== "OtherHand")
                {
                    Deck= GameObject.Find("Enemy Hand").GetComponent<PlayerDeck>();
                    Deck.Instanciate(card, Deck.playerZone, Deck.prefabCarta);
                }
                if(PlayerOwner!= null)
                {
                    GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
                    card.LocationBoard= (bool)PlayerOwner;
                    card.Owner= GM.WhichPlayer((bool)PlayerOwner);
                }
                list.Add(item);
            }
            else
                throw new Exception(Id+ " list, is not available to Add elements, due to its Ambiguity");
        }

        public static void RemoveCard<T>(this List<T> list, T item)
        {
            if(item is UnityCard card)
            {//En caso de que la carta este siendo mostrada en la interfaz, 
            //ella seteará internamente que debe ser destruida
            //y los scripts que heredan de MonoBehavior lo detectarán, y la destruirán automaticamente
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
        public static bool? AddPosibility = null;
        public static string Id = "";
        public static void UpdateId<T>(this List<T> list, string newId)
        {
            Id = newId; 
        }
    }
}

