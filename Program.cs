using System.Net;
using System.Text;
using System.Text.RegularExpressions;
namespace RegularExpressions
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            string content = string.Empty;
            Console.WriteLine("Введите ссылку:");
            string tempUrl = Console.ReadLine();
            List<string> urls = new List<string>();
            request = (HttpWebRequest)WebRequest.Create(tempUrl);
            response = (HttpWebResponse)request.GetResponse();
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
            {
                content = streamReader.ReadToEnd();
            }
            string pattern = @"<img[^>]+?src=['""](.*?)['""][^>]*>";
            var matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);
            foreach (Match tmpMath in matches)
            {
                Console.WriteLine(tmpMath.Groups[1]);
                Console.WriteLine();
            }
        }
    }
}