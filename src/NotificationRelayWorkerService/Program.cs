using NotificationRelayWorkerService.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceBus(builder.Configuration);   
builder.AddMassTransit();
var host = builder.Build();
host.Run();
