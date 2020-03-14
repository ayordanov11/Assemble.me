using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Dropbox.Api;
using Dropbox.Api.Files;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Assemble.me
{
    /// <summary>
    /// Interaction logic for DropBoxGui.xaml
    /// </summary>
    public partial class DropBoxGui : Window
    {
        public readonly DropboxClient dbx;
        public  Window1 main;

        public DropBoxGui(DropboxClient db, Window1 mainwindow)
        {
            InitializeComponent();
            this.dbx = db;
            this.main = mainwindow;
        }

        /// <summary>
        /// Loads an icon by a provided path.
        /// </summary>
        /// <param name="item">File name.</param>
        /// <returns></returns>
        private BitmapImage LoadIcon(string item)
        {
            if(item == "folder")
                return new BitmapImage(new Uri("icos/folder.png", UriKind.Relative));
            else if (item == "file")
                return new BitmapImage(new Uri("pack://application:,,,/"));
            else
                return null;       
        }

        /// <summary>
        /// Provides the folders and files to the gui.
        /// </summary>
        /// <param name="path">Path to the folder.</param>
        private async void GetContent(string path)
        {
            try
            {
                var list = await this.dbx.Files.ListFolderAsync(path);
                DropBoxLB.Items.Clear();
                foreach (var item in list.Entries.Where(i => i.IsFolder))
                {
                    DropBoxLB.Items.Add(new ListItems(item,"folder"));
                }
                foreach (var item in list.Entries.Where(i => i.IsFile))
                {
                    DropBoxLB.Items.Add(new ListItems(item,"file"));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Upload method for a file with content.
        /// </summary>
        /// <param name="dbx">The context of the dropbox client.</param>
        /// <param name="folder">The folder which is going to be created.</param>
        /// <param name="file">The name and the extension of the file.</param>
        /// <param name="content">The content of the file.</param>
        private async Task Upload(DropboxClient dbx, string folder, string file, string content)
        {
            using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                var updated = await dbx.Files.UploadAsync(
                    folder + "/" + file,
                    WriteMode.Overwrite.Instance,
                    body: mem);
                DataHelper.Success("Item was successfully uploaded.");
            }
            DataHelper.Fail("Uploading failed. Please, try again!");
        }

        /// <summary>
        /// Upload method with provided file.
        /// </summary>
        /// <param name="localPath">local file</param>
        /// <param name="remotePath">remote file</param>
        public async Task Upload(string localPath, string remotePath)
        {
            const int ChunkSize = 4096 * 1024;
            bool flag = false;
            using (var fileStream = File.Open(localPath, FileMode.Open))
            {
                if (fileStream.Length <= ChunkSize)
                {
                    await this.dbx.Files.UploadAsync(remotePath, body: fileStream);
                    DataHelper.Success("Item was successfully uploaded.");
                    flag = true;
                }
                else
                {
                    await this.ChunkUpload(remotePath, fileStream, ChunkSize);
                    DataHelper.Success("Item was successfully uploaded.");
                    flag = true;
                }
            }
            if(!flag)
                DataHelper.Fail("Uploading failed. Please, try again!");
        }

        /// <summary>
        /// Creates a folder in the Dropbox cloud. If the folder already exists,
        /// nothing happens.
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        public async Task CreateDirectory(string path)
        {
            try
            {
                // gets contents at requested path to check if a folder exists
                var metaData = await dbx.Files.GetMetadataAsync(path);
            }
            catch (Exception exc)
            {
                if (exc.Message.Contains("path/not_found"))
                {
                    // folder does not exist
                    dbx.Files.CreateFolderAsync(path);
                }
            }
        } 

        /// <summary>
        /// Helper method for Upload.
        /// </summary>
        private async Task ChunkUpload(String path, FileStream stream, int chunkSize)
        {
            int numChunks = (int)Math.Ceiling((double)stream.Length / chunkSize);
            byte[] buffer = new byte[chunkSize];
            string sessionId = null;
            bool flag = false;
            for (var idx = 0; idx < numChunks; idx++)
            {
                var bytesRead = stream.Read(buffer, 0, chunkSize);

                using (var memStream = new MemoryStream(buffer, 0, bytesRead))
                {
                    if (idx == 0)
                    {
                        var result = await this.dbx.Files.UploadSessionStartAsync(false, memStream);
                        sessionId = result.SessionId;
                    }
                    else
                    {
                        var cursor = new UploadSessionCursor(sessionId, (ulong)((uint)chunkSize * (uint)idx));

                        if (idx == numChunks - 1)
                        {
                            await this.dbx.Files.UploadSessionFinishAsync(cursor, new CommitInfo(path), memStream);
                        }
                        else
                        {
                            await this.dbx.Files.UploadSessionAppendV2Async(cursor, false, memStream);
                        }
                    }
                    flag = true;
                    DataHelper.Success("Item was successfully uploaded.");
                }
                
            }
            if(!flag)
                DataHelper.Fail("Uploading failed. Please, try again!");
        }

        private void DropBoxLB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DropBoxLB.SelectedIndex != -1)
            {
                //Assemble.me.ListItems c = (Assemble.me.ListItems) DropBoxLB.SelectedItems[0];
                //string b = c.Message;
                //DropBoxLB.Items.Clear();
                //CurrPath.Content = CurrPath.Content + b + "/";
                //GetContent(CurrPath.Content.ToString());
            }
        }

        private async void Download_Click(object sender, RoutedEventArgs e)
        {
            if (DropBoxLB.SelectedIndex != -1)
            {
                Assemble.me.ListItems c = (Assemble.me.ListItems)DropBoxLB.SelectedItems[0];
                string file = c.Message;
                await Download(dbx, CurrPath.Content.ToString(), file);
            }
        }

        /// <summary>
        /// Downloads the specified file from the specified folder from the cloud.
        /// </summary>
        /// <param name="dbx">The context of the dropbox client.</param>
        /// <param name="folder">The folder containing the file.</param>
        /// <param name="file">The name and the extension of the file.</param>
        /// <returns></returns>
        async Task Download(DropboxClient dbx, string folder, string file)
        {
            try
            {
                bool flag = false;
                var response = await dbx.Files.DownloadAsync(folder + file);

                Directory.CreateDirectory("../../3dsModelsTemp");

                using (var fileStream = File.Create("../../3dsModelsTemp/" + file))
                {
                    (await response.GetContentAsStreamAsync()).CopyTo(fileStream);
                    flag = true;
                }
                if (flag)
                {
                    Connector.LoadModelFromCloud("../../3dsModelsTemp/" + file, main);
                    DataHelper.Success("Item was successfully downloaded.");
                    this.Hide();
                }
                else
                {
                    DataHelper.Fail("Download failed. Please, try again!");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                DataHelper.Fail("Download failed. Please, try again!");
            }
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Create the user folder upon form initializiation
            await CreateDirectory(@"/" + Connector.GetCurrentCustomer().ID);

            CurrPath.Content = "/" + Connector.GetCurrentCustomer().ID + "/";
            GetContent(@"/" + Connector.GetCurrentCustomer().ID+ "/");
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DropBoxLB.Items.Clear();
            CurrPath.Content = "/" + Connector.GetCurrentCustomer().ID+"/";
            GetContent(@"/" + Connector.GetCurrentCustomer().ID + "/");
        }
    }
}
