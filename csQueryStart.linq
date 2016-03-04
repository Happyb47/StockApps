<Query Kind="Program">
  <Reference Relative="libraries\CSQuery\CsQuery.dll">&lt;MyDocuments&gt;\GitHub\StockApps\libraries\CSQuery\CsQuery.dll</Reference>
  <Namespace>CsQuery</Namespace>
</Query>

void Main()
{
	CQ dom2 = CQ.CreateFromUrl("http://www.reuters.com/article/us-usa-ojsimpson-knife-idUSKCN0W61ZW");
	dom2["#articleText"].Text().Dump(1);
}
