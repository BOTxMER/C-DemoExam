List<Order> repo =[
    new (1,new(2008,6,21),"Laptop","Не работает","пролил колу на laptop", "Джесси Пинкман", "В ожидании")
];

var builder = WebApplication.CreateBuilder();
builder.Services.AddCors();
var app = builder.Build();

app.UseCors(option => option
.AllowAnyOrigin()
.AllowAnyMethod()
.AllowAnyHeader());

string message = "";

app.MapGet("orders", (int param = 0) => 
{
    string buffer = message;
    message="";
    if(param !=0)
        return new { repo = repo.FindAll(x=> x.Number == param), message= buffer};
    return new {repo, message= buffer};
});
app.MapGet("create", ([AsParameters] Order dto) =>repo.Add(dto));
app.MapGet("update", ([AsParameters]UpdateOrderDTO dto) =>
{
    var order = repo.Find(x=>x.Number == dto.Number);
    if(order == null)
        return;
    if(dto.Status != order.Status && dto.Status != "")
    {
        order.Status=dto.Status;
        message += $"Статус заявки №{order.Number} изменен\n";
        if(order.Status == "выполнено")
        {
            message += $"Заявка №{order.Number} завершена\n";
            order.EndDate=DateOnly.FromDateTime(DateTime.Now);
        }
    }
    if(dto.Description!="")
         order.Description=dto.Description;
    if(dto.Master !="")
        order.Master=dto.Master;
    if(dto.Comment !="")
        order.Comments.Add(dto.Comment);
});

int complete_count() => repo.FindAll(x=>x.Status =="выполнено").Count;
Dictionary<string,int> get_problem_type_stat() =>
    repo.GroupBy(x=>x.ProblemType).Select(x=>(x.Key, x.Count())).ToDictionary(k => k.Key, v=>v.Item2); 

double get_average_time_to_complete() =>
    complete_count()==0?0:
    repo.FindAll(x=>x.Status=="выполнено").Select(x=>x.EndDate.Value.DayNumber-x.StartDate.DayNumber).Sum()/complete_count();

app.MapGet("/statistics",()=>new{
    complete_count = complete_count(),
    problem_type_stat = get_problem_type_stat(),
    average_time_to_complete = get_average_time_to_complete(),
}
);

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

