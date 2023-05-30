using System.Net;
using System.Text;

namespace TreasurehuntApi.lib
{
    public static class GoogleSpreadSheet
    {
        public static (List<string>, List<string[]>) GetSheetAsCsvData(string spreadsheetId, string sheetId)
        {
            var url = $"https://docs.google.com/spreadsheets/d/{spreadsheetId}/export?format=csv&id={spreadsheetId}&gid={sheetId}";
            var stream = new MemoryStream(new WebClient().DownloadData(url));

            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                // Read the header line
                string headerLine = reader.ReadLine();
                var headers = headerLine?.Split(',').ToList();

                // Print the headers
                //Console.WriteLine("CSV Headers:");
                //foreach (string header in headers)
                //{
                //    Console.WriteLine(header);
                //}

                // Read and process the data lines
                Console.WriteLine("CSV Data:");
                string dataLine;

                var data = new List<string[]>();

                while ((dataLine = reader.ReadLine()) != null)
                {
                    string[] values = dataLine.Split(',');
                    data.Add(values);
                }

                return (headers, data);
            }
        }
    }
}
