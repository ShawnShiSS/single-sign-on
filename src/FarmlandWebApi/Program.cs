using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure token authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        // This is the URL for our authentication server
        options.Authority = "https://localhost:5001";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

// Authorization policy to require an API scope called farmlandwebapi in the JWT token.
// "farmlandwebapi" should be the same value defined in authentication server, see farmlandwebapi in IdentityServerConfigurationDbSeeder.cs.
// Without this, the API would accept any access token issued by the authentication server.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequiresFarmlandWebApiApiScope", policy =>
    {
        // Requires the user to be authenticated
        policy.RequireAuthenticatedUser();
        // Requires the user to have access to the farmland web api.
        policy.RequireClaim("scope", "farmlandwebapi");
    });
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Requires token authentication configured above
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers()
   // Apply the authorization policy globally
   .RequireAuthorization("RequiresFarmlandWebApiApiScope");

app.Run();
