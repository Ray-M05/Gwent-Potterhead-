using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using LogicalSide;
using UnityEngine.UIElements;
using System;
using JetBrains.Annotations;
using System.Diagnostics;
using System.Threading;
using UnityEngine.Rendering;
public class GameManager : MonoBehaviour
{
    public GameObject PanelWinner;
    public TextMeshProUGUI WinnerTxt;
    public GameObject Visualizer;
    public GameObject Eff;
    public GameObject World;
    public GameObject Rounds;
    public GameObject RoundsEnemy;
    public TMP_Text Pwrplayer;
    public TMP_Text Pwroponent;
    public TextMeshProUGUI Message;
    public GameObject ButtonOK;
    public GameObject MessagePanel;
    public UnityEngine.UI.Button Pass;
    public GameObject prefabCard;
    public GameObject prefabLeader;
    private bool _Turn=true;
    public MenuGM Sounds;
    public Player P1;
    public Player P2;
    public GameObject PlayerZone;
    public GameObject EnemyZone;
    public bool CardFilter = false;


    private Queue<string> SMS;

    public bool Turn
    {
        get { return _Turn;}
        set 
        { 
            if(_Turn != value)
            {
                _Turn = value;
                Rotate();
            }
            if (Turn)
            {
                P1.SetedUp = P1.SetedUp;
                SendPrincipal("Turno de " + P1.name);
            }
            else
            {
                P2.SetedUp = P2.SetedUp;
                SendPrincipal("Turno de " + P2.name);
            }
            VisibilityGM();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SMS = new();
        SavedData data= null;
        Compilation compi= null;
        if (GameObject.Find("SoundManager")!= null)
        {
            data = GameObject.Find("SoundManager").GetComponent<SavedData>();
            compi = data.Compi;
        }
        if (data != null && !data.debug)
        {
            P1 = new Player(data.faction_1, data.name_1, true, compi.CardsPlayer1);
            P2 = new Player(data.faction_2, data.name_2,false, compi.CardsPlayer2);
        }
        else
        {
            P1 = new Player(1, "Gryffindor",true,null);
            P2 = new Player(2, "Slytherin",false, null);
        }
        SetupPLayers();
        Sounds = GameObject.Find("Menus").GetComponent<MenuGM>();
        Turn = true;
    }
    private void Update()
    {
        if (SMS != null)
        {
            if (SMS.Count > 0)
            {
                Send(SMS.Peek(), Message);
            }
            if (Input.anyKey && SMS.Count != 0)
            {
                GetPrincipal();
                Delay(1);
            }
        }
        else
            SMS = new();
        
    }
    private Sprite GiveMeBack(Player player)
    {
        if(player.faction== 1){
            return Resources.Load<Sprite>("gryffreverse");
        }
        return Resources.Load<Sprite>("slythreverse");
    }
    public void SetupPLayers()
    {
        GameObject deck = GameObject.Find("Deck");
        if (deck != null )
        {
            PlayerDeck setup = deck.GetComponent<PlayerDeck>();
            setup.deck = CardDataBase.GetDeck(P1);
            setup.Shuffle(setup.deck, P1.Cards.Count);
            setup.SetSprite(GiveMeBack(P1));
        }
        deck = GameObject.Find("DeckEnemy");
        if (deck != null)
        {
            PlayerDeck setup = deck.GetComponent<PlayerDeck>();
            setup.deck = CardDataBase.GetDeck(P2);
            setup.Shuffle(setup.deck, P2.Cards.Count);
            setup.SetSprite(GiveMeBack(P2));
        }
    }
    public void AddScore(bool Downboard, int value)
    {
        if(Downboard)
        {
            Pwrplayer.text= (System.Convert.ToInt32(Pwrplayer.text) + value).ToString();
        }
        else
            Pwroponent.text = (System.Convert.ToInt32(Pwroponent.text) + value).ToString();
    }
    private void Rotate()
    {
        World.transform.Rotate(0,0,180);
        Pwrplayer.transform.Rotate(0, 0, 180);
        Pwroponent.transform.Rotate(0, 0, 180);
        Pass.transform.Rotate(0, 0, 180);
    }
   
    public void VisibilityGM()
    {
        if (Turn)
        {
            for(int i= 0; i< PlayerZone.transform.childCount; i++)
            {
                PlayerZone.transform.GetChild(i).transform.GetChild(3).gameObject.SetActive(false);
            }
            for (int i = 0; i < EnemyZone.transform.childCount; i++)
            {
                EnemyZone.transform.GetChild(i).transform.GetChild(3).gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < PlayerZone.transform.childCount; i++)
            {
                PlayerZone.transform.GetChild(i).transform.GetChild(3).gameObject.SetActive(true);
            }
            for (int i = 0; i < EnemyZone.transform.childCount; i++)
            {
                EnemyZone.transform.GetChild(i).transform.GetChild(3).gameObject.SetActive(false);
            }
        }
    }
    int indexP =0; int indexE=0;
    public void EndRound()
    {
        int diff = Convert.ToInt32(Pwrplayer.text) - Convert.ToInt32(Pwroponent.text);
        if (diff > 0)
        {
            //Gana el P1
            Rounds.transform.GetChild(indexE).gameObject.SetActive(true);
            indexE++;
            SendPrincipal((P1.name + " Gano la ronda"));
            Turn = true;
        }
        else if(diff<0)
        {
            RoundsEnemy.transform.GetChild(indexP).gameObject.SetActive(true);
            indexP++;
            SendPrincipal(P2.name + " Gano la ronda");
            Turn =false;
        }
        else
        {
            bool alwayswin=false;
            string result = "";
            bool turno=false;
            if (P1.AlwaysAWinner == true)
            {
                Rounds.transform.GetChild(indexE).gameObject.SetActive(true);
                indexE++;
                result = ((P1.name + " Gano la ronda aplicando el efecto de su líder"));
                turno = true;
            }
            else
            {
                alwayswin= !alwayswin;
            }
            if (P2.AlwaysAWinner == true)
            {
                RoundsEnemy.transform.GetChild(indexP).gameObject.SetActive(true);
                indexP++;
                result= ((P2.name + " Gano la ronda aplicando el efecto de su líder"));
                turno = false;
            }
            else
            {
                alwayswin = !alwayswin;
            }
            if (!alwayswin)
            {
                if (!P1.AlwaysAWinner)
                {
                    Rounds.transform.GetChild(indexP).gameObject.SetActive(true);
                    indexP++;
                    RoundsEnemy.transform.GetChild(indexE).gameObject.SetActive(true);
                    indexE++;
                    turno = true;
                }
                SendPrincipal(("Ronda Empatada"));
            }
            else
            { 
                SendPrincipal(result); 
            }
            Turn = turno;
        }

        if (indexE == indexP && indexP == 2)
            EndGame("Ambos", null);
        else if (indexP == 2)
            EndGame(P2.name, P2);
        else if (indexE == 2)
            EndGame(P1.name, P1);
        Eff.GetComponent<Efectos>().ToCementery();


        string r = "";
        PlayerDeck deck = GameObject.Find("Deck").GetComponent<PlayerDeck>();
        if (P1.Stealer)
        {
            deck.InstanciateLastOnDeck(3, false);
            r = P1.name + " ha robado una carta de más gracias al efecto de su lider";
        }
        else
        {
            deck.InstanciateLastOnDeck(2, false);
        }
        deck = GameObject.Find("DeckEnemy").GetComponent<PlayerDeck>();
        if (P2.Stealer)
        { 
            deck.InstanciateLastOnDeck(3, false);
            r = P2.name + " ha robado una carta de más gracias al efecto de su lider";
        }
        else
            deck.InstanciateLastOnDeck(2, false);
        if (P1.Stealer && P2.Stealer)
            r = "Ambos jugadores han robado una carta de más gracias al efecto de su lider";
        if(r!= "")
        {
            SendPrincipal(r);
        }

        VisibilityGM();
        P1.Surrender = false;
        P2.Surrender = false;
    }
    public void PassedTurn()
    {
        if (Turn)
            P1.Surrender = true;
        else
            P2.Surrender = true;
        if (P1.Surrender && P2.Surrender)
            EndRound();
        else
        {
            Turn = !Turn;
        }
        
    }
    public void EndGame(string winner,Player Winner)
    {
        Sprite Win = Resources.Load<Sprite>("_bd85d92f-3322-4ebc-add3-3bebd08a0e69");
        if (winner == "Ambos")
            WinnerTxt.text = "El juego acaba en empate";
            
        else
        {
            WinnerTxt.text = winner + " ha ganado la partida";
            if (Winner.faction == 1)
                Win = Resources.Load<Sprite>("Gryffindor wins");
            else
                Win = Resources.Load<Sprite>("Slytherin wins");
        }
        PanelWinner.GetComponent<UnityEngine.UI.Image>().sprite = Win;
        PanelWinner.SetActive(true);   
    }
    public void Send(string message, TextMeshProUGUI Mess)
    {
        Mess.gameObject.SetActive(true);
        Mess.text = message;
        
    }
    public void EndMessage(TextMeshProUGUI Mess) 
    {
        Mess.gameObject.SetActive(false);
        Mess.text = "";
    }
    public void SendPrincipal(string s)
    {
        string sQ;
        SMS.TryPeek(out sQ);
        if (s!= sQ)
        SMS.Enqueue(s);
        Message.gameObject.SetActive(true);
        MessagePanel.SetActive(true);
    }
    public string GetPrincipal()
    {
        if (SMS.Count == 1)
        {
            MessagePanel.gameObject.SetActive(false);
        }
        return SMS.Dequeue();
    }
    public void OK()
    {
        WhichPlayer(Turn).SetedUp = true;
    }

    public Player WhichPlayer(bool b)
    {
        if (b == P1.P)
            return P1;
        return P2;
    }
    public void Delay(float seconds)
    {
        Thread.Sleep(Convert.ToInt32(seconds*1000));
    }
}
public class Player: Compiler.Player
{
    public int faction;
    public new string name;
    public int lifes;
    public bool Surrender;
    public bool P;
    private bool _seted;
    public List<UnityCard> Cards;
    public bool SetedUp
    {
        get
        {
            return _seted;
        }
        set
        {
            _seted= value;GameManager GM = GameObject.Find("GameManager").GetComponent<GameManager>();
            if(!_seted && GM.Turn== P)
            {
                GM.ButtonOK.SetActive(true);

            }
            else
            {
                GM.ButtonOK.SetActive(false);

            }
        }
    }
    private int _cards; 
    public bool Stealer;
    public bool AlwaysAWinner;
    public int cardsExchanged 
    {
        get
        {
            return _cards;
        }
        set
        {
            _cards = value;
            if (_cards == 2)
            {
                SetedUp = true;
            }
        }
    }
    public Player(int faction, string name, bool b, List<UnityCard> cards)
    {

        this.name = name;
        this.faction = faction;
        Surrender = false;
        this.P = b;
        Turn = b;
        SetedUp = false;
        cardsExchanged = 0;
        Cards = cards;
    }
}

