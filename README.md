# RazorCharts
Tutorial illustrating using Chart.JS and Razor pages with AJAX.

https://www.dreamincode.net/forums/topic/416106-razor-pages-core-21-chartjs-and-ajax/


=================
dreamincode.net tutorial backup ahead of decommissioning


 Post icon  Posted 13 May 2019 - 10:03 PM 
 
 You may come across a day where you need a light weight, but super handy, chart to really make your data pop.  numbers are good, but visuals really help drive it home.  Then there's the headache of having chart labels dynamically show up (say if your charts show the last five days of data) let alone data that keeps shifting.

These issues can be cleared up with exercising some interesting use of Chart.JS and AJAX in your Razor pages!

[img]https://i.imgur.com/B9jxCJB.png[/img]


[u]Software:[/u]
-- Visual Studios 2019

[u]Concepts:[/u]
-- C#
-- Razor Pages
-- [url="https://www.chartjs.org/"]Chart.js[/url] 
-- AJAX

[u]Github link:[/u]   https://github.com/modi1231/RazorCharts/

[u]AJAX[/u]
A little background on AJAX from the link, but it is a javascript way to ask for data from the server.  AJAX is super cool functionality where you can read/write data after the page POST or GET without having to do a full trip.  Say you have a drop down and want to fill textboxes from the database.. ajax can do the heavy lifting and not need a full page post!

https://developer.mozilla.org/en-US/docs/Web/Guide/AJAX
https://www.w3schools.com/js/js_ajax_intro.asp


The general flow is going to be this.
- the page loads
- the JS activates 
- the AJAX makes the call for a specific function on the server.
- the Razor routes it to the c# function.
- this function fills a collection from a data source.
- the function peels apart the "label" and "data" and passes the string arrays back to the AJAX call.
- the JS then fills in the chart.JS label and data
- everything renders right.

AJAX -> C# function -> Data Load -> into string arrays -> back to AJAX -> chart.js

Easy peasy, lemon squeezy, right?



[b]!! Note [/b]- the JS sections are broken up, and near their respective HTML bits.  Usually I would have these in a separate file but for this tutorial I found it easier to see what was going on when near each other.




Check out my previous tutorial on setting up Razor projects "Razor Pages, Core 2.1 - GULP, NPM, and basic project workflow " to get a base line.
https://www.dreamincode.net/forums/showtopic414490.htm

1.  With the base project, folders, etc setup our first step is heading to 'package.json' and add in a dependency to chart.js.

[code]"chart.js": "2.8.0" [/code]

2.  Next head to gulpfile.js to add the gulp movement of the chart.js and chart.css files to the respective folders in the wwwroot.

 [code]   gulp.src(["node_modules/chart.js/dist/chart.js"])
        .pipe(gulp.dest("wwwroot/js")) [/code]

and 

 [code]   gulp.src(["node_modules/chart.js/dist/Chart.css"])
        .pipe(gulp.dest("wwwroot/css")); [/code]

3.  It's a good time to rebuild.

4.  To make sure these are applied to every Razor page, go to _Layout.cshtml and add references into the 'head' there.  I simply drag the files from their wwwroot files to the head and it does the routing and all that for me. 

   [code] <script src="~/js/chart.js"></script>
    <link href="~/css/Chart.css" rel="stylesheet" /> [/code]




With the perfunctory setup done I am going to walk through the 'Simple' way first.

5.  In the 'Data' folder I create a class called 'MyData_Simple' with two properties: a string and an integer.
[code]
    public class MyData_Simple
    {
        public string DAY { get; set; }
        public int COUNT { get; set; }
    } [/code]

This is going to represent a count of widgets for the last five days.

6.  In my 'DataAccess' class I make a function that will return a collection of this simple dataset.

Mind you if this was hooked to a database you would connect here and query your data to be returned in the collection, but this is just a stubbed out example.

