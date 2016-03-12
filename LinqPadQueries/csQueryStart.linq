<Query Kind="Program">
  <Reference Relative="libraries\CSQuery\CsQuery.dll">&lt;MyDocuments&gt;\GitHub\StockApps\libraries\CSQuery\CsQuery.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Data.dll</Reference>
  <Namespace>CsQuery</Namespace>
  <Namespace>System.Data</Namespace>
  <Namespace>System.Data.Sql</Namespace>
  <Namespace>System.Data.SqlClient</Namespace>
</Query>

void Main()
{
	var sqlConnectionString = "Data Source=(local);Initial Catalog=Stocks;User ID=sa;Password=!Thunderbird486";
	CQ dom = null;
	CQ dom2 = null;
	var connection = new SqlConnection(sqlConnectionString);
	connection.Open();
	for (int y = 1; y < 17; y++)
	{
		dom = CQ.CreateFromUrl("http://www.nasdaq.com/screening/companies-by-industry.aspx?industry=ALL&exchange=NASDAQ&pagesize=200&page=" + y);
		var trs = dom["#CompanylistResults tbody tr"];

		for (var x = 0; x < 400 && x < trs.Count(); x += 2)
			{
			dom2 = trs[x].InnerHTML;
			trs[x].InnerHTML.Dump(1);
			var Name = dom2["td:first"].Text().Trim().Replace("'","");
			Name = !String.IsNullOrEmpty(Name) ? Name : "Missing Name";
			var Symbol = String.Empty;
			if (dom2["a"].Count() == 1)
			{
				Symbol = dom2["a"][0].InnerHTML.Trim();
			}
			else
			{
			    Symbol = dom2["a"][1].InnerHTML.Trim();
			}
			Symbol = !String.IsNullOrEmpty(Symbol)?Symbol:"Missing Symbol";
			var Url = dom2["a:first"].Attr("href").Trim();
			Url = !String.IsNullOrEmpty(Url)?Url:"NULL";
			var Sector = dom2["td"][6].InnerHTML.Trim();
			Sector = !String.IsNullOrEmpty(Sector) && Sector != "n/a"?Sector:"NULL";
			var IPOYear = dom2["td"][5].InnerHTML.Trim();
			IPOYear = !String.IsNullOrEmpty(IPOYear) && IPOYear != "n/a"?IPOYear:"NULL";
		var insertString = String.Format(@"INSERT INTO [dbo].[Company]
           ([MarketId]
           ,[Name]
           ,[Symbol]
           ,[Url]
           ,[SubSector]
           ,[IPOYear])
     VALUES
           (1
           ,'{0}'
           ,'{1}'
           ,'{2}'
           ,'{3}'
           ,{4})",Name,Symbol,Url,Sector,IPOYear);
		var command = new SqlCommand(insertString, connection);
		Console.WriteLine(String.Format("Executing: {0}",insertString));
		command.ExecuteNonQuery();
		
			}

		//URL	dom2["a:first"].Attr("href").Dump(1);
		//NAME	dom2["a:first"].Html().Dump(1);
		//Symbol	dom2["a"][1].InnerHTML.Dump(1);
		//	dom2.Render().Dump(1);
		//SubSector	dom2["td"][6].InnerHTML.Dump(1);
		//IPOYEAR dom2["td"][5].InnerHTML.Dump(1);
	}
	connection.Close();
}
