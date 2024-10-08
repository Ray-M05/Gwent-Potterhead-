using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LogicalSide;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// The MenuGM class handles the game menu operations, including volume adjustments, panel transitions, and user interactions.
/// </summary>
public class MenuGM : MonoBehaviour
{
    [Header("Options")]
    public Slider Volume;
    public Slider VolumeFX;
    public AudioMixer mixer;
    [Header("Panels")]
    public GameObject MainPanel;
    public GameObject PanelP1;
    public GameObject PanelP2;
    public GameObject CompilerPanel;
    public AudioClip ClickSound;
    public AudioClip ErrorSound;
    public AudioSource FXsource;
    public SavedData SoundGM;
    public GameObject ButtonSlyth1;
    public GameObject ButtonGryff1;
    private void Awake()
    {
        if (Volume != null && VolumeFX != null)
        {
            Volume.onValueChanged.AddListener(ChangeVolumeMaster);
            VolumeFX.onValueChanged.AddListener(ChangeVolumeFX);
        }
        FXsource = GameObject.Find("SoundManager").GetComponent<AudioSource>();
        SoundGM = GameObject.Find("SoundManager").GetComponent<SavedData>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(MainPanel.activeSelf)
            {
                MainPanel.SetActive(false);
            }
            else
            OpenPanel(MainPanel);
        }
    }
    public GameObject PanelSaved;
    public void SaveThis(GameObject Panel)
    {
        PanelSaved = Panel;
    }

    public void ComeBack()
    {
        OpenPanel(PanelSaved);
    }


    public void OpenPanel(GameObject panel)
    {
        if(MainPanel!=null)
        MainPanel.SetActive(false);
        if (CompilerPanel != null)
            CompilerPanel.SetActive(false);
        if(PanelP1 != null)
        PanelP1.SetActive(false);
        if(PanelP2 != null)
        PanelP2.SetActive(false);
        
        panel.SetActive(true);
        PlaySoundButton();
        if (panel == PanelP1)
            SoundGM.Name1= GameObject.Find("Name1").GetComponent<TMP_InputField>();
        else if (panel == PanelP2)
            SoundGM.Name2 = GameObject.Find("Name2").GetComponent<TMP_InputField>();
    }
    public void NextBtn(GameObject Panel)
    {
        if (SoundGM.faction_1 != 0 && SoundGM.name_1 != "")
            OpenPanel(Panel);
        else
            PlayError();
    }
    public void Play(bool debug)
    {
        if ((SoundGM.faction_2 != 0 && SoundGM.name_2 != ""))
            SceneManager.LoadScene(1);
        else if (debug)
        {
            SoundGM.debug = true;
            SceneManager.LoadScene(1);
        }
        else
            PlayError();
    }
    public void RestartGame()=> SceneManager.LoadScene(0);
    
    public void Faction2OnClick(int Faction)
    {
        SoundGM.faction_2 = Faction;
    }
    public void Faction1OnClick(int Faction)
    {
        SoundGM.faction_1 = Faction;
    }
    public void NameCompleted(int P)
    {
        if (P == 1)
        {
            SoundGM.name_1 = SoundGM.Name1.text;
        }
        else
        {
            SoundGM.name_2 = SoundGM.Name2.text;
        }
    }

    public void ChangeVolumeMaster(float volume)
    {
        mixer.SetFloat("VolMaster", volume);
    }
    public void ChangeVolumeFX(float volume)
    {
        mixer.SetFloat("VolFX", volume);
    }
    public void PlaySoundButton()
    {
        FXsource.PlayOneShot(ClickSound);
    }
    public void PlayError()
    {
        FXsource.PlayOneShot(ErrorSound);
    }
    

    public void Exit()
    {
        Application.Quit();
    }
}
