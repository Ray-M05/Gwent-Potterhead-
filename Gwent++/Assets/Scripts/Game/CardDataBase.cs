using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LogicalSide 
{ 
    public class CardDataBase: MonoBehaviour
    {
        public static List<UnityCard> GetDeck(Player P)
        {
            List<UnityCard> Deck = new();
            if (P.faction == 1)
            {
                #region Gryffindor
                Deck.Add(new UnityCard(P.P, "Harry Potter", "Lider", 0, P, KindofCard.None, "L", Effect.None, "", Resources.Load<Sprite>("harrypotter"), "Ganar en caso de empate"));
                Deck.Add(new UnityCard(P.P, "Minerva", "Oro", 9, P, KindofCard.Golden, "U", Effect.None, "MRS", Resources.Load<Sprite>("minerva"), "No es afectada por ninguna habilidad especial"));
                Deck.Add(new UnityCard(P.P, "Dumbledore","Oro", 8, P, KindofCard.Golden, "U", Effect.None, "MRS", Resources.Load<Sprite>("dumbledore"), "No es afectada por ninguna habilidad especial"));
                Deck.Add(new UnityCard(P.P, "Sirius", "Oro", 7, P, KindofCard.Golden, "U", Effect.None, "MRS", Resources.Load<Sprite>("sirius"), "No es afectada por ninguna habilidad especial"));
                Deck.Add(new UnityCard(P.P, "Lupin", "Oro", 6, P, KindofCard.Golden, "U", Effect.None, "MRS", Resources.Load<Sprite>("Lupin"), "No es afectada por ninguna habilidad especial"));
                Deck.Add(new UnityCard(P.P, "Fred & George", "Plata", 3, P, KindofCard.Silver, "U", Effect.ZoneCleaner, "M", Resources.Load<Sprite>("fred"), "Limpia la fila con menor cantidad de unidades del campo"));
                Deck.Add(new UnityCard(P.P, "Fred & George", "Plata", 3, P, KindofCard.Silver, "U", Effect.ZoneCleaner, "M", Resources.Load<Sprite>("fred"), "Limpia la fila con menor cantidad de unidades del campo"));
                Deck.Add(new UnityCard(P.P, "Ginny", "Plata", 3, P, KindofCard.Silver, "U", Effect.Raise, "S", Resources.Load<Sprite>("ginny"), "Agrega una unidad de aumento a la fila de la carta que posee la habilidad"));
                Deck.Add(new UnityCard(P.P, "Ginny", "Plata", 3, P, KindofCard.Silver, "U", Effect.Raise, "S", Resources.Load<Sprite>("ginny"), "Agrega una unidad de aumento a la fila de la carta que posee la habilidad"));
                Deck.Add(new UnityCard(P.P, "Hermione", "Plata", 4, P, KindofCard.Silver, "U", Effect.None, "MS", Resources.Load<Sprite>("hermione"),"No tiene efecto especial"));
                Deck.Add(new UnityCard(P.P, "Hermione", "Plata", 4, P, KindofCard.Silver, "U", Effect.None, "MS", Resources.Load<Sprite>("hermione"), "No tiene efecto especial"));
                Deck.Add(new UnityCard(P.P, "Neville", "Plata", 3, P, KindofCard.Silver, "U", Effect.Stealer, "R", Resources.Load<Sprite>("neville"), "Roba una carta"));
                Deck.Add(new UnityCard(P.P, "Neville", "Plata", 3, P, KindofCard.Silver, "U", Effect.Stealer, "R", Resources.Load<Sprite>("neville"), "Roba una carta"));
                Deck.Add(new UnityCard(P.P, "Ron", "Plata",5, P, KindofCard.Silver, "U", Effect.None, "M" + "R", Resources.Load<Sprite>("ron"), "No tiene efecto especial"));
                Deck.Add(new UnityCard(P.P, "Ron", "Plata", 5, P, KindofCard.Silver, "U", Effect.None, "M" + "R", Resources.Load<Sprite>("ron"), "No tiene efecto especial"));
                Deck.Add(new UnityCard(P.P, "Basilisco", "Clima", 0, P, KindofCard.None, "C", Effect.Weather, "M", Resources.Load<Sprite>("basilisco"), "Afecta la fila seleccionada disminuyendo en 3 el poder de cada carta en ambos campos"));
                Deck.Add(new UnityCard(P.P, "Dementor", "Clima", 0, P, KindofCard.None, "C", Effect.Weather, "R", Resources.Load<Sprite>("dementor"), "Afecta la fila seleccionada disminuyendo en 3 el poder de cada carta en ambos campos"));
                Deck.Add(new UnityCard(P.P, "Sectum Sempra","Clima", 0, P, KindofCard.None, "C", Effect.Weather, "S", Resources.Load<Sprite>("sectumsempra"), "Afecta la fila seleccionada disminuyendo en 3 el poder de cada carta en ambos campos"));
                Deck.Add(new UnityCard(P.P, "Jugo de Mandragora","Despeje", 0, P, KindofCard.None, "C", Effect.Cleaner, "M", Resources.Load<Sprite>("mandragora"), "Elimina todas las unidades de clima del campo"));
                Deck.Add(new UnityCard(P.P, "Patronus", "Despeje", 0, P, KindofCard.None, "C", Effect.Cleaner, "R", Resources.Load<Sprite>("patronus"), "Elimina todas las unidades de clima del campo"));
                Deck.Add(new UnityCard(P.P, "Vulnera Sanetur", "Aumento",0, P, KindofCard.None, "C", Effect.Cleaner, "S", Resources.Load<Sprite>("vulnerasanentur"), "Elimina todas las unidades de clima del campo"));
                Deck.Add(new UnityCard(P.P, "Nimbus 2000", "Aumento",0, P, KindofCard.None, "AM", Effect.Raise, "M", Resources.Load<Sprite>("nimbus2000"), "Bonifica la fila seleccionada aumentando en 2 el poder de cada carta en el campo propio"));
                Deck.Add(new UnityCard(P.P, "Varita de Sauco","Aumento", 0, P, KindofCard.None, "AR", Effect.Raise, "R", Resources.Load<Sprite>("varita"), "Bonifica la fila seleccionada aumentando en 2 el poder de cada carta en el campo propio"));
                Deck.Add(new UnityCard(P.P, "Suerte Liquida", "Aumento", 0, P, KindofCard.None, "AS", Effect.Raise, "S", Resources.Load<Sprite>("suerteliquida"), "Bonifica la fila seleccionada aumentando en 2 el poder de cada carta en el campo propio"));
                Deck.Add(new UnityCard(P.P, "Sr Nicholas", "Señuelo", 0, P, KindofCard.None, "D", Effect.Decoy, "MRS", Resources.Load<Sprite>("srnicolas"), "Coloca una carta con poder 0 en el lugar de la cseleccionada y regresa a la mano"));

                #endregion
                foreach (var card in Deck)
                {
                    card.OnConstruction = true;
                    card.Faction = "Gryffindor";
                    card.OnConstruction = false;
                }
                P.AlwaysAWinner = true;
            }
            else if(P.faction==2)
            {
                #region Slytherin
                Deck.Add(new UnityCard(P.P, "Voldemort", "Lider", 0, P, KindofCard.None, "L", Effect.None, "", Resources.Load<Sprite>("voldemort"), "Roba una carta extra al final de cada ronda"));
                Deck.Add(new UnityCard(P.P, "Snape","Oro", 9, P, KindofCard.Golden, "U", Effect.None, "MRS", Resources.Load<Sprite>("snape"), "No es afectada por ninguna habilidad"));
                Deck.Add(new UnityCard(P.P, "Bellatrix","Oro", 8, P, KindofCard.Golden, "U", Effect.None, "MRS", Resources.Load<Sprite>("bellatrix"), "No es afectada por ninguna habilidad especial"));
                Deck.Add(new UnityCard(P.P, "Umbridge","Oro", 7, P, KindofCard.Golden, "U", Effect.None, "MRS", Resources.Load<Sprite>("doloresumbridge"), "No es afectada por ninguna habilidad especial"));
                Deck.Add(new UnityCard(P.P, "Horace","Oro", 6, P, KindofCard.Golden, "U", Effect.None, "MRS", Resources.Load<Sprite>("horace"), "No es afectada por ninguna habilidad especial"));
                Deck.Add(new UnityCard(P.P, "Marcus","Plata", 3, P, KindofCard.Silver, "U", Effect.Weather, "M", Resources.Load<Sprite>("marcusflint"), "Afecta la fila seleccionada disminuyendo en 3 el poder de cada carta en ambos campos"));
                Deck.Add(new UnityCard(P.P, "Marcus", "Plata",3, P, KindofCard.Silver, "U", Effect.Weather, "M", Resources.Load<Sprite>("marcusflint"), "Afecta la fila seleccionada disminuyendo en 3 el poder de cada carta en ambos campos"));
                Deck.Add(new UnityCard(P.P, "Vincent", "Plata",3, P, KindofCard.Silver, "U", Effect.LessPwr, "S", Resources.Load<Sprite>("vincentcrabbe"), "Elimina la carta con menor poder del campo")); 
                Deck.Add(new UnityCard(P.P, "Vincent","Plata", 3, P, KindofCard.Silver, "U", Effect.LessPwr, "S", Resources.Load<Sprite>("vincentcrabbe"), "Elimina la carta con menor poder del campo"));
                Deck.Add(new UnityCard(P.P, "Draco","Plata", 5, P, KindofCard.Silver, "U", Effect.None, "MS", Resources.Load<Sprite>("draco"), "No tiene efecto especial"));
                Deck.Add(new UnityCard(P.P, "Draco","Plata", 5, P, KindofCard.Silver, "U", Effect.None, "MS", Resources.Load<Sprite>("draco"), "No tiene efecto especial"));
                Deck.Add(new UnityCard(P.P, "Baron Sanguinario","Plata", 3, P, KindofCard.Silver, "U", Effect.Stealer, "R", Resources.Load<Sprite>("baronsanguinario"), "Roba una carta"));
                Deck.Add(new UnityCard(P.P, "Baron Sanguinario", "Plata",3, P, KindofCard.Silver, "U", Effect.Stealer, "R", Resources.Load<Sprite>("baronsanguinario"), "Roba una carta"));
                Deck.Add(new UnityCard(P.P, "Blaise","Plata", 4, P, KindofCard.Silver, "U", Effect.None, "MR", Resources.Load<Sprite>("blaisezabini"), "No tiene efecto especial"));
                Deck.Add(new UnityCard(P.P, "Blaise", "Plata",4, P, KindofCard.Silver, "U", Effect.None, "MR", Resources.Load<Sprite>("blaisezabini"), "No tiene efecto especial"));
                Deck.Add(new UnityCard(P.P, "Basilisco","Clima", 0, P, KindofCard.None, "C", Effect.Weather, "M", Resources.Load<Sprite>("basilisco"), "Afecta la fila seleccionada disminuyendo en 3 el poder de cada carta en ambos campos"));
                Deck.Add(new UnityCard(P.P, "Dementor", "Clima",0, P, KindofCard.None, "C", Effect.Weather, "R", Resources.Load<Sprite>("dementor"), "Afecta la fila seleccionada disminuyendo en 3 el poder de cada carta en ambos campos"));
                Deck.Add(new UnityCard(P.P, "Sectum Sempra","Clima", 0, P, KindofCard.None, "C", Effect.Weather, "S", Resources.Load<Sprite>("sectumsempra"), "Afecta la fila seleccionada disminuyendo en 3 el poder de cada carta en ambos campos"));
                Deck.Add(new UnityCard(P.P, "Jugo de Mandragora","Despeje", 0, P, KindofCard.None, "C", Effect.Cleaner, "M", Resources.Load<Sprite>("mandragora"), "Elimina todas las unidades de clima del campo"));
                Deck.Add(new UnityCard(P.P, "Patronus", "Despeje", 0, P, KindofCard.None, "C", Effect.Cleaner, "R", Resources.Load<Sprite>("patronus"), "Elimina todas las unidades de clima del campo"));
                Deck.Add(new UnityCard(P.P, "Vulnera Sanetur", "Aumento",0, P, KindofCard.None, "C", Effect.Cleaner, "S", Resources.Load<Sprite>("vulnerasanentur"), "Elimina todas las unidades de clima del campo"));
                Deck.Add(new UnityCard(P.P, "Nimbus 2000", "Aumento",0, P, KindofCard.None, "AM", Effect.Raise, "M", Resources.Load<Sprite>("nimbus2000"), "Bonifica la fila seleccionada aumentando en 2 el poder de cada carta en el campo propio"));
                Deck.Add(new UnityCard(P.P, "Varita de Sauco","Aumento", 0, P, KindofCard.None, "AR", Effect.Raise, "R", Resources.Load<Sprite>("varita"), "Bonifica la fila seleccionada aumentando en 2 el poder de cada carta en el campo propio"));
                Deck.Add(new UnityCard(P.P, "Suerte Liquida", "Aumento", 0, P, KindofCard.None, "AS", Effect.Raise, "S", Resources.Load<Sprite>("suerteliquida"), "Bonifica la fila seleccionada aumentando en 2 el poder de cada carta en el campo propio"));
                Deck.Add(new UnityCard(P.P, "Sr Nicholas", "Señuelo",0, P, KindofCard.None, "D", Effect.Decoy, "MRS", Resources.Load<Sprite>("srnicolas"), "Coloca una carta con poder 0 en el lugar de la seleccionada y regresa a la mano"));

                #endregion
                foreach (var card in Deck)
                {
                    card.OnConstruction = true;
                    card.Faction = "Slytherin";
                    card.OnConstruction = false;
                }
                P.Stealer = true;
            }

            //Barajear la lista sin mover el cero
            int conta = 24;
            if(P.Cards!=null)
            foreach (var card in P.Cards)
            {
                if(card.Type!= "Lider")
                    Deck[conta--]= card;
                else
                    Deck[0]= card;
                card.OnConstruction = true;
                card.Owner = P;
                card.OnConstruction = false;
            }
            return Deck;
        }




    }

    

}
