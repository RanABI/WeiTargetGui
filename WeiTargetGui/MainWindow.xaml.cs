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
using System.Windows.Threading;
using System.Threading;
using WeiTargetGui.Component;
using WeiTargetGui.Server;
using WeiTargetGui.EventArgs;
using System.ComponentModel;

namespace WeiTargetGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int hitCount;
        Target instance = Target.Instance;
        public static int count;


        Thread checkConnectionThread;
        private double updateGuiInterval = 300 * 3.0; // every 60 sec
        public static System.Timers.Timer Timer = null;
        public static bool flag = false;

        public MainWindow()
        {
            InitializeComponent();
            TCPServer server = new TCPServer();
            count = 0;
            ServerRequestHandler.changeButtonEvent += changeButton;
            try
            {
                Timer = new System.Timers.Timer(updateGuiInterval);
                Timer.Enabled = true;
                Timer.Elapsed += new System.Timers.ElapsedEventHandler(updateGUI);
                Timer.AutoReset = true;
                Timer.Start();
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
            }
            checkConnectionThread = new Thread(checkConnection);
            checkConnectionThread.Start();

        }


        //Change the visability of a button uppon a target hit
        public void changeButton(Object sender, ChangeButtonEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.readyButton.Visibility = Visibility.Hidden;
            });
            Thread.Sleep(350);
            this.Dispatcher.Invoke(() =>
            {
                this.readyButton.Visibility = Visibility.Visible;
            });
        }

        //Update GUI parameters
        internal void updateGUI(object Sender, System.Timers.ElapsedEventArgs e)
        {

            this.Dispatcher.Invoke(() =>
            {
                this.targetId.Text = instance.id.ToString();
            });

            this.Dispatcher.Invoke(() =>
            {
                this.totalHits.Text = instance.totalHitCount.ToString();
            });

            this.Dispatcher.Invoke(() =>
            {
                this.currentHits.Text = instance.currentHitCount.ToString();
            });
        }

        //Reset the current score counter
        private void Reset_cur_score_Click(object sender, RoutedEventArgs e)
        {
            instance.currentHitCount = 0;
            this.Dispatcher.Invoke(() =>
            {
                this.currentHits.Text = instance.currentHitCount.ToString();
            });
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (yesNoMessageBox("בחרת ביציאה. להמשך לחץ כן"))
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                Environment.Exit(0);

            }

            else return;
        }

        public static bool yesNoMessageBox(string msg)
        {
            return System.Windows.MessageBox.Show(msg, "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

        //Disallow the closing of the program window from the top right 'X' sign - ensure proper close to the system
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
        }

        public void checkConnection()
        {
            //Check target's connection
            //If the target has not made a connection in x time the button connection button will be colored in red, else it will remain green.
            while (true)
            {
                DateTime now = DateTime.Now;
                DateTime last = Target.Instance.lastHitTime;
                TimeSpan span = now.Subtract(last);
                
                if(span.Seconds <  3)
                {
                    
                    this.Dispatcher.Invoke(() =>
                    {
                        this.connectionButton.Background = Brushes.Green; 
                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.connectionButton.Background = Brushes.Red;
                    });
                }
                Thread.Sleep(500);
            }
        }
    }
}
