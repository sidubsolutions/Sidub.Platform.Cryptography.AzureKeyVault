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

#region Imports

using Sidub.Platform.Authentication;
using Sidub.Platform.Authentication.Credentials;
using Sidub.Platform.Core;
using Sidub.Platform.Core.Services;
using Sidub.Platform.Cryptography.Connectors;
using Sidub.Platform.Storage;

#endregion

namespace Sidub.Platform.Cryptography
{

    public static class ServiceRegistryExtensions
    {

        #region Public static methods

        /// <summary>
        /// Adds cryptography services to the metadata service registry.
        /// </summary>
        /// <typeparam name="TMetadataService">The type of the metadata service registry.</typeparam>
        /// <param name="serviceRegistry">The metadata service registry.</param>
        /// <param name="context">The cryptography service reference.</param>
        /// <param name="serviceUri">The service URI of the key vault service.</param>
        /// <param name="credential">The client credential for the key vault service.</param>
        /// <param name="parentContext">The parent service reference.</param>
        /// <returns>The updated metadata service registry.</returns>
        public static IServiceRegistry AddCryptography<TMetadataService>(this TMetadataService serviceRegistry,
            CryptographyServiceReference context,
            string serviceUri,
            IClientCredential? credential = null,
            ServiceReference? parentContext = null) where TMetadataService : IServiceRegistry
        {
            var keyVaultService = new StorageServiceReference($"{context.Name}-api");

            var keyConnector = new AzureKeyVaultConnector(keyVaultService);
            serviceRegistry.RegisterServiceReference(context, keyConnector);

            var keyVaultServiceInfo = new ODataStorageConnector()
            {
                ServiceUri = serviceUri
            };
            serviceRegistry.RegisterServiceReference(keyVaultService, keyVaultServiceInfo, parentContext);

            if (credential is not null)
            {
                var keyVaultServiceAuth = new AuthenticationServiceReference($"{context.Name}-auth");

                serviceRegistry.RegisterServiceReference(keyVaultServiceAuth, credential, keyVaultService);
            }

            return serviceRegistry;
        }

        #endregion

    }

}
