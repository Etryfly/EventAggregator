using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using EventAggregator.Events;

namespace EventAggregator
{
    class Program
    {
        static string connectionString = "Server= localhost; Database= localdb; Integrated Security=True;";

        private static IEventAggregator eventAggregator;
        
        static async Task Main(string[] args)
        {
            eventAggregator =  new EventAggregator();
            var subscriber1 = new ChangesPrinter("1");
            var subscriber2 = new ChangesPrinter("2");
            eventAggregator.SubsribeEvent(subscriber1);
            eventAggregator.SubsribeEvent(subscriber2);
            
            SqlDependency.Start(connectionString);

            var dt = getDataWithSqlDependency();

            Console.WriteLine("Waiting for data changes");
            Console.WriteLine("Press enter to quit");
            Console.ReadLine();

            SqlDependency.Stop(connectionString);
        }

        static DataTable getDataWithSqlDependency()
        {
            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT SomeColumn, ID FROM dbo.SomeTable;", connection))
            {
                var dt = new DataTable();

                var dependency = new SqlDependency(cmd);
                dependency.OnChange += new OnChangeEventHandler(onDependencyChange);

                connection.Open();
                dt.Load(cmd.ExecuteReader(CommandBehavior.CloseConnection));

                return dt;
            }
        }

        static void onDependencyChange(object sender,
            SqlNotificationEventArgs e)
        {
            if ((e.Info != SqlNotificationInfo.Invalid)
                && (e.Type != SqlNotificationType.Subscribe))
            {
                var dt = getDataWithSqlDependency();
                eventAggregator.PublishEvent(new TableChanged()
                {
                    Change = $"{e.Info.ToString()} (Total: {dt.Rows.Count} rows)" 
                });
            }
        }
    }

}