Create a class that inherits from **ILogElement**

<pre class="language-csharp">
<code>
using ActionFramework.Logging;
using System;

namespace DemoApp
{
    internal sealed class CustomLog : ILogElement
    {
        private int amount = 0;
        private DateTime date = DateTime.Now;

        public int Amount
        {
            get { return amount; }
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public string Message { get; set; }

        /// <summary>
        /// increase count by 1
        /// </summary>
        public void Count()
        {
            amount++;
        }

        /// <summary>
        /// increase count by n
        /// </summary>
        public void Count(int n)
        {
            amount += n;
        }
    }
}
</code>
</pre>

Use the custom logging class in an action class.

<pre class="language-csharp">
<code>
//create the custom log and add values
var log = new CustomLog()
{
    Date = DateTime.Now,
    Message = "This is the custom message"
};

//add a count
log.Count(25);

//create the log
Log.Custom(log);
</code>
</pre>

Log output (XML).

<pre class="language-markup">
<code>
<CustomLog>
    <Amount>25</Amount>
    <Date>2015-12-02T13:58:28.9374725Z</Date>
    <Message>This is the custom message</Message>
</CustomLog>
</code>
</pre>
