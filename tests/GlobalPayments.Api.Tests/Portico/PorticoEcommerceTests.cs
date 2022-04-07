﻿using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Portico {
    [TestClass]
    public class PorticoEcommerceTests {
        CreditCardData card;
        private string _token;  

        public PorticoEcommerceTests() {
            ServicesContainer.ConfigureService(new PorticoConfig {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            });

            card = TestCards.VisaManual();
            _token = card.Tokenize();
        }

        [TestMethod]
        public void EcomWithMoto() {
            Transaction response = card.Charge(9m)
                .WithCurrency("USD")
                .WithEcommerceInfo(new EcommerceInfo {
                    Channel = EcommerceChannel.MOTO
                })
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EcomWithDirectMarketShipDate() {
            Transaction response = card.Charge(10m)
                .WithCurrency("USD")
                .WithEcommerceInfo(new EcommerceInfo {
                    ShipDay = 25,
                    ShipMonth = 12
                })
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EcomWithDirectMarketInvoiceNoShipDate() {
            Transaction response = card.Charge(10m)
                .WithCurrency("USD")
                .WithEcommerceInfo(new EcommerceInfo {})
                .WithInvoiceNumber("1234567890")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EcomWithDirectMarketDataInvoiceAndShipDate() {
            Transaction response = card.Charge(11m)
                .WithCurrency("USD")
                .WithEcommerceInfo(new EcommerceInfo {
                    ShipDay = 25,
                    ShipMonth = 12
                })
                .WithInvoiceNumber("1234567890")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EcomWithSecureEcommerceThreeDSecure() {
            card.ThreeDSecure = new ThreeDSecure {
                Cavv = "XXXXf98AAajXbDRg3HSUMAACAAA=",
                Eci = "5",
                Version = Secure3dVersion.One,
                Xid = "0l35fwh1sys3ojzyxelu4ddhmnu5zfke5vst"
            };
            Transaction response = card.Charge(10m)
                .WithCurrency("USD")
                .WithInvoiceNumber("1234567890")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }


        [TestMethod]
        public void EcomWithSecureEcommerceWithoutMobileType() {
            card.ThreeDSecure = new ThreeDSecure {
                PaymentDataSource = "ApplePay",
                Cavv = "XXXXf98AAajXbDRg3HSUMAACAAA=",
                Eci = "7"
            };
            Transaction response = card.Charge(10m)
                .WithCurrency("USD")
                .WithInvoiceNumber("1234567890")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void EcomWithSecureEcommerceWalletDataWithMobileType() {
            card.ThreeDSecure = new ThreeDSecure {
                PaymentDataSource = "ApplePay",
                Cavv = "XXXXf98AAajXbDRg3HSUMAACAAA=",
                Eci = "5"
            };
            card.MobileType = "ApplePay";
            card.Token = _token;

            Transaction response = card.Charge(10m)
                .WithCurrency("USD")
                .WithInvoiceNumber("1234567890")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
    }
}
