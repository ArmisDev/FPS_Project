using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.ProjectDebug
{
    public abstract class Debug_ConsoleCommand : ScriptableObject, IConsoleCommand
    {
        [SerializeField] private string commandWord = string.Empty;
        public string CommandWord => commandWord;

        public abstract bool Process(string[] args);
    }
}