using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace File_Manager
{
    public class GoToCommand : Command
    {
        public override string Name
        {
            get { return "goto"; }
        }
        public override bool CanHandle(string cmd)
        {
            return cmd == Name;
        }
        public override void Handle(string path) // команда по переходу в другой каталог по полному пути либо по названию подпапки из текущего активного каталога
        {
            PrintList printList = new PrintList();
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.SetCurrentDirectory(path);
                    printList.ListDirsAndFiles();
                }
                else
                {
                    Directory.GetCurrentDirectory();
                    printList.ListDirsAndFilesFromInput(path);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Couldn't access path: Access Denied");
            }
        }
    }
}
