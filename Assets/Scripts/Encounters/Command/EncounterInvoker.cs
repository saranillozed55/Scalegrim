using UnityEngine;
using Zeedo.LS.Encounter.Interfaces;

namespace Zeedo.LS.Encounter.Commmand
{
    public static class EncounterInvoker
    {
        public static void ExecuteEncounterCommand(IEncounter command)
        {
            command.ExecuteEncounter();
        }
    }
}
