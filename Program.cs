using System;
using System.IO;

namespace UE_Assistant
{
    class MainClass
    {
        //TODO:  
        //         -> modify GetGrades(int) in order to getting specifed semester grades
        //         -> replace RenderGradesForSemester() with modified PostJson
        private static string login = "top_secret_login";
        private static string password = "top_secret_password";
        public static void Main(string[] args)
        {
            RequestManager requester = new RequestManager(login, password);
            requester.SignIn();
            var gradeList = requester.GetGrades(-1);

            foreach (Grade grade in gradeList)
                Console.WriteLine("==================================\n" + grade.ToString());
        }
    }
}
