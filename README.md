# Samples for IdentityModel.OidcClient

> [!IMPORTANT]  
> The samples are now maintained in Duende Software's [FOSS repository](https://github.com/DuendeSoftware/foss/tree/main/identity-model-oidc-client/samples).


This repository contains samples that demonstrate how to use
[IdentityModel.OidcClient](https://github.com/IdentityModel/IdentityModel) to create
OpenId Connect client applications with a variety of platforms and tools, including
- [.NET MAUI](https://github.com/IdentityModel/IdentityModel.OidcClient.Samples/tree/main/Maui)
- [WPF with the system browser](https://github.com/IdentityModel/IdentityModel.OidcClient.Samples/tree/main/Wpf)
- [WPF with an embedded browser](https://github.com/IdentityModel/IdentityModel.OidcClient.Samples/tree/main/WpfWebView2)
- [WinForms with an embedded browser](https://github.com/IdentityModel/IdentityModel.OidcClient.Samples/tree/main/WinFormsWebView2)
- [Cross Platform Console Applications](https://github.com/IdentityModel/IdentityModel.OidcClient.Samples/tree/main/NetCoreConsoleClient) (relies on kestrel for processing the callback)
- [Windows Console Applications](https://github.com/IdentityModel/IdentityModel.OidcClient.Samples/tree/main/HttpSysConsoleClient) (relies on an HttpListener - a wrapper around the windows HTTP.sys driver)
- [Windows Console Applications using custom uri schemes](https://github.com/IdentityModel/IdentityModel.OidcClient.Samples/tree/main/WindowsConsoleSystemBrowser)

All samples use a demo instance of Duende.IdentityServer (https://demo.duendesoftware.com)
as their OIDC Provider. You can see its source code
[here](https://github.com/DuendeSoftware/demo.duendesoftware.com).

You can login with `alice/alice` or `bob/bob`

## Additional samples

* [Unity3D/Unity3D.Authentication.Example](https://github.com/peterhorsley/Unity3D.Authentication.Example)
* [Unity3D/UnityOidcClient](https://github.com/EversongWoods/UnityOidcClient)

## No longer maintained

These samples are no longer maintained because their underlying technology is no
longer supported. 

- [UWP](https://github.com/IdentityModel/IdentityModel.OidcClient.Samples/tree/archived/uwp/Uwp)
- [Xamarin](https://github.com/IdentityModel/IdentityModel.OidcClient.Samples/tree/archived/xamarin/XamarinAndroidClient)
- [Xamarin Forms](https://github.com/IdentityModel/IdentityModel.OidcClient.Samples/tree/archived/xamarin/XamarinForms)
- [Xamarin iOS - AuthenticationServices](https://github.com/IdentityModel/IdentityModel.OidcClient.Samples/tree/archived/xamarin/iOS_AuthenticationServices)
- [Xamarin iOS - SafariServices](https://github.com/IdentityModel/IdentityModel.OidcClient.Samples/tree/archived/xamarin/iOS_SafariServices)
- [WinForms with WebBrowser](https://github.com/IdentityModel/IdentityModel.OidcClient.Samples/releases/tree/archived/webbrowser/WinFormsWebView) (this used the WebBrowser component, which used an ActiveX control to embed Internet Explorer into your application and render the login UI)
- [WPF with WebBrowser](https://github.com/IdentityModel/IdentityModel.OidcClient.Samples/releases/tag/tree/archived/webbrowser/WpfWebView) (similar to Winforms with WebBrowser above, this used the WebBrowser component)
