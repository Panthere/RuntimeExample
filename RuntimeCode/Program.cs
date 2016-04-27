using System.Windows.Forms;

namespace RuntimeCode
{
    class Program
    {
        static void Main(string[] args)
        {
            // Do something
            // This is set to a windows form application, so console will not show.
            MessageBox.Show(args[0], args[1]);
        }
    }
}
