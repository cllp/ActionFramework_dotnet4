Build the Action class.
<pre class="language-csharp">
<code>
public class OnDemandDemo : ActionBase
{
    //GET (no parameters)
        public override object Execute()
        {
            try
            {
                XDocument result = XDocument.Parse("<xml>content</xml>");

                Log.Info("Parsed filecontent successfully");

                return result;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        //POST (object input parameter)
        public override object Execute(object input)
        {
            try
            {
                Log.Info("Object Input: " + input.ToString());

                return input;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
}
</code>
</pre>

Call the agent API endpoints (GET/POST).
<pre class="language-csharp">
<code>
string uri = "http://localhost:1011/";

//GET:
var getclient = new RestClient(uri);
var getrequest = new RestRequest("api/agent/runaction/ondemanddemo", Method.GET);
var getresponse = getclient.Execute(getrequest);
Console.WriteLine(getresponse.Content);
Console.ReadLine();

//POST:
var postclient = new RestClient(uri);
var postrequest = new RestRequest("api/agent/runaction?name=ondemanddemo", Method.POST);
postrequest.RequestFormat = DataFormat.Json;
postrequest.AddBody(new TestModel()
{
    Id = 1,
    Name = "Test"
});

var postresponse = postclient.Execute(postrequest);
Console.WriteLine(postresponse.Content);
Console.ReadLine();
</code>
</pre>
