# aspnetcore-azurekeyvault-sample
A sample ASP.NET core project that demonstrates accessing an azure key vault using X.509 certificate. 
To try out the example, first need to replace the values in the Startup file. 

```
 "KeyVault":  {
    "URL": "Replace with DNS value of your key vault",
    "SubjectDistinguishedName": "Replace with the value you set when generating the certificate"
  }, 
  "AzureAD":  {
    "AppId": "Replace with the client/app id of the app you registered on azure."
  }
```

Checkout the accompaning [blog post](https://www.taithienbo.com/access-azure-key-vault-from-an-aspnetcore-app-on-iis-using-x509-certificate/) for more info. 
