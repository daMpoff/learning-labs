namespace OOP_2_3_threads
{
    public partial class Form1 : Form
    {
        private static List<int> data = new List<int>();
        private CancellationTokenSource cancellationTokenSource;
        private bool isPaused = false;
        private object pauseLock = new object();
        private int progress = 0;

        public Form1()
        {
            InitializeComponent();
            UpdateListBox();
            progressBar.Minimum = 0;
            progressBar.Value = 0;
        }

        private void RndGenerate(int a)
        {
            Random rnd = new Random();
            for (int i = 0; i < a; i++)
            {
                int tmp = rnd.Next(1, 100);
                data.Add(tmp);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(textboxCounter.Text, out int temp))
                {
                    progressBar.Maximum = temp - 1;
                    data.Clear(); // Очистка списка
                    RndGenerate(temp);
                }
            }
            catch
            {
                MessageBox.Show("Возникло исключение!");
            }

            UpdateListBox(); // Очистка ListBox
            progress = 0; // Сброс прогресса
            progressBar.Value = 0; // Сброс ProgressBar

            cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => MergeSort(data, 0, data.Count - 1, cancellationTokenSource.Token));
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            isPaused = !isPaused;
            btnPause.Text = isPaused ? "Resume" : "Pause";

            if (!isPaused)
            {
                lock (pauseLock)
                {
                    Monitor.PulseAll(pauseLock);
                }
            }
        }

        private async Task MergeSort(List<int> array, int left, int right, CancellationToken token)
        {
            if (left < right)
            {
                int middle = (left + right) / 2;

                await MergeSort(array, left, middle, token);
                await MergeSort(array, middle + 1, right, token);

                await Merge(array, left, middle, right, token);

                // Обновление после сортировкиы
                progress++;
                this.BeginInvoke((MethodInvoker)delegate
                {
                    progressBar.Value = progress;
                });
            }
            else if (left == right && progress == progressBar.Maximum - 1)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    progressBar.Value = progressBar.Maximum;
                });
            }
        }

        private async Task Merge(List<int> array, int left, int middle, int right, CancellationToken token)
        {
            int n1 = middle - left + 1;
            int n2 = right - middle;

            List<int> leftArray = new List<int>(n1);
            List<int> rightArray = new List<int>(n2);

            for (int i = 0; i < n1; i++)
                leftArray.Add(array[left + i]);

            for (int j = 0; j < n2; j++)
                rightArray.Add(array[middle + 1 + j]);

            int k = left;
            int iIndex = 0, jIndex = 0;

            while (iIndex < n1 && jIndex < n2)
            {
                if (leftArray[iIndex] <= rightArray[jIndex])
                {
                    array[k] = leftArray[iIndex];
                    iIndex++;
                }
                else
                {
                    array[k] = rightArray[jIndex];
                    jIndex++;
                }
                k++;

                // Обновляем интерфейс
                UpdateListBox();
                await Task.Delay(500);

                //Заглушка на паузу
                lock (pauseLock)
                {
                    while (isPaused)
                        Monitor.Wait(pauseLock);
                }

                //Проверка на обновления
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
            }

            while (iIndex < n1)
            {
                array[k] = leftArray[iIndex];
                iIndex++;
                k++;

                //Обновление интерфейса
                UpdateListBox();
                await Task.Delay(500);

                //Заглушка на паузу
                lock (pauseLock)
                {
                    while (isPaused)
                        Monitor.Wait(pauseLock);
                }

                //Проверка на получение токена отмены
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
            }

            while (jIndex < n2)
            {
                array[k] = rightArray[jIndex];
                jIndex++;
                k++;

                //Обновление интерфейса
                UpdateListBox();
                await Task.Delay(500);

                //Заглушка на паузу
                lock (pauseLock)
                {
                    while (isPaused)
                        Monitor.Wait(pauseLock);
                }

                //Проверка на отмену
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
            }
        }

        private void UpdateListBox()
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    listBox.Items.Clear();
                    foreach (var item in data)
                    {
                        listBox.Items.Add(item);
                    }
                });
            }
            else
            {
                // Если дескриптор окна еще не создан, мы можем выполнить обновление без Invoke
                listBox.Items.Clear();
                foreach (var item in data)
                {
                    listBox.Items.Add(item);
                }
            }
        }
    }
}