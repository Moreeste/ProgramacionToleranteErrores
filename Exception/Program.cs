bool isOver = false;

do
{
    try
    {
        Console.WriteLine("Escribe un número: ");
        int a = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Escribe otro número: ");
        int b = Convert.ToInt32(Console.ReadLine());

        int result = a / b;
        Console.WriteLine("El resultado de la división es: " + result);

        isOver = true;
    }
    catch (FormatException ex)
    {
        Console.WriteLine("Debes escribir números");
    }
    catch (DivideByZeroException ex)
    {
        Console.WriteLine("No puedes dividir entre cero");
    }
    catch (OverflowException ex)
    {
        Console.WriteLine("Escribe números pequeños");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Ocurrió un error: " + ex.Message);
    }
    finally
    {
        Console.WriteLine("....................");
    }
}
while (!isOver);

Console.WriteLine("Programa finalizado");