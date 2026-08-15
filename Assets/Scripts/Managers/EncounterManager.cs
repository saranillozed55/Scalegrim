using UnityEngine;
using UnityEngine.InputSystem;

public class EncounterManager : MonoBehaviour
{
    //this script wants to initialize all of the properties in managers and should run before all of the others
    
    //Scriptable Objects to pass to EnemyEncounter;
    [SerializeField] private BlueprintRetriever blueprintRetriever;
    [SerializeField] private CardGroupRetriever cardGroupRetriever;

    [Header("Testing")]
    [SerializeField] private EnemyEncounterData enemyEncounterData; // this will not be a drag and drop, rather this will be determined after we click on it on the map
                                                                    // i believe, each Node on the map will hold their data and then it will send it to the designated managers
    private EnemyEncounter currentEncounter;


    private void Start()
    {
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        CombatManager.Instance.OnCombatTurnEnded += StartNextTurn;
    }

    private void UnsubscribeFromEvents()
    {
        if(CombatManager.Instance != null)
            CombatManager.Instance.OnCombatTurnEnded -= StartNextTurn;
    }

    //testing update
    private void Update()
    {
        if(Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            StartEncounter(); // want to try two different encounters later
        }
        if(Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            StartEncounter();
        }
    }

    private void StartEncounter()
    {
        currentEncounter = new EnemyEncounter(enemyEncounterData, 15, blueprintRetriever, cardGroupRetriever);
        currentEncounter.OnEncounterStart();
    }

    public void StartNextTurn()
    {
        currentEncounter.OnPrepareNextTurnHandler();
    }
    
}
