using Liber.Onlinebok;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hermods.Novo.Client.Tests
{
    [TestClass]
    public class HermodsNovoClientTests
    {
        /// <summary>
        /// You can set it in PowerShell using [Environment]::SetEnvironmentVariable("HERMODS_NOVO_USERNAME", "firstname.lastname@domain.com", 'User').
        /// Visual Studio probably needs to be restarted after doing it.
        /// </summary>
        private static readonly string USERNAME = Environment.GetEnvironmentVariable("HERMODS_NOVO_USERNAME");
        /// <summary>
        /// You can set it in PowerShell using [Environment]::SetEnvironmentVariable("HERMODS_NOVO_PASSWORD", "SuperSecret123", 'User')
        /// Visual Studio probably needs to be restarted after doing it.
        /// </summary>
        private static readonly string PASSWORD = Environment.GetEnvironmentVariable("HERMODS_NOVO_PASSWORD");

        private static HermodsNovoClient _client = new HermodsNovoClient();
        private static Task<bool> _tryAuthenticateTask;
        private static Task<HermodsNovoEbook[]> _getEbooksTask;
        private static Task<HermodsNovoPersonalInformation> _getPersonalInformationTask;
        private static Task<LiberOnlinebokClient> _getLiberOnlinebokClientTask;
        private static Task<LiberOnlinebokDocument> _getLiberOnlinebokDocumentTask;

        [TestMethod]
        public void ConstructAndDispose()
        {
            HermodsNovoClient client = new HermodsNovoClient();
            client.Dispose();
        }

        [TestMethod]
        public async Task TryAuthenticateAsync()
        {
            Assert.IsTrue(await (_tryAuthenticateTask ??= _client.TryAuthenticateAsync(USERNAME, PASSWORD)), "Authentication failed.");
        }

        [TestMethod]
        public async Task GetEbooksAsync()
        {
            await TryAuthenticateAsync();

            var ebooks = await (_getEbooksTask ??= _client.GetEbooksAsync());

            Assert.IsTrue(ebooks.Length > 0, "No e-books retrieved.");
        }

        [TestMethod, TestCategory("Personal Information")]
        public async Task GetPersonalInformationAsync()
        {
            await TryAuthenticateAsync();

            var personalInformation = await (_getPersonalInformationTask ??= _client.GetPersonalInformationAsync());

            Assert.IsNotNull(personalInformation);
            Assert.AreEqual("Bolinder", personalInformation.LastName, "The first name is invalid.");
        }

        [TestMethod, TestCategory("Personal Information")]
        public async Task UpdatePersonalInformationAsync()
        {
            await GetPersonalInformationAsync();

            var personalInformation = await _getPersonalInformationTask;

            // Setting it to something random and valid
            personalInformation.WorkPhone = "08" + (new Random()).Next(1000000, 9999999);

            var newPersonalInformation = await _client.UpdatePersonalInformationAsync(personalInformation);

            Assert.AreEqual(personalInformation.WorkPhone, newPersonalInformation.WorkPhone, "The work phone didn't update.");
        }

        #region Liber Onlinebok

        [TestMethod, TestCategory("Liber Onlinebok")]
        public async Task GetLiberOnlinebokClientAsync()
        {
            await GetEbooksAsync();

            LiberOnlinebokClient liberClient = await (_getLiberOnlinebokClientTask ??= _client.GetLiberOnlinebokClientAsync(_getEbooksTask.Result[0]));

            Assert.IsNotNull(liberClient, "The Liber Onlinebok Client was null.");            
        }

        [TestMethod, TestCategory("Liber Onlinebok")]
        public async Task GetLiberOnlinebokDocumentAsync()
        {
            await GetLiberOnlinebokClientAsync();

            var document = await (_getLiberOnlinebokDocumentTask ??= _getLiberOnlinebokClientTask.Result.GetDocumentAsync());

            Assert.IsNotNull(document, "Document is null");

            Assert.IsTrue(document.Content.ContentItems.Length > 0, "No content items");
        }

        [TestMethod, TestCategory("Liber Onlinebok")]
        public async Task GetLiberOnlinebokAssetsLocationAsync()
        {
            await GetLiberOnlinebokClientAsync();

            var assetLocation = await _getLiberOnlinebokClientTask.Result.GetAssetsLocationAsync();

            Assert.IsNotNull(assetLocation, "Asserts location is null");
        }

        #endregion Liber Onlinebok
    }
}
