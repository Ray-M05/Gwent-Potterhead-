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
        public Sprite Artwork;
        private Compiler.Player _owner;
        public override Compiler.Player Owner
        {
            get { return _owner; }
            set
            {
                if(OnConstruction)
                    _owner = value;
                else
                {
                GameManager GM = null;
                GameObject Object = GameObject.Find("GameManager");
                if (Object != null)
                    GM = Object.GetComponent<GameManager>();
                GM.SendPrincipal($"No se puede cambiar el owner de las cartas (ReadOnly)");
                }
            }
        }
        
        private string _name;
        public override string Name
        {
            get { return _name; }
            set
            {
                if (OnConstruction)
                    _name = value;
                else
                {
                    GameManager GM = null;
                    GameObject Object = GameObject.Find("GameManager");
                    if (Object != null)
                        GM = Object.GetComponent<GameManager>();
                    GM.SendPrincipal($"No se puede cambiar el nombre de las cartas (ReadOnly)");
                }
            }
        }
        private string _faction;
        [SerializeField]public override string Faction
        {
            get { return _faction; }
            set
            {
                if (OnConstruction)
                    _faction = value;
                else
                {
                    GameManager GM = null;
                    GameObject Object = GameObject.Find("GameManager");
                    if (Object != null)
                        GM = Object.GetComponent<GameManager>();
                    GM.SendPrincipal($"No se puede cambiar la faccion de las cartas (ReadOnly)");
                }
            }
        }
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
                    if (GM != null && TypeOfCard != null && (TypeOfCard == "C" || TypeOfCard.IndexOf("A") != -1))
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
        private string _range;
        public override string Range
        {
            get { return _range; }
            set
            {
                if (OnConstruction)
                    _range = value;
                else
                {
                    GameManager GM = null;
                    GameObject Object = GameObject.Find("GameManager");
                    if (Object != null)
                        GM = Object.GetComponent<GameManager>();
                    GM.SendPrincipal($"No se puede cambiar el rango de las cartas (ReadOnly)");
                }
            }
        }
        public override string Type 
        {
             get { return _type; }
            set
            {
                if (OnConstruction)
                    _type = value;
                else
                {
                    GameManager GM = null;
                    GameObject Object = GameObject.Find("GameManager");
                    if (Object != null)
                        GM = Object.GetComponent<GameManager>();
                    GM.SendPrincipal($"No se puede cambiar el tipo de las cartas (ReadOnly)");
                }
            }
        }
        public string CurrentPlace;
        private string _type;
        public string TypeOfCard{get; set;}
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
            UnityCard card = new UnityCard(LocationBoard,Name, Type, Power,Owner,unit,TypeOfCard,SuperPower,Range,Artwork,description);
            card.Effects= Effects;
            card.OnConstruction = true;
            card.Faction= Faction;
            card.OnConstruction = false;
            return card;
        }
        
        //public bool Equals(object obj)
        //{
        //    if(obj is UnityCard card)
        //    {
        //        if(this.Name == card.Name && this.Power == card.Power && this.Range == card.Range &&
        //        this.unit == card.unit && this.Type == card.Type && this.description == card.description &&
        //        this.SuperPower == card.SuperPower && this.Owner == card.Owner && Effects==card.Effects)
        //            return true;
        //    }
        //    return false;
        //}

        public UnityCard(bool LocationBoard ,string name, string Type ,int points,Compiler.Player Player,KindofCard unit,string typeofcard ,Effect ability,string attackRange, Sprite Img, string description)
        {
            this.Name = name;
            this.Power = points;
            OriginalPoints = points;
            this.description = description;
            this.Range = attackRange;
            this.Artwork = Img;
            this.TypeOfCard = typeofcard;
            this.Type = Type;
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