I have a random instance getting values, and a for loop to count back from 'today' to five days back.

I try to keep _all_ my functions and methods async going forward.  It helps with response times and doesn't mix weird burdens later on.
[code]
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
        } [/code]

7.  In the 'index' code behind I setup the public 'OnGet' for the AJAX to call.

Due to the nature of the Chart.JS I know I will need two arrays to return from the AJAX call - a list of labels (in this case days) and a list of data.  ChartJS knows to match up the data to label by index.

In this case I use linq to quickly get both arrays and return them as a JSONResult array.
[code]
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
        } [/code]

8.  The web side gets a little more complicated, but starts simply by getting the ChartJS object on to the canvas.

[code]<div style="width: 55%;
        margin-left: auto;
        margin-right: auto;">
    <canvas id="myChart_simple" width="500" height="400"></canvas>
</div> [/code]

9.  The Javascript really digs into the meat of the process.

First is a function to encapsulate the AJAX call.

The AJAX call type needs to be set.. a GET or POST depending.  

The URL (for Razor routing) is the function name from step 7.  

To make things easier - we tell it to turn off ASYNC here, and outline a success and error function.

Success pushes the response into the return value, and the error dumps information to the F12 developer tools console.

 [code]  function GetJSON_Simple() {
        var resp = [];
        $.ajax({
            type: "GET",
            url: '/?handler=MyAjax_Simple',
            async: false,
            contentType: "application/json",
            success: function (data) {
                resp.push(data);
            },
            error: function (req, status, error) {
                // do something with error
                console.log("error");
                console.log(req);
                console.log(status);
                console.log(error);
                alert("error");
            }
        });

        return resp;
    } [/code]


10.  Outside of the function I catch the AJAX return.
 [code]   var simpleData = GetJSON_Simple(); [/code]


11.  I get a handle to the ChartJS control.
[code]var ctx_simple = document.getElementById('myChart_simple'); [/code]

12.  I start setting up the ChartJS options.

First it tell it which type of chart I want.
Next I tell it about the data to display.  Chiefly being the labels (days in this case), color of the bar chart, and the data for the dataset is the other array.

The options are not necessary, but look nice.

 [code]   var myChart_simple = new Chart(ctx_simple, {
        type: 'bar',
        data: {
            labels: simpleData[0].mylabels,
            datasets: [{
                label: 'COUNT',
                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                borderColor: 'rgb(255, 99, 132)',
                borderWidth: 1,
                data: simpleData[0].myCount
            }
            ]
        },
        options: {
            scales: {
                xAxes: [{
                    stacked: true
                }],
                yAxes: [{
                    stacked: false
                }]
            },

        }
    }); [/code]

If everything goes well you should see this (or some variant since each time the page is refreshed the data is randomized.

[img]https://i.imgur.com/uao7kUz.png[/img]



Not bad.. Remember the AJAX happens as the page is rendering, and certainly I could have looked at wedging C# in there but it gets a little messy and weird.



Using that logic, and reading up on Chart.JS's functionality, we can push this a little more to make a stacked bar graph!  Say I wanted to see how many calls, emails, and tweets I logged in a day.  That requires a slightly more complex data object but the rest falls in line with what we know.


13.  In the 'Data' folder I create a class called 'MyData_Complex'.  This will have the days, but tracking three things per day!

[code]    public class MyData_Complex
    {
         //complex
        public string DAY { get; set; }
        public int COUNT_EMAILS { get; set; }
        public int COUNT_CALLS { get; set; }
        public int COUNT_TWEETS { get; set; }
    }
 [/code]
14.  In the DataAccess I create a function that returns a randomized collection of these new numbers.  Again - this is where you would have your call to a database, but I am stubbing out the data.  Bless be abstraction.

[code]        internal async Task<List<MyData_Complex>> LoadData_ComplexAsync()
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
        } [/code]

