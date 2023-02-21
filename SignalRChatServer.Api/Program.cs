using SignalRChatServer.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddCors(opt => opt.AddDefaultPolicy(policy =>
policy.AllowCredentials()
       .AllowAnyHeader()
       .AllowAnyMethod().SetIsOriginAllowed(x => true)
)); 

builder.Services.AddSignalR();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.UseAuthorization();

app.UseCors();
app.MapControllers();
app.MapHub<ChatHub>("/ChatHub");
app.Run();
