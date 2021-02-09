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
        private static readonly string USERNAME = Environment.GetEnvironmentVariable("HERMODS_NOVO_USERNAME");
        private static readonly string PASSWORD = Environment.GetEnvironmentVariable("HERMODS_NOVO_PASSWORD");

        private static HermodsNovoClient _client = new HermodsNovoClient();
        private static Lazy<Task<bool>> _tryAuthenticateTask = new Lazy<Task<bool>>(() => _client.TryAuthenticateAsync(USERNAME, PASSWORD));
        private static Lazy<Task<HermodsNovoEbook[]>> _getEbooksTask = new Lazy<Task<HermodsNovoEbook[]>>(() => _client.GetEbooksAsync());
        private static Lazy<Task<HermodsNovoPersonalInformation>> _getPersonalInformationTask = new Lazy<Task<HermodsNovoPersonalInformation>>(() => _client.GetPersonalInformationAsync());
        private static Lazy<Task<LiberOnlinebokClient>> _getLiberOnlinebokClientTask = new Lazy<Task<LiberOnlinebokClient>>(() => _client.GetLiberOnlinebokClientAsync(_getEbooksTask.Value.Result[0]));
        private static Lazy<Task<LiberOnlinebokDocument>> _getLiberOnlinebokDocumentTask = new Lazy<Task<LiberOnlinebokDocument>>(() => _getLiberOnlinebokClientTask.Value.Result.GetDocumentAsync());

        [TestMethod]
        public async Task TryAuthenticateAsync()
        {
            Assert.IsTrue(await _tryAuthenticateTask.Value, "Authentication failed.");
        }

        [TestMethod]
        public async Task GetEbooksAsync()
        {
            await TryAuthenticateAsync();

            var ebooks = await _getEbooksTask.Value;

            Assert.IsTrue(ebooks.Length > 0, "No e-books retrieved.");
        }

        [TestMethod, TestCategory("Personal Information")]
        public async Task GetPersonalInformationAsync()
        {
            await TryAuthenticateAsync();

            var personalInformation = await _getPersonalInformationTask.Value;

            Assert.IsNotNull(personalInformation);
            Assert.AreEqual("Mikael Dúi", personalInformation.PublicFirstName, "The first name is invalid.");
        }

        [TestMethod, TestCategory("Personal Information")]
        public async Task UpdatePersonalInformationAsync()
        {
            await GetPersonalInformationAsync();

            var personalInformation = await _getPersonalInformationTask.Value;

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

            LiberOnlinebokClient liberClient = await _getLiberOnlinebokClientTask.Value;

            Assert.IsNotNull(liberClient, "The Liber Onlinebok Client was null.");            
        }

        [TestMethod, TestCategory("Liber Onlinebok")]
        public async Task GetLiberOnlinebokDocumentAsync()
        {
            await GetLiberOnlinebokClientAsync();

            var document = await _getLiberOnlinebokDocumentTask.Value;

            Assert.IsNotNull(document, "Document is null");

            Assert.IsTrue(document.Content.ContentItems.Length > 0, "No content items");
        }

        [TestMethod, TestCategory("Liber Onlinebok")]
        public async Task GetLiberOnlinebokAssetsLocationAsync()
        {
            await GetLiberOnlinebokClientAsync();

            var assetLocation = await _getLiberOnlinebokClientTask.Value.Result.GetAssetsLocationAsync();

            Assert.IsNotNull(assetLocation, "Asserts location is null");
        }

        #endregion Liber Onlinebok
    }
}
