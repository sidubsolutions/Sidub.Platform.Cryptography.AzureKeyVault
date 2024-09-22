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

//#region Imports

//using Sidub.Platform.Core.Attributes;
//using Sidub.Platform.Core.Entity;

//#endregion

//namespace Sidub.Platform.Cryptography.Providers.AzureKeyVault.Actions
//{

//    /// <summary>
//    /// Represents the parameters for encrypting data using Azure Key Vault.
//    /// </summary>
//    [Entity("keys/{id}/{version}/encrypt?api-version=7.3")]
//    internal class EncryptDataParameters : IEntity
//    {

//        #region Public properties

//        /// <summary>
//        /// Gets or sets the ID of the key used for encryption.
//        /// </summary>
//        [EntityKey<string>("id")]
//        public string KeyId { get; set; } = string.Empty;

//        /// <summary>
//        /// Gets or sets the version of the key used for encryption.
//        /// </summary>
//        [EntityKey<string>("version")]
//        public string KeyVersion { get; set; } = string.Empty;

//        /// <summary>
//        /// Gets or sets the algorithm used for encryption.
//        /// </summary>
//        [EntityField<string>("alg")]
//        public string Algorithm { get; set; } = string.Empty;

//        /// <summary>
//        /// Gets or sets the data to be encrypted.
//        /// </summary>
//        [EntityField<string>("value")]
//        public string Data { get; set; } = string.Empty;

//        #endregion

//        #region Constructors

//        /// <summary>
//        /// Initializes a new instance of the <see cref="EncryptDataParameters"/> class.
//        /// </summary>
//        public EncryptDataParameters()
//        {
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="EncryptDataParameters"/> class with the specified key descriptor, data, and initialization vector.
//        /// </summary>
//        /// <param name="keyDescriptor">The key descriptor.</param>
//        /// <param name="data">The data to be encrypted.</param>
//        /// <param name="initializationVector">The initialization vector.</param>
//        public EncryptDataParameters(KeyDescriptor keyDescriptor, byte[] data, byte[] initializationVector)
//        {
//            KeyId = keyDescriptor.Id.ToString("D");
//            KeyVersion = keyDescriptor.Version.ToString();
//            Algorithm = "ECDH-ES";
//            Data = Convert.ToBase64String(data);
//            //InitializationVector = System.Text.Encoding.UTF8.GetString(initializationVector);
//        }

//        #endregion

//        #region IEntity implementation

//        /// <summary>
//        /// Gets or sets a value indicating whether the entity is retrieved from storage.
//        /// </summary>
//        bool IEntity.IsRetrievedFromStorage { get; set; }

//        #endregion

//    }

//}
