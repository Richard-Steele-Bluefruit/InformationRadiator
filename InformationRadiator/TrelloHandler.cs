using System;
using System.IO;
using System.Net;

namespace InformationRadiator
{
    class TrelloHandler
    {
        private const string ToDoAPIRequest = "https://api.trello.com/1/cards/5dcd73735d7a1a4e0d47dcef/desc?key=d9d235426acb16e21c4aded93259e918&token=06cd41b5191799dd60d2da56917c26c22cfa127e7221b76e210018a2efea746f";
        private string mToDoDisplayInfo;

        private const string CalendarAPIRequest = "https://api.trello.com/1/cards/5dcec79ff4f4aa3919bf592d/desc?key=d9d235426acb16e21c4aded93259e918&token=06cd41b5191799dd60d2da56917c26c22cfa127e7221b76e210018a2efea746f";
        private string mCalendarDisplayInfo;

        public TrelloHandler()
        {
            CallTrelloAPI();
        }

        public string ToDo
        {
            get { return mToDoDisplayInfo; }
        }

        public string Calendar
        {
            get { return mCalendarDisplayInfo; }
        }

        public void CallTrelloAPI()
        {
            mToDoDisplayInfo = GetAPIResponse(ToDoAPIRequest);
            mCalendarDisplayInfo = GetAPIResponse(CalendarAPIRequest);
        }
        
        private string GetAPIResponse(string _api)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebRequest wr = WebRequest.Create(_api);
            HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseString = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            return ParseResponse(responseString);
        }

        private string ParseResponse(string _response)
        {
            string[] responseItems = _response.Split('"');

            try
            {
                string returnString = responseItems[3];

                // Stupid trello new line formatting
                string[] lines = returnString.Split(new string[] { "\\n" }, StringSplitOptions.None);
                returnString = "";
                foreach (string line in lines)
                {
                    returnString += line + System.Environment.NewLine;
                }
                return returnString;
            }
            catch
            {
                return "";
            }
        }
    }
}
