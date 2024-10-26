namespace SLAE.ViewModels
{
    public class ViewModel
    {
        private int equationsCount = 1;
        private int variablesCount = 1;
        public int EquationsCount
        {
            get
            {
                return equationsCount;
            }
            set
            {
                equationsCount = value;
            }
        }
        public int VariablesCount
        {
            get
            {
                return variablesCount;
            }
            set
            {
                variablesCount = value;
            }
        }
        public List<int> Numbers { get; } = Enumerable.Range(1, 10).ToList();
        /// <summary>
        /// Инициализурует новый экземпляр <see cref="ViewModel"/>
        /// </summary>
        public ViewModel()
        {

        }
    }
}
