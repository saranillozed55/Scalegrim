using System.Collections.Generic;
using UnityEngine;

public class CardAttackInvoker
{
    private Queue<ICommand> _commandQueue = new Queue<ICommand>();
    private ICommand _currentCommand;


    public void EnqueueCommand(ICommand command)
    {
        _commandQueue.Enqueue(command);
    }

    public void Tick()
    {
        if (_currentCommand != null && !_currentCommand.IsFinished) return;

        if (_commandQueue.Count > 0)
        {
            _currentCommand = _commandQueue.Dequeue();
            _currentCommand.Execute();
        }

        if (CombatManager.Instance.IsInCombat && _commandQueue.Count <= 0)
        {
            //CombatManager.Instance.EndCombat();
        }
    }
}
