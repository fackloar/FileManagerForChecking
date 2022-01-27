using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace File_Manager
{
    class HelpCommand : Command
    {
        public override string Name
        {
            get { return "help"; }
        }

        public override bool CanHandle(string cmd)
        {
            return cmd == Name;
        }
        public override void Handle(string path) // команда по выводу помощи на экран
        {
            var helpTable = new Table();
            helpTable.AddColumn("Command");
            helpTable.AddColumn("Argument");
            helpTable.AddColumn("Description");
            helpTable.AddRow("Help", "---", "Lists all available commands");
            helpTable.AddRow("GoTo", "Path to a directory OR name of any current sub directory", "Opens corresponding path");
            helpTable.AddRow("Up", "---", "Goes to an upper directory");
            helpTable.AddRow("Page", "Page number", "Goes to a corresponding page number");
            helpTable.AddRow("Copy", "1. Path of file or directory you want to copy 2. To 3. Path to a directory in which you want to copy", "Copies a file or a directory");
            helpTable.AddRow("Delete", "Path to a directory or a file", "Deletes a corresponding directory or file");
            helpTable.AddRow("Info", "Path to a directory or file OR name of any current subdirectory or file", "Writes information about a file or directory");
            helpTable.AddRow("Exit", "---", "Turns off the File Manager");
            helpTable.Alignment(Justify.Right);
            helpTable.RightAligned();
            AnsiConsole.Render(helpTable);
        }
    }
}
