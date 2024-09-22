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

using Sidub.Platform.Cryptography.Providers.AzureKeyVault.Entities;
using Sidub.Platform.Storage.Commands;

#endregion

namespace Sidub.Platform.Cryptography.Providers.AzureKeyVault.Actions
{

    /// <summary>
    /// Represents an action to import a key.
    /// </summary>
    internal class ImportKeyAction : IActionCommand<ImportKeyParameters, KeyBundle>
    {

        #region Public properties

        /// <summary>
        /// Gets or sets the import key parameters.
        /// </summary>
        public ImportKeyParameters Parameters { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportKeyAction"/> class.
        /// </summary>
        /// <param name="parameters">The import key parameters.</param>
        public ImportKeyAction(ImportKeyParameters parameters)
        {
            Parameters = parameters;
        }

        #endregion

        #region IActionCommand implementation

        /// <summary>
        /// Gets the action command type.
        /// </summary>
        ActionCommandType IActionCommand<ImportKeyParameters, KeyBundle>.ActionCommand { get => ActionCommandType.Upsert; }

        #endregion

    }

}