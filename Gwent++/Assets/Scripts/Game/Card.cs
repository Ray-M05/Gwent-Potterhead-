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

        /// <summary>
        /// Stores the owner of the card. The owner can only be set during construction, for example when you add a card from an owner to another.
        /// Attempts to change the owner post-construction will result in a warning message in the GameManager.
        /// </summary>
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
                GM.SendMessage($"No se puede cambiar el owner de las cartas (ReadOnly)");
                }
            }
        }
        

        /// <summary>
        /// The name of the card, which can only be set during construction.
        /// Any attempts to change the name post-construction will result in a warning in the GameManager.
        /// </summary>
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
                    GM.SendMessage($"No se puede cambiar el nombre de las cartas (ReadOnly)");
                }
            }
        }

        /// <summary>
        /// Represents the faction of the card.
        /// This value is only editable during construction like when setting a compiler card property. Any post-construction change will trigger a warning.
        /// </summary>
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
                    GM.SendMessage($"No se puede cambiar la faccion de las cartas (ReadOnly)");
                }
            }
        }


        /// <summary>
        /// Represents the card's power or strength. Adjustments to power trigger updates to in-game score.
        /// Certain card types (e.g., Weather, Raise) cannot have their power modified post-construction.
        /// </summary>
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
                        GM.SendMessage("Has tratado de modificar el poder de un clima o un aumento, lo cual no es permitido pues no tienen");
                    if (GM != null && CurrentPlace != "" && CurrentPlace != null)
                    {
                        GM.ActualScore(LocationBoard, _points - temp);
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


        /// <summary>
        /// The original power value of the card, before any in-game modifications.
        /// </summary>
        public int OriginalPoints;

        /// <summary>
        /// A brief description of the card's abilities or role in the game.
        /// </summary>
        public string description;


        /// <summary>
        /// Defines the range of the card's effect or attack. This can only be set during construction.
        /// Post-construction modifications will trigger a warning message.
        /// </summary>
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
                    GM.SendMessage($"No se puede cambiar el rango de las cartas (ReadOnly)");
                }
            }
        }


        /// <summary>
        /// It defines the card's role and category in the game.
        /// This property is only editable during construction.
        /// </summary>
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
                    GM.SendMessage($"No se puede cambiar el tipo de las cartas (ReadOnly)");
                }
            }
        }

        /// <summary>
        /// Represents the current position of the card when invoke.
        /// </summary>
        public string CurrentPlace;
        private string _type;
        public string TypeOfCard{get; set;}



        /// <summary>
        /// Specifies the specific category of the card (Golden, Silver, None).
        /// </summary>
        public KindofCard unit;

        /// <summary>
        /// Defines the card's unique ability from effects manager.
        /// </summary>
        public Effect SuperPower;

        /// <summary>
        /// Indicates whether the card is located in the board.
        /// True if its the first player.
        /// </summary>
        public bool LocationBoard;
        public TextMeshProUGUI PwrText= new();
        

        /// <summary>
        /// Indicates whether the card is still in the construction phase. This flag controls whether certain properties can be modified.
        /// </summary>
        public bool OnConstruction = true;

        /// <summary>
        /// Controls whether the card should be destroyed and removed from play.
        /// </summary>
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

        /// <summary>
        /// A list of the effects from compilation phase associated with the card.
        /// </summary>
        public override List<IEffect> Effects{get; set;}
        public override Card GetCopy()
        {
            UnityCard card = new UnityCard(LocationBoard,Name, Type, OriginalPoints,Owner,unit,TypeOfCard,SuperPower,Range,Artwork,description);
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
