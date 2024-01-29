using System.Text.Json.Serialization.Metadata;
using CrdtLib;
using CrdtLib.Db;
using LcmCrdtModel;
using Lexbox.ClientServer.Hubs;
using TypedSignalR.Client.DevTools;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
    {
        Modifiers = { LcmCrdtKernel.PolyTypeListBuilder().MakeModifier() }
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
    {
        Modifiers = { LcmCrdtKernel.PolyTypeListBuilder().MakeModifier() }
    };
});
builder.Services.AddLcmCrdtClient("tmp.sqlite");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.UseSignalRHubSpecification();
    app.UseSignalRHubDevelopmentUI();
}

app.UseHttpsRedirection();

app.MapHub<LexboxApiHub>("/api/hub/project");

await app.Services.GetRequiredService<CrdtDbContext>().Database.EnsureCreatedAsync();
app.Run();
