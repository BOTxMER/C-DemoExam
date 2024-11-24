var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

class Order
{
    public int Number { get; set;}
    public DateOnly StartDate { get; set;}
    public string Device { get; set;}
    public string ProblemType { get; set;}
    public string Description { get; set;}
    public string CLient { get; set;}
    public string Status {get; set;}
    public DateOnly? EndDate { get; set;}=null;
    public string? Master { get; set;} = "Не назначен";
    public List<string>? Comments { get; set;} = [];
}
