using System.Collections.ObjectModel;
using System.ComponentModel;
using SNE.Interfaces;
using SNE.Models;

namespace SNE.Viewmodels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public int ExpressionCount { get; set; } = 0;
        public ObservableCollection<IIterativeMethod> SolvingMethods { get; set; } = new ObservableCollection<IIterativeMethod>();
        public bool IsTwoFormNeed { get; set; } = false;
        public MainViewModel()
        {
            Initialize();
        }
        private void Initialize()
        {
            BuildSolvers();
        }
        private void BuildSolvers()
        {
            SolvingMethods = new ObservableCollection<IIterativeMethod>(SolversBuilder.BuildSolvers());
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
