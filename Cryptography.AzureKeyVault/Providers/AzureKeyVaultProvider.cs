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
using Sidub.Platform.Core.Entity;
using Sidub.Platform.Core.Extensions;
using Sidub.Platform.Core.Serializers;
using Sidub.Platform.Core.Services;
using Sidub.Platform.Cryptography.Connectors;
using Sidub.Platform.Cryptography.Providers.AzureKeyVault.Actions;
using Sidub.Platform.Storage.Services;
using System.Net;
using System.Security.Cryptography;

#endregion

namespace Sidub.Platform.Cryptography.Providers
{

    /// <summary>
    /// Represents a provider for cryptographic operations using Azure Key Vault.
    /// </summary>
    public class AzureKeyVaultProvider : ICryptographyProvider
    {

        #region Member variables

        private readonly IEntitySerializerService _serializerService;
        private readonly IQueryService _queryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureKeyVaultProvider"/> class.
        /// </summary>
        /// <param name="serviceRegistry">The service registry.</param>
        /// <param name="serializerService">The entity serializer service.</param>
        /// <param name="queryService">The query service.</param>
        public AzureKeyVaultProvider(IEntitySerializerService serializerService, IQueryService queryService)
        {
            _serializerService = serializerService;
            _queryService = queryService;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines if the specified key connector is handled by this provider.
        /// </summary>
        /// <param name="keyConnector">The key connector to check.</param>
        /// <returns><c>true</c> if the key connector is handled by this provider; otherwise, <c>false</c>.</returns>
        public bool IsHandled(IKeyConnector keyConnector)
        {
            return keyConnector is AzureKeyVaultConnector;
        }

        /// <summary>
        /// Creates a symmetric key using the specified key connector.
        /// </summary>
        /// <param name="keyConnector">The key connector to use.</param>
        /// <returns>The created symmetric key descriptor.</returns>
        public async Task<KeyDescriptor> CreateSymmetricKey(IKeyConnector keyConnector)
        {
            if (keyConnector is not AzureKeyVaultConnector azKeyConnector)
                throw new Exception($"The provided key connector type '{keyConnector.GetType().Name}' is not supported.");

            var keyId = Guid.NewGuid();

            var actionParameters = new CreateKeyParameters(keyId, CreateKeyParameters.KeyTypeType.Aes);
            var action = new CreateKeyAction(actionParameters);

            var actionResult = await _queryService.Execute(azKeyConnector.KeyVaultServiceReference, action);
            var keyBundle = actionResult.Result
                ?? throw new Exception("A null result was returned from the key creation action.");

            // retrieve the key version by pulling the last URL segment from the returned key id...
            var keyVersion = keyBundle.Key?.Id.Split('/').Last();

            var result = new KeyDescriptor(keyId, keyVersion);

            return result;
        }

        /// <summary>
        /// Creates an asymmetric key using the specified key connector.
        /// </summary>
        /// <param name="keyConnector">The key connector to use.</param>
        /// <returns>The created asymmetric key descriptor.</returns>
        public async Task<KeyDescriptor> CreateAsymmetricKey(IKeyConnector keyConnector)
        {
            if (keyConnector is not AzureKeyVaultConnector azKeyConnector)
                throw new Exception($"The provided key connector type '{keyConnector.GetType().Name}' is not supported.");

            var keyId = Guid.NewGuid();

            var actionParameters = new CreateKeyParameters(keyId, CreateKeyParameters.KeyTypeType.EC);
            var action = new CreateKeyAction(actionParameters);

            var actionResult = await _queryService.Execute(azKeyConnector.KeyVaultServiceReference, action);
            var keyBundle = actionResult.Result
                ?? throw new Exception("A null result was returned from the key creation action.");

            // retrieve the key version by pulling the last URL segment from the returned key id...
            var keyVersion = keyBundle.Key?.Id.Split('/').Last();

            var result = new KeyDescriptor(keyId, keyVersion);

            return result;
        }

        /// <summary>
        /// Signs the specified data using the specified key connector and key descriptor.
        /// </summary>
        /// <param name="keyConnector">The key connector to use.</param>
        /// <param name="keyDescriptor">The key descriptor to use.</param>
        /// <param name="data">The data to sign.</param>
        /// <returns>The signed data.</returns>
        public async Task<byte[]> SignData(IKeyConnector keyConnector, KeyDescriptor keyDescriptor, byte[] data)
        {
            if (keyConnector is not AzureKeyVaultConnector azKeyConnector)
                throw new Exception($"The provided key connector type '{keyConnector.GetType().Name}' is not supported.");
            var sha256 = SHA256.Create();
            var actionParameters = new SignDataParameters(keyDescriptor, sha256.ComputeHash(data));
            var action = new SignDataAction(actionParameters);

            var actionResult = await _queryService.Execute(azKeyConnector.KeyVaultServiceReference, action);
            var keyOperationResult = actionResult.Result
                ?? throw new Exception("A null result was returned from the sign data action.");

            var result = Base64UrlEncoder.DecodeBytes(keyOperationResult.Value);

            return result;
        }

        /// <summary>
        /// Verifies the specified data using the specified key connector and key descriptor.
        /// </summary>
        /// <param name="keyConnector">The key connector to use.</param>
        /// <param name="keyDescriptor">The key descriptor to use.</param>
        /// <param name="data">The data to verify.</param>
        /// <param name="signature">The signature to verify.</param>
        /// <returns><c>true</c> if the data is verified; otherwise, <c>false</c>.</returns>
        public async Task<bool> VerifyData(IKeyConnector keyConnector, KeyDescriptor keyDescriptor, byte[] data, byte[] signature)
        {
            if (keyConnector is not AzureKeyVaultConnector azKeyConnector)
                throw new Exception($"The provided key connector type '{keyConnector.GetType().Name}' is not supported.");

            bool? result = null;

            // first check for a public key secret for the given key descriptor; if we find one, leverage it - likely a client verification scenario
            //  where the client only has the public key available...
            try
            {
                var publicKeyParameters = new GetPublicKeyParameters(keyDescriptor);
                var publicKeyAction = new GetPublicKeyAction(publicKeyParameters);
                var publicKeyResult = await _queryService.Execute(azKeyConnector.KeyVaultServiceReference, publicKeyAction);

                if (publicKeyResult.IsSuccessful && publicKeyResult.Result is not null)
                {
                    var publicKey = publicKeyResult.Result;

                    var idUrl = new Flurl.Url(publicKey.Id).PathSegments.TakeLast(2);
                    var id = idUrl.First();
                    var version = idUrl.Last();

                    var publicKeyId = Guid.Parse(id);
                    var publicKeyData = Base64UrlEncoder.DecodeBytes(publicKey.Value);

                    var key = new AsymmetricKey(publicKeyId, publicKeyData)
                    {
                        Version = version
                    };

                    using var cryptographyProvider = CryptographyProviderHelper.GetECDsaProvider(key);
                    result = cryptographyProvider.VerifyData(data, signature, HashAlgorithmName.SHA256);
                }
            }
            catch (Flurl.Http.FlurlHttpException httpException)
            {
                if (httpException.StatusCode != (int)HttpStatusCode.NotFound)
                    throw;
            }

            // if we do not have a result yet, continue to leveraging AZK for the verification process; the vault should
            //  have the private key present...
            if (result is null)
            {
                var sha256 = SHA256.Create();
                var verifyDataParameters = new VerifyDataParameters(keyDescriptor, sha256.ComputeHash(data), signature);
                var verifyData = new VerifyDataAction(verifyDataParameters);

                var verifyDataResult = await _queryService.Execute(azKeyConnector.KeyVaultServiceReference, verifyData);
                var keyVerifyResult = verifyDataResult.Result
                    ?? throw new Exception("A null result was returned from the sign data action.");

                result = keyVerifyResult.IsValid;
            }

            return result.Value;
        }

        /// <summary>
        /// Signs the specified entity using the specified key connector and key descriptor.
        /// </summary>
        /// <typeparam name="TEntitySigned">The type of the entity to sign.</typeparam>
        /// <param name="keyConnector">The key connector to use.</param>
        /// <param name="keyDescriptor">The key descriptor to use.</param>
        /// <param name="entity">The entity to sign.</param>
        /// <returns>The signed entity.</returns>
        public async Task<TEntitySigned> SignEntity<TEntitySigned>(IKeyConnector keyConnector, KeyDescriptor keyDescriptor, TEntitySigned entity) where TEntitySigned : IEntitySigned
        {
            var serializerOptions = SerializerOptions.New(SerializationLanguageType.Json).With(x => x.ExcludedFields.Add(SignatureEntityField.Instance));
            var data = _serializerService.Serialize(entity, serializerOptions);
            var signature = await SignData(keyConnector, keyDescriptor, data);

            entity.Signature = signature;

            return entity;
        }

        /// <summary>
        /// Verifies the specified entity using the specified key connector and key descriptor.
        /// </summary>
        /// <typeparam name="TEntitySigned">The type of the entity to verify.</typeparam>
        /// <param name="keyConnector">The key connector to use.</param>
        /// <param name="keyDescriptor">The key descriptor to use.</param>
        /// <param name="entity">The entity to verify.</param>
        /// <returns><c>true</c> if the entity is verified; otherwise, <c>false</c>.</returns>
        public async Task<bool> VerifyEntity<TEntitySigned>(IKeyConnector keyConnector, KeyDescriptor keyDescriptor, TEntitySigned entity) where TEntitySigned : IEntitySigned
        {
            if (entity.Signature is null)
                throw new Exception("No signature exists.");

            bool isValid = false;
            var signature = entity.Signature;

            var serializerOptions = SerializerOptions.New(SerializationLanguageType.Json).With(x => x.ExcludedFields.Add(SignatureEntityField.Instance));
            var data = _serializerService.Serialize(entity, serializerOptions);
            isValid = await VerifyData(keyConnector, keyDescriptor, data, signature);

            return isValid;
        }

        /// <summary>
        /// Gets the asymmetric key from the specified key connector and key descriptor.
        /// </summary>
        /// <param name="keyConnector">The key connector to use.</param>
        /// <param name="keyDescriptor">The key descriptor to use.</param>
        /// <param name="exportPrivate">Indicates whether to export the private key. Default is false.</param>
        /// <returns>The asymmetric key.</returns>
        public async Task<AsymmetricKey> GetAsymmetricKey(IKeyConnector keyConnector, KeyDescriptor keyDescriptor, bool exportPrivate = false)
        {
            if (exportPrivate)
                throw new NotSupportedException("Exporting private keys from Azure Key Vault is not supported.");

            if (keyConnector is not AzureKeyVaultConnector azKeyConnector)
                throw new ArgumentException($"The provided key connector type '{keyConnector.GetType().Name}' is not supported.", nameof(keyConnector));

            AsymmetricKey? key = null;

            // first check for a public key secret for the given key descriptor; this covers situations where a public-only
            //  key was imported to AZK (as it does not support public-only keys)...
            try
            {
                var publicKeyParameters = new GetPublicKeyParameters(keyDescriptor);
                var publicKeyAction = new GetPublicKeyAction(publicKeyParameters);
                var publicKeyResult = await _queryService.Execute(azKeyConnector.KeyVaultServiceReference, publicKeyAction);

                if (publicKeyResult.IsSuccessful && publicKeyResult.Result is not null)
                {
                    var publicKeyEntity = publicKeyResult.Result;

                    var idUrl = new Flurl.Url(publicKeyEntity.Id).PathSegments.TakeLast(2);
                    var id = idUrl.First();
                    var version = idUrl.Last();

                    var publicKeyId = Guid.Parse(id);
                    var publicKeyData = Base64UrlEncoder.DecodeBytes(publicKeyEntity.Value);

                    key = new AsymmetricKey(publicKeyId, publicKeyData)
                    {
                        Version = version
                    };
                }
            }
            catch (Flurl.Http.FlurlHttpException httpException)
            {
                if (httpException.StatusCode != (int)HttpStatusCode.NotFound)
                    throw;
            }

            // if we do not have a result yet, continue to leveraging AZK for the verification process; the vault should
            //  have the private key present...
            if (key is null)
            {
                var actionParameters = new GetKeyParameters(keyDescriptor.Id);
                var action = new GetKeyAction(actionParameters);

                var actionResult = await _queryService.Execute(azKeyConnector.KeyVaultServiceReference, action);
                var keyBundle = actionResult.Result
                    ?? throw new Exception("A null result was returned from the key creation action.");

                if (keyBundle.Key is null)
                    throw new Exception("A null key was returned from the key creation action.");

                var ecParams = keyBundle.Key.ToECParameters();

                using var cryptoProvider = ECDsa.Create(ecParams);

                var publicKey = cryptoProvider.ExportSubjectPublicKeyInfo();

                key = new AsymmetricKey(keyDescriptor.Id, publicKey, null);
            }

            return key;
        }

        /// <summary>
        /// Imports an asymmetric key using the specified key connector and key.
        /// </summary>
        /// <param name="keyConnector">The key connector to use.</param>
        /// <param name="key">The asymmetric key to import.</param>
        /// <returns>The imported key descriptor.</returns>
        public async Task<KeyDescriptor> ImportAsymmetricKey(IKeyConnector keyConnector, AsymmetricKey key)
        {
            if (keyConnector is not AzureKeyVaultConnector azKeyConnector)
                throw new ArgumentException($"The provided key connector type '{keyConnector.GetType().Name}' is not supported.", nameof(keyConnector));

            var descriptor = new KeyDescriptor(key);

            // AZK does not support storing keys which do not have a private portion... as such, we're forced to storing
            //  public-only keys as a secret...
            if (key.IsPrivateKey)
            {
                var importParameters = new ImportKeyParameters(key);
                var import = new ImportKeyAction(importParameters);

                var importResult = await _queryService.Execute(azKeyConnector.KeyVaultServiceReference, import);

                if (importResult.Result is null)
                    throw new Exception("A null result was returned from the key import action.");
            }
            else
            {
                var importParameters = new ImportSecretParameters(key);
                var import = new ImportSecretAction(importParameters);

                var importResult = await _queryService.Execute(azKeyConnector.KeyVaultServiceReference, import);

                if (importResult.Result is null)
                    throw new Exception("A null result was returned from the key import action.");
            }

            return descriptor;
        }

        /// <summary>
        /// Not supported. Retrieving keys from Azure Key Vault is not supported.
        /// </summary>
        /// <param name="keyConnector"></param>
        /// <param name="keyDescriptor"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public Task<SymmetricKey> GetSymmetricKey(IKeyConnector keyConnector, KeyDescriptor keyDescriptor)
        {
            throw new NotSupportedException("Exporting symmetric keys from Azure Key Vault is not supported.");
        }

        public Task<SymmetricData> EncryptData(IKeyConnector keyConnector, KeyDescriptor keyDescriptor, byte[] data)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> DecryptData(IKeyConnector keyConnector, KeyDescriptor keyDescriptor, SymmetricData data)
        {
            throw new NotImplementedException();
        }

        public Task<SymmetricData> EncryptData(IKeyConnector keyConnector, KeyDescriptor keyDescriptor, byte[] data, byte[] publicKey)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> DecryptData(IKeyConnector keyConnector, KeyDescriptor keyDescriptor, SymmetricData data, byte[] publicKey)
        {
            throw new NotImplementedException();
        }

        public Task<SymmetricEntity<TEntity>> EncryptEntity<TEntity>(IKeyConnector keyConnector, KeyDescriptor keyDescriptor, TEntity entity) where TEntity : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> DecryptEntity<TEntity>(IKeyConnector keyConnector, KeyDescriptor keyDescriptor, SymmetricEntity<TEntity> symmetricEntity) where TEntity : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<SymmetricEntity<TEntity>> EncryptEntity<TEntity>(IKeyConnector keyConnector, KeyDescriptor keyDescriptor, TEntity entity, byte[] publicKey) where TEntity : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> DecryptEntity<TEntity>(IKeyConnector keyConnector, KeyDescriptor keyDescriptor, SymmetricEntity<TEntity> symmetricEntity, byte[] publicKey) where TEntity : IEntity
        {
            throw new NotImplementedException();
        }

        public Task<KeyDescriptor> ImportSymmetricKey(IKeyConnector keyConnector, SymmetricKey key)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}
