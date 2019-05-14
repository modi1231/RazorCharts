using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorCharts.Data
{
    public class DataAccess
    {
        private string _connection { get; }
        public DataAccess(string connection)
        {
            _connection = connection;
        }

        internal async Task<List<MyData_Simple>> LoadData_SimpleAsync()
        {
            List<MyData_Simple> temp = new List<MyData_Simple>();

            Random r = new Random();
            DateTime now = DateTime.Today;

            await Task.Run(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    temp.Add(new MyData_Simple()
                    {
                        DAY = $"{now.Month}/{now.Day}",
                        COUNT = r.Next(20)
                    });

                    now = now.AddDays(-1);
                }

            });

            return temp;
        }

        internal async Task<List<MyData_Complex>> LoadData_ComplexAsync()
        {
            List<MyData_Complex> temp = new List<MyData_Complex>();

            Random r = new Random();
            DateTime now = DateTime.Today;

            await Task.Run(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    temp.Add(new MyData_Complex()
                    {
                        DAY = $"{now.Month}/{now.Day}",
                        COUNT_CALLS = r.Next(10),
                         COUNT_EMAILS = r.Next(10),
                         COUNT_TWEETS= r.Next(10)
                    });

                    now = now.AddDays(-1);
                }
                
            });

            return temp;
        }

        internal async Task<List<MyData_Donut>> LoadData_DonutAsync()
        {
            List<MyData_Donut> temp = new List<MyData_Donut>();

            Random r = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            await Task.Run(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    temp.Add(new MyData_Donut()
                    {
                        STOCK_NAME = $"{chars[r.Next(chars.Length)]}{chars[r.Next(chars.Length)]}{chars[r.Next(chars.Length)]}",
                        COUNT = r.Next(5)+1
                    });
                }

            });

            return temp;
        }
    }
}