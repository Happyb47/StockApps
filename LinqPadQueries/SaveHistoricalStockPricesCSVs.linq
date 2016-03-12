<Query Kind="Program">
  <Reference Relative="..\libraries\CSQuery\CsQuery.dll">&lt;MyDocuments&gt;\GitHub\StockApps\libraries\CSQuery\CsQuery.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Data.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.IO.Compression.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Namespace>CsQuery</Namespace>
  <Namespace>System.Data</Namespace>
  <Namespace>System.Data.Sql</Namespace>
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.IO.Compression</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Web</Namespace>
</Query>

void Main()
{
	var sqlConnectionString = "Data Source=(local);Initial Catalog=Stocks;User ID=sa;Password=!Thunderbird486";
	var connection = new SqlConnection(sqlConnectionString);
	connection.Open();
	var command = new SqlCommand("select * from company", connection);
	var reader = command.ExecuteReader();
	while (reader.Read())
	{
		reader.GetString(3).Dump(1);
		MakeRequests(reader.GetString(3));
	}
	connection.Close();
	
}

// Define other methods and classes here
private void MakeRequests(string symbol)
{
	HttpWebResponse response;
	string responseText;
	try
	{
		if (Request_www_reuters_com(symbol, out response))
		{
			ReadResponse(symbol, response);

			response.Close();
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine("Error for " + symbol);
	}
}

private void ReadResponse(string symbol, HttpWebResponse response)
{
	using (Stream responseStream = response.GetResponseStream())
	{
		Stream streamToRead = responseStream;
		if (response.ContentEncoding.ToLower().Contains("gzip"))
		{
			streamToRead = new GZipStream(streamToRead, CompressionMode.Decompress);
		}
		else if (response.ContentEncoding.ToLower().Contains("deflate"))
		{
			streamToRead = new DeflateStream(streamToRead, CompressionMode.Decompress);
		}

		using (StreamReader streamReader = new StreamReader(streamToRead, Encoding.UTF8))
		{
			using (var fileStream = File.Create(@"C:\Users\jcassell\Documents\GitHub\StockApps\CSVFiles\" + symbol + ".csv"))
			{
				//streamToRead.Seek(0, SeekOrigin.Begin);
				streamToRead.CopyTo(fileStream);
			}
		}
	}
}

private bool Request_www_reuters_com(string symbol, out HttpWebResponse response)
{
	response = null;

	try
	{
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://real-chart.finance.yahoo.com/table.csv?s=" + symbol + "&ignore=.csv");

		request.KeepAlive = true;
		request.Headers.Set(HttpRequestHeader.CacheControl, "max-age=0");
		request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
		request.Headers.Add("Upgrade-Insecure-Requests", @"1");
		request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.116 Safari/537.36";
		request.Referer = "http://www.reuters.com/";
		request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, sdch");
		request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
		//request.Headers.Set(HttpRequestHeader.Cookie, @"adDisplayManager=session; tns=dataSource=cookie; info=edition=US; __gads=ID=0574bcd0aeb5e1c8:T=1457129642:S=ALNI_MYtvVB4Ho0hfYZzcCWLwHzFbM6Fhw; OX_plg=swf|shk|pm; _cb_ls=1; pm[sess_r]=http%3A//www.reuters.com/; _gat__pm_ga=1; rsi_segs=D08734_70252|D08734_70028|I07714_10383|I07714_10173|I07714_10267|I07714_10272|D08734_70092|I07714_10445|D08734_71960|I07714_10382|I07714_10533|I07714_10540|I07714_10583|I07714_10587|I07714_10588|I07714_10589|I07714_0; WT_FPC=id=b27adea3-48db-4d99-b683-144010bc9473:lv=1457133690220:ss=1457151128079; _ga=GA1.2.934907578.1457129526; pm[t][ppg]=http%3A//www.reuters.com/article/us-usa-ojsimpson-knife-idUSKCN0W61ZW; _parsely_session={%22sid%22:1%2C%22surl%22:%22http://www.reuters.com/%22%2C%22sref%22:%22%22%2C%22sts%22:1457129527085%2C%22slts%22:0}; _parsely_visitor={%22id%22:%2268351896-37ea-43fc-be4a-d1e3fe794676%22%2C%22session_count%22:1%2C%22last_session_ts%22:1457129527085}; _chartbeat2=p3Ou6KuvGtCylhWj.1457129532510.1457130094745.1; _sp_id.23dd=c06e2407c0d1174f.1457129530.1.1457130152.1457129530; _sp_ses.23dd=*");

		response = (HttpWebResponse)request.GetResponse();
	}
	catch (WebException e)
	{
		if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
		else return false;
	}
	catch (Exception)
	{
		if (response != null) response.Close();
		return false;
	}

	return true;
}