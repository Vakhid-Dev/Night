using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Integration.Mvc;
using System.Web.Mvc;


namespace Sample
{

    namespace DI
    {
        namespace DI_Abstraction
        {
            public class CustomerBusinessLogic
            {
                ICustomerDataAccess _dataAccess;

                public CustomerBusinessLogic(ICustomerDataAccess custDataAccess)
                {
                    _dataAccess = custDataAccess;
                }

                public CustomerBusinessLogic()
                {
                    _dataAccess = new CustomerDataAccess();
                }

                public string ProcessCustomerData(int id)
                {
                    return _dataAccess.GetCustomerName(id);
                }
            }

            public interface ICustomerDataAccess
            {
                string GetCustomerName(int id);
            }

            public class CustomerDataAccess : ICustomerDataAccess
            {
                public CustomerDataAccess()
                {
                }

                public string GetCustomerName(int id)
                {
                    //get the customer name from the db in real application        
                    return "Dummy Customer Name";
                }
            }

            public class CustomerService
            {
                CustomerBusinessLogic _customerBL;

                public CustomerService()
                {
                    _customerBL = new CustomerBusinessLogic(new CustomerDataAccess());
                }

                public string GetCustomerName(int id)
                {
                    return _customerBL.ProcessCustomerData(id);
                }
            }
        }

        namespace DI.Autofac
        {
            namespace DI.Autofac_EmployeeSample
            {
                void Run()
                {
                    var container = ContainerConfig.Configure();
                    var employeeSerive = container.Resolve<IEmployeeService>();
                    employeeSerive.PrintEmployee(1);
                    employeeSerive.PrintEmployee(2);
                    employeeSerive.PrintEmployee(3);

                }
                public class ContainerConfig
                {
                    public static IContainer Configure()
                    {
                        var builder = new ContainerBuilder();
                        builder.RegisterType<EmployeeRepository>().As<IEmployeeRepository>();
                        builder.RegisterType<EmployeeService>().As<IEmployeeService>();
                        return builder.Build();
                    }

                }
                public class EmployeeService : IEmployeeService
                {

                    private readonly IEmployeeRepository _repository;
                    public EmployeeService(IEmployeeRepository repository)
                    {
                        _repository = repository;
                    }
                    public void PrintEmployee(int id)
                    {
                        var employee = _repository.FindEmployee(id);
                        Console.WriteLine(employee != null ? $"Id:{employee.Id}, Name:{employee.Name}"
                            : $"Employee with Id:{id} not found.");
                    }

                }
                public interface IEmployeeService
                {
                    void PrintEmployee(int id);
                }
                public class EmployeeRepository : IEmployeeRepository
                {
                    private readonly List<Employee> _data = new List<Employee>()
            {
                new Employee { Id = 1, Name = "Ivan"},
                new Employee { Id = 2, Name = "Max"},
            };

                    public Employee FindEmployee(int id)
                    {
                        return _data.FirstOrDefault(x => x.Id == id);
                    }
                }
                public interface IEmployeeRepository
                {
                    Employee FindEmployee(int id);
                }

                public class Employee
                {
                    public int Id { get; set; }
                    public string Name { get; set; }
                }

            }

            namespace Di.Autofac_NotificationSample
            {
                public void Run()
                {
                    var builder = new ContainerBuilder();
                    builder.RegisterType<SMSService>().As<IMobileServive>();
                    builder.RegisterType<EmailService>().As<IMailService>();
                    var container = builder.Build();

                    // container.Resolve<IMobileServive>().Execute();
                    // container.Resolve<IMailService>().Execute();
                    var not = new NotificationSender(new SMSService());
                    not.SetMailService = new EmailService();
                    not.SendNotification();
                }

                public interface IMobileServive
                {
                    void Execute();
                }
                public class SMSService : IMobileServive
                {
                    public void Execute()
                    {
                        Console.WriteLine("SMS service executing.");
                    }
                }

                public interface IMailService
                {
                    void Execute();
                }

                public class EmailService : IMailService
                {
                    public void Execute()
                    {
                        Console.WriteLine("Email service Executing.");
                    }
                }

                public class NotificationSender
                {
                    public IMobileServive ObjMobileSerivce = null;
                    public IMailService ObjMailService = null;

                    //injection through constructor  
                    public NotificationSender(IMobileServive tmpService)
                    {
                        ObjMobileSerivce = tmpService;
                    }
                    //Injection through property  
                    public IMailService SetMailService
                    {
                        set { ObjMailService = value; }
                    }
                    public void SendNotification()
                    {

                        ObjMobileSerivce.Execute();
                        ObjMailService.Execute();
                    }
                }
            }

            namespace DI.Autofac.Mvc_BookSample
            {
                public class Book
                {
                    public int Id { get; set; }
                    public string Name { get; set; }
                }

                public class BookContext : DbContext
                {
                    //  public BookContext() : base("DefaultConnection") { }

                    public DbSet<Book> Books { get; set; }
                }

                public interface IRepository
                {
                    // определение методов
                    List<object> List();
                }

                public class BookRepository : IDisposable, IRepository
                {
                    private BookContext db = new BookContext();
                    protected void Dispose(bool disposing)
                    {
                        if (!disposing)
                        {
                            if (db != null)
                            {
                                db.Dispose();
                                db = null;
                            }

                        }
                    }
                    public void Dispose()
                    {
                        Dispose(true);
                        GC.SuppressFinalize(this);
                    }

                    public List<object> List()
                    {
                        throw new NotImplementedException();
                    }
                }

                public class HomeController : Controller
                {
                    IRepository repo;
                    public HomeController(IRepository r)
                    {
                        repo = r;
                    }
                    // or maybe ActionResult method
                    public List<object> Index()
                    {
                        return repo.List();
                    }
                }

                public class AutofacConfig
                {
                    public static void ConfigureContainer()
                    {
                        // получаем экземпляр контейнера
                        var builder = new ContainerBuilder();

                        // регистрируем контроллер в текущей сборке

                        // builder.RegisterControllers(typeof(App).Assembly);

                        // регистрируем споставление типов
                        builder.RegisterType<BookRepository>().As<IRepository>();

                        // создаем новый контейнер с теми зависимостями, которые определены выше
                        var container = builder.Build();

                        // установка сопоставителя зависимостей
                        DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
                    }
                }
                /* global asax mvc 5
                    protected void Application_Start()
                    {
                    AutofacConfig.ConfigureContainer();
                    }
                    */

            }
        }

    }


}


