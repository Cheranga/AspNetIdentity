* Install the following nuget packages
------------------------------------
Microsoft.AspNet.Identity.Owin
Microsoft.AspNet.Identity.EntityFramework
Microsoft.Owin.Host.SystemWeb
Microsoft.AspNet.WebApi.Owin
Microsoft.Owin.Security.OAuth
Microsoft.Owin.Cors

* Create a custom identity class and corresponding IdentityDbContext
------------------------------------------------------------------
public class ApplicationUser : IdentityUser
{
}

public class ApplicationDbContext : IdentityDbContext
{
}

* Add a connection string to the identity database
------------------------------------------------
<connectionStrings>
    <add name="security" connectionString="Data Source=(LocalDB)\MSSQLLocalDB; Initial Catalog=IdentityDb; Integrated Security=SSPI" providerName="System.Data.SqlClient"/>
</connectionStrings>

* Create the database and add the initial migration
-------------------------------------------------
Enable-Migrations
add-migration InitialCreate

* Seed the database with a default user and update the database
-----------------------------------------------------------------
In the "Migration -> Configuration.cs" edit the "Seed" method and add a default user
Update-Database

* Create a custom 'user manager' class
--------------------------------------
This class will handle the user/roles related operations

public class ApplicationUserManager : UserManager<ApplicationUser>
{
}

* Create the OWIN startup class
-------------------------------
This will be called by OWIN runtime, and we will use it to setup our application.

* Create a Base API Controller, ModelFactory and a DTO class to manipulate user operations
-------------------------------------------------------------------------------------------
BaseApiController - The base class which the other controller classes will inherit from.
ModelFactory - The factory class to transform internal classes to view model instances.
UserDto - The DTO instance which will be exposed to public.

* Create a separate class as a DTO to create a user and add an action method to the API controller
--------------------------------------------------------------------------------------------------
This DTO class will be used by the client when it needs to create a user.
The controller will validate the object, and if it's valid will create the user, otherwise will send an error response.


* Sending user confirmation emails to validate if the user is actually the user as per the email
------------------------------------------------------------------------------------------------
Register an email provider (e.x - 'SendGrid')
Create an 'EmailService' class which uses this to send an email
Register this email service instance with the 'UserManager'
Create an action method in the "AccountsController" and use the "ApplicationUserManager" to,
	First create a email confirmation token ("GenerateEmailConfirmationTokenAsync()")
	Then call the "SendEmailAsync()" with the email token. This will send the user an email.

Create an action method (B), to receive the email token and the userid from the user.
Once the user receives the email, he'll click on the link and the action method (B) will be called

NOTE:
The action method (B) create it as a HTTP POST, and let the user send username, password and the email token together. By doing this
the application can really determine that the user is what he's supposed to be.

* Change User / Password Policy
-------------------------------
If needed we can control the allowed usernames and passwords

// To change user policy
applicationUserManager.UserValidator = new UserValidator<ApplicationUser>(applicationUserManager)
{
	AllowOnlyAlphaNumericUserNames = true,
	RequireUniqueEmail = false
}

// To change the password policy
applicationUserManager.PasswordValidator = new PasswordValidator
{
	RequiredLength = 6,
	RequireNonLetterOrDigit = true,
	RequireDigit = false,
	RequireLowercase = true,
	RequireUppercase = true
}

* Custom user policy validator
------------------------------
public class CustomUserValidator : UserValidator<ApplicationUser>
{
    public CustomUserValidator(ApplicationUserManager manager) : base(manager)
    {
    }

	public override async Task<IdentityResult> ValidateAsync(ApplicationUser user)
    {
		// Implement the validation here
	}
}

Then register this implementation with the "ApplicationUserManager"

In the "Create" method of "ApplicationUserManager" class,

var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(applicationContext));
userManager.UserValidator = new CustomUserValidator(userManager);

* Custom password policy validator
----------------------------------
public class CustomPasswordValidator : PasswordValidator
{
	public override async Task<IdentityResult> ValidateAsync(string password)
    {
		// implement the validation here
	}
}

In the "Create" method of "ApplicationUserManager" class,

var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(applicationContext));
userManager.PasswordValidator = new CustomPasswordValidator
{
	RequireUniqueEmail = false	
}

* Changing Password
-------------------
Create an end point in "AccountsController"



* Delete User Account
---------------------
Create an end point in "AccountsController"


* Implementing authentication and authorization
-----------------------------------------------
This application will act as both the "authorization server" and the "resource server".

- Create OAuthProvider class

 public class CustomOAuthProvider : OAuthAuthorizationServerProvider
 {
	public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
    {
		// CONSIDERING THE REQUESTS ARE ALWAYS COMING FROM A TRUSTED SOURCE
        context.Validated();
        return Task.FromResult<object>(null);
    }
 
    public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
    {
		// grant JWT token
	}
 
 }

- Create "GenerateUserIdentityAsync" method in the "ApplicationUser"
This will create a "ClaimsIdentity" instance

public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager userManager, string authenticationType)
{
    return await userManager.CreateIdentityAsync(this, authenticationType);
}

- Issue JSON Web Tokens instead of Default Access Tokens

IMPORTANT - INSTALL OLD PACKAGE '4.0.2' FOR 'System.IdentityModel.Tokens.Jwt'

System.IdentityModel.Tokens.Jwt
Thinktecture.IdentityModel.Core

- Since ASP.NET has no direct support to issue JWT need to implement it

public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
{
	public string Protect(AuthenticationTicket data)
    {
		// This creates a custom JWT.
	}
}

- Add support for OAuth 2.0 JWT Generation
In "Startup" class

 var serverOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new CustomOAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat("http://localhost:51678")
            };
            builder.UseOAuthAuthorizationServer(serverOptions);


- Decorate the controller / action methods which needs authorization

- Configure JWT token consumption
Since we change the way the JWT creation, we need to configure our API to trust tokens issued by our authorization server only.
So install the following nuget package and in the startup class configure JWT consumption

Install-Package Microsoft.Owin.Security.Jwt

- Startup.cs

private void ConfigureOAuthTokenConsumption(IAppBuilder builder)
        {
            var issuer = "http://localhost:51678";
            var audienceId = ConfigurationManager.AppSettings.Get("as:AudienceId");
            var audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings.Get("as:AudienceSecret"));


            builder.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AllowedAudiences = new[] {audienceId},
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                {
                    new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                }
            });

        }


* Roles based authorization
---------------------------
- Create a custom role manager class
public class ApplicationRoleManager : RoleManager<IdentityRole>
{
}

- Assign the role manager class to the OWIN context
private void ConfigureOAuthTokenGeneration(IAppBuilder app)
{	
	app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
 
}

- Add a "RolesController", and add corresponding action methods

- Decorate action methods with roles if necessary

- Drop the database and modify the migration file to add a default user with "admin" priviledges
