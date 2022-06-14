using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Spectre.Console;

namespace File_Manager
{
    class InfoCommand : Command
    {
        public override string Name
        {
            get { return "info"; }
        }

        public override bool CanHandle(string cmd)
        {
            return cmd == Name;
        }
        public override void Handle(string path) // команда по выводу информации о папках или файлах 
        {
            if (File.Exists(path) == true)
            {
                var info = new FileInfo(path);

                AnsiConsole.Render(new BarChart()
                    
                    .Label("[darkcyan bold underline]File Info[/]")
                    .LeftAlignLabel()
                    .AddItem($"{info.Name} size in bytes", info.Length, Color.PaleGreen3));
            }
            else if (Directory.Exists(path) == true)
            {
                var info = new DirectoryInfo(path);
                int countFiles = 0;
                int countDirs = 0;
                FileCounter(info, ref countFiles, ref countDirs);
                AnsiConsole.Render(new BarChart()

                    .Label("[darkcyan bold underline]Catalogue Info[/]")
                    .LeftAlignLabel()
                    .AddItem($"{info.Name} sub directories:", countDirs, Color.Aquamarine1)
                    .AddItem($"{info.Name} files inside:", countFiles, Color.Aquamarine3));
                    
                AnsiConsole.Render(new BarChart()
                    .Label("[darkcyan bold underline]Catalogue Size in Bytes[/]")
                    .LeftAlignLabel()
                    .AddItem($"{info.Name} size in bytes:", DirSize(info), Color.PaleGreen3));

                var data = new Data();
                data.path = path;
                data.weight = DirSize(info);
                data.folders = countDirs;
                data.files = countFiles;

                Program.dataList.Add(data);
            }
        }
        private static void FileCounter(DirectoryInfo dir, ref int countFiles, ref int countDirs) // рекурсивный счетчик файлов
        {
            try
            {
                countFiles += dir.GetFiles().Length;
                countDirs += dir.GetDirectories().Length;
                var subDirs = dir.GetDirectories();
                foreach (var subDir in subDirs)
                {
                    FileCounter(subDir, ref countFiles, ref countDirs);
                }
            }
            catch (UnauthorizedAccessException)
            { }
        }
        private static long DirSize(DirectoryInfo dir) // получаем размер папки пройдя все файлы в ней и рекурсивно все папки
        {
            try
            {
                long size = 0;
                // размеры файлов
                FileInfo[] fis = dir.GetFiles();
                foreach (FileInfo fi in fis)
                {
                    size += fi.Length;
                }
                // размеры папок
                DirectoryInfo[] dis = dir.GetDirectories();
                foreach (DirectoryInfo di in dis)
                {
                    size += DirSize(di);
                }
                return size;
            }
            catch (UnauthorizedAccessException)
            { 
                return 0; 
            }
        }
    }
}
