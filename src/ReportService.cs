using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateEngine.Docx;

namespace File_Manager
{
    public sealed class ReportService
    {
        const string templatePath = @"C:\Users\Roman\Desktop\geekbrains\c# file manager\File Manager 3\bin\Debug\net5.0\template.docx";
        public void GenerateReport(List<Data> dataList, string output = "")
        {
            if (string.IsNullOrWhiteSpace(output))
            {
                output = Path.Combine(Directory.GetCurrentDirectory(), "Info.docx");
            }

            if (!File.Exists(output))
            {
                File.Copy(templatePath, output);
            }

            List<TableRowContent> rows = new List<TableRowContent>();
            foreach (Data data in dataList)
            {
                rows.Add(new TableRowContent(new List<FieldContent>()
                {
                    new FieldContent("Path", data.path),
                    new FieldContent("Weight", data.weight.ToString()),
                    new FieldContent("Folders", data.folders.ToString()),
                    new FieldContent("Files", data.files.ToString()),
                }));
            }

            var valuesToFill = new Content(
                TableContent.Create("Table", rows));

            using (var outputDocument = new TemplateProcessor(output)
                .SetRemoveContentControls(true))
            {
                outputDocument.FillContent(valuesToFill);
                outputDocument.SaveChanges();
            }
        }
    }
}
