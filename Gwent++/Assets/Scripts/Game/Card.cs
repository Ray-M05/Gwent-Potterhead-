using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Compiler;

namespace LogicalSide
{
    public class UnityCard: Card
    {
        public override Compiler.Player Owner{get; set;}
        public Sprite Artwork;
        public override string Name{get; set;}
        public override string Faction{get; set;}
        private int _points; 
        public override int Power
        {
            get { return _points; }
            set
            {
                if (!OnConstruction)
                {
                    int temp = _points;
                    _points = value;
                    GameManager GM = null;
                    GameObject Object = GameObject.Find("GameManager");
                    if (Object != null)
                        GM = Object.GetComponent<GameManager>();
                    if (GM != null && Type != null && (Type == "C" || Type.IndexOf("A") != -1))
                        GM.SendPrincipal("Has tratado de modificar el poder de un clima o un aumento, lo cual no es permitido pues no tienen");
                    if (GM != null && CurrentPlace != "" && CurrentPlace != null)
                    {
                        GM.AddScore(LocationBoard, _points - temp);
                    }
                    if (PwrText != null)
                        PwrText.text = _points.ToString();
                }
                else
                {
                    int temp = _points;
                    _points = value;
                }
            }
        }

        public int OriginalPoints;
        public string description;
        public override string Range{get; set;}
        public string CurrentPlace;
        public override string Type{get; set;}
        public KindofCard unit;
        public Effect SuperPower;
        public bool LocationBoard; //true if its down
        public TextMeshProUGUI PwrText= new();
        
        public bool OnConstruction = true;
        private bool _Destroy;
        public bool Destroy
        {
            get
            {
                return _Destroy;
            }
            set
            {
                if( value==true)
                {
                    if(Displayed)
                    {
                        _Destroy= true;
                        Displayed = false;
                    }
                }
                else
                    _Destroy = false;

            }
        }
        public bool Displayed=false;

         public override List<IEffect> Effects{get; set;}
        public override Card GetCopy()
        {
            UnityCard card = new UnityCard(LocationBoard,Name,Power,Owner,unit,Type,SuperPower,Range,Artwork,description);
            card.Effects= Effects;
            return card;
        }
        
        public override bool Equals(object obj)
        {
            if(obj is UnityCard card)
            {
                if(this.Name == card.Name && this.Power == card.Power && this.Range == card.Range &&
                this.unit == card.unit && this.Type == card.Type && this.description == card.description &&
                this.SuperPower == card.SuperPower && this.Owner == card.Owner && Effects==card.Effects)
                    return true;
            }
            return false;
        }

        public UnityCard(bool LocationBoard ,string name ,int points,Compiler.Player Player,KindofCard unit,string type ,Effect ability,string attackRange, Sprite Img, string description)
        {
            this.Name = name;
            this.Power = points;
            OriginalPoints = points;
            this.description = description;
            this.Range = attackRange;
            this.Artwork = Img;
            this.Type = type;
            this.unit = unit;
            this.SuperPower = ability;
            this.LocationBoard = LocationBoard;
            this.Owner = Player;
            this.OnConstruction = false;
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
