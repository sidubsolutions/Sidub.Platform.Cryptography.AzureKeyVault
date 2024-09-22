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
    /// Represents the parameters for retrieving a key from Azure Key Vault.
    /// </summary>
    [Entity("keys/{id}?api-version=7.4")]
    internal class GetKeyParameters : IEntity
    {

        #region Public properties

        /// <summary>
        /// Gets the ID of the key.
        /// </summary>
        [EntityKey<string>("id")]
        public string KeyId { get; } = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GetKeyParameters"/> class.
        /// </summary>
        public GetKeyParameters()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetKeyParameters"/> class with the specified key ID.
        /// </summary>
        /// <param name="keyId">The ID of the key.</param>
        public GetKeyParameters(Guid keyId)
        {
            KeyId = keyId.ToString("D");
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
