using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LogicalSide 
{ 
    public class CardDataBase: MonoBehaviour
    {
        public static List<UnityCard> GetDeck(Player P)
        {
            string MyFaction = "";
            List<UnityCard> Deck = new();
            if (P.faction == 1)
            {
                MyFaction = "Gryffindor";
                #region Gryffindor
                Deck.Add(new UnityCard(P.P, "Harry Potter", 0, P, KindofCard.None, "L", Effect.None, "", Resources.Load<Sprite>("harrypotter"), "Ganar en caso de empate"));
                Deck.Add(new UnityCard(P.P, "Minerva", 9, P, KindofCard.Golden, "U", Effect.None, "MRS", Resources.Load<Sprite>("minerva"), "No es afectada por ninguna habilidad especial"));
                Deck.Add(new UnityCard(P.P, "Dumbledore", 8, P, KindofCard.Golden, "U", Effect.None, "MRS", Resources.Load<Sprite>("dumbledore"), "No es afectada por ninguna habilidad especial"));
                Deck.Add(new UnityCard(P.P, "Sirius", 7, P, KindofCard.Golden, "U", Effect.None, "MRS", Resources.Load<Sprite>("sirius"), "No es afectada por ninguna habilidad especial"));
                Deck.Add(new UnityCard(P.P, "Lupin", 6, P, KindofCard.Golden, "U", Effect.None, "MRS", Resources.Load<Sprite>("Lupin"), "No es afectada por ninguna habilidad especial"));
                Deck.Add(new UnityCard(P.P, "Fred & George", 3, P, KindofCard.Silver, "U", Effect.ZoneCleaner, "M", Resources.Load<Sprite>("fred"), "Limpia la fila con menor cantidad de unidades del campo"));
                Deck.Add(new UnityCard(P.P, "Fred & George", 3, P, KindofCard.Silver, "U", Effect.ZoneCleaner, "M", Resources.Load<Sprite>("fred"), "Limpia la fila con menor cantidad de unidades del campo"));
                Deck.Add(new UnityCard(P.P, "Ginny", 3, P, KindofCard.Silver, "U", Effect.Raise, "S", Resources.Load<Sprite>("ginny"), "Agrega una unidad de aumento a la fila de la carta que posee la habilidad"));
                Deck.Add(new UnityCard(P.P, "Ginny", 3, P, KindofCard.Silver, "U", Effect.Raise, "S", Resources.Load<Sprite>("ginny"), "Agrega una unidad de aumento a la fila de la carta que posee la habilidad"));
                Deck.Add(new UnityCard(P.P, "Hermione", 4, P, KindofCard.Silver, "U", Effect.None, "MS", Resources.Load<Sprite>("hermione"),"No tiene efecto especial"));
                Deck.Add(new UnityCard(P.P, "Hermione", 4, P, KindofCard.Silver, "U", Effect.None, "MS", Resources.Load<Sprite>("hermione"), "No tiene efecto especial"));
                Deck.Add(new UnityCard(P.P, "Neville", 3, P, KindofCard.Silver, "U", Effect.Stealer, "R", Resources.Load<Sprite>("neville"), "Roba una carta"));
                Deck.Add(new UnityCard(P.P, "Neville", 3, P, KindofCard.Silver, "U", Effect.Stealer, "R", Resources.Load<Sprite>("neville"), "Roba una carta"));
                Deck.Add(new UnityCard(P.P, "Ron", 5, P, KindofCard.Silver, "U", Effect.None, "M" + "R", Resources.Load<Sprite>("ron"), "No tiene efecto especial"));
                Deck.Add(new UnityCard(P.P, "Ron", 5, P, KindofCard.Silver, "U", Effect.None, "M" + "R", Resources.Load<Sprite>("ron"), "No tiene efecto especial"));
                Deck.Add(new UnityCard(P.P, "Basilisco", 0, P, KindofCard.None, "C", Effect.Weather, "M", Resources.Load<Sprite>("basilisco"), "Afecta la fila seleccionada disminuyendo en 3 el poder de cada carta en ambos campos"));
                Deck.Add(new UnityCard(P.P, "Dementor", 0, P, KindofCard.None, "C", Effect.Weather, "R", Resources.Load<Sprite>("dementor"), "Afecta la fila seleccionada disminuyendo en 3 el poder de cada carta en ambos campos"));
                Deck.Add(new UnityCard(P.P, "Sectum Sempra", 0, P, KindofCard.None, "C", Effect.Weather, "S", Resources.Load<Sprite>("sectumsempra"), "Afecta la fila seleccionada disminuyendo en 3 el poder de cada carta en ambos campos"));
                Deck.Add(new UnityCard(P.P, "Jugo de Mandragora", 0, P, KindofCard.None, "C", Effect.Cleaner, "M", Resources.Load<Sprite>("mandragora"), "Elimina todas las unidades de clima del campo"));
                Deck.Add(new UnityCard(P.P, "Patronus", 0, P, KindofCard.None, "C", Effect.Cleaner, "R", Resources.Load<Sprite>("patronus"), "Elimina todas las unidades de clima del campo"));
                Deck.Add(new UnityCard(P.P, "Vulnera Sanetur", 0, P, KindofCard.None, "C", Effect.Cleaner, "S", Resources.Load<Sprite>("vulnerasanentur"), "Elimina todas las unidades de clima del campo"));
                Deck.Add(new UnityCard(P.P, "Nimbus 2000", 0, P, KindofCard.None, "AM", Effect.Raise, "M", Resources.Load<Sprite>("nimbus2000"), "Bonifica la fila seleccionada aumentando en 2 el poder de cada carta en el campo propio"));
                Deck.Add(new UnityCard(P.P, "Varita de Sauco", 0, P, KindofCard.None, "AR", Effect.Raise, "R", Resources.Load<Sprite>("varita"), "Bonifica la fila seleccionada aumentando en 2 el poder de cada carta en el campo propio"));
                Deck.Add(new UnityCard(P.P, "Suerte Liquida", 0, P, KindofCard.None, "AS", Effect.Raise, "S", Resources.Load<Sprite>("suerteliquida"), "Bonifica la fila seleccionada aumentando en 2 el poder de cada carta en el campo propio"));
                Deck.Add(new UnityCard(P.P, "Sr Nicholas", 0, P, KindofCard.None, "D", Effect.Decoy, "MRS", Resources.Load<Sprite>("srnicolas"), "Coloca una carta con poder 0 en el lugar de la cseleccionada y regresa a la mano"));

                #endregion
                foreach (var card in Deck)
                {
                    card.Faction = "Gryffindor";
                }
                P.AlwaysAWinner = true;
            }
            else if(P.faction==2)
            {
                MyFaction= "Slytherin";
                #region Slytherin
                Deck.Add(new UnityCard(P.P, "Voldemort", 0, P, KindofCard.None, "L", Effect.None, "", Resources.Load<Sprite>("voldemort"), "Roba una carta extra al final de cada ronda"));
                Deck.Add(new UnityCard(P.P, "Snape", 9, P, KindofCard.Golden, "U", Effect.None, "MRS", Resources.Load<Sprite>("snape"), "No es afectada por ninguna habilidad"));
                Deck.Add(new UnityCard(P.P, "Bellatrix", 8, P, KindofCard.Golden, "U", Effect.None, "MRS", Resources.Load<Sprite>("bellatrix"), "No es afectada por ninguna habilidad especial"));
                Deck.Add(new UnityCard(P.P, "Umbridge", 7, P, KindofCard.Golden, "U", Effect.None, "MRS", Resources.Load<Sprite>("doloresumbridge"), "No es afectada por ninguna habilidad especial"));
                Deck.Add(new UnityCard(P.P, "Horace", 6, P, KindofCard.Golden, "U", Effect.None, "MRS", Resources.Load<Sprite>("horace"), "No es afectada por ninguna habilidad especial"));
                Deck.Add(new UnityCard(P.P, "Marcus", 3, P, KindofCard.Silver, "U", Effect.Weather, "M", Resources.Load<Sprite>("marcusflint"), "Afecta la fila seleccionada disminuyendo en 3 el poder de cada carta en ambos campos"));
                Deck.Add(new UnityCard(P.P, "Marcus", 3, P, KindofCard.Silver, "U", Effect.Weather, "M", Resources.Load<Sprite>("marcusflint"), "Afecta la fila seleccionada disminuyendo en 3 el poder de cada carta en ambos campos"));
                Deck.Add(new UnityCard(P.P, "Vincent", 3, P, KindofCard.Silver, "U", Effect.LessPwr, "S", Resources.Load<Sprite>("vincentcrabbe"), "Elimina la carta con menor poder del campo")); Deck.Add(new UnityCard(P.P, "Ginny", 3, P, KindofCard.Silver, "U", Effect.Raise, "S", Resources.Load<Sprite>("ginny"), "Agrega una unidad de aumento a la fila de la carta que posee la habilidad"));
                Deck.Add(new UnityCard(P.P, "Vincent", 3, P, KindofCard.Silver, "U", Effect.LessPwr, "S", Resources.Load<Sprite>("vincentcrabbe"), "Elimina la carta con menor poder del campo"));
                Deck.Add(new UnityCard(P.P, "Draco", 5, P, KindofCard.Silver, "U", Effect.None, "MS", Resources.Load<Sprite>("draco"), "No tiene efecto especial"));
                Deck.Add(new UnityCard(P.P, "Draco", 5, P, KindofCard.Silver, "U", Effect.None, "MS", Resources.Load<Sprite>("draco"), "No tiene efecto especial"));
                Deck.Add(new UnityCard(P.P, "Baron Sanguinario", 3, P, KindofCard.Silver, "U", Effect.Stealer, "R", Resources.Load<Sprite>("baronsanguinario"), "Roba una carta"));
                Deck.Add(new UnityCard(P.P, "Baron Sanguinario", 3, P, KindofCard.Silver, "U", Effect.Stealer, "R", Resources.Load<Sprite>("baronsanguinario"), "Roba una carta"));
                Deck.Add(new UnityCard(P.P, "Blaise", 4, P, KindofCard.Silver, "U", Effect.None, "MR", Resources.Load<Sprite>("blaisezabini"), "No tiene efecto especial"));
                Deck.Add(new UnityCard(P.P, "Blaise", 4, P, KindofCard.Silver, "U", Effect.None, "MR", Resources.Load<Sprite>("blaisezabini"), "No tiene efecto especial"));
                Deck.Add(new UnityCard(P.P, "Basilisco", 0, P, KindofCard.None, "C", Effect.Weather, "M", Resources.Load<Sprite>("basilisco"), "Afecta la fila seleccionada disminuyendo en 3 el poder de cada carta en ambos campos"));
                Deck.Add(new UnityCard(P.P, "Dementor", 0, P, KindofCard.None, "C", Effect.Weather, "R", Resources.Load<Sprite>("dementor"), "Afecta la fila seleccionada disminuyendo en 3 el poder de cada carta en ambos campos"));
                Deck.Add(new UnityCard(P.P, "Sectum Sempra", 0, P, KindofCard.None, "C", Effect.Weather, "S", Resources.Load<Sprite>("sectumsempra"), "Afecta la fila seleccionada disminuyendo en 3 el poder de cada carta en ambos campos"));
                Deck.Add(new UnityCard(P.P, "Jugo de Mandragora", 0, P, KindofCard.None, "C", Effect.Cleaner, "M", Resources.Load<Sprite>("mandragora"), "Elimina todas las unidades de clima del campo"));
                Deck.Add(new UnityCard(P.P, "Patronus", 0, P, KindofCard.None, "C", Effect.Cleaner, "R", Resources.Load<Sprite>("patronus"), "Elimina todas las unidades de clima del campo"));
                Deck.Add(new UnityCard(P.P, "Vulnera Sanetur", 0, P, KindofCard.None, "C", Effect.Cleaner, "S", Resources.Load<Sprite>("vulnerasanentur"), "Elimina todas las unidades de clima del campo"));
                Deck.Add(new UnityCard(P.P, "Nimbus 2000", 0, P, KindofCard.None, "AM", Effect.Raise, "M", Resources.Load<Sprite>("nimbus2000"), "Bonifica la fila seleccionada aumentando en 2 el poder de cada carta en el campo propio"));
                Deck.Add(new UnityCard(P.P, "Varita de Sauco", 0, P, KindofCard.None, "AR", Effect.Raise, "R", Resources.Load<Sprite>("varita"), "Bonifica la fila seleccionada aumentando en 2 el poder de cada carta en el campo propio"));
                Deck.Add(new UnityCard(P.P, "Suerte Liquida", 0, P, KindofCard.None, "AS", Effect.Raise, "S", Resources.Load<Sprite>("suerteliquida"), "Bonifica la fila seleccionada aumentando en 2 el poder de cada carta en el campo propio"));
                Deck.Add(new UnityCard(P.P, "Sr Nicholas", 0, P, KindofCard.None, "D", Effect.Decoy, "MRS", Resources.Load<Sprite>("srnicolas"), "Coloca una carta con poder 0 en el lugar de la seleccionada y regresa a la mano"));

                #endregion
                foreach (var card in Deck)
                {
                    card.Faction = "Slytherin";
                }
                P.Stealer = true;
            }

            //Barajear la lista sin mover el cero
            //permitir la entrada de cualquier faccion
            int conta = 24;
            if(P.Cards!=null)
            foreach (var card in P.Cards)
            {
                if(card.Faction== MyFaction)
                {
                    if(card.Type!= "L")
                        Deck[conta--]= card;
                    else
                        Deck[0]= card;
                }
            }
            return Deck;
        }




    }

    

}
