using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Spectre.Console;

namespace File_Manager
{
    class CopyCommand : Command
    {
        public override string Name
        {
            get { return "copy"; }
        }

        public override bool CanHandle(string cmd)
        {
            return cmd == Name;
        }
        public override void Handle(string input) // команда по копированию папок или файлов в указанную папку
        {

            string[] splitInput = input.Split(" to ");
            string source = splitInput[0];
            string target = splitInput[1];
            if (Directory.Exists(source) == true) // если копируем папку в папку
            {
                Directory.CreateDirectory(target);
                CopyDir(source, target);
                AnsiConsole.Render(new Markup("[bold green]Copying Complete[/]"));
                Console.WriteLine();

            }
            else if ((File.Exists(source) == true && Directory.Exists(target) == false)) // если копируем файл в файл
            {
                File.Copy(source, target, true);
                AnsiConsole.Render(new Markup("[bold green]Copying Complete[/]"));
                Console.WriteLine();
            }
            else if ((File.Exists(source) == true && Directory.Exists(target) == true) || (Directory.Exists(source) == true && File.Exists(target) == true)) // если копируем папку в файл или файл в папку
            {
                AnsiConsole.Render(new Markup("[bold red]Input two files or two directories[/]"));
                Console.WriteLine();
            }
        }



        private static void CopyDir(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target) // метод для рекурсивного копирования всего внутри папки 
        {
            Directory.CreateDirectory(target.FullName);

            // копируем файлы
            foreach (FileInfo fi in source.GetFiles())
            {
                AnsiConsole.Progress().Start(ctx =>
                {
                    var task = ctx.AddTask($"[green]Copying {target.FullName}\\{fi.Name}[/]"); 
                    while (!ctx.IsFinished)
                    {
                        task.Increment(1.5);
                    }
                });
                try
                {
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine($"Couldn't copy {target.FullName}: Access Denied");
                }
            }

            // копируем подпапки
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                try
                {
                    DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                    CopyAll(diSourceSubDir, nextTargetSubDir);
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine($"Couldn't access directory: Access Denied");
                }
            }
        }
    }
}
