Various Samples
=======

This repository contains various samples which might be moved intto separate permanent repositories later.



## WebApiAzureActiveDirectory.sln ##

Shows how to do manual JWT token validation issued by Azure Active Directory. Owin infrastructure has JWT validation support and i commented it out in order to show how you can validate JWT token manually using SecurityKey obtained from Azure Active Directory.



- WebApiAzureAD.Client project is a small native client app which request JWT token.
- WebApiAzureAD.Api is OWIN based API which get request from client and manually validating JWT token issued by Azure AD.


