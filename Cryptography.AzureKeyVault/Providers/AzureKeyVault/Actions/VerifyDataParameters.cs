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

using Microsoft.IdentityModel.Tokens;
using Sidub.Platform.Core.Attributes;
using Sidub.Platform.Core.Entity;

#endregion

namespace Sidub.Platform.Cryptography.Providers.AzureKeyVault.Actions
{

    /// <summary>
    /// Represents the parameters for verifying data using Azure Key Vault.
    /// </summary>
    [Entity("keys/{id}/{version}/verify?api-version=7.3")]
    internal class VerifyDataParameters : IEntity
    {

        #region Public properties

        /// <summary>
        /// Gets or sets the ID of the key.
        /// </summary>
        [EntityKey<string>("id")]
        public string KeyId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the version of the key.
        /// </summary>
        [EntityKey<string>("version")]
        public string KeyVersion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the algorithm used for verification.
        /// </summary>
        [EntityField<string>("alg")]
        public string Algorithm { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the data to be verified.
        /// </summary>
        [EntityField<string>("digest")]
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the signature to be verified.
        /// </summary>
        [EntityField<string>("value")]
        public string Signature { get; set; } = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VerifyDataParameters"/> class.
        /// </summary>
        public VerifyDataParameters()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerifyDataParameters"/> class with the specified parameters.
        /// </summary>
        /// <param name="keyDescriptor">The key descriptor.</param>
        /// <param name="data">The data to be verified.</param>
        /// <param name="signature">The signature to be verified.</param>
        public VerifyDataParameters(KeyDescriptor keyDescriptor, byte[] data, byte[] signature)
        {
            KeyId = keyDescriptor.Id.ToString("D");
            KeyVersion = keyDescriptor.Version ?? string.Empty;
            Algorithm = "ES256";
            Data = Base64UrlEncoder.Encode(data);
            Signature = Base64UrlEncoder.Encode(signature);
        }

        #endregion

        #region IEntity implementation

        /// <summary>
        /// Gets or sets a value indicating whether the entity is retrieved from storage.
        /// </summary>
        bool IEntity.IsRetrievedFromStorage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion

    }

}
