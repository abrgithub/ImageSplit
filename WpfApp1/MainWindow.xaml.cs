using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            splittedLabel.Visibility = System.Windows.Visibility.Hidden;
            retrievedLabel.Visibility = System.Windows.Visibility.Hidden;
        }

        
        

        private void Split_Click(object sender, RoutedEventArgs e)
        {
            getImagesDetails(out string rootPath, out string fileName);
            Bitmap originalImage = new Bitmap(System.Drawing.Image.FromFile(fileName));
            int xAxis = 0, yAxis = 0;
            int imgWidth = originalImage.Width / 4;
            int imgHeight = originalImage.Height / 4;
            Bitmap tempImage = originalImage;
            for (int i=1;i<=4;i++)
            {
                xAxis = 0;
                for (int j = 1; j <= 4; j++)
                {
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(xAxis, yAxis, imgWidth, imgHeight);
                    tempImage= originalImage.Clone(rect, originalImage.PixelFormat);
                    fileName= System.IO.Path.Combine(rootPath, "part_"+i+j+".jpg");
                    tempImage.Save(fileName);
                    xAxis = xAxis + imgWidth;
                }
                yAxis = yAxis + imgHeight;
            }
            splittedLabel.Visibility = System.Windows.Visibility.Visible;
            split.Source=BitmapToImageSource(originalImage);
            System.Diagnostics.Process.Start(rootPath);
        }
        public System.Drawing.Image DownloadImage(string imageUrl)
        {
            System.Drawing.Image image = null;

            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(imageUrl);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;
                System.Net.WebResponse webResponse = webRequest.GetResponse();
                System.IO.Stream stream = webResponse.GetResponseStream();
                image = System.Drawing.Image.FromStream(stream);
                webResponse.Close();
            }
            catch (Exception ex)
            {
                return null;
            }
            
            return image;
        }

        private void Combine_Click(object sender, RoutedEventArgs e)
        {

            getImagesDetails(out string rootPath, out string fileName);
            Bitmap originalImage = new Bitmap(System.Drawing.Image.FromFile(fileName));
            int imgWidth = originalImage.Width/4;
            int imgHeight = originalImage.Height/4;
            int xAxis = 0, yAxis = 0;
            Bitmap bmp;
            using (bmp = new Bitmap(originalImage.Width, originalImage.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        xAxis = 0;
                        for (int j = 1; j <= 4; j++)
                        {
                            fileName = System.IO.Path.Combine(rootPath, "part_" + i + j + ".jpg");
                            System.Drawing.Image tempImage= new Bitmap(System.Drawing.Image.FromFile(fileName));
                            g.DrawImage(tempImage, xAxis, yAxis, imgWidth, imgHeight);
                            xAxis = xAxis + imgWidth;
                        }
                        yAxis = yAxis + imgHeight;
                    }

                }
                fileName = System.IO.Path.Combine(rootPath, "CombinedImage.jpg");
                bmp.Save(fileName);
                retrievedLabel.Visibility = System.Windows.Visibility.Visible;
                combine.Source = BitmapToImageSource(bmp);
            }
            System.Diagnostics.Process.Start(rootPath);

        }

        private void getImagesDetails(out string rootPath,out string fileName)
        {

            System.Drawing.Image image = DownloadImage("https://wallpapertag.com/wallpaper/full/0/2/1/316357-free-download-trump-wallpaper-2048x1365-hd-for-mobile.jpg");
            rootPath = @"C:\SplitImage";
            if (!System.IO.Directory.Exists(rootPath))
                System.IO.Directory.CreateDirectory(rootPath);

            fileName = System.IO.Path.Combine(rootPath, "Retrieved.jpg");
            image.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            
        }
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
