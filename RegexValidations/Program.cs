using System.Text.RegularExpressions;

var patterName = @"^[a-zA-ZÀ-ÿ\s]{1,50}$";

var regexName = new Regex(patterName);

var name = "Héctor de León";

if (regexName.IsMatch(name))
{
    Console.WriteLine($"{name} es un nombre válido");
}
else
{
    Console.WriteLine($"{name} no es un nombre válido");
}