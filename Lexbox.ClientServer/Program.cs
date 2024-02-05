using System.Text.Json.Serialization.Metadata;
using AppLayer.Api;
using CrdtLib;
using LcmCrdtModel;
using Lexbox.ClientServer.Hubs;
using lexboxClientContracts;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using TypedSignalR.Client.DevTools;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<JsonOptions>().PostConfigure<IOptions<CrdtConfig>>((jsonOptions, crdtConfig) =>
{
    jsonOptions.SerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
    {
        Modifiers = { crdtConfig.Value.MakeJsonTypeModifier() }
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR().AddJsonProtocol();
builder.Services.AddOptions<JsonHubProtocolOptions>().PostConfigure<IOptions<CrdtConfig>>((jsonOptions, crdtConfig) =>
{
    jsonOptions.PayloadSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
    {
        Modifiers = { crdtConfig.Value.MakeJsonTypeModifier() }
    };
});
var useCrdt = true;
if (useCrdt)
    builder.Services.AddLcmCrdtClient("tmp.sqlite");
else
{
    var api = await LexboxLcmApiFactory.CreateApi(@"C:\ProgramData\SIL\FieldWorks\Projects\sena-3\sena-3.fwdata",
        false);
    builder.Services.AddSingleton<ILexboxApi>(api);
}

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
app.Run();