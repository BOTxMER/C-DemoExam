List<Order> repo =[
    new (1,new(2008,6,21),"Aegis Hero",24,"Кока-колы", "Джесси Пинкман", "В ожидании доставки"),
    new (2,new(2007,2,14),"Waka",12,"Синие кириешки ", "Волтер Вайт", "В ожидании доставки")
];

var builder = WebApplication.CreateBuilder();
builder.Services.AddCors();
var app = builder.Build();

app.UseCors(option => option
.AllowAnyOrigin()
.AllowAnyMethod()
.AllowAnyHeader());

string message = "";

app.MapGet("/orders", (int param = 0) => 
{
    string buffer = message;
    message="";
    if(param !=0)
        return new { repo = repo.FindAll(x=> x.Number == param), message= buffer};
    return new {repo, message= buffer};
});
app.MapGet("/create", ([AsParameters] Order dto) =>repo.Add(dto));
app.MapGet("/update", ([AsParameters]UpdateOrderDTO dto) =>
{
    var order = repo.Find(x=>x.Number == dto.Number);
    if(order == null)
        return;
    if(dto.Status != order.Status && dto.Status != "")
    {
        order.Status=dto.Status;
        message += $"Статус заказа №{order.Number} изменен\n";
        if(order.Status == "Доставлено")
        {
            message += $"Заявка №{order.Number} завершена\n";
            order.EndDate=DateOnly.FromDateTime(DateTime.Now);
        }
    }
    if(dto.TasteLiquid!="")
         order.TasteLiquid=dto.TasteLiquid;
    if(dto.Diller !="")
        order.Diller=dto.Diller;
    if(dto.Comment !="")
        order.Comments.Add(dto.Comment);
});

int complete_count() => repo.FindAll(x=>x.Status =="доставлено").Count;
Dictionary<string,int> get_vape_rent_stat() =>
    repo.GroupBy(x=>x.VapeRent).Select(x=>(x.Key, x.Count())).ToDictionary(k => k.Key, v=>v.Item2); 

double get_average_time_to_complete() =>
    complete_count()==0?0:
    repo.FindAll(x=>x.Status=="выполнено").Select(x=>x.EndDate.Value.DayNumber-x.StartDate.DayNumber).Sum()/complete_count();

app.MapGet("/statistics",()=>new{
    complete_count = complete_count(),
    vape_rent_stat = get_vape_rent_stat(),
    average_time_to_complete = get_average_time_to_complete(),
}
);

app.Run();

class Order
{
    public Order(int number, DateOnly startDate, string vapeRent, int hour, string tasteLiquid, string client, string status)
    {   
        Number = number;
        StartDate = startDate;
        VapeRent = vapeRent;
        Hour = hour;
        TasteLiquid = tasteLiquid;
        Client = client;
        Status = status;
    }

    public int Number { get; set;}
    public DateOnly StartDate { get; set;}
    public string VapeRent { get; set;}
    public int Hour { get; set;}
    public string TasteLiquid { get; set;}
    public string Client { get; set;}
    public string Status {get; set;}
    public DateOnly? EndDate { get; set;}=null;
    public string? Diller { get; set;} = "Не назначен";
    public List<string>? Comments { get; set;} = [];
}

record class UpdateOrderDTO(int Number, string? Status = "", string? TasteLiquid = "", string? Diller = "", string? Comment = "");

