//-----------------------------------------------------------------------
// <copyright file="XmlReaderEx.cs" company="troncell">
//     Copyright © troncell. All rights reserved.
// </copyright>
// <author>William Wu</author>
// <email>wulixu@troncell.com</email>
// <date>2012-11-01</date>
// <summary>Provider a way to read crypto xml file.</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Linq;
using SensingBase.CSException;
using LogService;

namespace SensingBase.Utils
{
    public static class XmlReaderEx
    {
        private static string x302c4346ab49b3404;
        private static readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(XmlReaderEx));

        static XmlReaderEx()
        {
#if ANTU
            XDocument d = new XDocument(
                new XElement("RSAKeyValue",
                    new XElement("Modulus",
                        "0apbC+AvWSMTg17S0l0X7uC25ok++xej+gf7fUsxOSHanL58okw2UXiVhDj3o9WO4r6iAOsk/3S4AspSOHX/EIowvWi+QopQF8qOwXCgv0XrfJ6tvfPnMTnBUpERDkAfhO/xiSK5bWa4sD1nckNyL3J8f3bxW0xsmeBEdLN1mL8="),
                    new XElement("Exponent", "AQAB"),
                    new XElement("P",
                        "1OnLMVymR35/u6GL9jsG6OnYRHroa2OMezJkoVm0BgUZL6AWYRqA45CGvhJTTcWzxopFXHTBUngrFsezREIoIQ=="),
                    new XElement("Q",
                        "/BhOSVi1Hbi/EhNRBDyz2iYb9z7g9OMP3Ng5v8Er+iDS32xOef1yQQvRlLyLG2pbBEgrwFq2Mz5vCIP2KjAk3w=="),
                    new XElement("DP",
                        "KlnG3S68tIPpDH15xaTAHxxEtHpuOM44Z1kCw0WAlaH1/I8vZGlNMfbRloU2pMWqmNdwLI/c1Haqu4FFm1I9gQ=="),
                    new XElement("DQ",
                        "qyqNrTtv7jhMc6dt8OSOcWZCwsOM1nl9gcPGoi40/+Zdh5nwRuARPZ0atlS2Vu0F04h1PGvbHjwcA7ol4EtEHQ=="),
                    new XElement("InverseQ",
                        "kayYQ4h/migbqyAHdtx9v8fYdHrHTEeMBzavK/AywbUO3d0Sxds2v2NmNqLZ8O4852JckHKDiuzcoym9nPRPCw=="),
                    new XElement("D",
                        "T22Nk1ipJ0gR/t0f2eC5jd3kfD53NRFWJgT7IZKoQaARHPtO5P8hPskDP3WDXzYqrLySS+3I9NvHUAtMjaYp6258/E5TYhmcKavUcefPjy1xtF/kUn3QwkYl/GoFBa1nxaJBPHbEdjfwW9/8UFrJXWyXEFvV5d/kF9EHj54D6yE="))
                );
#else
            XDocument d = new XDocument(
                new XElement("RSAKeyValue",
                    new XElement("Modulus",
                        "rQpYR01j4gdCEUj/MR5MirPEJiw4LlcSNl1gCrRrwRNBvu7Lp5DwIMkbUgLP8dd7nDT/WZnYWki5XgOCGYRYCkrJjmDM3nDRZllJiJ5dUG7RKr3N9Xq8w4WP2ngIqHuzzts1qIBOGf3oiuKWfAGdXKZJpTHpTMU0EQBfSqpmqpE="),
                    new XElement("Exponent", "AQAB"),
                    new XElement("P",
                        "tXzVNOribY99Qt48s4C6MpvLD1Dadhk7ljSM9lGhk5sl1vJUndBmhf7C/2GBsHJK6jkNJcb4fSCS8lTo+Tl6dw=="),
                    new XElement("Q",
                        "9BWr183UMMfO1aozwZ9c+v6LltLqBSNd2bMRhzhsnDC0EpG4tpkXBkEpyFWRvC4ReCUioEnedG++/za00tk9Nw=="),
                    new XElement("DP",
                        "f7vim6INx6rMMLoV4wjhBCmem6L+f0x3IdrOs7b0j90MnuJkJ8a7wedy+yd3jeaDT6Lj3AjUb8zzX9ffOTn5NQ=="),
                    new XElement("DQ",
                        "hZfu7Djt8J0L6go0Nb+hxobXo/9gsts+nM7Twyzw1TQa+ybi8J4t6fB0i/+5ukOEsDuzDgQgS5517X3lzpm0VQ=="),
                    new XElement("InverseQ",
                        "JEPKEX3qp6682VPf8DQpZVgxeWRg+rpgf6f3d/AuP2MrNnDo2lyq2ftfT5vlpn+82Jq9lZH51a5nKZdo2iw3sw=="),
                    new XElement("D",
                        "AWb9aG2rciwCnPraapOIHceUSl+SEBNJv0TEXBNNp/GtS28EjiCFw1oiO/R/2V0zsuz218NVi6eUOKBaYQ5Y24Cut4ulkZ+IS/9pKW5+bXJuSbJsTtQKwca8J4RHbNRpKaXW/X3MGc4ZcxGXMolVGuKcsKaojwISBWKz2Cxa1Us="))
                );
#endif

            x302c4346ab49b3404 = d.ToString();
        }

        public static XDocument ReadXmlFile(string filePath,bool isCrypto = true)
        {
            XDocument result = null;
            try
            {
                if (isCrypto)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filePath);
                    result = DecodeXML(doc);
                }
                else
                {
                    result = XDocument.Load(filePath);
                }
            }
            catch (Exception ex)
            {
                string exceptionMSG = "Load Xml File Exception at File =" + filePath;
                logger.Error(exceptionMSG, ex);
                //throw new CoreException(exceptionMSG, ex);
            }
            return result;
        }

        public static XDocument DecodeXML(XmlDocument readDoc)
        {
            if (readDoc == null) return null;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(x302c4346ab49b3404);
            EncryptedXml encXml = new EncryptedXml(readDoc);
            encXml.AddKeyNameMapping("asyncKey", rsa);
            encXml.DecryptDocument();
            return XDocument.Parse(readDoc.OuterXml);
        }


        public static XDocument EncodeXML(XmlDocument srcDoc)
        {
            if (srcDoc == null) return null;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(x302c4346ab49b3404);
            EncryptedXml encXml = new EncryptedXml(srcDoc);
            encXml.AddKeyNameMapping("asyncKey", rsa);
            var docElement = srcDoc.DocumentElement;
            if (docElement != null)
            {
                EncryptedData encryptedData = encXml.Encrypt(docElement, "asyncKey");
                EncryptedXml.ReplaceElement(docElement, encryptedData, false);
            }
            return XDocument.Parse(srcDoc.OuterXml);
        }
    }
}