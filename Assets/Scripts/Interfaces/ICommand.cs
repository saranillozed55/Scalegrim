using UnityEngine;

public interface ICommand
{
    public void Execute();
    public bool IsFinished { get; }
}
