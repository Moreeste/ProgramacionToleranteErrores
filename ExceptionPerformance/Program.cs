using System.Diagnostics;

int n = 1000;
string[] data = { "123", "XYZ" };

var sw = new Stopwatch();
sw.Start();
int successNumbers = 0;

for (int i = 0; i < n; i++)
{
    var val = data[i % 2];

	//try
	//{
	//	int.Parse(val);
	//	successNumbers++;
	//}
	//catch (FormatException)
	//{

	//}

	if (int.TryParse(val, out int number))
	{
		successNumbers++;
    }
}

sw.Stop();
Console.WriteLine($"Tiempo: {sw.ElapsedMilliseconds} ms, éxitos {successNumbers}");