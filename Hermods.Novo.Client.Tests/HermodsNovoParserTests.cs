using Hermods.Novo.Client.Tests.SampleData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hermods.Novo.Client.Tests
{
    [TestClass]
    public class HermodsNovoParserTests
    {
        [TestMethod]
        public async Task ParseEbooksAsync()
        {
            var html = await SampleDataHelper.GetStringAsync("Ebooks.html");

            var result = await HermodsNovoParser.ParseEbooksAsync(html);

            Assert.IsNotNull(result, "The result is null.");
            Assert.AreEqual(2, result.Length, "Wrong number of e-books returned.");
        }

        [TestMethod]
        public async Task ParsePersonalInformationAsync()
        {
            var html = await SampleDataHelper.GetStringAsync("PersonalInformation.html");

            var result = await HermodsNovoParser.ParsePersonalInformationAsync(html);

            Assert.IsNotNull(result, "Result is null.");
            Assert.AreEqual("Bolinder", result.PublicLastName, "The first name is invalid.");
            Assert.AreEqual("321654", result.UserId, "The hidden user ID is invalid.");
        }
    }
}
