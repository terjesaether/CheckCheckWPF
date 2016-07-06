using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CheckCheckWPF
{
    public class CreatePanel
    {
        private static int counter;

        public static WrapPanel CreateNewWrapPanel()
        {
            counter++;
            WrapPanel w = new WrapPanel { Name = "myWrapPanel" + counter, Margin = new Thickness(25, 6, 6, 6) };
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

        public static void btn_Click(object sender, EventArgs e)
        {
            Button objButton = (Button)sender;
            WrapPanel objWrap = (WrapPanel)objButton.Parent;
            StackPanel objStackPanel = (StackPanel)objWrap.Parent;

            MessageBox.Show(objWrap.Name.ToString() + " ble fjernet.");

            objStackPanel.Children.Remove(objWrap);


        }
    }
}
