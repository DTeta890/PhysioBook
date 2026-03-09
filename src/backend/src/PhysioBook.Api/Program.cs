var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO: Configure JWT authentication
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options => { ... });

// TODO: Register MediatR
// builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(...));

// TODO: Register FluentValidation
// builder.Services.AddFluentValidationAutoValidation();

// TODO: Register SignalR
// builder.Services.AddSignalR();

// TODO: Register Infrastructure services (DbContext, repositories, MassTransit)

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// TODO: Add tenant middleware
// app.UseTenantMiddleware();

// TODO: Add authentication & authorization middleware
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

// TODO: Map SignalR hubs
// app.MapHub<CalendarHub>("/hubs/calendar");

app.Run();
