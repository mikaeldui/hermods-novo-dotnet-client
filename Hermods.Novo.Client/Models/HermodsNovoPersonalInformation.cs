using System;
using System.Collections.Generic;
using System.Text;

namespace Hermods.Novo
{
    /// <summary>
    /// Model for the informatino at https://novo.hermods.se/?action=personalinfo.
    /// </summary>
    public class HermodsNovoPersonalInformation
    {
        /// <summary> In the UI "Förnamn". </summary>
        public string PublicFirstName { get; set; }

        /// <summary> In the UI "Efternamn". </summary>
        public string PublicLastName { get; set; }

        /// <summary> In the UI "Person-/org.nummer.". </summary>
        public string IdentityNumber { get; set; }

        /// <summary> In the UI "Adressrad 1". </summary>
        public string Address { get; set; }

        /// <summary> In the UI "Adressrad 2". </summary>
        public string Address2 { get; set; }

        /// <summary> In the UI "Postnummer". </summary>
        public string PostalCode { get; set; }

        /// <summary> In the UI "Stad". </summary>
        public string City { get; set; }

        /// <summary> In the UI "land". Two letter code. E.g. "SE". </summary>
        public string Country { get; set; }

        /// <summary> In the UI "E-post". </summary>
        public string Email { get; set; }

        /// <summary> In the UI "Mobiltelefon". </summary>
        public string CellPhone { get; set; }

        /// <summary> In the UI "Hemtelefon". </summary>
        public string HomePhone { get; set; }

        /// <summary> In the UI "Arbetstelefon". </summary>
        public string WorkPhone { get; set; }

        #region Hidden fields

        /// <summary>
        /// E.g. "123456". Hidden.
        /// </summary>
        internal string UserId { get; set; }

        /// <summary>
        /// E.g. "Sven". Hidden.
        /// </summary>
        internal string FirstName { get; set; }

        /// <summary>
        /// E.g. "Svensson". Hidden.
        /// </summary>
        internal string LastName { get; set; }

        /// <summary>
        /// Should be "personal_info". Hidden.
        /// </summary>
        internal string WwwAction { get; set; }

        /// <summary>
        /// E.g. "0". Hidden.
        /// </summary>
        internal string OriginalProtectedIdentity { get; set; }

        #endregion Hidde fields
    }
}
