using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LogicalSide
{
    [CreateAssetMenu(fileName ="New Card", menuName = "Card")]
    public class Card: ScriptableObject
    {
        public Player Owner;
        public Sprite Artwork;
        public string Name;
        private int _pwr; // Campo de respaldo
        public int Pwr
        {
            get { return _pwr; }
            set
            {
                int provi = _pwr;
                _pwr = value; // Almacena el valor en el campo de respaldo
                GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
                if (GM != null&& current_Rg!=""&& current_Rg!=null)
                {
                    GM.AddScore(DownBoard,_pwr-provi);
                }
                if(PwrText!=null)
                PwrText.text = _pwr.ToString();
            }

        }

        public int OriginPwr;
        public string description;
        public string Atk_Rg;
        public string current_Rg;
        public string type;
        public TypeUnit unit;
        public string Eff;
        public bool DownBoard;
        public TextMeshProUGUI PwrText= new();
        

        public Card(bool DownBoard ,string name ,int pwr,Player Owner,TypeUnit unit,string type ,string Eff,string atk_Rg, Sprite Img, string description)
        {
            this.Name = name;
            this.Pwr = pwr;
            OriginPwr = pwr;
            this.description = description;
            this.Atk_Rg = atk_Rg;
            this.Artwork = Img;
            this.type = type;
            this.unit = unit;
            this.Eff = Eff;
            this.DownBoard = DownBoard;
            this.Owner = Owner;
        }

    }
    public enum Type
    {
        Leader,
        Weather,
        Unity,
        Decoy,
        Raise,
    }
    public enum TypeUnit
    {
        Golden,
        Silver,
        None
    }
}
