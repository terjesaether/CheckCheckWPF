using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CheckCheckWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int counter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnRescanFolder_Click(object sender, RoutedEventArgs e)
        {
            
            WrapPanel newWrapPanel = CreateNewWrapPanel();
            spShowResult.Children.Add(newWrapPanel);
            
        }

        private WrapPanel CreateNewWrapPanel()
        {
            counter++;
            WrapPanel w = new WrapPanel { Name = "myWrapPanel" + counter, Margin = new Thickness(6) };
            TextBlock tb = new TextBlock { Text = "Dette er tekst nummer: " + counter, Name = "textblock" + counter };
            Button btn = new Button
            {
                Content = "Fjern rad nummer " + counter,
                Name = "button" + counter,
                Margin = new Thickness(10, 0, 10, 0)
            };
            CheckBox chk = new CheckBox { Content = "Dette er sjekkboks: " + counter, Name = "checkbox" + counter };

            // Event for klikk
            btn.Click += btn_Click;

            w.Children.Add(tb);
            w.Children.Add(btn);
            w.Children.Add(chk);

            return w;
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Button objButton = (Button)sender;
            WrapPanel objWrap = (WrapPanel)objButton.Parent;

            MessageBox.Show(objWrap.Name.ToString() + " ble fjernet.");

            spShowResult.Children.Remove(objWrap);


        }

        private void btnCheckAllEps_Click(object sender, RoutedEventArgs e)
        {
            lboxShowFiles.Items.Add("Ny tekst");
        }
    }
}
