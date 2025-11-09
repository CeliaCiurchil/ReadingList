using ReadingList.Cli.Menus;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Welcome to the Reading List Application!");
        await Menu.MenuLoop();
    }
}