/*
 * Sidub Platform - Cryptography - Azure Key Vault
 * Copyright (C) 2024 Sidub Inc.
 * All rights reserved.
 *
 * This file is part of Sidub Platform - Cryptography - Azure Key Vault (the "Product").
 *
 * The Product is dual-licensed under:
 * 1. The GNU Affero General Public License version 3 (AGPLv3)
 * 2. Sidub Inc.'s Proprietary Software License Agreement (PSLA)
 *
 * You may choose to use, redistribute, and/or modify the Product under
 * the terms of either license.
 *
 * The Product is provided "AS IS" and "AS AVAILABLE," without any
 * warranties or conditions of any kind, either express or implied, including
 * but not limited to implied warranties or conditions of merchantability and
 * fitness for a particular purpose. See the applicable license for more
 * details.
 *
 * See the LICENSE.txt file for detailed license terms and conditions or
 * visit https://sidub.ca/licensing for a copy of the license texts.
 */

using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Sidub.Platform.Authentication.Credentials;
using Sidub.Platform.Core;
using Sidub.Platform.Core.Services;
using Sidub.Platform.Cryptography.AzureKeyVault.Test.Models;
using Sidub.Platform.Cryptography.Connectors;
using Sidub.Platform.Cryptography.Providers;
using Sidub.Platform.Cryptography.Services;
using System.Text;

namespace Sidub.Platform.Cryptography.AzureKeyVault.Test
{
    [TestClass]
    public class AzureKeyVaultKeyProviderTest
    {

        private readonly CryptographyServiceReference LocalCryptographyServiceReference = new CryptographyServiceReference("local");
        private readonly CryptographyServiceReference RemoteCryptographyServiceReference = new CryptographyServiceReference("remote");

        private readonly IServiceRegistry _serviceRegistry;
        private readonly ICryptographyService _cryptographyService;
        private readonly AzureKeyVaultProvider _keyProvider;

        public AzureKeyVaultKeyProviderTest()
        {
            // initialize dependency injection environment...
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSidubPlatform(serviceProvider =>
            {
                var metadata = new InMemoryServiceRegistry();

                // register local cryptography service
                var localCryptographyProvider = new EphemeralKeyConnector();
                metadata.RegisterServiceReference(LocalCryptographyServiceReference, localCryptographyProvider);

                var keyVaultUrl = Environment.GetEnvironmentVariable("KEYVAULT_URL");
                if (string.IsNullOrEmpty(keyVaultUrl))
                {
                    throw new Exception("Key Vault URL not provided.");
                }
                //"unittestingvault.vault.azure.net"
                var keyVaultCredential = new ServiceTokenCredential(new DefaultAzureCredential(new DefaultAzureCredentialOptions() { ExcludeSharedTokenCacheCredential = true }), "https://vault.azure.net/.default");
                metadata.AddCryptography(RemoteCryptographyServiceReference, keyVaultUrl, keyVaultCredential);

                return metadata;
            });

            serviceCollection.AddSidubCryptographyForAzureKeyVault();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            _serviceRegistry = serviceProvider.GetService<IServiceRegistry>() ?? throw new Exception("IServiceRegistry not initialized.");
            _cryptographyService = serviceProvider.GetService<ICryptographyService>() ?? throw new Exception("ICryptographyService not initialized.");

            var provider = _cryptographyService.GetProvider(RemoteCryptographyServiceReference);

            if (provider is not AzureKeyVaultProvider baseProvider)
                throw new Exception("Unit tests currently only support base cryptography implementation.");

            _keyProvider = baseProvider;
        }

        [TestMethod]
        public async Task AzureKeyVaultKeyProviderTest_ImportKey01()
        {
            var descriptor = await _cryptographyService.CreateAsymmetricKey(LocalCryptographyServiceReference);
            var key = await _cryptographyService.GetAsymmetricKey(LocalCryptographyServiceReference, descriptor, true);

            // import the key to remote
            var remoteDescriptor = await _cryptographyService.ImportAsymmetricKey(RemoteCryptographyServiceReference, key);
            var remoteKey = await _cryptographyService.GetAsymmetricKey(RemoteCryptographyServiceReference, remoteDescriptor);

            var entity = new TestEntity()
            {
                Id = 12,
                Name = "Foo"
            };

            var signed = await _cryptographyService.SignEntity(RemoteCryptographyServiceReference, remoteDescriptor, entity);
            var localVerify = await _cryptographyService.VerifyEntity(RemoteCryptographyServiceReference, remoteDescriptor, signed);

            Assert.IsTrue(localVerify);
        }

        [TestMethod]
        public async Task AzureKeyVaultKeyProviderTest_ImportKey02()
        {
            var descriptor = await _cryptographyService.CreateAsymmetricKey(LocalCryptographyServiceReference);
            var key = await _cryptographyService.GetAsymmetricKey(LocalCryptographyServiceReference, descriptor, true);
            key.Id = Guid.NewGuid();
            key.PrivateKey = null;

            // import the key to remote
            var remoteDescriptor = await _cryptographyService.ImportAsymmetricKey(RemoteCryptographyServiceReference, key);
            var remoteKey = await _cryptographyService.GetAsymmetricKey(RemoteCryptographyServiceReference, remoteDescriptor);

            var entity = new TestEntity()
            {
                Id = 12,
                Name = "Foo"
            };

            var signed = await _cryptographyService.SignEntity(LocalCryptographyServiceReference, descriptor, entity);
            var localVerify = await _cryptographyService.VerifyEntity(RemoteCryptographyServiceReference, remoteDescriptor, signed);

            Assert.IsTrue(localVerify);
        }

        [TestMethod]
        public async Task AzureKeyVaultKeyProviderTest_SignData01()
        {
            var someData = Encoding.UTF8.GetBytes("some string data");

            var key = await _cryptographyService.CreateAsymmetricKey(RemoteCryptographyServiceReference);
            var sign = await _cryptographyService.SignData(RemoteCryptographyServiceReference, key, someData);
            var verify = await _cryptographyService.VerifyData(RemoteCryptographyServiceReference, key, someData, sign);

            Assert.IsTrue(verify);
        }

        [TestMethod]
        public async Task GetAsymmetricKeyTest01()
        {
            var keyDescriptor = await _cryptographyService.CreateAsymmetricKey(RemoteCryptographyServiceReference);
            Assert.IsNotNull(keyDescriptor);

            var retrieved = await _cryptographyService.GetAsymmetricKey(RemoteCryptographyServiceReference, keyDescriptor, false);
            Assert.IsNotNull(retrieved);
        }

    }
}
