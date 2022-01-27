using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace File_Manager
{
    class PrintList
    {
        public string[] dirs { get; set; }
        public string[] files { get; set; }
        public List<string> dirNames { get; set; }
        public List<string> fileNames { get; set; }
        public List<string> root { get; set; }
        public Queue<string> subLevel { get; set; }
        public static List<Tree> Pages { get; set; }
        public static int TotalPages { get; set; }

        public PrintList()
        {
            dirNames = new List<string>();
            fileNames = new List<string>();
            root = new List<string>();
            subLevel = new Queue<string>();
        }

        public void InitialStart() //первый запуск либо самая верхняя точка программы где выводятся доступные жесткие диски
        {
            var rule = new Rule("[red]Drive Catalogue:[/]:");
            rule.Alignment = Justify.Left;
            rule.RuleStyle("purple dim");
            var table = new Table();
            table.Expand();
            table.AddColumn("[darkcyan]Drives:[/]");

            DriveInfo[] di = DriveInfo.GetDrives();
            foreach (DriveInfo drive in di)
            {
                table.AddRow(new Markup($"[blue]{drive.Name}[/]"));
            }
            AnsiConsole.Render(rule);
            AnsiConsole.Render(table);
        }
        public void ListDirsAndFiles() //основной метод по выводу на экран списка файлов и каталогов, здесь же прописан почти весь UI
        {

            var rule = new Rule("[red]File Catalogue:[/]");
            rule.Alignment = Justify.Left;
            rule.RuleStyle("purple dim");
            RefreshDirs();
            Console.Clear();
            DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            Queue<int> subElementsCounters = new Queue<int>(); // очередь для счетчиков под-элементов основной папки
            foreach (string dirName in dirNames) //получаем два списка: 1) папки и файлы внутри активного каталога 2) папки и файлы внутри каждой папки основного каталога
            {
                root.Add($@"[darkcyan]{ParseForAnsi(dirName)}[/]"); 
                int subElementsCounter = 0;
                try
                {
                    foreach (string subDir in Directory.GetDirectories(dirName))
                    {
                        //Console.WriteLine($"    {subDir}");
                        subLevel.Enqueue($@"[steelblue]{ParseForAnsi(subDir)}[/]");
                        subElementsCounter++;
                    }

                    foreach (string fileInSubDir in Directory.GetFiles(dirName))
                    {
                        //Console.WriteLine($"    {fileInSubDir}");

                        subLevel.Enqueue($"[steelblue]{ParseForAnsi(fileInSubDir)}[/]");
                        subElementsCounter++;
                    }
                    subElementsCounters.Enqueue(subElementsCounter);
                }
                catch (UnauthorizedAccessException)
                {
                    root.Add("[red]Access Denied[/]");
                }
            }
            foreach (string fileName in fileNames)
            {
                try
                {
                    //Console.WriteLine(fileName);
                    root.Add($@"[darkcyan]{ParseForAnsi(fileName)}[/]");
                }
                catch (UnauthorizedAccessException)
                {
                    root.Add("[red]Access Denied[/]");
                }
            }
            int pageLength = Properties.PageLength;
            int elements = root.Count + subLevel.Count; //общее количество элементов
            int totalPages = elements / pageLength + 1; // количество страниц
            List<Tree> pages = new List<Tree>(); 
            int currentElements = 0; // текущее количество элементов на странице
            int mainDirs = 0; // основные папки активного каталога
            bool stoppedInSubfolder = false; // проверка на то, остановилась ли прошлая страница посреди вывода подпапок
            int stopIndex = 0; // индекс остановки
            int stopCounter = 0; // счетчик после остановки
            string tempSubSaved = "";  // сохранение последней подпапки или файла для отображение его в верном каталоге
            for (int pageNumber = 0; pageNumber < totalPages; pageNumber++)
            {
                var page = new Tree($"[blue]{directoryInfo.FullName}[/]"); // создание страницы
                page.Style("blue on black");
                page.Guide(TreeGuide.BoldLine);
                while ((mainDirs < root.Count) & (currentElements < pageLength))
                {
                    if (stoppedInSubfolder == false)
                    {

                        var main = page.AddNode(root[mainDirs]);
                        currentElements++;
                        mainDirs++;
                        if (mainDirs == root.Count)
                        {
                            continue;
                        }
                        if (root[mainDirs] == "[red]Access Denied[/]")
                        {
                            mainDirs++;
                            continue;
                        }
                        subElementsCounters.TryDequeue(out int tempCounter);
                        for (int subDirs = 0; subDirs < tempCounter; subDirs++)
                        {
                            if (tempSubSaved != "")
                            {
                                var sub = main.AddNode(tempSubSaved);
                                tempSubSaved = "";
                                subDirs++;
                            }
                            if (subLevel.TryDequeue(out string tempSub) == true)
                            {
                                var sub = main.AddNode(tempSub);
                                currentElements++;
                                if (currentElements >= pageLength)
                                {
                                    stoppedInSubfolder = true;
                                    sub = main.AddNode("...");
                                    stopIndex = mainDirs - 1;
                                    stopCounter = tempCounter;
                                    break;
                                }
                            }

                            else break;
                        }
                    }
                    else
                    {
                        var main = page.AddNode(root[stopIndex]);
                        var sub = main.AddNode("...");
                        for (int subDirs = 0; subDirs < stopCounter; subDirs++)
                        {
                            if (subLevel.TryDequeue(out string tempSub) == true)
                            {
                                var twoNames = ParseForNameCheck(root.ToArray(), stopIndex, tempSub);
                                if (twoNames[0] != twoNames[1]) // проверяем одинаковый ли путь и если нет выходим из цикла
                                {
                                    tempSubSaved = tempSub;
                                    stoppedInSubfolder = false;
                                    break;
                                }
                                sub = main.AddNode(tempSub);
                                currentElements++;
                                if (currentElements >= pageLength)
                                {
                                    stoppedInSubfolder = true;
                                    stopIndex = mainDirs - 1;
                                    break;
                                }
                                else
                                {
                                    stoppedInSubfolder = false;
                                }
                            }
                            else break;
                        }
                    }

                }
                pages.Add(page);
                currentElements = 0;

            }
            Pages = pages;
            TotalPages = totalPages;
            AnsiConsole.Render(rule);
            AnsiConsole.Render(pages[0]);
            AnsiConsole.Render(new Markup($"[purple dim]Showing Page 1 of {totalPages}[/]"));
            Console.WriteLine();
        }



        public void ListDirsAndFilesFromInput(string input) //метод позволяет передвигать по каталогу без ввода полного пути, только названия подкаталога в текущем активном каталоге
        {
            RefreshDirs();
            input = input.ToLower();
            foreach (string name in dirNames)
            {
                var parsedName = name.ToLower();
                if (parsedName == input)
                {
                    Directory.SetCurrentDirectory(name);
                    break;
                }
            }
            ListDirsAndFiles();
        }
        public void GoUp() // метод позволяет переходить "вверх" по каталогам, к родителю текущего активного
        {
            DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory());
            if (di.Parent != null)
            {
                var parent = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
                Directory.SetCurrentDirectory(parent);
                ListDirsAndFiles();
            }
            else
            {
                Console.Clear();
                InitialStart();
            }
        }
        private void RefreshDirs() // метод обновляет текущие папки и файлы, очищая списки и заполняя новыми 
        {
            dirs = Directory.GetDirectories(Directory.GetCurrentDirectory(), ".", SearchOption.TopDirectoryOnly);
            files = Directory.GetFiles(Directory.GetCurrentDirectory(), ".", SearchOption.TopDirectoryOnly);
            dirNames.Clear();
            fileNames.Clear();
            root.Clear();
            subLevel.Clear();
            for (int i = 0; i < dirs.Length; i++)
            {
                var di = new DirectoryInfo(dirs[i]).Name;
                dirNames.Add(di);
            }
            for (int i = 0; i < files.Length; i++)
            {
                var fi = new FileInfo(files[i]).Name;
                fileNames.Add(fi);
            }
        }

        private string ParseForAnsi(string path) // парсинг названий для правильного отображения с текущей реализацией UI
        {
            var parsedFile = path.Replace('[', '{');
            parsedFile = parsedFile.Replace(']', '}');
            return parsedFile;
        }

        private string[] ParseForNameCheck(string[] root, int stopIndex, string tempSub) // парсинг путей для сравнения названия папки
        {
            var tempRoot = root[stopIndex].Split(']', 2);
            var tempRootName = tempRoot[1].Split('[', 2);
            var tempParseSub = tempSub.Split(']', 2);
            var tempDoubleParseSub = tempParseSub[1].Split('\\', 2);
            string[] twoNames = new string[] { tempRootName.First(), tempDoubleParseSub.First() };
            return twoNames;
        }
    }
}
