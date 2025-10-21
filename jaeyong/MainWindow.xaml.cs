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

        //사이드바의 형재 상태를 추적
        private bool isSidebarOpen = false;
        //사이드바가 열렸을 때 너비
        //private const int SidebarWidth = 100;

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
                Display.Text = button.Content.ToString();
                isNewEntry = false; //false로 바꾸기 때문에 다음에 숫자를 다시 입력해도 false로 display가 초기화되는 것을 막는다.
            }
            else
            {
                Display.Text += button.Content.ToString(); // 위에서 false로 변환하면서 다음 숫자는 else로 접근하여 연결해서 추가
            }
        }

        // 연산자 버튼
        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if(double.TryParse(Display.Text, out double newValue)) //문자열을 double 형식으로 변환하여 유효한 숫자인 경우에는 값을  newValue에 저장
            {
                if (!isNewEntry)
                {
                    Calculate(newValue);
                }
                currentOperator = button.Content.ToString();

                if (currentOperator == "%")
                {
                    double percentageValue = currentValue * (newValue / 100);
                    Calculate(percentageValue);
                    currentOperator = "";
                }
                else
                {
                    currentValue = newValue;
                }
                isNewEntry = true;
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
                    else {
                        MessageBox.Show("0으로 나눌 수 없습니다.", "Error");
                    }
                    break;
            }
            Display.Text = currentValue.ToString();
        }


        // 취소 버튼
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Display.Text.Length > 0)
            {
                Display.Text = Display.Text.Substring(0, Display.Text.Length - 1);
            }

            if (Display.Text.Length == 0)
            {
                Display.Text = "0";
                isNewEntry = true;
            }
        }

        // 계산 결과
        private void Result_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Display.Text, out double newValue))
            {
                Calculate(newValue);
                currentOperator = "";
                isNewEntry=true;
            }
        }

        // 초기화 ( C )
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            currentValue = 0;
            currentOperator = "";
            Display.Text = "0";
            isNewEntry = true;
        }

        // 초기화 ( CE )
        private void ClearEntry_Click(object sender, RoutedEventArgs e)
        {
            Display.Text = "0";
            isNewEntry = true;
        }

        //역수
        private void Reciprocal_Click(object sender, RoutedEventArgs e)
        {
            if(double.TryParse(Display.Text, out double x))
            {
                if (x != 0)
                {
                    currentValue = 1 / x;
                    Display.Text = currentValue.ToString();
                    isNewEntry = true; 
                } else
                {
                    Display.Text = "Error";
                }
            }
        }

        //제곱
        private void Square_Click(object sender, RoutedEventArgs e)
        {
           if(double.TryParse(Display.Text, out double x))
            {
                currentValue = x * x;
                Display.Text = currentValue.ToString();
                isNewEntry = true;
            }
        }
        
        //제곱근
        private void SquareRoot_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Display.Text, out double x))
            {
                currentValue = Math.Sqrt(x);

                Display.Text = currentValue.ToString();
                isNewEntry = true;
            }
            else
            {
                Display.Text = "Error";
                isNewEntry= true;
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
    }
}
