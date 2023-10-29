using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KB.ConsoleCompanion.DataModels
{
    public class ConsoleCommand
    {
        public enum ECommandType : ushort
        {
            Info,
            UserInput,
            Warning,
            Error,
        }
        
        public ConsoleCommand(string command, ECommandType type)
        {
            Command = command;
            Type = type;
        }
        
        public string Command { get; }
        public ECommandType Type { get; }
    }
}
