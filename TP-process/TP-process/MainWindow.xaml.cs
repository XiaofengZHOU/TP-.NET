using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;


namespace TP_process
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<String> processList { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            //list of string binding with list view to show the launched process info
            processList = new ObservableCollection<string>();
            
        }

        

        int num_console = 0; //number of console process lauched
        int num_ballon = 0;  //number of ballon  process lauched
        List<int> premier_list = new List<int>(); //process id list of console 
        List<int> ballons_list = new List<int>(); //process id list of balllon
        List<int> t = new List<int>();


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            
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
            if(process_info.Contains("ballon"))
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
            if(processList.Count>=1)
            {
                String   last_process = processList.Last();
                String[] last_process_split = last_process.Split('\t');
                int pid = Int32.Parse(last_process_split[1]);
                String type = get_type_process(last_process);
                Process p = Process.GetProcessById(pid);
                p.Kill();
                processList.RemoveAt(processList.Count - 1);

                if(type=="ballon")
                {
                    num_ballon--;
                }
                if (type == "premier")
                {
                    num_console--;
                }
            }
            else
            {
                MessageBox.Show("Error: no process to delete");
            }
        }

        //delete the last ballon process launched
        private void dernier_ballon_Click(object sender, RoutedEventArgs e)
        {
            if(num_ballon>0)
            {
                for (int i = processList.Count-1; i >= 0; i--)
                {
                    String process_info = processList[i];
                    String type = get_type_process(process_info);
                    int pid = get_pid_process(process_info);
                    if (type == "ballon")
                    {
                        Process p = Process.GetProcessById(pid);
                        p.Kill();
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
                for (int i = processList.Count-1; i >= 0; i--)
                {
                    String process_info = processList[i];
                    String type = get_type_process(process_info);
                    int pid = get_pid_process(process_info);
                    if (type == "premier")
                    {
                        Process p = Process.GetProcessById(pid);
                        p.Kill();
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
            int count = processList.Count;
            for (int i =0; i < count; i++)
            {
                String process_info = processList[0];
                int pid = get_pid_process(process_info);
                Process p = Process.GetProcessById(pid);
                p.Exited += new EventHandler(myProcess_Exited);
                p.Kill();
                processList.RemoveAt(0);

            }

            num_console = 0;
            num_ballon = 0;
            
        }


        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            tout_Click(sender,e);
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

        // function to be activated when user stop a process by clicking the close button
        private void myProcess_Exited(object sender, System.EventArgs e)
        {
            Process p = (Process)sender;
            delete_from_processList(p.Id);

        }

        //lauch a new console process
        private void Console_Click(object sender, RoutedEventArgs e)
        {
            if(num_console<5)
            {
                var p = new Process();
                p.StartInfo.FileName = "Premier.exe";
                p.EnableRaisingEvents = true;
                p.Exited += new EventHandler(myProcess_Exited);
                p.Start();
                var procId = p.Id;
                premier_list.Add(procId);

                num_console++;
                processList.Add("new process of premier"+ Convert.ToString(num_console) + "\t" +Convert.ToString(procId));
                DataContext = this;
            }
            else
            {
                MessageBox.Show("Error : you can't add new ballon process! You need to kill some process!");
            }
            
        }

        //lauch a new ballon process
        private void Ballon_Click(object sender, RoutedEventArgs e)
        {
            if (num_ballon < 5)
            {
                var p = new Process();
                p.StartInfo.FileName = "Ballon.exe";
                p.EnableRaisingEvents = true;
                p.Exited += new EventHandler(myProcess_Exited);
                p.Start();
                var procId = p.Id;
                ballons_list.Add(procId);

                num_ballon++;
                processList.Add("new process of ballon" + Convert.ToString(num_ballon)+ "\t" + Convert.ToString(procId));
                DataContext = this;


            }
            else
            {
                
                MessageBox.Show("Error : you can't add new ballon process! You need to kill some process!");
                
            }

        }

        
    }

}
