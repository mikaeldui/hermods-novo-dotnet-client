using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hermods.Novo
{
    internal static class HermodsNovoParser
    {
        public static async Task<HermodsNovoEbook[]> ParseEbooksAsync(string ebookHtml)
        {
            return await Task.Run(() =>
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(ebookHtml);

                var activeEbooks = doc.DocumentNode.Descendants().Where(n => n.HasClass("active_ebook")).ToArray();

                var result = new HermodsNovoEbook[activeEbooks.Length];

                for (int i = 0; i < activeEbooks.Length; i++)
                {
                    var ebook = activeEbooks[i];
                    result[i] = new HermodsNovoEbook()
                    {
                        Title = ebook.Descendants().First(n => n.HasClass("teaching_materials_title")).FirstChild.InnerText,
                        Publisher = ebook.Descendants().First(n => n.HasClass("teaching_materials_publisher")).FirstChild.InnerText,
                        Status = ebook.Descendants().First(n => n.HasClass("ebook_status")).FirstChild.InnerText.Trim(),
                        Isbn = ebook.Attributes["data-isbn"].Value,
                        StartDate = DateTime.Parse(ebook.Attributes["data-startdate"].Value),
                        EndDate = DateTime.Parse(ebook.Attributes["data-enddate"].Value),
                        Url = new Uri("https://novo.hermods.se/ham/" + ebook.Attributes["data-ebookurl"].Value)
                    };
                }

                return result;
            });
        }

        public static async Task<HermodsNovoPersonalInformation> ParsePersonalInformationAsync(string personalInformationHtml) =>
            await Task.Run(() =>
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(personalInformationHtml);

                var form = doc.GetElementbyId("ham_user_form");

                return new HermodsNovoPersonalInformation
                {
                    PublicFirstName = doc.GetElementbyId("upf_firstname_public")?.Attributes["value"]?.Value,
                    PublicLastName = doc.GetElementbyId("upf_lastname_public")?.Attributes["value"]?.Value,
                    IdentityNumber = doc.GetElementbyId("upf_personnr")?.Attributes["value"]?.Value,
                    Address = doc.GetElementbyId("upf_address").Attributes["value"].Value,
                    Address2 = doc.GetElementbyId("upf_address2").Attributes["value"].Value,
                    PostalCode = doc.GetElementbyId("upf_postcode").Attributes["value"].Value,
                    City = doc.GetElementbyId("upf_city").Attributes["value"].Value,
                    //Country = doc.GetElementbyId("upf_postcode").Attributes["value"].Value,
                    Email = doc.GetElementbyId("upf_email").Attributes["value"].Value,
                    CellPhone = doc.GetElementbyId("upf_cellphone").Attributes["value"].Value,
                    HomePhone = doc.GetElementbyId("upf_homephone").Attributes["value"].Value,
                    WorkPhone = doc.GetElementbyId("upf_workphone").Attributes["value"].Value,

                    // The hidden fields
                    UserId = form.SelectSingleNode("//input[@name='user_id']").Attributes["value"].Value,
                    FirstName = doc.GetElementbyId("upf_firstname").Attributes["value"].Value,
                    LastName = doc.GetElementbyId("upf_lastname").Attributes["value"].Value,
                    WwwAction = doc.GetElementbyId("www_action").Attributes["value"].Value,
                    OriginalProtectedIdentity = doc.GetElementbyId("original_protected_identity").Attributes["value"].Value
                };
            });
    }
}
