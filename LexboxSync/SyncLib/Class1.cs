using System;
using System.Text.Json;
using AppLayer.Api;
using CrdtLib;
using CrdtLib.Db;
using LcmCrdtModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LexboxSync;

public class LexSyncer
{
  protected IServiceProvider _services;
  public DataModel DataModel;
  private CrdtDbContext _crdtDbContext;

  public async Task Code()
  {
    var lcmApi = await LexboxLcmApiFactory.CreateApi("C:/ProgramData/SIL/FieldWorks/Projects/test-chris-02/test-chris-02.fwdata", false);

    _services = new ServiceCollection()
        .AddLcmCrdtClient(":memory:")
        .BuildServiceProvider();
    _crdtDbContext = _services.GetRequiredService<CrdtDbContext>();
    DataModel = _services.GetRequiredService<DataModel>();

  }
}
