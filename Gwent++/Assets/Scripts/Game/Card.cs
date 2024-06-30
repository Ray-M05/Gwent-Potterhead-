using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LogicalSide
{
    // [CreateAssetMenu(fileName ="New Card", menuName = "Card")]
    public class Card: ScriptableObject
    {
        public Player Faction;
        public Sprite Artwork;
        public string Name;
        private int _points; 
        public int Points
        {
            get { return _points; }
            set
            {
                int temp = _points;
                _points = value; 
                GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
                if (GM != null&& CurrentPlace!=""&& CurrentPlace!=null)
                {
                    GM.AddScore(LocationBoard,_points-temp);
                }
                if(PwrText!=null)
                PwrText.text = _points.ToString();
            }

        }

        public int OriginalPoints;
        public string description;
        public string AttackPlace;
        public string CurrentPlace;
        public string type;
        public KindofCard unit;
        public Effect SuperPower;
        public bool LocationBoard; //true if its down
        public TextMeshProUGUI PwrText= new();
        

        public Card(bool LocationBoard ,string name ,int points,Player Player,KindofCard unit,string type ,Effect ability,string attackRange, Sprite Img, string description)
        {
            this.Name = name;
            this.Points = points;
            OriginalPoints = points;
            this.description = description;
            this.AttackPlace = attackRange;
            this.Artwork = Img;
            this.type = type;
            this.unit = unit;
            this.SuperPower = ability;
            this.LocationBoard = LocationBoard;
            this.Faction = Player;
        }

    }

    public enum Effect
    {
        Weather,
        Raise,
        MostPwr,
        LessPwr,
        Multpwr,
        ZoneCleaner,
        Stealer,
        Cleaner,
        Average,
        Decoy,
        None,
    }
    public enum Type
    {
        Leader,
        Weather,
        Unity,
        Decoy,
        Raise,
    }
    public enum KindofCard
    {
        Golden,
        Silver,
        None,
    }

}
