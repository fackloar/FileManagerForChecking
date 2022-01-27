using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Configuration;
using System.Xml.Serialization;
using Spectre.Console;
using System.Text;

namespace File_Manager
{
    class Program
    {
        public static string userCommand { get; set; }
        public static List<Data> dataList = new List<Data>();
        static void Main(string[] args)

        {
            Console.OutputEncoding = Encoding.UTF8;
            string lastStateSave;
            PrintList printList = new PrintList(); // класс для вывода списка 
            Properties properties = new Properties(); // класс настроек
            properties.SetProperetiesPath(); 
            if (File.Exists(properties.ProperetiesPath)) //если файл настроек уже существует - загрузка настроек
            {
                properties.LoadSettings();
            }
            if (properties.LastState != null) // если существует настройка с последним активным каталогом - загружаем его
            {
                Directory.SetCurrentDirectory(properties.LastState);
                printList.ListDirsAndFiles();
            }
            else //в другом случае проводим первый старт программы
            {
                printList.InitialStart();
                var helpPanel = new Table();
                helpPanel.AddColumn("write help to get controls");
                helpPanel.Collapse();
                helpPanel.Alignment(Justify.Right);
                helpPanel.RightAligned();
                helpPanel.HeavyBorder();
                AnsiConsole.Render(helpPanel);
                Properties.PageLength = 100;
            }
            List<Command> commands = new List<Command>(); // создаем список доступных команд
            commands.Add(new GoToCommand());
            commands.Add(new CopyCommand());
            commands.Add(new DeleteCommand());
            commands.Add(new InfoCommand());
            commands.Add(new HelpCommand());
            commands.Add(new PageCommand());
            while (true)
            {
                var table = new Table();
                userCommand = AnsiConsole.Ask<string>("Command Line:");
                string userCommandToLowerCase = userCommand.ToLower();
                if (userCommandToLowerCase == "exit") // комманда выведена отдельно вне списка т.к. не имеет аргументов и исполняется в одну строку
                {
                    break;
                }
                else if (userCommandToLowerCase == "up") //тоже вне спика т.к. нет аргументов и метод исполнения внутри класса PrintList
                {
                    printList.GoUp();
                }
                
                var parsedCommand = Regex.Match(userCommandToLowerCase, @"^([\w\-]+)").ToString(); // считывание команды
                var parsedArgument = userCommandToLowerCase.Split(' ', 2).Skip(1).FirstOrDefault(); // считывание аргумента

                foreach (Command command in commands)
                {
                    if (command.CanHandle(parsedCommand))
                    {
                        command.Handle(parsedArgument);
                    }
                }
                
            }
            lastStateSave = Directory.GetCurrentDirectory();
            var reportService = new ReportService();
            reportService.GenerateReport(dataList);
            properties.Save(lastStateSave, Properties.PageLength); // сохранение настроек при завершении программы

        }
    }
}
