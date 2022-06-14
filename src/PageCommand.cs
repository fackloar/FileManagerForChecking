using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace File_Manager
{
    class PageCommand : Command
    {
        public override string Name
        {
            get { return "page"; }
        }

        public override bool CanHandle(string cmd)
        {
            return cmd == Name;
        }
        public override void Handle(string pageNum) // команда для переключения текущей страницы
        {
            List<Tree> pages = PrintList.Pages;
            int totalPages = PrintList.TotalPages;
            bool tryParse = Int32.TryParse(pageNum, out int parsedPageNum);
            if (tryParse == true && parsedPageNum <= totalPages && parsedPageNum > 0)
            {
                Console.Clear();
                var rule = new Rule("[red]File Catalogue:[/]");
                rule.Alignment = Justify.Left;
                rule.RuleStyle("purple dim");
                AnsiConsole.Render(rule);
                AnsiConsole.Render(pages[parsedPageNum-1]);
                AnsiConsole.Render(new Markup($"[purple dim]Showing Page {parsedPageNum} of {totalPages}[/]"));
                Console.WriteLine();
            }
            else
            {
                AnsiConsole.Render(new Markup($"[red]Something is wrong! Write page number from 1 to {totalPages}[/]"));
                Console.WriteLine();
            }
        }
    }
}
