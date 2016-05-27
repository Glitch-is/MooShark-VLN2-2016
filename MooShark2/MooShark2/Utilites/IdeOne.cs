using System.Net;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MooShark2.Utilites
{
    public class IdeOne
    {
        private string access_token = "44714f226100e663b3facf97fa19ea23";
        private string url = "http://29fa545a.compilers.sphere-engine.com/api/v3/";

        public IdeOne()
        {

        }

        public int runCode(string code, int language, string input)
        {
            var request = (HttpWebRequest)WebRequest.Create(url + "submissions?access_token=" + access_token);

            string postData = "&sourceCode=" + code;
            postData += "&language=" + language;
            postData += "&input=" + input;

            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            if ((int)response.StatusCode == 200)
                return JsonConvert.DeserializeObject<Dictionary<string, int>>(responseString)["id"];
            else
                return -1;
        }

        public Dictionary<string, string> getCodeStatus(int id)
        {

            var request = (HttpWebRequest)WebRequest.Create(url + "submissions/" + id + "?withOutput=true&withCmpinfo=true&access_token=" + access_token);

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
        }

        public Dictionary<string, string> getAllLanguages()
        {

            var request = (HttpWebRequest)WebRequest.Create(url + "languages?access_token=" + access_token);

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
        }

        public string getLanguageFromId(int id)
        {
            switch (id)
            {
                case 7:
                    return "Ada";
                case 4:
                case 99:
                case 116:
                    return "Python";
                case 17:
                    return "Ruby";
                case 40:
                    return "SQL";
                case 29:
                    return "Php";
                case 54:
                    return "Perl";
                case 112:
                case 35:
                    return "Javascript";
                case 10:
                    return "Java";
                case 11:
                case 1:
                case 44:
                    return "C++";
                case 27:
                    return "C#";
                default:
                    return "Other";
            }
        }
    }
}
