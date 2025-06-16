var builder = WebApplication.CreateBuilder(args);
ConfigureService();
var app = builder.Build();
Configure();
app.Run();

void ConfigureService()
{

}
void Configure()
{
	app.MapGet("/", () => "Hello World!");
}