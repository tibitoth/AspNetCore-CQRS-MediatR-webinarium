# AspNetCore-CQRS-MediatR-webinarium

https://www.talentera.hu/esemenyek/cqrs-es-mediator-tervezesi-mintak-hasznalata-asp-net-core-ral

Talentera előadássorozat demó.

Kiinduló projekt a `starter` ágon, a teljes megoldást pedig a `main` ágon találjátok.

[Prezentáció](T%C3%B3th%20Tibor%20-%20Talentera%20CQRS%20MediatR%2020210427.pdf) szintén a repóban található.

## Előkészületek

* Induljunk ki a `starter` ágból.
* Migráljuk fel az adatbázist egy PowerShellben kiadott paranccsal a Dal projektben állva `Update-Database`
* Próbáljuk ki a meglévő alkalmazás funkcionalitását

### Kiinduló projekt ismertetése

TBD

## MediatR beüzemelése, első Query készítése

Vegyük fel a Bll projektbe a MediatR NuGet csomagot

```xml
<PackageReference Include="MediatR" Version="9.0.0" />
```

Az Api projektbe pedig a MediatR.Extensions.Microsoft.DependencyInjection csomagot, ami ASP.NET Core DI integrációt is tartalmaz, illetve implicit behúzza a MediatR csomagot is.

```xml
<PackageReference Include="MediatR.Extensions.Microsoft.DependencyInjection" Version="9.0.0" />
```

Vegyünk fel a mappastruktúrát a Bll projektbe, ahova a CQRS elemeink fognak kerülni:

```
.
└── CqrsMediator.Demo.Bll
    └── Features
        ├── Catalog
        │   ├── Queries
        │   └── Commands
        └── Order
            ├── Queries
            └── Commands
```

A Catalog / Queries mappába vegyünk fel egy új osztályt `FindProduct` néven, ami az `ICatalogService.FindProduct` listázó metódusunkat fogja kiváltani.

Ebbe az osztályba beágyazott osztályként vegyük fel a `Query` osztályunkat, ami a kérés paramétereit fogja tartalmazni. Ennek a MediatR-os  `IRequest<TResult>` interfészt kell megvalósítania.

```cs
public static class FindProduct
{
    public class Query : IRequest<List<Product>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
```

Szintén a `FindProduct` osztályba kerüljön beágyazott osztályként a fenti request `Handler`-je.

Ebbe emeljük át a CatalogService megfelelő implementációját, amiből aztán törölhetjük is azt.

```cs
public static class FindProduct
{
    public class Query : IRequest<List<Product>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Hander : IRequestHandler<Query, List<Product>>
    {
        private readonly AppDbContext _dbContext;

        public Hander(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Product>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _dbContext.Products
                .Where(p => request.Name == null || p.Name.Contains(request.Name))
                .Where(p => request.Description == null || p.Description.Contains(request.Description))
                .ToListAsync();
        }
    }
}
```
Használjuk a `CatalogController`ben az új Query-nket, amit majd az `IMediator` objektumon keresztül el tudunk küldeni a megfelelő Handler számára.

Kérjük el ezt a konstruktorban a DI konténertől.

```cs
private readonly IMediator _mediator;

public CatalogController(ICatalogService catalogService, IMapper mapper, IMediator mediator)
{
    // ...
    _mediator = mediator;
}
```

A Query-t akár DTO-ként is kezelhetjük minden további nélkül a controller action-ben.

```cs
public async Task<ActionResult<List<Dto.Product>>> GetProducts([FromQuery] FindProduct.Query query)
{
    return _mapper.Map<List<Dto.Product>>(await _mediator.Send(query));
}
```

Ahhoz, hogy ez működjön fel kell konfiguráljuk a DI konténert a MediatR számára a `Startup` osztályban. Itt több lehetőségünk is van, mi most megadtunk egy olyan típust, aminek az assemblyjéből felolvassa az összes MediatR-hez kapcsolódó osztályt.

```cs
services.AddMediatR(typeof(FindProduct));
```

**Próbáljuk ki!**

## CreateProduct Command

Készítsünk most egy commandot. Vegyük fel a Catalog / Commands mappába a `CreateProduct` osztályt, amibe kerülni fog a `Command` és a `Handler` beágyazott osztály. A `Command` szintén az `IRequest<TResult>` interfészt fogja megvalósítani, ebben nincs különbség a mediátor számára.

```cs
public static class CreateProduct
{
    public class Command : IRequest<Product>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class Handler : IRequestHandler<Command, Product>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product> Handle(Command request, CancellationToken cancellationToken)
        {
            var p = new Product
            {
                Name = request.Name,
                Description = request.Description,
                UnitPrice = request.UnitPrice,
                Stock = 10,
            };

            _dbContext.Products.Add(p);

            await _dbContext.SaveChangesAsync();

            return p;
        }
    }
}
```

Emeljük át ide a megfelelő `CatalogService` implementációt, amit akár ki is törölhetünk.

