# Sidub Platform - Azure Key Vault Cryptography

This repository contains the Azure Key Vault cryptography module for the Sidub
Platform. It provides connectors and handlers that allow the cryptography
framework to interact with the Azure Key Vault services.

## Main Components
This library simply provides the connectors and handlers for Azure Key Vault
services.

### Registering an Azure Key Vault service
To connect to an Azure Key Vault service, register it within the service
registry using the `CryptographyServiceReference` and `AzureKeyVaultConnector`
classes.

```csharp
serviceCollection.AddSidubPlatform(serviceProvider =>
{
    var metadata = new InMemoryServiceRegistry();

    var cryptographyReference = new CryptographyServiceReference("vault");
    var keyConnector = new AzureKeyVaultConnector();
    metadata.RegisterServiceReference(cryptographyReference, keyConnector);

    return metadata;
});
```

### Supported operations
Not all operations are supported by the Azure Key Vault service. The following
operations are supported:
 - Create, import symmetric key
 - Create, import, get (public only) asymmetric key
 - Sign, verify dadta using asymmetric key

### Unsupported operations
The following operations are not supported by the Azure Key Vault service:
 - Encrypt, decrypt data using asymmetric key exchange
 - Encrypt, decrypt data using symmetric key (planned with HSM support)

To interact with the HTTP data service, use any of the functionality defined
within the storage framework, simply passing the storage service reference
associated with the Gremlin connector.

## License
This project is dual-licensed under the AGPL v3 or a proprietary license. For
details, see [https://sidub.ca/licensing](https://sidub.ca/licensing) or the
LICENSE.txt file.