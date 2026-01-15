Console.WriteLine("Escribe tu edad: ");
string sAge = Console.ReadLine();

// Validación sintactica -> formatos
if (!int.TryParse(sAge, out int age))
{
    Console.WriteLine($"[{sAge}] no tiene un formato de edad valido");
    return;
}

// Validación semántica -> Coherencia
if (age < 0 || age > 125)
{
    Console.WriteLine($"[{sAge}] la edad está fuera de un rango real");
    return;
}
if (age < 18)
{
    Console.WriteLine("Debes ser mayor de edad");
    return;
}

Console.WriteLine($"Tu edad {sAge} es correcta, continuar...");