Használjuk a `CatalogController`ben az új műveletünket. A Commandot itt is használhatjuk DTO-ként, így az eddigi DTO-t törölhetjük.

```cs
public async Task<ActionResult> CreateProduct([FromBody] CreateProduct.Command request)
{
    var p = await _mediator.Send(request);
    return CreatedAtAction(nameof(GetProducts), new { productId = p.ProductId }, _mapper.Map<Dto.Product>(p));
}
```

**Próbáljuk ki!**

### CreateOrder Command

Most készítsünk el egy másik commandot, ami a megrendelés létrehozását fogja elvégezni, az előzőek mintájára. Nyugodtan kiindulhatunk a DTO-kból a Command elkészítésénél. 

A `Handler` elkészítésénél most még megtartjuk azt az implementációt, hogy a készletinformációk beállítását az `ICatalogService`-en keresztül tesszük meg. Ezt a későbbiekben egy jobb megoldásra fogjuk cserélni.

Töröljük az `OrderService`-ből az átemelt részeket.

```cs
public static class CreateOrder
{
    public class Command : IRequest<Dal.Entities.Order>
    {
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }

        public List<CreateOrderItem> OrderItems { get; set; }

        public class CreateOrderItem
        {
            public int ProductId { get; set; }
            public int Amount { get; set; }
        }
    }

    public class Handler : IRequestHandler<Command, Dal.Entities.Order>
    {
        private readonly AppDbContext _dbContext;
        private readonly ICatalogService _catalogService;

        public Handler(AppDbContext dbContext, ICatalogService catalogService)
        {
            _dbContext = dbContext;
            _catalogService = catalogService;
        }

        public async Task<Dal.Entities.Order> Handle(Command request, CancellationToken cancellationToken)
        {
            var order = new Dal.Entities.Order
            {
                Name = request.CustomerName,
                Address = request.CustomerAddress,
                OrderTime = DateTimeOffset.UtcNow,
                Status = OrderStatus.Active,
                OrederItems = request.OrderItems.Select(oi => new OrderItem
                {
                    ProductId = oi.ProductId,
                    Amount = oi.Amount,
                }).ToList(),
            };

            _dbContext.Add(order);

            await _dbContext.SaveChangesAsync();

            foreach (var item in order.OrederItems)
            {
                await _catalogService.ChangeProductStockAsync(item.ProductId, -item.Amount);
            }

            return order;
        }
    }
}
```

Használjuk ezt a commandot az `OrderControllerben`.

```cs
private readonly IMediator _mediator;

public OrderController(IOrderService orderService, IMapper mapper, IMediator mediator)
{
    // ...
    _mediator = mediator;
}
```

```cs
public async Task<ActionResult> CreateProduct([FromBody] CreateOrder.Command request)
{
    var o = await _mediator.Send(request);
    return CreatedAtAction(nameof(GetOrder), new { orderId = o.OrderId }, _mapper.Map<Dto.Order>(o));
}
```

**Próbáljuk ki!**

### Notification használata domain eseményekhez

Az előző pontban is láthattuk, hogy a `CreateOrder.Handler` olyan logikát tartalmaz, ami nem feltétlenül az ő felelőssége lenne: a készletinformációk karbantartása. Helyette azt a mintát fogjuk követni, hogy elsütünk egy domain eseményt `INotification` formájában, amit az kezel le aki szeretne. Esetünkben a Catalog "module" fogja ezt megtenni egy Handler formájában, viszont ez az Order szempontjából nem kötelező, ő csak elsüti az eseményt.

Készítsünk az Order / Events mappába egy új osztályt, ami magát az eseményt fogja tartalmazni. Most az egyszerűség kedvéért tegyük bele a teljes adattartalmat, ami a Handler számára szükséges lehet, viszont itt több más stratégia is elképzelhető lenne. (pl.: csak minimális adattartalmat küldünk át, itt elég lehetne csak az ID is)

```cs
public class OrderCreatedEvent : INotification
{
    public Dal.Entities.Order Order { get; set; }
}
```

Süssük el ezt az eseményt a `CreateOrder.Handler`-ben a `foreach` helyett.

```cs
private readonly IMediator _mediator;

public Handler(AppDbContext dbContext, ICatalogService catalogService, IMediator mediator)
{
    // ...
    _mediator = mediator;
}
```

```cs
await _mediator.Publish(new OrderCreatedEvent() { Order = order });
```

Készítsünk egy osztályt a Catalog / EventHandlers mappába `OrderCreatedEventHandler` néven. Ez alapvetően `CatalogService` `ChangeProductStockAsync` metódusára épül, de már a productok kigyűjtése is a Handler felelőssége az orderből.

