using HtmlAgilityPack;
using Liber.Onlinebok;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hermods.Novo
{
    public class HermodsNovoClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClientHandler _httpClientHandler;
        private readonly CookieContainer _cookieContainer;

        public HermodsNovoClient()
        {
            _cookieContainer = new CookieContainer();
            _httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                CookieContainer = _cookieContainer,
                UseCookies = true
            };
            _httpClient = new HttpClient(_httpClientHandler);
        }

        /// <summary>
        /// Throws <see cref="HermodsNovoInvalidCredentialsException"/> if the credentials are invalid.
        /// </summary>
        public async Task<bool> TryAuthenticateAsync(string username, string password)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            });

            var response = await _httpClient.PostAsync("https://novo.hermods.se/login/index.php", content);

            if (response.IsSuccessStatusCode && response.RequestMessage.RequestUri.ToString() == "https://novo.hermods.se/theme/frigg/layout/views/student/")
                return true;

            return false;
        }

        #region E-books

        public async Task<HermodsNovoEbook[]> GetEbooksAsync()
        {
            var response = await _httpClient.GetAsync("https://novo.hermods.se/?action=ebooks");

            _ensureSuccess(response);

            var html = await response.Content.ReadAsStringAsync();

            return await HermodsNovoParser.ParseEbooksAsync(html);
        }

        #region LiberOnlinebokClient

        public async Task<LiberOnlinebokClient> GetLiberOnlinebokClientAsync(HermodsNovoEbook ebook)
        {
            if (ebook.Publisher != "Liber")
                throw new ArgumentException("The e-book is not published by Liber.");

            return await GetLiberOnlinebokClientAsync(ebook.Url.ToString());
        }

        public async Task<LiberOnlinebokClient> GetLiberOnlinebokClientAsync(string ebookUrl)
        {
            var response = await _httpClient.GetAsync(ebookUrl);

            _ensureSuccess(response);

            if (response.RequestMessage.RequestUri.ToString().StartsWith("https://novo.hermods.se/ham/linkresolver.php"))
                throw new ApplicationException("Redirection didn't work");

            return LiberOnlinebokClient.From(response.RequestMessage.RequestUri, _cookieContainer);
        }

        #endregion LiberOnlinebokClient

        #endregion

        #region Personal Information

        public async Task<HermodsNovoPersonalInformation> GetPersonalInformationAsync()
        {
            var response = await _httpClient.GetAsync("https://novo.hermods.se/?action=personalinfo");

            _ensureSuccess(response);

            var html = await response.Content.ReadAsStringAsync();

            return await HermodsNovoParser.ParsePersonalInformationAsync(html);
        }

        /// <summary>
        /// Throws an exception if the update fails. If it succeeds then the new information will be return.
        /// </summary>
        public async Task<HermodsNovoPersonalInformation> UpdatePersonalInformationAsync(HermodsNovoPersonalInformation personalInformation)
        {
            const string url = "https://novo.hermods.se/ham/submit.php?action=user_personal";

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "address", personalInformation.Address },
                { "address2", personalInformation.Address2 },
                { "postcode", personalInformation.PostalCode },
                { "city", personalInformation.City },
                { "country", personalInformation.Country },
                { "email", personalInformation.Email },
                { "cellphone", personalInformation.CellPhone },
                { "homephone", personalInformation.HomePhone },
                { "workphone", personalInformation.WorkPhone },
                { "user_id", personalInformation.UserId },
                { "firstname", personalInformation.FirstName },
                { "lastname", personalInformation.LastName },
                { "www_action", personalInformation.WwwAction },
                { "original_protected_identity", personalInformation.OriginalProtectedIdentity }
            });

            var response = await _httpClient.PostAsync(url, content);

            _ensureSuccess(response);

            if (response.RequestMessage.RequestUri.ToString() == "https://novo.hermods.se/ham/index.php?action=personal_info&open_section=personal&feedback[]=success_save")
            {
                var html = await response.Content.ReadAsStringAsync();
                return await HermodsNovoParser.ParsePersonalInformationAsync(html);
            }

            throw new HermodsNovoInvalidPersonalInformationException("The personal information submitted was invalid.", personalInformation);
        }

        #endregion

        #region Helpers

        private bool _ensureSuccess(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            if (response.RequestMessage.RequestUri.ToString() == "https://novo.hermods.se/login/index.php")
                throw new HermodsNovoUnauthorizedException("Redirected to https://novo.hermods.se/login/index.php");

            return true;
        }

        public void Dispose() => _httpClient.Dispose();

        #endregion
    }
}