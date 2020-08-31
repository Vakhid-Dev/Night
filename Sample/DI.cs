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
        // Constructor Injection 
       
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
