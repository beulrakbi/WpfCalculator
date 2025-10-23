using System.Numerics;
using System.Printing;
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
        //표준 =============
        private double currentValue = 0;
        private string currentOperator = "";
        private bool isNewEntry = false;
        private bool isSidebarOpen = false;
        private string currentExpression = "";
        private double memoryValue = 0;
        private List<double> memoryList = new List<double>();

        //프로그래머 ========
        private long programmerCurrentValue = 0;
        private string programmerCurrentOperator = "";

        private int currentBase = 16;

        public MainWindow()
        {
            InitializeComponent();
            UpdateMemoryButtonState();
        }
        //숫자 버튼
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (isNewEntry)
            {
                ResultDisplay.Text = button.Content.ToString();
                isNewEntry = false;
            }
            else
            {
                ResultDisplay.Text += button.Content.ToString();
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
                if (!isNewEntry)
                {
                    if (string.IsNullOrEmpty(currentOperator))
                    {
                        currentValue = newValue;
                    }
                    else
                    {
                        currentExpression += ResultDisplay.Text;
                        Calculate(newValue);
                    }
                }

                currentOperator = newOperator;
                isNewEntry = true;

                if (currentOperator == "%")
                {
                    double percentageValue = currentValue * (newValue / 100);
                    Calculate(percentageValue);
                    currentOperator = "";
                    currentExpression = "";
                }
                else
                {

                    currentExpression = currentValue.ToString() + $" {currentOperator} ";
                }


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
            TextBox display = (ProgrammerCalculatorUi.Visibility == Visibility.Visible) ?
                      ProgrammerResultDisplay :
                      ResultDisplay;

            if (display.Text.Length > 0)
            {
                display.Text = display.Text.Substring(0, display.Text.Length - 1);
            }

            if (display.Text.Length == 0)
            {
                display.Text = "0";
                isNewEntry = true;
            }
        }

        // 계산 결과
        private void Result_Click(object sender, RoutedEventArgs e)
        {

            if (double.TryParse(ResultDisplay.Text, out double newValue))
            {
                string finalEquation = currentExpression + ResultDisplay.Text + " =";

                Calculate(newValue); // 계산 실행. Display.Text와 currentValue가 업데이트됨

                AddToHistory(finalEquation, ResultDisplay.Text);

                ExpressionDisplay.Text = finalEquation;

                currentOperator = "";
                isNewEntry = true;
                currentExpression = "";
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

            UpdateMemoryButtonState();
        }

        // 초기화 ( CE )
        private void ClearEntry_Click(object sender, RoutedEventArgs e)
        {
            TextBox display = (ProgrammerCalculatorUi.Visibility == Visibility.Visible) ?
                      ProgrammerResultDisplay :
                      ResultDisplay;
            display.Text = "0";
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
                isSidebarOpen = false;
            }
            else
            {
                SidebarPanel.Visibility = Visibility.Visible;
                isSidebarOpen = true;
            }

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

        private void SidebarModeButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null) return;

            String mode = clickedButton.Content.ToString();

            SidebarPanel.Visibility = Visibility.Collapsed;
            HistoryPanel.Visibility = Visibility.Collapsed;
            isSidebarOpen = false; // 사이드바 상태 업데이트

            if (mode == "프로그래머")
            {
                StandardCalculatorUi.Visibility = Visibility.Collapsed;
                ProgrammerCalculatorUi.Visibility = Visibility.Visible;
                UpdateProgrammerButtons();
            }
            else if (mode == "표준")
            {
                StandardCalculatorUi.Visibility = Visibility.Visible;
                ProgrammerCalculatorUi.Visibility = Visibility.Collapsed;
            }

        }

        private void ClearAllMemory_Click(object sender, RoutedEventArgs e) // 이 이름!
        {
            memoryList.Clear();

            memoryValue = 0;

            RefreshMemoryListUI();
            UpdateDefaultStyle();
            MemoryPanel.Visibility = Visibility.Collapsed;
        }

        



        //====================== programmer button ====================
        private void ProgrammerButton_Click(Object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;

            string input = button.Content.ToString().ToUpper();

            TextBox display = ProgrammerResultDisplay;



            if (display.Text == "0" || isNewEntry)
            {
                if (input == "0" && display.Text == "")
                {

                }
                else
                {
                    display.Text = input;
                    isNewEntry = false;
                }
            }
            else
            {
                display.Text += input;
            }

            if (!string.IsNullOrEmpty(programmerCurrentOperator))
            {
                ProgrammerExpressionDisplay.Text = $"{programmerCurrentValue} {programmerCurrentOperator} {display.Text}";
            }
        }

        private void Shift_Click(Object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;

            if (long.TryParse(ProgrammerResultDisplay.Text, out long operand))
            {
                if (programmerCurrentOperator == "")
                {
                    programmerCurrentValue = operand;
                }

                programmerCurrentOperator = button.Content.ToString();

                ProgrammerExpressionDisplay.Text = $"{ProgrammerResultDisplay.Text} {programmerCurrentOperator}";

                isNewEntry = true;
            }
        }

        private void ProgrammerResult_Click(Object sender, RoutedEventArgs e)
        {

            if (long.TryParse(ProgrammerResultDisplay.Text, out long secondOperand))
            {
                if (!string.IsNullOrEmpty(programmerCurrentOperator))
                {
                    string finalEquation = ProgrammerExpressionDisplay.Text + " =";


                    if (programmerCurrentOperator == "<<" || programmerCurrentOperator == ">>")
                    {
                        CalculateProgrammerResult(secondOperand);
                    }
                    else
                    {
                        ProgrammerCalculate(secondOperand);
                    }

                    ProgrammerExpressionDisplay.Text = finalEquation;

                    programmerCurrentOperator = "";
                    isNewEntry = true;
                }
            }
        }

        //시프트연산
        private void CalculateProgrammerResult(long shiftCount)
        {
            long result = 0;
            string equation = $"{programmerCurrentValue} {programmerCurrentOperator} {shiftCount}";

            switch (programmerCurrentOperator)
            {
                case "<<":
                    result = programmerCurrentValue << (int)shiftCount;
                    break;

                case ">>":
                    result = programmerCurrentValue >> (int)shiftCount;
                    break;

                default:
                    result = programmerCurrentValue;
                    break;
            }
            ProgrammerResultDisplay.Text = result.ToString();

            ProgrammerExpressionDisplay.Text = result.ToString();

            programmerCurrentOperator = "";

            isNewEntry = true;

            ProgrammerExpressionDisplay.Text = "";
        }

        private void ProgrammerCalculate(long newValue)
        {
            long result = programmerCurrentValue;

            switch (programmerCurrentOperator)
            {
                case "+":
                    result += newValue;
                    break;

                case "-":
                    result -= newValue;
                    break;

                case "X":
                    result *= newValue;
                    break;

                case "%":
                    if (newValue != 0)
                    {
                        result %= newValue;
                    }
                    else
                    {
                        MessageBox.Show("0으로 나눌 수 없습니다.", "Error");
                        return;
                    }
                    break;

                case "/":
                    if (newValue != 0)
                    {
                        result /= newValue;
                    }
                    else
                    {
                        MessageBox.Show("0으로 나눌 수 없습니다.", "Error");
                        return;
                    }
                    break;
                default:
                    break;
            }
            programmerCurrentValue = result;
            ProgrammerResultDisplay.Text = result.ToString();
        }

        private void ProgrammerOperator_Click(Object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;

            string newOperator = button.Content.ToString();


            if (long.TryParse(ProgrammerResultDisplay.Text, out long newValue))
            {
                if (!string.IsNullOrEmpty(programmerCurrentOperator) && !isNewEntry)
                {
                    ProgrammerCalculate(newValue);
                }
                else if (string.IsNullOrEmpty(programmerCurrentOperator) || isNewEntry)
                {
                    programmerCurrentValue = newValue;
                }


                programmerCurrentOperator = newOperator;
                isNewEntry = true;

                ProgrammerExpressionDisplay.Text = $"{programmerCurrentValue} {programmerCurrentOperator} ";
            }
        }

        //진수 변경
        private void BaseChanged(object sender, RoutedEventArgs e)
        {
            TextBox displayTextBox = ProgrammerResultDisplay;

            RadioButton selectedRadioButton = sender as RadioButton;
            if (selectedRadioButton == null || selectedRadioButton.Tag == null)
                return;

            if (!int.TryParse(selectedRadioButton.Tag.ToString(), out int targetBase))
                return;

            int oldBase = currentBase;

            currentBase = targetBase;

            UpdateProgrammerButtons();

            string input = displayTextBox.Text.Trim();
            if (string.IsNullOrEmpty(input) || input == "0")
            {
                currentBase = targetBase;
                displayTextBox.Text = "0"; // 입력 값이 비어있을 경우 0으로 설정
                return;
            }

            try
            {
                // ⭐️ 수정된 부분: BigInteger.Parse의 오버로드에 맞게 NumberStyles를 지정합니다.
                System.Globalization.NumberStyles style = System.Globalization.NumberStyles.Integer;

                switch (oldBase)
                {
                    case 16:
                        style = System.Globalization.NumberStyles.HexNumber;
                        break;
                    case 10:
                        style = System.Globalization.NumberStyles.Integer; // 10진수
                        break;
                    case 8:
                    case 2:
                        BigInteger baseValue = ParseCustomBase(input, oldBase);
                        string outputCustom = ConvertBigIntegerToBase(baseValue, targetBase);
                        displayTextBox.Text = outputCustom;
                        currentBase = targetBase;
                        return; // 커스텀 파싱/변환 후 종료
                }

                // 10진수 또는 16진수 파싱
                BigInteger decimalValue = BigInteger.Parse(input, style);

                string output = ConvertBigIntegerToBase(decimalValue, targetBase);

                displayTextBox.Text = output;

                currentBase = targetBase;

                UpdateProgrammerButtons();
            }
            catch (FormatException)
            {
                MessageBox.Show($"'{input}'은(는) 현재 진수({currentBase}진수)의 유효한 값이 아닙니다. 진수 변환을 취소합니다.", "변환 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"변환 중 오류 발생: {ex.Message}", "시스템 오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private BigInteger ParseCustomBase(string input, int currentBase)
        {
            if (currentBase == 10 || currentBase == 16)
            {
                // BigInteger.Parse가 처리할 수 있는 경우
                System.Globalization.NumberStyles style = (currentBase == 16) ?
                                                            System.Globalization.NumberStyles.HexNumber :
                                                            System.Globalization.NumberStyles.Integer;
                return BigInteger.Parse(input, style);
            }

            BigInteger result = 0;
            BigInteger power = 1;

            // 문자열을 역순으로 처리하여 BigInteger로 변환
            for (int i = input.Length - 1; i >= 0; i--)
            {
                char c = input[i];
                int digit = 0;

                if (c >= '0' && c <= '9')
                {
                    digit = c - '0';
                }
                else if (c >= 'A' && c <= 'F' && currentBase == 16)
                {
                    digit = 10 + (c - 'A');
                }
                else if (c >= 'a' && c <= 'f' && currentBase == 16)
                {
                    digit = 10 + (c - 'a');
                }
                else
                {
                    // 유효하지 않은 문자
                    throw new FormatException($"유효하지 않은 문자 '{c}'가 {currentBase}진수 입력에 포함되어 있습니다.");
                }

                if (digit >= currentBase)
                {
                    // 진수 범위 초과
                    throw new FormatException($"자리수 '{digit}'가 {currentBase}진수 범위({currentBase - 1} 이하)를 초과합니다.");
                }

                result += digit * power;
                power *= currentBase;
            }

            return result;
        }

        private string ConvertBigIntegerToBase(BigInteger value, int targetBase)
        {
            if (value == 0)
                return "0";
            if (targetBase < 2 || targetBase > 36) // 진수는 2~36 사이가 일반적 (A-Z 사용)
                throw new ArgumentOutOfRangeException(nameof(targetBase), "진수는 2에서 36 사이여야 합니다.");

            string baseChars = "0123456789ABCDEFghijklmnopqrstuvwxyz";
            string result = "";
            BigInteger tempValue = value;
            BigInteger targetBaseBig = new BigInteger(targetBase);

            if (tempValue < 0)
            {
                tempValue = BigInteger.Abs(tempValue);
            }

            while (tempValue > 0)
            {
                BigInteger remainder = tempValue % targetBaseBig;
                result = baseChars[(int)remainder] + result;
                tempValue /= targetBaseBig;
            }

            return result.ToUpper(); // HEX, OCT, BIN은 대문자로 표시
        }


        private void MemoryDropdown_Click(object sender, RoutedEventArgs e)
        {
            bool isVisible = (MemoryPanel.Visibility == Visibility.Visible);

            if (isVisible)
            {
                MemoryPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                // 목록을 표시하기 전에 항상 최신 상태로 업데이트
                RefreshMemoryListUI();
                MemoryPanel.Visibility = Visibility.Visible;
            }
        }


        private void RefreshMemoryListUI()
        {
            MemoryStackPanel.Children.Clear();

            if (memoryList.Count == 0)
            {
                // 메모리가 비어있음을 알리는 텍스트
                MemoryStackPanel.Children.Add(new TextBlock
                {
                    Text = "저장된 메모리가 없습니다.",
                    Margin = new Thickness(10),
                    Foreground = Brushes.Gray
                });
                return;
            }
            foreach (double memoryItem in memoryList)
            {
                // 메모리 목록 항목 UI 생성 (TextBox 또는 Button 형태)
                Button memoryButton = new Button
                {
                    Content = memoryItem.ToString(),
                    FontSize = 18,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(10, 5, 10, 5),
                    Background = Brushes.LightYellow, 
                    Tag = memoryItem 
                };
                MemoryStackPanel.Children.Add(memoryButton);
            }
        }


        private void UpdateProgrammerButtons()
        {
            // 버튼들이 모여있는 UniformGrid를 찾습니다.
            if (ProgrammerButtonGrid == null) return; // XAML에 ProgrammerButtonGrid 이름을 부여하지 않았다면 이 라인을 제거하고 다른 방식으로 접근해야 합니다.

            foreach (UIElement element in ProgrammerButtonGrid.Children)
            {
                if (element is Button button)
                {
                    string content = button.Content.ToString().ToUpper();
                    bool shouldBeEnabled = false;

                    // 숫자 및 알파벳 버튼만 검사하고, 연산자나 기능 버튼은 항상 활성화합니다.
                    // (A-F, 0-9)
                    if (content.Length == 1 && (char.IsLetterOrDigit(content[0])))
                    {
                        if (char.IsDigit(content[0])) // 0-9 숫자 처리
                        {
                            int digit = int.Parse(content);
                            // 현재 진법보다 작은 숫자만 활성화합니다.
                            shouldBeEnabled = (digit < currentBase);
                        }
                        else if (char.IsLetter(content[0])) // A-F 알파벳 처리
                        {
                            char letter = content[0];
                            // 16진수(HEX)일 때만 A, B, C, D, E, F를 활성화합니다.
                            if (currentBase == 16)
                            {
                                shouldBeEnabled = (letter >= 'A' && letter <= 'F');
                            }
                            else
                            {
                                shouldBeEnabled = false;
                            }
                        }
                        button.IsEnabled = shouldBeEnabled;
                    }
                    else
                    {
                        // 연산자, =, CE, DEL 등 나머지 버튼은 항상 활성화
                        if (content != "." && content != "+/-") // 소수점과 부호 반전은 프로그래머 모드에서 보통 비활성화되지만, 일단 허용
                        {
                            button.IsEnabled = true;
                        }
                    }
                }
            }
        }


        private void MemoryButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;

            string memoryOperation = button.Content.ToString();

            if (memoryOperation == "M▽")
            {
                if (MemoryPanel.Visibility == Visibility.Collapsed)
                {
                    RefreshMemoryListUI();
                    MemoryPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    MemoryPanel.Visibility = Visibility.Collapsed;
                }
                HistoryPanel.Visibility = Visibility.Collapsed;
                isSidebarOpen = false; // 사이드바 상태 업데이트
                SidebarPanel.Visibility = Visibility.Collapsed;

                return; 
            }

            if (double.TryParse(ResultDisplay.Text, out double displayValue))
            {
                switch (memoryOperation)
                {
                    case "MC":
                        memoryValue = 0;
                        memoryList.Clear();
                        break;
                    case "MR":
                        ResultDisplay.Text = memoryValue.ToString();
                        isNewEntry = true;
                        break;
                    case "M+":
                        memoryValue += displayValue;
                        break;
                    case "M-":
                        memoryValue -= displayValue;
                        break;
                    case "MS": 
                        memoryValue = displayValue;
                        RefreshMemoryListUI();
                        memoryList.Insert(0, displayValue); // 리스트의 맨 앞에 추가
                        isNewEntry = true;
                        break;
                   
                }
                RefreshMemoryListUI();
                UpdateMemoryButtonState();
            }
            if (memoryOperation != "MR" && memoryOperation != "M▽")
            {
                ExpressionDisplay.Text = "";
                currentOperator = "";
                currentValue = 0;
            }
        }



        private void UpdateMemoryButtonState()
        {
            // 메모리 값이 0이 아니면 MC, MR 버튼 활성화
            bool hasMemory = memoryValue != 0 || memoryList.Count > 0;

            ButtonMR.IsEnabled = hasMemory;
            ButtonMC.IsEnabled = hasMemory;
            if (ButtonMC != null)
                ButtonMC.IsEnabled = hasMemory;

            if (ButtonMR != null)
                ButtonMR.IsEnabled = hasMemory;

            if (ButtonM != null)
            {
                ButtonM.IsEnabled = hasMemory;
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }

}