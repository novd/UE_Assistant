using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace UE_Assistant
{
    public static class HtmlScrapper
    {
        public static List<KeyValuePair<string,string>> GetSingInPostHeaders(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var inputs = document.DocumentNode.SelectNodes("//*[@id=\"fm1\"]/div[1]/div/div/div/div/input").ToList();

            var ListOfHeaders = new List<KeyValuePair<string, string>>();
            foreach(var input in inputs)
            {
                if(input.GetAttributeValue("type", "").Equals("hidden"))
                {
                    ListOfHeaders.Add(
                    new KeyValuePair<string, string>(
                        input.GetAttributeValue("name", "unknown"),
                        input.GetAttributeValue("value", "unknown")
                        ));
                }
            }


            return ListOfHeaders;
        }

        public static string GetStudentIdFromGradesPage(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var scriptContainingID = document.DocumentNode.SelectNodes("/html/body/script").First();

            int startIndex = scriptContainingID.InnerText.IndexOf("=") + 2;
            int stopIndex = scriptContainingID.InnerText.IndexOf(";");
            return scriptContainingID.InnerText.Substring(startIndex, stopIndex-startIndex);
        }

        public static List<Grade> GetGradesFromGradesPage(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);

            var listOfGradeInfos = document.DocumentNode.Descendants("tr")
                                   .Where(tr => tr.GetAttributeValue("style", "")
                                   .Contains("cursor:pointer"));

            var listofGradeDetails = document.DocumentNode.Descendants("tr")
                                     .Where(tr => tr.GetAttributeValue("id", "")
                                     .Contains("szczegoly"));

            List<Grade> gradesToReturn = new List<Grade>();
            foreach(var gradeInfo in listOfGradeInfos)
            {
                var gradeElements = gradeInfo.Descendants("td").Select(element => element.InnerText).ToArray();

                gradesToReturn.Add(new Grade(
                gradeElements[1],
                gradeElements[2],
                gradeElements[3],
                int.Parse(gradeElements[4]),
                DateTime.ParseExact(gradeElements[5], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                int.Parse(gradeElements[6]),
                gradeElements[7],
                gradeElements[8] ));
            }

            return gradesToReturn;
        }

    }
}
