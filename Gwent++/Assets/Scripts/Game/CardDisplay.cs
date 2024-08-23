using LogicalSide;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace LogicalSide
{
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
                DescriptionText.text = "";//cardTemplate.description;
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
                        GM.SendPrincipal("Error en la ejecucion del efecto:");
                        GM.SendPrincipal(ex.Message);
                    }
                }
            }
        }
    }
}
