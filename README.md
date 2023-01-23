# Single Sign-On (SSO) solution using ASP.NET Core Identity and Identity Server
A starting point to build a single sign-on (SSO) solution that is ready for real world applications using ASP.NET Core Identity for user membership management, and Identity Server for OpenID Connect and OAuth 2.0 implementation.

# Features
* User membership management using ASP.NET Core Identity and SQL database
* OpenID Connect and OAuth 2.0 implementation using IdentityServer
* Configuration data storage (i.e., API resource, Identity resource, clients, etc.) in SQL database
* Persisted grants data storage (i.e. refresh token, digital signing keys, etc.) in SQL database
* Entity Framework Core support for database schema management using migrations (Code first approach)
* Support database auto-creation at application startup in development environment for fast up and running
* Support initial data-seeding at application startup in development environment for fast up and running
* (future feature) Support API endpoints to the SSO application as local API endpoints to manage users 

# Getting Started
* Clone the repo
* Run the "SsoServer" project in Visual Studio or in command line

# Additional Resources
I have published some short articles to cover different aspects of this project. Please feel free to give them a read.

* [Single Sign-On (SSO) Simplified: Understanding How SSO Works in Plain English](https://medium.com/geekculture/single-sign-on-sso-simplified-understanding-how-sso-works-in-plain-english-7d5739d23aeb)

# License
This repo is for development purpose.

**The IdentityServer library used in this project has its own license requirement. Please refer to [Duende IdentityServer](https://duendesoftware.com/products/identityserver) for additional license information.**
