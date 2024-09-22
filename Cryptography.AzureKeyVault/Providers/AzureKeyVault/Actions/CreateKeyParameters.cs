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
    /// Represents the parameters for creating a key in Azure Key Vault.
    /// </summary>
    [Entity("keys/{id}/create?api-version=7.3")]
    internal class CreateKeyParameters : IEntity
    {

        #region Enumerations

        /// <summary>
        /// Specifies the type of the key.
        /// </summary>
        public enum KeyTypeType
        {
            EC,
            Aes
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the ID of the key.
        /// </summary>
        [EntityKey<string>("id")]
        public string KeyId { get; } = string.Empty;

        /// <summary>
        /// Gets the type of the key.
        /// </summary>
        [EntityField<string>("kty")]
        public string KeyType { get; } = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateKeyParameters"/> class.
        /// </summary>
        public CreateKeyParameters()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateKeyParameters"/> class.
        /// </summary>
        /// <param name="keyId">The ID of the key.</param>
        /// <param name="keyType">The type of the key.</param>
        public CreateKeyParameters(Guid keyId, KeyTypeType keyType)
        {
            KeyId = keyId.ToString("D");
            KeyType = keyType switch
            {
                KeyTypeType.EC => "EC",
                KeyTypeType.Aes => "oct-aes256",
                _ => throw new Exception("Unhandled KeyTypeType mapping.")
            };
        }

        #endregion

        #region IEntity implementation

        /// <summary>
        /// Gets or sets a value indicating whether the entity is retrieved from storage.
        /// </summary>
        public bool IsRetrievedFromStorage { get; set; }

        #endregion

    }

}
