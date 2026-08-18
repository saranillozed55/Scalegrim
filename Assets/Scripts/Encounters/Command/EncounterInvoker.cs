using UnityEngine;

public static class EncounterInvoker 
{
    public static void ExecuteEncounterCommand(IEncounterCommand command)
    {
        command.ExecuteEncounter();
    }
}
