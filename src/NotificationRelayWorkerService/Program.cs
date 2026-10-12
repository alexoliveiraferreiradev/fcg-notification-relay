using NotificationRelayWorkerService.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddServiceBus();
builder.Services.AddMassTransitExtension(builder.Configuration);
var host = builder.Build();
host.Run();
