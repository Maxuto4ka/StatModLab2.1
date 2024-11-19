using System.Windows.Forms.DataVisualization.Charting;

namespace StatModLab2._1
{
    internal static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            double lambda = 2.0;
            int numberOfEvents = 150;
            int N = 100;
            int eventIndex = 4;
            double timestamp = 5.0;

            var eventTimes = GeneratePoissonProcess(lambda, numberOfEvents);
            PlotProcess(eventTimes, lambda);
            PlotTimeOfOccurrence(lambda, N, numberOfEvents, eventIndex);
            PlotIntervals(lambda, N, numberOfEvents);
            PlotOccurrences(lambda, N, numberOfEvents, timestamp);
        }

        private static List<double> GeneratePoissonProcess(double lambda, int numberOfEvents)
        {
            var random = new Random();
            var eventTimes = new List<double>();
            double time = 0;

            for (int i = 0; i < numberOfEvents; i++)
            {
                double interval = -Math.Log(1 - random.NextDouble()) / lambda;
                time += interval;
                eventTimes.Add(time);
            }

            return eventTimes;
        }

        private static void PlotProcess(List<double> eventTimes, double lambda)
        {
            Form form = new Form();
            Chart chart = new Chart { Dock = DockStyle.Fill };

            ChartArea chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            Series series = new Series
            {
                ChartType = SeriesChartType.StepLine,
                BorderWidth = 2
            };

            for (int i = 0; i < eventTimes.Count; i++)
            {
                series.Points.AddXY(eventTimes[i], i + 1);
            }

            chart.Series.Add(series);
            form.Controls.Add(chart);

            Application.Run(form);
        }

        private static void PlotTimeOfOccurrence(double lambda, int N, int numberOfEvents, int eventIndex)
        {
            if (eventIndex < 0 || eventIndex >= N)
            {
                MessageBox.Show($"���� {eventIndex + 1} �������� �� ��� ����������� �������� (0-{N - 1}).", "�������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ����� ���� ����� ������ ��䳿 � ������ ������
            var eventTimes = new List<double>();

            for (int i = 0; i < numberOfEvents; i++)
            {
                var process = GeneratePoissonProcess(lambda, N);
                if (eventIndex < process.Count)
                {
                    eventTimes.Add(process[eventIndex]);
                }
            }

            if (eventTimes.Count == 0)
            {
                MessageBox.Show($"��� ��䳿 {eventIndex + 1} ���� �����!", "�������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var form = new Form { Text = $"ó�������� ���� ����� ��䳿 {eventIndex + 1}" };
            var chart = new Chart { Dock = DockStyle.Fill };

            var chartArea = new ChartArea
            {
                AxisX =
                {
                    Title = "��� ����� ��䳿",
                    Minimum = Math.Max(0, eventTimes.Min()),
                    IntervalAutoMode = IntervalAutoMode.VariableCount
                },
                AxisY =
                {
                    Title = "�������",
                    Minimum = 0
                }
            };
            chart.ChartAreas.Add(chartArea);

            var series = new Series
            {
                ChartType = SeriesChartType.Column,
                Color = Color.Green
            };

            int bins = 20; // ʳ������ ��������� (�������� ���������)
            double maxTime = eventTimes.Max();
            double binWidth = maxTime / bins;

            var histogramData = new int[bins];

            foreach (var time in eventTimes)
            {
                int binIndex = Math.Min((int)(time / binWidth), bins - 1);
                histogramData[binIndex]++;
            }

            for (int i = 0; i < bins; i++)
            {
                double binCenter = i * binWidth + binWidth / 2;
                series.Points.AddXY(binCenter, histogramData[i]);
            }

            chart.Series.Add(series);
            form.Controls.Add(chart);
            form.ShowDialog();
        }

        private static List<double> GetIntervals(List<double> eventTimes)
        {
            var intervals = new List<double>();

            for (int i = 1; i < eventTimes.Count; i++)
            {
                intervals.Add(eventTimes[i] - eventTimes[i - 1]);
            }

            return intervals;
        }
        private static void PlotIntervals(double lambda, int N, int numberOfEvents)
        {
            // ����� ��������� �� ������ ��� ��� �������
            var intervals = new List<double>();

            for (int i = 0; i < numberOfEvents; i++)
            {
                var process = GeneratePoissonProcess(lambda, N);
                var processIntervals = GetIntervals(process);
                intervals.AddRange(processIntervals);
            }

            if (intervals.Count == 0)
            {
                MessageBox.Show("��������� �� ������ ������!", "�������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var form = new Form { Text = "ó�������� ��������� �� ������" };
            var chart = new Chart { Dock = DockStyle.Fill };

            var chartArea = new ChartArea
            {
                AxisX =
                {
                    Title = "�������� �� ������",
                    Minimum = Math.Max(0, intervals.Min()),
                    IntervalAutoMode = IntervalAutoMode.VariableCount
                },
                AxisY =
                {
                    Title = "�������",
                    Minimum = 0
                }
            };
            chart.ChartAreas.Add(chartArea);

            var series = new Series
            {
                ChartType = SeriesChartType.Column,
                Color = Color.Blue
            };

            int bins = 20;
            double maxInterval = intervals.Max();
            double binWidth = maxInterval / bins;

            var histogramData = new int[bins];

            foreach (var interval in intervals)
            {
                int binIndex = Math.Min((int)(interval / binWidth), bins - 1);
                histogramData[binIndex]++;
            }

            for (int i = 0; i < bins; i++)
            {
                double binCenter = i * binWidth + binWidth / 2;
                series.Points.AddXY(binCenter, histogramData[i]);
            }

            chart.Series.Add(series);
            form.Controls.Add(chart);
            form.ShowDialog();
        }

        // ϳ�������� ������� ���� �� �������� ����
        private static List<int> GetOccurrences(double lambda, int N, int numberOfEvents, double timestamp)
        {
            var occurrences = new List<int>();

            for (int i = 0; i < numberOfEvents; i++)
            {
                var process = GeneratePoissonProcess(lambda, N);
                occurrences.Add(process.Count(t => t <= timestamp)); // ʳ������ ���� �� timestamp
            }

            return occurrences;
        }

        private static void PlotOccurrences(double lambda, int N, int numberOfEvents, double timestamp)
        {
            var occurrences = GetOccurrences(lambda, N, numberOfEvents, timestamp);

            var form = new Form { Text = $"������� ������� ���� �� ���� {timestamp}" };
            var chart = new Chart { Dock = DockStyle.Fill };

            var chartArea = new ChartArea
            {
                AxisX = { Title = "ʳ������ ����", Minimum = 0 },
                AxisY = { Title = "�������", Minimum = 0 }
            };
            chart.ChartAreas.Add(chartArea);
            var series = new Series
            {
                ChartType = SeriesChartType.Column,
                Color = System.Drawing.Color.Purple
            };

            var histogram = occurrences.GroupBy(x => x)
                                       .OrderBy(g => g.Key)
                                       .ToDictionary(g => g.Key, g => g.Count());

            foreach (var bin in histogram)
            {
                series.Points.AddXY(bin.Key, bin.Value);
            }

            chart.Series.Add(series);
            form.Controls.Add(chart);
            form.ShowDialog();
        }
    }
}