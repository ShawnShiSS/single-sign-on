# Authentication server for Single Sign-On (SSO) using ASP.NET Core Identity and Identity Server
A starting point to build a central authentication server for single sign-on (SSO). The project is ready for real world applications using ASP.NET Core Identity for user membership management, and Identity Server for OpenID Connect and OAuth 2.0 implementation.

# Features
* User membership management using ASP.NET Core Identity and SQL database
* OpenID Connect and OAuth 2.0 implementation using IdentityServer
* Configuration data storage (i.e., API resource, Identity resource, clients, etc.) in SQL database
* Persisted grants data storage (i.e. refresh token, digital signing keys, etc.) in SQL database
* Entity Framework Core support for database schema management using migrations (Code first approach)
* Support database auto-creation at application startup in development environment for fast up and running
* Support initial data-seeding at application startup in development environment for fast up and running
* Support API endpoints for user management hosted in the SSO application as local API endpoints

# Getting Started
* Clone the repo
* Run the "SsoServer" project in Visual Studio or in command line

# Additional Resources
I have published some short articles to cover different aspects of this project. Please feel free to give them a read.

* [Single Sign-On (SSO) Simplified: Understanding How SSO Works in Plain English](https://medium.com/geekculture/single-sign-on-sso-simplified-understanding-how-sso-works-in-plain-english-7d5739d23aeb)
* [Build Your Own Authentication Server for Single Sign-On (SSO) in ASP.NET Core](https://medium.com/@shawn-shi/build-your-own-single-sign-on-sso-server-in-asp-net-core-4344f6b390d1)
* [REST API for User Management in Authentication Server for Single Sign-On](https://shawn-shi.medium.com/rest-api-best-practices-design-patterns-for-building-maintainable-web-apis-in-asp-net-core-b95addad084) : discuss how to develop REST API endpoints to manage users in an SSO server at the design level
* [REST API for User Management in Authentication Server for Single Sign-On (2)](https://shawn-shi.medium.com/rest-api-best-practices-implement-design-patterns-for-maintainable-web-apis-in-asp-net-core-4b9118df39a)  : discuss how to develop REST API endpoints to manage users in an SSO server at the code level
* [Protect Web API using Your Own Authentication Server](https://shawn-shi.medium.com/protect-web-api-using-your-own-authentication-server-f0d04047bdd0)
* [Access Web API Protected by Your Own Authentication Server](https://shawn-shi.medium.com/access-web-api-protected-by-your-own-authentication-server-846d40c7317d)

# Give a star
:star: If you enjoy this project, or are using this project to start your exciting new project, or are just forking it to play, please give it a star. Much appreciated! :star: 

# License
This repo is for development purpose.

**The IdentityServer library used in this project has its own license requirement. Please refer to [Duende IdentityServer](https://duendesoftware.com/products/identityserver) for additional license information.**
