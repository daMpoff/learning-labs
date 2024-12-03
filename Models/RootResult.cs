using System.Collections.ObjectModel;

namespace SLAE.Models
{
    public class RootResult
    {
        public string MethodName { get; set; } // Название метода
        public ObservableCollection<double> Roots { get; set; } // Список корней
    }
}
