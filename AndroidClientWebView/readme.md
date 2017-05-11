


Updated 2016-12-07

Tested on Android Emulator
* 7.1.1
* 5.0
* 4.4


# Identity Server Configuraion


## IdenityServer 3 Client Configuration
```
new Client
{
    ClientName = "Native Client (Hybrid Flow with PKCE)",
    ClientId = "native",
    Flow = Flows.HybridWithProofKey,
    RedirectUris = new List<string>
    {
        "https://someUri",
    },
    ClientSecrets = new List<Secret>
    {
        new Secret("secret".Sha256())
        },
    AllowAccessToAllScopes = true
},
```

[Taken from Demo Server Source Here](https://github.com/IdentityServer/demo.identityserver.io/blob/master/AzureWebSitesDeployment/Config/Clients.cs)



## IdenityServer 4 Client Configuration

Warning: I got the android client to work using the below client. While I believe its correct I'd really like others to review my settings here to ensure its secured correctly. 

```
  new Client
    {
        ClientName = "Native Client (Hybrid Flow with PKCE)",
        ClientId = "native",
        AllowedGrantTypes = GrantTypes.Hybrid,
        RedirectUris = new List<string>
        {
            "io.identitymodel.native://callback",
        },
        ClientSecrets = new List<Secret>
        {
            new Secret("secret".Sha256())
        },
        AllowOfflineAccess = true,
        RequirePkce = true,
        AllowAccessTokensViaBrowser = true,
        AllowedScopes =
        {
            IdentityServerConstants.StandardScopes.OpenId,
            IdentityServerConstants.StandardScopes.Profile,
            IdentityServerConstants.StandardScopes.OfflineAccess
            // The IdenityServer4 Quick Starts use 'api1' vs 'api' if you do use 'api1' be sure to change the client scope to match 
            "api1", 

        },
    },
```

# Notes

Contributed by https://github.com/joaomello - please contact João directly if there are issues.
