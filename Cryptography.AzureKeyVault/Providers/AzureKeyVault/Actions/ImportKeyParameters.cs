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
using Sidub.Platform.Cryptography.Providers.AzureKeyVault.Entities;

#endregion

namespace Sidub.Platform.Cryptography.Providers.AzureKeyVault.Actions
{

    /// <summary>
    /// Represents the parameters for importing a key.
    /// </summary>
    [Entity("keys/{kid}?api-version=7.4")]
    internal class ImportKeyParameters : IEntity
    {

        #region Public properties

        /// <summary>
        /// Gets or sets the ID of the key.
        /// </summary>
        [EntityKey<string>("kid")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the key to be imported.
        /// </summary>
        [EntityField<WebKey>("key")]
        public WebKey? Key { get; set; } = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportKeyParameters"/> class.
        /// </summary>
        public ImportKeyParameters()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportKeyParameters"/> class with the specified key.
        /// </summary>
        /// <param name="key">The key to import.</param>
        public ImportKeyParameters(AsymmetricKey key)
        {
            Id = key.Id.ToString("D");
            Key = WebKey.CreateFromKey(key);
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
