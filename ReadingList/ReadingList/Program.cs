using ReadingList.Cli.Menus;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Reading List Application!");
        BookMenu.MenuLoop();
    }
}