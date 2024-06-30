using LogicalSide;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LogicalSide
{
    public class CardDisplay : MonoBehaviour
    {
        public Card cardTemplate;
        public TextMeshProUGUI PwrTxt;
        public TextMeshProUGUI DescriptionText;
        public Image ArtworkImg;
        public Image Back;

        void Update()
        {
            if (cardTemplate != null)
            { 
              if (cardTemplate.Points != 0)
                {
                    PwrTxt.text = cardTemplate.Points.ToString();
                    cardTemplate.PwrText = PwrTxt;
                }
                else
                    PwrTxt.text = "";
                DescriptionText.text = "";//cardTemplate.description;
                ArtworkImg.sprite = cardTemplate.Artwork;
                if(Back!=null)
                if(cardTemplate.Faction.faction==1)
                    Back.sprite = Resources.Load<Sprite>("gryffreverse");
                else
                    Back.sprite = Resources.Load<Sprite>("slythreverse");
            }
        }
    }
}
