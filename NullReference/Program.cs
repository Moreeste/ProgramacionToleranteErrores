//Person person = null;
Person person = new Person("Juan", new Address("CDMX"));

//Console.WriteLine($"Hola {person.Name}");
//Null conditional
Console.WriteLine($"Hola {person?.Name}");

//Null coalescing
int length = person?.Name.Length ?? 0;
Console.WriteLine($"El nombre tiene {length} letras");

Console.WriteLine($"Hola {person?.Name} que es de la ciudad {person?.Address?.City}");

record Address(string City);
record Person(string Name, Address Address);