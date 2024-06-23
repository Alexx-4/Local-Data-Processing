using LocalDataProcessing.App;
using LocalDataProcessing.Core.Services;
using LocalDataProcessing.Logic.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IPaymentDataProcessor, PaymentDataProcessor>();

builder.Services.AddHostedService<PaymentCsvWorker>();

var host = builder.Build();
host.Run();