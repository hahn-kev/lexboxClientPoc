using System.Text.Json.Serialization.Metadata;
using AppLayer.Api;
using CrdtLib;
using LcmCrdtModel;
using Lexbox.ClientServer.Hubs;
using lexboxClientContracts;
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