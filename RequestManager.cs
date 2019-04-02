using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UE_Assistant
{
    public class RequestManager
    {
        private string Login { get; set; }
        private string Password { get; set; }
        private string StudentId { get; set; }
        private HttpClient Client { get; set; }
        private CookieContainer CookieContainer { get; set; }
        private HttpClientHandler Handler { get; set; }

        private Uri baseAddress;
        private string signInAddress;
        private string gradesAddress;

        public RequestManager(String login, String password)
        {
            this.Login = login;
            this.Password = password;

            this.baseAddress = new Uri("https://e-uczelnia.ue.katowice.pl/");
            this.signInAddress = "cas/login?service=https%3A%2F%2Fe-uczelnia.ue.katowice.pl%2Fwu%2Fj_spring_cas_security_check";
            this.gradesAddress = "wu/redirectToUrl?redirectUrlParameter=pages/studia/oceny.html";
        }

        public void SignIn()
        {
            this.CookieContainer = new CookieContainer();
            this.Handler = new HttpClientHandler() { CookieContainer = CookieContainer };
            this.Client = new HttpClient(Handler) { BaseAddress = baseAddress };

            Client.GetAsync("/").Result.EnsureSuccessStatusCode();

            var userData = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("username", Login),
                new KeyValuePair<string, string>("password", Password)
             };
            var headers = HtmlScrapper.GetSingInPostHeaders(Client.GetAsync(signInAddress).Result.Content.ReadAsStringAsync().Result);
            userData = userData.Concat(headers).ToList();

            var content = new FormUrlEncodedContent(userData);

            Client.PostAsync(signInAddress, content).Result.EnsureSuccessStatusCode();
            Client.GetAsync("/wu/start?&locale=pl").Result.EnsureSuccessStatusCode();

            StudentId = this.GetStudentId();

        }

        public List<Grade> GetGrades(int semester) // -1 return list of all grades
        {
            //Getting fields of study to collect essential data
            var fieldsOfStudyAddress = "/wsrest/rest/templating/student/kierunki";
            var fieldOfStudyjsonAsString = string.Format("{{\"id_studenta\":{0}}}", StudentId);

            var fieldOfStudyId = ((JArray)this.PostJson(fieldsOfStudyAddress,fieldOfStudyjsonAsString,true))
                                                                        [0]["id_kierunek_student"].ToString();
            //==================================================================================================

            //Getting editions of study to collect essential data
            var editionsOfStudyAddress = "/wsrest/rest/templating/student/edycje";
            var editionsJsonAsString = string.Format("{{\"id_studenta\":{0}, \"id_kierunek_student\":{1}}}",
                                             StudentId, fieldOfStudyId);

            var editionOfStudy = ((JArray)this.PostJson(editionsOfStudyAddress,editionsJsonAsString,false))
                                                                        [0]["id_edycja"].ToString();
            //==================================================================================================

            //Gettind semesters of given edition to collect essential data
            var semestersOfEditionAddress = "/wsrest/rest/templating/student/semestry";
            var semestersJsonAsString = string.Format("{{\"id_studenta\":{0}, \"id_kierunek_student\":{1}, \"id_edycja\":{2}}}",
                                             StudentId, fieldOfStudyId, editionOfStudy);

            var semesterOfEdition = ((JArray)this.PostJson(semestersOfEditionAddress, semestersJsonAsString, false))
                                                                        [2]["semestr"].ToString();



            this.Authenticate();

            return HtmlScrapper.GetGradesFromGradesPage(this.RenderGradesForSemester(fieldOfStudyId, editionOfStudy, "3", "\"pl\""));

        }

        private JToken PostJson(string address, string jsonAsString, bool needToAuthenticate)
        {
            if (needToAuthenticate)
                this.Authenticate().EnsureSuccessStatusCode();

            var content = new StringContent(jsonAsString, Encoding.UTF8, "application/json");
            var jsonResponse = JObject.Parse(Client.PostAsync(address, content).Result.Content.ReadAsStringAsync().Result);

            return jsonResponse["result"];
        }

        private string RenderGradesForSemester(string fieldOfStudyId, string editionId, string numberOfSemester, string language)
        {
            var renderGradesAddress = "/wsrest/rest/templating/renderTemplate";
            var jsonAsString = string.Format("{{\"id_studenta\":{0}, \"id_kierunek_student\":{1}, \"id_edycja\":{2}," +
            	                             " \"semestr\":{3}, \"klucz\":{4}, \"lang\":{5}}}",
                                             StudentId, fieldOfStudyId, editionId, numberOfSemester, "\"student.oceny\"", language);
            var content = new StringContent(jsonAsString, Encoding.UTF8, "application/json");
            var jsonResponse = JObject.Parse(Client.PostAsync(renderGradesAddress, content).Result.Content.ReadAsStringAsync().Result);
            return jsonResponse["result"].ToString();
        }

        private string GetStudentId()
        {
            var gradesPageHtml = Client.GetAsync(gradesAddress).Result.Content.ReadAsStringAsync().Result;
            return HtmlScrapper.GetStudentIdFromGradesPage(gradesPageHtml);
        }

        private HttpResponseMessage Authenticate()
        {
            var authenticateAddress = "/wsrest/rest/authenticate";
            return Client.GetAsync(authenticateAddress).Result;
        }


    }
}
