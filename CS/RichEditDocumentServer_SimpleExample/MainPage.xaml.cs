using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System.Windows.Media.Imaging;
using DevExpress.XtraRichEdit.Utils;

namespace RichEditDocumentServer_SimpleExample
{
    public partial class MainPage : UserControl
    {
        RichEditDocumentServer richServer;
        int imageIndex = 0;
        DocumentImageCollection imgs;

        public MainPage()
        {
            InitializeComponent();

        }

private void button1_Click(object sender, RoutedEventArgs e)
{
    OpenFileDialog ofdlg = new OpenFileDialog();
    ofdlg.Multiselect = false;
    ofdlg.Filter = "Word 97-2003 Files (*.doc)|*.doc";
    if (ofdlg.ShowDialog() == true) {
        image1.Source = null;
        textBlock1.Text = string.Empty;
        #region #richserverload
        richServer = new RichEditDocumentServer();
        richServer.CreateNewDocument();
        try {
            richServer.LoadDocument(ofdlg.File.OpenRead(), DocumentFormat.Doc);
            imgs = richServer.Document.GetImages(richServer.Document.Range);
            if (imgs.Count > 0) {
                ShowCurrentImage();
            }
            textBlock1.Text = richServer.Document.Text;
        }
        catch (Exception ex) {
            textBlock1.Text = "Exception occurs:\n" +  ex.Message;
        }
        #endregion #richserverload

        button2.IsEnabled = true;
        this.SimpleAnimation.Completed += new EventHandler(SimpleAnimaton_Completed);
        this.SimpleAnimation.Begin();
    }
}

void SimpleAnimaton_Completed(object sender, EventArgs e)
{
    if (imageIndex == imgs.Count-1)
    imageIndex = 0;
    else 
    imageIndex ++;

    ShowCurrentImage();

    this.SimpleAnimation.Begin();
}

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            #region #richserversave
            SubDocument doc = richServer.Document.Sections[0].BeginUpdateFooter();
            DocumentRange docRange = doc.InsertText(doc.Range.Start, textBox1.Text);
            doc.InsertParagraph(docRange.End);
            richServer.Document.Sections[0].EndUpdateFooter(doc);

            SaveFileDialog sfdlg = new SaveFileDialog();
            sfdlg.DefaultExt = ".docx";
            sfdlg.Filter = "Word Document (*.docx)|*.docx";
            if (sfdlg.ShowDialog() == true) {
                Stream fs = sfdlg.OpenFile();
                richServer.SaveDocument(fs, DocumentFormat.OpenXml);
                fs.Close();
            }
            #endregion #richserversave
        }
        private void ShowCurrentImage()
        {
            MemoryStream ms = new MemoryStream(imgs[imageIndex].Image.GetImageBytesSafe(RichEditImageFormat.Png));
            BitmapImage bi = new BitmapImage();
            bi.SetSource(ms);
            image1.Source = bi;
            ms.Close();
        }
    }
}
