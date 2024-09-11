using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace LogicalSide
{
    /// <summary>
    /// The SavedData class is responsible for storing and persisting game data such as player names and factions.
    /// </summary>
    public class SavedData : MonoBehaviour
    {
        public string name_1 = "";
        public string name_2 = "";
        public int faction_1 = 0;
        public int faction_2 = 0;

        public static SavedData Instance;
        public TMP_InputField Name1;
        public TMP_InputField Name2;
        public bool debug = false;



        public Compilation Compi;

        /// <summary>
        /// This method ensures that the class follows the singleton pattern by either preserving or destroying instances accordingly.
        /// </summary>
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        
        
        

    }
    
    
}
