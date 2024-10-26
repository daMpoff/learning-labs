namespace SLAE.ViewModels
{
    public class ViewModel
    {
        private int equationsCount = 1;
        private int variablesCount = 1;

        /// <summary>
        /// Количество уравнений СЛАУ
        /// </summary>
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

        /// <summary>
        /// Количество переменных СЛАУ
        /// </summary>
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

        /// <summary>
        /// Служебное свойство, которое является источником данных для combobox
        /// </summary>
        /// <remarks>
        /// Это свойство используется для того, чтобы заполнить элементы ComboBox, которые
        /// позволяют выбрать количество уравнений и количество переменных
        /// </remarks>
        public List<int> Numbers { get; } = Enumerable.Range(1, 10).ToList();

        /// <summary>
        /// Инициализурует новый экземпляр <see cref="ViewModel"/>
        /// </summary>
        public ViewModel()
        {

        }
    }
}
