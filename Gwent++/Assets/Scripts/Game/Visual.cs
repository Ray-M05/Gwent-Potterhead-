using LogicalSide;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace LogicalSide
{
    /// <summary>
    /// This class is responsible for displaying a UnityCard's information in the UI, including its power, artwork, and description.
    /// It also handles leader card interactions, allowing the player to activate leader abilities when clicked.
    /// </summary>
    public class CardDisplay : MonoBehaviour
    {
        public UnityCard cardTemplate;
        public TextMeshProUGUI PwrTxt;
        public TextMeshProUGUI DescriptionText;
        public Image ArtworkImg;
        public Image Back;
        public bool LeaderAct = false;

        void Update()
        {
            if (cardTemplate != null)
            {
                cardTemplate.Displayed = true;
                  if (cardTemplate.Power != 0)
                    {
                        PwrTxt.text = cardTemplate.Power.ToString();
                        cardTemplate.PwrText = PwrTxt;
                    }
                else
                    PwrTxt.text = "";
                DescriptionText.text = "";
                ArtworkImg.sprite = cardTemplate.Artwork;
                if(Back!=null)
                if(cardTemplate.Faction == "Gryffindor")
                    Back.sprite = Resources.Load<Sprite>("gryffreverse");
                else if(cardTemplate.Faction == "Slytherin")
                    Back.sprite = Resources.Load<Sprite>("slythreverse");
                else
                    Back.sprite= Resources.Load<Sprite>("Ravenclaw back");
                
            }
        }


        /// <summary>
        /// Method to handle when the leader card is clicked by the player.
        /// It checks if the card is a leader, if it belongs to the current turn, and if the leader's ability hasn't already been used.
        /// </summary>
        public void LeaderClicked()
        {
            GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
            if (cardTemplate != null&& cardTemplate.TypeOfCard== "L" && cardTemplate.LocationBoard== GM.Turn && !LeaderAct)
            {
                if (cardTemplate.Effects != null && cardTemplate.Effects.Count > 0)
                {
                    Efectos efectos = GameObject.Find("Effects").GetComponent<Efectos>();
                    try
                    {
                        LeaderAct = true;
                        cardTemplate.Execute(efectos);
                        GM.Turn = !GM.Turn;

                    }
                    catch (System.Exception ex)
                    {
                        GM.SendMessage("Error en la ejecucion del efecto:");
                        GM.SendMessage(ex.Message);
                    }
                }
            }
        }
    }
}
