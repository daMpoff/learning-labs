using System.Collections.ObjectModel;

namespace SLAE.Models
{
    public class RootResult
    {
        public string MethodName { get; set; }
        public ObservableCollection<double> Roots { get; set; }
    }
}
