using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SLAE.ViewModels
{
    public class ResultViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<RootResult> results;

        public ObservableCollection<RootResult> Results
        {
            get => results;
            set
            {
                results = value;
                OnPropertyChanged(nameof(Results));
            }
        }

        public ResultViewModel()
        {
            Results = new ObservableCollection<RootResult>
        {
            new RootResult
            {
                MethodName = "Метод Гаусса",
                Roots = new ObservableCollection<double> { 1.5, 2.3, 4.1 }
            },
            new RootResult
            {
                MethodName = "Метод Якоби",
                Roots = new ObservableCollection<double> { 1.7, 2.6, 3.8, 4.9 }
            },
            new RootResult
            {
                MethodName = "Метод Зейделя",
                Roots = new ObservableCollection<double> { 0.9, 1.1 }
            }
        };
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class RootResult
    {
        public string MethodName { get; set; }
        public ObservableCollection<double> Roots { get; set; }
    }
}
