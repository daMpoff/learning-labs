using SNE.Interfaces;
using SNE.Models;

namespace SNE.Viewmodels
{
    public class MainViewModel
    {
        private int expressionCounter = 1;
        private List<IIterativeMethod> solvingMethods = new List<IIterativeMethod>();
        public MainViewModel()
        {
            Initialize();
        }
        private void Initialize()
        {
        }

        private void BuildSolvers()
        {
            
        }
    }
}
