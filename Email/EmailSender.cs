using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;

namespace TqiiLanguageTest.Email {

    public class EmailSender : IEmailSender {
        private const string _from = "no-reply@education.illinois.edu";
        private const string _serverId = "47862";
        private const string _url = "https://api.socketlabs.com/v2/servers/47862/credentials/injection-api";
        private readonly string _auth = "";

        public EmailSender(string auth) {
            _auth = auth;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage) {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(htmlMessage)) {
                return Task.CompletedTask;
            }
            var injectionApi = "";
            var injectionUrl = "";
            using (var client = new HttpClient()) {
                using (var message = new HttpRequestMessage(HttpMethod.Get, _url)) {
                    message.Headers.Add("Authorization", "Bearer " + _auth);
                    message.Headers.Add("Accept", "application/json");
                    var task = client.SendAsync(message).Result;
                    if (!task.IsSuccessStatusCode) {
                        return Task.CompletedTask;
                    }
                    dynamic authentication = JsonConvert.DeserializeObject(task.Content.ReadAsStringAsync().Result);
                    injectionApi = authentication.data.apiKey.ToString();
                    injectionUrl = authentication.data.gateway.ToString();
                }
            }

            if (string.IsNullOrWhiteSpace(injectionUrl) || string.IsNullOrWhiteSpace(injectionApi)) {
                return Task.CompletedTask;
            }
            subject = CleanText(subject);
            htmlMessage = CleanText(htmlMessage);

            var json = "{\"serverId\": " + _serverId + ", \"APIKey\": \"" + injectionApi + "\", \"Messages\": [ { \"To\": [ { \"emailAddress\" : \"" + email + "\" } ], \"From\": { \"emailAddress\": \"noreply@education.illinois.edu\" },\"Subject\": \"" + subject + "\", \"HtmlBody\": \"" + htmlMessage + "\", \"MailingId\": \"TQII_Registration\" } ] }";

            using (var client = new HttpClient()) {
                using (var message = new HttpRequestMessage(HttpMethod.Post, injectionUrl)) {
                    message.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    message.Headers.Add("Accept", "application/json");
                    var task = client.SendAsync(message).Result;
                    return Task.CompletedTask;
                }
            }
        }

        private string CleanText(string s) => string.IsNullOrWhiteSpace(s) ? string.Empty : s.Replace("\"", "");
    }
}