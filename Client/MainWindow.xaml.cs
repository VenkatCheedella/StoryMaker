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
using System.Windows.Forms;
using System.IO;


namespace Client
{
    public class File
    {
        public bool select { get; set; }
        public string fileName { get; set; }
    };
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<File> fil = new List<File>();
        string apiPath = "http://localhost:13103/api/File";

        public MainWindow()
        {
            InitializeComponent();

           // getFiles();
        }

        private void getFiles()
        {
            ClientT c = new ClientT(apiPath);
            string[] files = c.getAvailableFiles();

            fil = new List<File>();

            for (int i = 0; i < files.Length; i++)
            {
                File fi = new File();
                fi.select = false;
                fi.fileName = files[i];
                fil.Add(fi);
            }

           // lvUsers.ItemsSource = fil;
        }
        private void btnUploadfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClientT c = new ClientT(apiPath);

                System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
                ofd.Multiselect = true;
                System.Windows.Forms.DialogResult result = ofd.ShowDialog();
                string[] files = null;
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    files = ofd.FileNames;

                    foreach (var file in files)
                    {
                        c.upLoadFile(file);
                    }

                    System.Windows.MessageBox.Show("Files are uploaded");
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show("A handled exception just occurred: " + ex.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnDownloadfile_Click(object sender, RoutedEventArgs e)
        {
            ClientT c = new ClientT(apiPath);

            foreach (File file in fil)
            {
                if (file.select == true)
                {
                    c.downLoadFile(file.fileName);
                    file.select = false;
                }
            }

            getFiles();

            System.Windows.MessageBox.Show("Files are downloaded");
        }
    }
}
