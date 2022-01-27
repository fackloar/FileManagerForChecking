using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace File_Manager
{
    class DeleteCommand : Command
    {
        public override string Name
        {
            get { return "delete"; }
        }

        public override bool CanHandle(string cmd)
        {
            return cmd == Name;
        }
        public override void Handle(string path) // команда для удаления папок и файлов
        {
            if (File.Exists(path) == true)
            {
                File.Delete(path);
            }
            else if (Directory.Exists(path) == true)
            {
                string[] files = Directory.GetFiles(path);
                if (files.Length == 0)
                {
                    Directory.Delete(path);
                }
                else
                {
                    
                    Directory.Delete(path, recursive: true);
                }
            }
        }
    }
}