```cs
public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly ICatalogService _catalogService;
    private readonly AppDbContext _dbContext;

    public OrderCreatedEventHandler(ICatalogService catalogService, AppDbContext dbContext)
    {
        _catalogService = catalogService;
        _dbContext = dbContext;
    }

    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        foreach (var item in notification.Order.OrederItems)
        {
            await ChangeProductStockAsync(item.ProductId, -item.Amount);
        }
    }

    private async Task<int> ChangeProductStockAsync(int productId, int stockChange)
    {
        var p = await _catalogService.GetProductAsync(productId);
        p.Stock += stockChange;

        await _dbContext.SaveChangesAsync();

        return p.Stock;
    }
}
```

### MediatR pipeline behavior használata

Használjuk ki a MediatR legnagyobb előnyét, mégpedig, hogy a handlerek végrehajtásához könnyen kiegészíthető pipeline tartozik.

Vegyük azt a problémát, hogy az adatmódosítással járó műveleteket atomi módon kell végrehajtani, amit szeretnénk ki is kényszeríteni. Ehhez természetesen minden üzleti műveletben nyithatnánk egy tranzakciót kézzel, figyelhetnénk, hogy történt-e kivétel és commitolhatnánk/rollbackelhetnénk kézzel. Viszont ez felesleges boilerplate kód, amit egy pipeline behaviorrel könnyen ki tudunk szervezni. De hasonló lenne a request-ek validációja, naplózása esetleg hibakezelési eljárások alkalmazása is (pl.: retry).

Konkrét példánkban egészítsük ki az előző `OrderCreatedEventHandler`-t olyan logikával, hogy ha 0 alá esne a megrendelés általi stock változás, akkor hibával szálljon el a művelet. Ilyenkor természetesen a már beszúrt megrendelésnek sem szabadna letárolódnia, ezért magát az order létrehozását és az `OrderCreatedEvent` Handlerjeit atomiként fogjuk végrehajtani.

Elsőként módosítsuk a validációs logikával az `OrderCreatedEventHandler`-t.

```cs
private async Task<int> ChangeProductStockAsync(int productId, int stockChange)
{
    var p = _catalogService.GetProduct(productId);

    var newStock = p.Stock + stockChange;
    if (newStock < 0)
    {
        throw new InvalidOperationException("Not enough stock.");
    }

    p.Stock = newStock;
    await _dbContext.SaveChangesAsync();

    return p.Stock;
}
```

Csak a módosító műveletekre szeretnénk tranzakciót indítani, ezért készítsünk egy `IRequest` leszármazott interfészt, amivel ezt jelezni tudjuk. Ezt használjuk azoknál a módosító műveleteknél, ahol tranzakciót szeretnénk indítani.

```cs
public interface ICommand<TResult> : IRequest<TResult>
{
}
```

```cs
public static class CreateProduct
{
    public class Command : ICommand<Product>
    {
        // ...
    }

    // ...
```

```cs
public static class CreateOrder
{
    public class Command : ICommand<Dal.Entities.Order>
    {
        // ...
    }

    // ...
```

Maga a pipeline behavior nagyon egyszerű lesz. Készítsünk egy új osztályt a Bll / Infrastructure mappába `TransactionBehavior` néven. Ez egy generikus osztály lesz, ami bármilyen `ICommand<TResult>`-ot megvalósító requestet lekezelni kívánó handler előtt meghívódik. Technikailag ez egy `IPipelineBehavior` lesz, ahol megkapjuk az ASP.NET Core pipelinehoz hasonlóan a következő láncszemet, amit természetesen meg is kell hívjunk. Ha nem becsomagolni akarjuk ezt, akkor van lehetőség csak a handler előtt vagy után vagy exception-re lefutó műveleteket is definiálni. Nekünk most kifejezetten az `IPipelineBehavior` mechanizmusa lesz jó ehhez a problémához.

A `Handle` metódusban nyitunk egy tranzakciót, amit egy try-catch-csel vezérlünk.

```cs
public class TransactionBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : ICommand<TResult>
{
    private readonly AppDbContext _dbContext;

    public TransactionBehavior(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TResult> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResult> next)
    {
        using var tran = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var result = await next();
            await tran.CommitAsync();
            return result;
        }
        catch (System.Exception)
        {
            await tran.RollbackAsync();
            throw;
        }
    }
}
```

A `Startup` osztályban regisztráljuk be open generics-ként ezt a behaviort.

```cs
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
```

**Próbáljuk ki!** Ha többet szeretnénk rendelni egy olyan termékből, ami nincs készleten, akkor nem szabad a megrendelésnek sem létrejönnie. Illetve nem szabad tranzakciót indítania a behaviornek akkor ha nem `ICommand` a request.

## Összefoglalás

A fenti példából jól láthattuk, hogy a CQRS és a Mediátor minta milyen rugalmas és jól értetődő kódszervezést ad a kezünkbe. Ha kicsit is szeretnénk túlmutatni a klasszikus többrétegű architektúrán, akkor érdemes ezzel a megközelítéssel kezdeni a kódbázisunk továbbfejlesztését.
