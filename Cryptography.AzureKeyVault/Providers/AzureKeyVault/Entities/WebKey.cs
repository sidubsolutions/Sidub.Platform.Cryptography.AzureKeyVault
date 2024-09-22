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
using System.Security.Cryptography;

#endregion

namespace Sidub.Platform.Cryptography.Providers.AzureKeyVault.Entities
{

    /// <summary>
    /// Represents a web key entity.
    /// </summary>
    [Entity("WebKey")]
    internal class WebKey : IEntity
    {

        #region Public properties

        /// <summary>
        /// Gets or sets the identifier of the web key.
        /// </summary>
        [EntityKey<string>("kid")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the key.
        /// </summary>
        [EntityField<string>("kty")]
        public string KeyType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the curve.
        /// </summary>
        [EntityField<string>("crv")]
        public string CurveName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the D value of the curve point.
        /// </summary>
        [EntityField<string>("d")]
        public string CurvePointD { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the X value of the curve point.
        /// </summary>
        [EntityField<string>("x")]
        public string CurvePointX { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Y value of the curve point.
        /// </summary>
        [EntityField<string>("y")]
        public string CurvePointY { get; set; } = string.Empty;

        #endregion

        #region Public methods

        /// <summary>
        /// Converts the web key to EC parameters.
        /// </summary>
        /// <returns>The EC parameters.</returns>
        public ECParameters ToECParameters()
        {
            var parms = new ECParameters()
            {
                Curve = CurveName switch
                {
                    "P-256" => ECCurve.NamedCurves.nistP256,
                    _ => throw new Exception("Unhandled curve encountered.")
                },
                Q = new ECPoint()
                {
                    X = Base64UrlEncoder.DecodeBytes(CurvePointX), //Base64UrlEncoder.DecodeBytes
                    Y = Base64UrlEncoder.DecodeBytes(CurvePointY) //Base64UrlEncoder.DecodeBytes
                }
            };

            return parms;
        }

        #endregion

        #region IEntity implementation

        /// <summary>
        /// Gets or sets a value indicating whether the entity is retrieved from storage.
        /// </summary>
        bool IEntity.IsRetrievedFromStorage { get; set; }

        #endregion

        #region Public static methods

        /// <summary>
        /// Creates a web key from an asymmetric key.
        /// </summary>
        /// <param name="key">The asymmetric key.</param>
        /// <returns>The created web key.</returns>
        public static WebKey CreateFromKey(AsymmetricKey key)
        {
            var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);

            if (key.PrivateKey is not null)
                ecdsa.ImportPkcs8PrivateKey(key.PrivateKey, out var _);
            else
                ecdsa.ImportSubjectPublicKeyInfo(key.PublicKey, out var _);

            var ecParam = ecdsa.ExportExplicitParameters(key.PrivateKey is not null);

            var importParameters = new WebKey();
            importParameters.Id = key.Id.ToString("D");
            importParameters.KeyType = "EC";
            importParameters.CurveName = "P-256";
            importParameters.CurvePointX = Base64UrlEncoder.Encode(ecParam.Q.X); //Base64UrlEncoder.Encode
            importParameters.CurvePointY = Base64UrlEncoder.Encode(ecParam.Q.Y); //Base64UrlEncoder.Encode

            if (key.PrivateKey is not null)
                importParameters.CurvePointD = Base64UrlEncoder.Encode(ecParam.D); //Base64UrlEncoder.Encode

            return importParameters;
        }

        #endregion

    }

}
