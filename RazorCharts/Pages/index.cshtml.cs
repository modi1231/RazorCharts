using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorCharts.Data;

/// <summary>
/// 2019.05.13 - Razor tutorial on CHART.JS.
/// DreamInCode.NET
/// </summary>
namespace RazorCharts.Pages
{
    public class indexModel : PageModel
    {
        [TempData]
        public string Message { get; set; }// no private set b/c we need data back

        private readonly string _connection;

        public async Task OnGetAsync()
        {
            //await Task.Run(() =>
            //{
            //  //  Message = "Test Message";
            //});
        }
        public async Task<ActionResult> OnGetMyAjax_SimpleAsync()
        {
            DataAccess data = new DataAccess(_connection);
            List<MyData_Simple> temp = await data.LoadData_SimpleAsync();

            string[] labels = null;
            string[] COUNT = null;


            labels = (from x in temp
                      select x.DAY).ToArray();

            COUNT = (from x in temp
                     select x.COUNT.ToString()).ToArray();

            return new JsonResult(new
            {
                mylabels = labels,
                myCount = COUNT
            });
        }

        public async Task<ActionResult> OnGetMyAjax_ComplexAsync()
        {
            DataAccess data = new DataAccess(_connection);
            List<MyData_Complex> temp = await data.LoadData_ComplexAsync();

            string[] labels = null;
            string[] COUNT_CALLS = null;
            string[] COUNT_EMAILS = null;
            string[] COUNT_TWEETS = null;


            labels = (from x in temp
                      select x.DAY).ToArray();

            COUNT_CALLS = (from x in temp
                           select x.COUNT_CALLS.ToString()).ToArray();

            COUNT_EMAILS = (from x in temp
                            select x.COUNT_EMAILS.ToString()).ToArray();

            COUNT_TWEETS = (from x in temp
                            select x.COUNT_TWEETS.ToString()).ToArray();

            return new JsonResult(new
            {
                mylabels = labels,
                myCOUNT_CALLS = COUNT_CALLS,
                myCOUNT_EMAILS = COUNT_EMAILS,
                myCOUNT_TWEETS = COUNT_TWEETS
            });
        }

        public async Task<ActionResult> OnGetMyAjax_DonutAsync()
        {
            DataAccess data = new DataAccess(_connection);
            List<MyData_Donut> temp = await data.LoadData_DonutAsync();

            string[] labels = null;
            string[] COUNT = null;


            labels = (from x in temp
                      select x.STOCK_NAME).ToArray();

            COUNT = (from x in temp
                     select x.COUNT.ToString()).ToArray();

            return new JsonResult(new
            {
                mylabels = labels,
                myCount = COUNT
            });
        }
    }
}