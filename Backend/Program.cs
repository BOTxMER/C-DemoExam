var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

class Order
{
    public Order(int number, DateOnly startDate, string device, string problemType, string description, string cLient, string status)
    {   
        Number = number;
        StartDate = startDate;
        Device = device;
        ProblemType = problemType;
        Description = description;
        CLient = cLient;
        Status = status;
    }

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

record class UpdateOrderDTO(int Number, string? Status = "", string? Description = "", string? Master = "", string? Comment = "");
