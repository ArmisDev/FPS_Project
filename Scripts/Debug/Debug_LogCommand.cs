using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.ProjectDebug
{
    [CreateAssetMenu(fileName = "New Log Command", menuName = "Debug/DeveloperConsole/Commands/Log Command")]
    public class Debug_LogCommand : Debug_ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            string logText = string.Join(" ", args);
            Debug.Log(logText);
            return true;
        }
    }
}