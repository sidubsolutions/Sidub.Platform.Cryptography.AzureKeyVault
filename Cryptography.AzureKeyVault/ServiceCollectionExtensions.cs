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

using Microsoft.Extensions.DependencyInjection;
using Sidub.Platform.Authentication;
using Sidub.Platform.Cryptography.Providers;
using Sidub.Platform.Storage;

#endregion

namespace Sidub.Platform.Cryptography
{

    /// <summary>
    /// Provides extension methods for configuring cryptography services for Azure Key Vault.
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        #region Public static methods

        /// <summary>
        /// Adds the Sidub cryptography services for Azure Key Vault to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddSidubCryptographyForAzureKeyVault(this IServiceCollection services)
        {
            services.AddTransient<ICryptographyProvider, AzureKeyVaultProvider>();
            services.AddSidubCryptography();
            services.AddSidubAuthenticationForHttp();
            services.AddSidubStorageForHttp();

            return services;
        }

        #endregion

    }

}