15.  The index code behind looks super duper similar to the simple one.. this time just creating more string arrays of the data.  Again, Chart.JS knows that data at specific indexes - across arrays - belong together.. be it labels and data or more data and data.


[code]        public async Task<ActionResult> OnGetMyAjax_ComplexAsync()
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
        } [/code]

16.  In the HTML things are setup like before.  New div, new chart object.
[code]<div style="width: 55%;
        margin-left: auto;
        margin-right: auto;">
    <canvas id="myChart" width="500" height="400"></canvas>
</div> [/code]


17.  The AJAX is tweaked only to point to the complex function.
[code]    function GetJSON_Complex() {
        var resp = [];
        $.ajax({
            type: "GET",
            url: '/?handler=MyAjax_Complex',
            async: false,
            contentType: "application/json",
            success: function (data) {
                resp.push(data);
            },
            error: function (req, status, error) {
                // do something with error
                console.log("error");
                console.log(req);
                console.log(status);
                console.log(error);
                alert("error");
            }
        });

        return resp;
    } [/code]

18.  The Chart.JS looks a little more complex as I specifically enumerate the different groups with different colors and legend information.

[code]    var theData = GetJSON_Complex();

    var ctx = document.getElementById('myChart');
    var myChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: theData[0].mylabels,
            datasets: [{
                label: 'CALLS',
                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                borderColor: 'rgb(255, 99, 132)',

                borderWidth: 1,
                data: theData[0].myCOUNT_CALLS
            },
            {
                label: 'EMAILS',
                backgroundColor: 'rgba(54, 162, 235, 0.2)',
                borderColor: 'rgb(54, 162, 235)',
                borderWidth: 1,
                data: theData[0].myCOUNT_EMAILS
            }
                ,
            {
                label: 'TWEETS',
                backgroundColor: 'rgba(255, 206, 86, 0.2)',
                borderColor: 'rgb(255, 206, 86)',
                borderWidth: 1,
                data: theData[0].myCOUNT_TWEETS
            }

            ]
        },
        options: {
            scales: {
                xAxes: [{
                    stacked: true
                }],
                yAxes: [{
                    stacked: true
                }]
            },

        }
    }); [/code]


19.  I have a little plugin bit that is super Chart.JS and not really needed, but looks neat.  It was created from reading their plugin and documentation system and interacts with the data in the chart rendering.  Basically it goes through and adds the actual amount right below the top line of each section if the value is greater than 0.

Assuming it all looks well - you should see something like this.

[img]https://i.imgur.com/HB0gWWy.png[/img]


20.  Finally - I branch out of the charts and make a 'donut'  to track "my stocks".  Similar setup for 'Data' folder, data returned, and splitting up the information.  

The only major difference is I set up the 'data' and 'options' into their own objects to make the chart rendering a little cleaner.  Basically pulling them out and assigning to the bottom.


[code]
    var data = {
        datasets: [{
            data: theDataDonut[0].myCount,
            backgroundColor: ['rgb(255, 99, 132)','rgb(54, 162, 235)','rgb(255, 206, 86)'
            ]
        }],
        // These labels appear in the legend and in the tooltips when hovering different arcs
        labels: theDataDonut[0].mylabels
    };

    var options = {
        responsive: true,
        legend: {
            position: 'top',
        },
        title: {
            display: true,
            text: 'My Stocks'
        },
        animation: {
            animateScale: true,
            animateRotate: true
        }
    };

    var ctx_Donut = document.getElementById('myDonutChart');
    var myChart_Donut = new Chart(ctx_Donut, {
        type: 'doughnut',
        data: data,
        options: options
    });
 [/code]    

[img]https://i.imgur.com/LcIqSXc.png[/img]


There are a large number of charts and types, but this gets the basic foundation out of the way.  The joy is if the page is loaded another day the data shifts!  Dynamic is awesome.

Hopefully this helps a few of you out there to add neat charts to your Razor sites.
