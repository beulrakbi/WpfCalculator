using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double currentValue = 0; //이전에 입력한 결과 값 저장
        private string currentOperator = ""; //사용자가 마지막으로 누른 연산자 저장
        private bool isNewEntry = false; //true: 계산기에서 +, - , %, =을 누른 직후, false: 사용자가 숫자를 연속해서 입력
        private bool isSidebarOpen = false;
        private string currentExpression = "";
        private double memoryValue = 0;

        public MainWindow()
        {
            InitializeComponent();
        }
        //숫자 버튼
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (isNewEntry)
            {
                ResultDisplay.Text = button.Content.ToString();
                isNewEntry = false; //false로 바꾸기 때문에 다음에 숫자를 다시 입력해도 false로 display가 초기화되는 것을 막는다.
            }
            else
            {
                ResultDisplay.Text += button.Content.ToString(); // 위에서 false로 변환하면서 다음 숫자는 else로 접근하여 연결해서 추가
            }
            if (!string.IsNullOrEmpty(currentExpression) || currentValue != 0)
            {
                ExpressionDisplay.Text = currentExpression + ResultDisplay.Text;
            }
        }

        // 연산자 버튼
        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string newOperator = button.Content.ToString();

            if (double.TryParse(ResultDisplay.Text, out double newValue))
            {
                // 1. 숫자를 입력 중인 경우 (3 + 5 에서 5를 입력하고 X 누를 때)
                if (!isNewEntry)
                {
                    if (string.IsNullOrEmpty(currentOperator))
                    {
                        currentValue = newValue; // ⭐️ 첫 연산: currentValue = 5
                    }
                    else
                    {
                        currentExpression += ResultDisplay.Text;
                        Calculate(newValue); // ⭐️ 연속 연산: 이전 값으로 계산 (currentValue 업데이트)
                    }
                }

                currentOperator = newOperator;
                isNewEntry = true; // 다음 숫자 입력 시 ResultDisplay를 덮어쓰도록 설정

                if (currentOperator == "%")
                {
                    double percentageValue = currentValue * (newValue / 100);
                    Calculate(percentageValue);
                    currentOperator = "";
                    currentExpression = ""; // % 계산 후 수식 초기화
                }
                else
                {
                    // 4. 수식 업데이트: currentValue(예: 8)와 새로운 연산자(+)로 수식을 다시 구성합니다.
                    currentExpression = currentValue.ToString() + $" {currentOperator} ";
                }

                // 5. 수식 창에 표시
                ExpressionDisplay.Text = currentExpression;
            }
        }

        // 사칙 연산
        private void Calculate(double newValue)
        {
            switch (currentOperator)
            {
                case "+": currentValue += newValue; break;
                case "-": currentValue -= newValue; break;
                case "X": currentValue *= newValue; break;
                case "/":
                    if (newValue != 0)
                    {
                        currentValue /= newValue;
                    }
                    else
                    {
                        MessageBox.Show("0으로 나눌 수 없습니다.", "Error");
                    }
                    break;
            }
            ResultDisplay.Text = currentValue.ToString();
        }


        // 취소 버튼
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ResultDisplay.Text.Length > 0)
            {
                ResultDisplay.Text = ResultDisplay.Text.Substring(0, ResultDisplay.Text.Length - 1);
            }

            if (ResultDisplay.Text.Length == 0)
            {
                ResultDisplay.Text = "0";
                isNewEntry = true;
            }
        }

        // 계산 결과
        private void Result_Click(object sender, RoutedEventArgs e)
        {
            //if (double.TryParse(Display.Text, out double newValue))
            //{
            //    Calculate(newValue);
            //    currentOperator = "";
            //    isNewEntry = true;
            //}
            if (double.TryParse(ResultDisplay.Text, out double newValue))
            {
                // 1. 최종 수식 완성: 현재까지의 Expression + Display 값 + "="
                string finalEquation = currentExpression + ResultDisplay.Text + " =";

                Calculate(newValue); // 계산 실행. Display.Text와 currentValue가 업데이트됨

                // 2. History에 추가
                AddToHistory(finalEquation, ResultDisplay.Text);

                ExpressionDisplay.Text = finalEquation;

                // 3. 상태 및 수식 초기화
                currentOperator = "";
                isNewEntry = true;
                currentExpression = ""; // 기록 저장 후 수식 추적 변수 초기화

            }
        }

        // 초기화 ( C )
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            currentValue = 0;
            currentOperator = "";
            ResultDisplay.Text = "0";
            ExpressionDisplay.Text = "";
            isNewEntry = true;
        }

        // 초기화 ( CE )
        private void ClearEntry_Click(object sender, RoutedEventArgs e)
        {
            ResultDisplay.Text = "0";
            isNewEntry = true;
        }

        //역수
        private void Reciprocal_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(ResultDisplay.Text, out double x))
            {
                if (x != 0)
                {
                    currentValue = 1 / x;
                    ResultDisplay.Text = currentValue.ToString();
                    isNewEntry = true;
                }
                else
                {
                    ResultDisplay.Text = "Error";
                }
            }
        }

        //제곱
        private void Square_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(ResultDisplay.Text, out double x))
            {
                currentValue = x * x;
                ResultDisplay.Text = currentValue.ToString();
                isNewEntry = true;
            }
        }

        //제곱근
        private void SquareRoot_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(ResultDisplay.Text, out double x))
            {
                currentValue = Math.Sqrt(x);

                ResultDisplay.Text = currentValue.ToString();
                isNewEntry = true;
            }
            else
            {
                ResultDisplay.Text = "Error";
                isNewEntry = true;
            }
        }

        //사이드바 
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (isSidebarOpen)
            {
                SidebarPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                SidebarPanel.Visibility = Visibility.Visible;
            }

            isSidebarOpen = !isSidebarOpen;
        }

        private void HistoryButton_Click(Object sender, RoutedEventArgs e)
        {
            bool isVisible = (HistoryPanel.Visibility == Visibility.Visible);

            if (isVisible)
            {
                HistoryPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                HistoryPanel.Visibility = Visibility.Visible;

                SidebarPanel.Visibility = Visibility.Collapsed;
            }

        }

        //계산 기록 생성
        private void AddToHistory(string equation, string result) //equation에는 계산에 사용된 전체 수식, result에는 결과만
        {
            Grid historyItem = new Grid() { Margin = new Thickness(10, 5, 10, 5) };

            //수식
            historyItem.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            //결과
            historyItem.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            TextBlock equationText = new TextBlock
            {
                Text = equation, //수식 전달 받는 부분
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100)),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Grid.SetRow(equationText, 0);

            TextBlock resultText = new TextBlock
            {
                Text = result,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 0, 5)
            };
            Grid.SetRow(resultText, 1);

            historyItem.Children.Add(equationText);
            historyItem.Children.Add(resultText);

            HistoryStackPanel.Children.Insert(0, historyItem);

        }

        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            HistoryStackPanel.Children.Clear();
        }
    }
}
