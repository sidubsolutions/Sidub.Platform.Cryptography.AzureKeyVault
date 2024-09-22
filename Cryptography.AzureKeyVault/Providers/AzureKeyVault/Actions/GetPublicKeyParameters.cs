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

using Sidub.Platform.Core.Attributes;
using Sidub.Platform.Core.Entity;

#endregion

namespace Sidub.Platform.Cryptography.Providers.AzureKeyVault.Actions
{

    /// <summary>
    /// Represents the parameters for retrieving public key from Azure Key Vault.
    /// </summary>
    [Entity("secrets/{id}/{version}?api-version=7.4")]
    internal class GetPublicKeyParameters : IEntity
    {

        #region Public properties

        /// <summary>
        /// Gets the key ID.
        /// </summary>
        [EntityKey<string>("id")]
        public string KeyId { get; } = string.Empty;

        /// <summary>
        /// Gets the key version.
        /// </summary>
        [EntityKey<string>("version")]
        public string Version { get; } = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GetPublicKeyParameters"/> class.
        /// </summary>
        public GetPublicKeyParameters()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetPublicKeyParameters"/> class with the specified key descriptor.
        /// </summary>
        /// <param name="key">The key descriptor.</param>
        public GetPublicKeyParameters(KeyDescriptor key)
        {
            KeyId = key.Id.ToString("D");
            Version = key.Version ?? string.Empty;
        }

        #endregion

        #region IEntity implementation

        /// <summary>
        /// Gets or sets a value indicating whether the entity is retrieved from storage.
        /// </summary>
        bool IEntity.IsRetrievedFromStorage { get; set; }

        #endregion

    }

}
