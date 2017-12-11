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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using WpfAppliTh;
using ClassLibraryPremier;


namespace TP_thread
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ManualResetEvent _event = new ManualResetEvent(true);
        public ObservableCollection<String> processList { get; set; }
        public List<Thread> threads = new List<Thread>();
        public MainWindow()
        {
            processList = new ObservableCollection<string>();
            InitializeComponent();
            DataContext = this;
        }

        int num_console = 0; //number of console process lauched
        int num_ballon = 0;  //number of ballon  process lauched
        Boolean flag_pause = false;

        //lauch a new console process
        private void Console_Click(object sender, RoutedEventArgs e)
        {
            //Console.SetWindowSize(20, 30);
            Thread th = new Thread(new ThreadStart(ClassLibraryPremier.ClassPremier.ThreadFunction));
            th.IsBackground = true;
            th.Start();
            num_console++;
            processList.Add("new process of premier" + Convert.ToString(num_console) + "\t" + Convert.ToString(th.ManagedThreadId));
            threads.Add(th);
        }

        //lauch a new ballon process
        private void Ballon_Click(object sender, RoutedEventArgs e)
        {
            Thread th = new Thread(delegate () 
            {
                Window window = new WindowBallon();
                window.Show();
                window.Closed += (s, evenetment) =>
                {
                    System.Windows.Threading.Dispatcher.ExitAllFrames();
                    Ballon_exit();
                    
                };
                System.Windows.Threading.Dispatcher.Run();

            });
            th.SetApartmentState(ApartmentState.STA);;
            th.Start();
            num_ballon++;
            processList.Add("new process of ballon" + Convert.ToString(num_ballon) + "\t" + Convert.ToString(th.ManagedThreadId));
            threads.Add(th);
        }



        private void Ballon_exit()
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                for (int i = 0; i < processList.Count; i++)
                {
                    if (processList[i].Contains(Convert.ToString(id)))
                    {
                        processList.RemoveAt(i);
                        threads.RemoveAt(i);
                        break;
                    }
                }
            });
            
        }

        //get process id using the infomation of a process
        public int get_pid_process(String process_info)
        {
            String[] last_process_split = process_info.Split('\t');
            int pid = Int32.Parse(last_process_split[1]);
            return pid;
        }

        //get process type(ballon or console) using the infomation of a process
        public String get_type_process(String process_info)
        {
            if (process_info.Contains("ballon"))
            {
                return "ballon";
            }
            if (process_info.Contains("premier"))
            {
                return "premier";
            }
            return "error";

        }

        //delete the last process launched
        private void dernier_Click(object sender, RoutedEventArgs e)
        {
            if (processList.Count >= 1)
            {
                String last_process = processList.Last();
                String[] last_process_split = last_process.Split('\t');
                int pid = Int32.Parse(last_process_split[1]);
                String type = get_type_process(last_process);
                if (type == "ballon")
                {
                    num_ballon--;
                }
                if (type == "premier")
                {
                    num_console--;
                }
                Thread th = threads[threads.Count - 1];
                th.Abort();
                threads.RemoveAt(threads.Count - 1);
                processList.RemoveAt(processList.Count - 1);
            }
            else
            {
                MessageBox.Show("Error: no process to delete");
            }
        }

        //delete the last ballon process launched
        private void dernier_ballon_Click(object sender, RoutedEventArgs e)
        {
            if (num_ballon > 0)
            {
                for (int i = processList.Count - 1; i >= 0; i--)
                {
                    String process_info = processList[i];
                    String type = get_type_process(process_info);
                    int pid = get_pid_process(process_info);
                    if (type == "ballon")
                    {
                        threads[i].Abort();
                        threads.RemoveAt(i);
                        processList.RemoveAt(i);
                        num_ballon--;
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Error: no ballon process to delete");
            }


        }

        //delete the last console process launched
        private void dernier_premier_Click(object sender, RoutedEventArgs e)
        {
            if (num_console > 0)
            {
                for (int i = processList.Count - 1; i >= 0; i--)
                {
                    String process_info = processList[i];
                    String type = get_type_process(process_info);
                    int pid = get_pid_process(process_info);
                    if (type == "premier")
                    {
                        threads[i].Abort();
                        threads.RemoveAt(i);
                        processList.RemoveAt(i);
                        num_console--;
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Error: no ballon process to delete");
            }

        }

        //delete all the process launched
        private void tout_Click(object sender, RoutedEventArgs e)
        {
            foreach (Thread th in threads)
            {
                th.Abort();
            }

            num_console = 0;
            num_ballon = 0;
            threads.Clear();
            processList.Clear();            

        }

        //pause and restore the threads
        private void Pause_Restore(object sender, RoutedEventArgs e)
        {
            if (flag_pause == false)
            {
                foreach (Thread th in threads)
                {
                    
                    th.Suspend();
                    
                }
                flag_pause = !flag_pause;
            }
            else 
            {
                foreach (Thread th in threads)
                {
                    th.Suspend();
                    th.Resume();
                    
                }
                flag_pause = !flag_pause;
            }
        }

        //quit application
        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            tout_Click(sender, e);
            this.Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //change the list view after deleting a process
        private void delete_from_processList(int pid)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                int count = processList.Count;
                for (int i = 0; i < count; i++)
                {
                    String process_info = processList[i];
                    if (process_info.Contains(Convert.ToString(pid)))
                    {
                        String type = get_type_process(process_info);
                        processList.Remove(process_info);
                        if (type == "ballon")
                        {
                            num_ballon--;
                        }
                        if (type == "premier")
                        {
                            num_console--;
                        }
                    }
                }
            });
        }


    }
}

