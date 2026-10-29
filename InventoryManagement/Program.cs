using InventoryManagement.Forms;

namespace InventoryManagement
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new ProductForm());
        }
    }
}