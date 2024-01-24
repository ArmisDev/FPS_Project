using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.ProjectDebug
{
    public interface IConsoleCommand
    {
        string CommandWord { get; }
        bool Process(string[] args);
    }
}
