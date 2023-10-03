using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Расчет_значений_трубопровода_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        Polinom polinomLeft;
        Polinom polinomRight;

        Pipeline leftPipeline;
        Pipeline rightPipeline;

        string fileLeftPoints;
        string fileRightPoints;

        double pipelineLength;//длина всего участка трубы
        double lc1;//расстояние от левого края до места 1 разреза
        double lc2;//расстояние от левого края до места 2 разреза
        double hyLeft = 0;// высота центрирования левого участка трубы по вертикали
        double hzLeft = 0;//высота центрирования левого участка трубы по горизонтали
        double hyRight = 0;//высота центрирования правого участка трубы по вертикали
        double hzRight = 0;//высота центрирования правого участка трубы по горизонтали
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Получениие коэффициентов колинома, введенных вручную
        /// </summary>
        private void GetPolinom()
        {
            if ((bool)CheckBoxPolinom.IsChecked)
            {
                double[] left_v;
                double[] left_h;
                double[] right_v;
                double[] right_h;

                left_v = new double[4]
                {
                    double.Parse(TextBoxCoeffLeftAV.Text),
                    double.Parse(TextBoxCoeffLeftBV.Text),
                    double.Parse(TextBoxCoeffLeftCV.Text),
                    double.Parse(TextBoxCoeffLeftDV.Text)
                };
                left_h = new double[4]
                {
                    double.Parse(TextBoxCoeffLeftAH.Text),
                    double.Parse(TextBoxCoeffLeftBH.Text),
                    double.Parse(TextBoxCoeffLeftCH.Text),
                    double.Parse(TextBoxCoeffLeftDH.Text)
                };
                right_v = new double[4]
                {
                    double.Parse(TextBoxCoeffRightAV.Text),
                    double.Parse(TextBoxCoeffRightBV.Text),
                    double.Parse(TextBoxCoeffRightCV.Text),
                    double.Parse(TextBoxCoeffRightDV.Text)
                };
                right_h = new double[4]
{
                    double.Parse(TextBoxCoeffRightAH.Text),
                    double.Parse(TextBoxCoeffRightBH.Text),
                    double.Parse(TextBoxCoeffRightCH.Text),
                    double.Parse(TextBoxCoeffRightDH.Text)
};
                polinomLeft = new Polinom(left_v, left_h);
                polinomRight = new Polinom(right_v, right_h);
            }
            else
            {
                polinomLeft = new(fileLeftPoints);
                polinomRight = new(fileRightPoints);

            }
            buttinBuildLeftVertPolinome.IsEnabled = true;
            buttinBuildLeftHorPolinome.IsEnabled = true;
            buttinBuildRightHorPolinome.IsEnabled = true;
            buttinBuildRightVertPolinome.IsEnabled = true;
        }
        /// <summary>
        /// Метод,вызывающий окна с графиками
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickBuildGraph(object sender, RoutedEventArgs e)
        {
            Button button =(Button) sender;

            double ll = double.Parse(textBoxDistanceLeftEdgeToFirstCut.Text);//длина левой части
            double lr = double.Parse(textBoxPipelineLength.Text) - double.Parse(textBoxDistanceLeftEdgeToFirstCut.Text);//длина правой части

            if (button.Name == "buttinBuildLeftVertPolinome")
            {
                Graph graph = new Graph("Вертикальный прогиб левого участка трубопровода", polinomLeft.coeffV, ll);
                graph.Show();
            }
            if (button.Name == "buttinBuildLeftHorPolinome")
            {
                Graph graph = new Graph("Горизонтальный прогиб левого участка трубопровода", polinomLeft.coeffH, ll);
                graph.Show();
            }
            if (button.Name == "buttinBuildRightVertPolinome")
            {
                Graph graph = new Graph("Вертикальный прогиб правого участка трубопровода", polinomRight.coeffV, lr);
                graph.Show();
            }
            if (button.Name == "buttinBuildRightHorPolinome")
            {
                Graph graph = new Graph("Горизонтальный прогиб правого участка трубопровода", polinomRight.coeffH, lr);
                graph.Show();
            }
            if (button.Name == "buttinBuildSigmaZleft")
            {
                Graph graph = new Graph("Напряжения в левом участке трубопровода", leftPipeline.SigmaZlist.ToArray());
                graph.Show();

            }
            if (button.Name == "buttinBuildSigmaZright")
            {
                Graph graph = new Graph("Напряжения в правом участке трубопровода", rightPipeline.SigmaZlist.ToArray());
                graph.Show();

            }

        }

        /// <summary>
        /// Событие нажатия на кнопу с загрузкой файла с координатами 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickChoseLeftFile(object sender, RoutedEventArgs e)
        {
            //Вызов диалогового окна с возможностью загружать текстовые файлы
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                if (!ReadFileCoords(openFileDialog.FileName))
                {
                    MessageBox.Show("Неверный формат введенных данных", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                textBlockFileLeftCoord.Text = System.IO.Path.GetFileName(openFileDialog.FileName);
                fileLeftPoints = openFileDialog.FileName;
            }      
        }

        /// <summary>
        /// Событие нажатия на кнопу с загрузкой файла с координатами  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickChoseRightFile(object sender, RoutedEventArgs e)
        {
            //Вызов диалогового окна с возможностью загружать текстовые файлы
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                if (!ReadFileCoords(openFileDialog.FileName))
                {
                    MessageBox.Show("Неверный формат введенных данных", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                textBlockFileRightCoord.Text = System.IO.Path.GetFileName(openFileDialog.FileName);
                fileRightPoints = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// Проверка введенных значений в TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Allow only digits (0-9), the minus sign (-), and the period or comma as a decimal separator
            if (!char.IsDigit(e.Text[0]) && e.Text[0] != '-' && e.Text[0] != '.' && e.Text[0] != ',')
            {
                e.Handled = true;
            }
            if (e.Text[0] == ',' && ((TextBox)sender).Text.Contains(','))
            {
                e.Handled = true;
            }
            if (e.Text[0] == '-')
            {
                if (((TextBox)sender).SelectionStart != 0 || ((TextBox)sender).Text.Contains('-'))
                {
                    e.Handled = true;
                }
            }

        }

        /// <summary>
        /// Проверка файла с координатами на возможность чтения
        /// </summary>
        /// <param name="path"> путь к файлу</param>
        /// <returns></returns>
        private bool ReadFileCoords(string path)
        {
            try
            {
                using (StreamReader fs = new StreamReader(path))
                {
                    int i = 0;
                    while (true)
                    {
                        // Читаем строку из файла во временную переменную.
                        var temp1 = fs.ReadLine();
                        // Если достигнут конец файла, прерываем считывание.
                        if (temp1 == null) break;

                        if (temp1[0] == 'P') temp1 = temp1.Substring(temp1.IndexOf(",") + 1);
                        var temp = temp1.Replace("\t", " ").Trim().Replace(".", ",").Split(' ').ToArray();

                        double.Parse(temp[0].Replace(".", ","));
                        double.Parse(temp[1].Replace(".", ","));
                        double.Parse(temp[2].Replace(".", ","));

                        i++;
                    }

                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Замена всех '.' на ',' внутри TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Text = textBox.Text.Replace('.', ',');
        }

        /// <summary>
        /// В случае, если в TextBox первым символом является ',', то перед ней ставим 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.EndsWith(",")) textBox.Text += '0';
        }

        /// <summary>
        /// Расчет значения полинома в точке x
        /// </summary>
        /// <param name="p">массив коэффициентов полинома</param>
        /// <param name="x">точка в которой нудно найти значение</param>
        /// <returns></returns>
        private double CalcY(double[] p, double x)
        {
            return p[0] * x * x * x * x + p[1] * x * x * x + p[2] * x * x + p[3] * x;
        }

        private void GetH()
        {
            pipelineLength = double.Parse(textBoxPipelineLength.Text);//длина всего участка трубы
            lc1 = double.Parse(textBoxDistanceLeftEdgeToFirstCut.Text);//расстояние от левого края до места 1 разреза
            lc2 = double.Parse(textBoxDistanceLeftEdgeToSecondCut.Text);//расстояние от левого края до места 2 разреза

            double ll;//длина левого участка до вырезания дефектного
            double llc;//длина левого участка после вырезания дефектного
            double lr;//длина правого участка до вырезания дефектного
            double lrc;//длина правого участка после вырезания дефектного

            hyLeft = 0;// высота центрирования левого участка трубы по вертикали
            hzLeft = 0;//высота центрирования левого участка трубы по горизонтали

            hyRight = 0;//высота центрирования правого участка трубы по вертикали
            hzRight = 0;//высота центрирования правого участка трубы по горизонтали

            if (lc1 > lc2)
            {
                ll = lc1;
                llc = lc2;
                lr = pipelineLength - lc1;
                lrc = pipelineLength - lc1;
            }
            else
            {
                ll = lc1;
                llc = lc1;
                lr = pipelineLength - lc1;
                lrc = pipelineLength - lc2;
            }
            if (llc > lrc)
            {
                hyLeft = CalcY(polinomRight.coeffV, lrc) + CalcY(polinomLeft.coeffV, ll) - CalcY(polinomRight.coeffV, lr);
                hzLeft = CalcY(polinomRight.coeffH, lrc) + CalcY(polinomLeft.coeffH, ll) - CalcY(polinomRight.coeffH, lr);

                hyRight = CalcY(polinomRight.coeffV, lrc);
                hzRight = CalcY(polinomRight.coeffH, lrc);
            }
            if (llc < lrc)
            {
                hyLeft = CalcY(polinomLeft.coeffV, llc);
                hzLeft = CalcY(polinomLeft.coeffH, llc);

                hyRight = CalcY(polinomLeft.coeffV, llc) - CalcY(polinomLeft.coeffV, ll) + CalcY(polinomRight.coeffV, lr);
                hzRight = CalcY(polinomLeft.coeffH, llc) - CalcY(polinomLeft.coeffH, ll) + CalcY(polinomRight.coeffH, lr);
            }
            if (llc == lrc)
            {
                hyLeft = CalcY(polinomLeft.coeffV, llc) - (CalcY(polinomLeft.coeffV, llc) - (CalcY(polinomRight.coeffV, lrc) + CalcY(polinomLeft.coeffV, ll) - CalcY(polinomRight.coeffV, lr))) / 2;
                hzLeft = CalcY(polinomLeft.coeffH, llc) - (CalcY(polinomLeft.coeffH, llc) - (CalcY(polinomRight.coeffH, lrc) + CalcY(polinomLeft.coeffH, ll) - CalcY(polinomRight.coeffH, lr))) / 2;

                hyRight = CalcY(polinomRight.coeffV, lrc) - (CalcY(polinomRight.coeffV, lrc) - (CalcY(polinomLeft.coeffV, llc) - CalcY(polinomLeft.coeffV, ll) + CalcY(polinomRight.coeffV, lr))) / 2;
                hzRight = CalcY(polinomRight.coeffH, lrc) - (CalcY(polinomRight.coeffH, lrc) - (CalcY(polinomLeft.coeffH, llc) - CalcY(polinomLeft.coeffH, ll) + CalcY(polinomRight.coeffH, lr))) / 2;
            }
        }

        /// <summary>
        /// Расчет необходимых величин для ремота трубопровода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickGetRes(object sender, RoutedEventArgs e)
        {


            if (!AllDataInput())
            {
                MessageBox.Show("Вы ввели не все данные", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //Получение коэффициентов полинома
            GetPolinom();

            //Расчет высоты центрирования трубопровода 
            GetH();

            //расчет значений правого участка трубопровода
            rightPipeline = new(
                double.Parse(textBoxOutsideDiameter.Text),
                double.Parse(textBoxInnerDiameter.Text),
                double.Parse(textBoxYoungModulus.Text),
                double.Parse(textBoxMaterialDensity.Text),
                double.Parse(textBoxPipelineLength.Text) - double.Parse(textBoxDistanceLeftEdgeToFirstCut.Text),
                double.Parse(textBoxPipelineLength.Text) - double.Parse(textBoxDistanceLeftEdgeToSecondCut.Text),
                double.Parse(textBoxPipelineLength.Text),
                hyRight,
                hzRight,
                polinomRight.coeffV,
                polinomRight.coeffH,
                double.Parse(textBoxPermissibleStresses.Text),
                double.Parse(textBoxPermissibleLoads.Text)
                );
            //расчет значений левого участка трубопровода
            leftPipeline = new(
                double.Parse(textBoxOutsideDiameter.Text),
                double.Parse(textBoxInnerDiameter.Text),
                double.Parse(textBoxYoungModulus.Text),
                double.Parse(textBoxMaterialDensity.Text),
                double.Parse(textBoxDistanceLeftEdgeToFirstCut.Text),
                double.Parse(textBoxDistanceLeftEdgeToSecondCut.Text),
                double.Parse(textBoxPipelineLength.Text),
                hyLeft,
                hzLeft,
                polinomLeft.coeffV,
                polinomLeft.coeffH,
                double.Parse(textBoxPermissibleStresses.Text),
                double.Parse(textBoxPermissibleLoads.Text)
                );

            //если при текущих данных нет такого положения, при котором не будут привышаться допустимые параметры, то Требуется дополнительное откапывание трубопровода
            if (leftPipeline.X1 ==0 || rightPipeline.X1 == 0)
            {
                MessageBox.Show("Требуется дополнительное откапывание трубопровода", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //Делаем кнопки, показывающие грфики напряжений активными
            buttinBuildSigmaZleft.IsEnabled = true;
            buttinBuildSigmaZright.IsEnabled = true;

            //Заполнение textBox данными
            textBoxLeftL1.Text = leftPipeline.ResL1.ToString();
            textBoxLeftL2.Text = leftPipeline.ResL2.ToString();
            textBoxLeftXH1.Text = leftPipeline.X_H[0].ToString();
            textBoxLeftXH2.Text = leftPipeline.X_H[1].ToString();
            textBoxLeftXV1.Text = leftPipeline.X_V[0].ToString();
            textBoxLeftXV2.Text = leftPipeline.X_V[1].ToString();
            textBoxLeftX1.Text = leftPipeline.X1.ToString();
            textBoxLeftX2.Text = leftPipeline.X2.ToString();
            textBoxLeftPH1.Text = leftPipeline.P_H[0].ToString();
            textBoxLeftPH2.Text = leftPipeline.P_H[1].ToString();
            textBoxLeftPV1.Text = leftPipeline.P_V[0].ToString();
            textBoxLeftPV2.Text = leftPipeline.P_V[1].ToString();
            textBoxLeftP1.Text = leftPipeline.P1.ToString();
            textBoxLeftP2.Text = leftPipeline.P2.ToString();
            textBoxLeftM.Text = leftPipeline.ResM0.ToString();
            textBoxLeftQ.Text = leftPipeline.ResQ0.ToString();

            textBoxRightL1.Text = rightPipeline.ResL1.ToString();
            textBoxRightL2.Text = rightPipeline.ResL2.ToString();
            textBoxRightXH1.Text = rightPipeline.X_H[0].ToString();
            textBoxRightXH2.Text = rightPipeline.X_H[1].ToString();
            textBoxRightXV1.Text = rightPipeline.X_V[0].ToString();
            textBoxRightXV2.Text = rightPipeline.X_V[1].ToString();
            textBoxRightX1.Text = rightPipeline.X1.ToString();
            textBoxRightX2.Text = rightPipeline.X2.ToString();
            textBoxRightPH1.Text = rightPipeline.P_H[0].ToString();
            textBoxRightPH2.Text = rightPipeline.P_H[1].ToString();
            textBoxRightPV1.Text = rightPipeline.P_V[0].ToString();
            textBoxRightPV2.Text = rightPipeline.P_V[1].ToString();
            textBoxRightP1.Text = rightPipeline.P1.ToString();
            textBoxRightP2.Text = rightPipeline.P2.ToString();
            textBoxRightM.Text = rightPipeline.ResM0.ToString();
            textBoxRightQ.Text = rightPipeline.ResQ0.ToString();
        }

        /// <summary>
        /// Метод, проверяющий, все ли данные были введены
        /// </summary>
        /// <returns></returns>
        private bool AllDataInput()
        {
            if (textBoxOutsideDiameter.Text == "") return false;
            if (textBoxInnerDiameter.Text == "") return false;
            if (textBoxYoungModulus.Text == "") return false;
            if (textBoxMaterialDensity.Text == "") return false;
            if (textBoxPipelineLength.Text == "") return false;
            if (textBoxDistanceLeftEdgeToFirstCut.Text == "") return false;
            if (textBoxDistanceLeftEdgeToSecondCut.Text == "") return false;
            if (textBoxPermissibleStresses.Text == "") return false;
            if (textBoxPermissibleLoads.Text == "") return false;

            if ((bool)CheckBoxPolinom.IsChecked)
            {
                if (TextBoxCoeffLeftAV.Text == "") return false;
                if (TextBoxCoeffLeftBV.Text == "") return false;
                if (TextBoxCoeffLeftCV.Text == "") return false;
                if (TextBoxCoeffLeftDV.Text == "") return false;
                if (TextBoxCoeffLeftAH.Text == "") return false;
                if (TextBoxCoeffLeftBH.Text == "") return false;
                if (TextBoxCoeffLeftCH.Text == "") return false;
                if (TextBoxCoeffLeftDH.Text == "") return false;

                if (TextBoxCoeffRightAV.Text == "") return false;
                if (TextBoxCoeffRightBV.Text == "") return false;
                if (TextBoxCoeffRightCV.Text == "") return false;
                if (TextBoxCoeffRightDV.Text == "") return false;
            }
            else
            {
                if (textBlockFileLeftCoord.Text == "") return false;
                if (textBlockFileRightCoord.Text == "") return false;
            }

            return true;
        }

        private void ClickClear(object sender, RoutedEventArgs e)
        {
            ClearTextBoxes(this);
        }
        /// <summary>
        ///  Очистка text из TextBox
        /// </summary>
        /// <param name="parent"></param>
        private void ClearTextBoxes(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is TextBox textBox)
                {
                    textBox.Text = "";
                }
                else
                {
                    ClearTextBoxes(child);
                }
            }
        }

        /// <summary>
        /// Если пользователь хочет ввести коэфф. полиномов самостоятельно, то показываем форму для ввода коэфф. и скрываем форму для ввода файлов с координатами
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxPolinomChecked(object sender, RoutedEventArgs e)
        {
            InputFromFile.Visibility = Visibility.Collapsed;
            InputUser.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Если пользователь хочет ввести коэфф. полиномов файлом с координатами, то показываем форму с файлами с координатами и скрываем форму для самостоятельного ввода коэфф.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxPolinomUnChecked(object sender, RoutedEventArgs e)
        {
            InputFromFile.Visibility = Visibility.Visible;
            InputUser.Visibility = Visibility.Collapsed;
        }
    }
}