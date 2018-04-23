Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports System.IO
Imports DevExpress.XtraRichEdit
Imports DevExpress.XtraRichEdit.API.Native
Imports System.Windows.Media.Imaging
Imports DevExpress.Office.Utils

Namespace RichEditDocumentServer_SimpleExample
	Partial Public Class MainPage
		Inherits UserControl
		Private richServer As RichEditDocumentServer
		Private imageIndex As Integer = 0
		Private imgs As DocumentImageCollection

		Public Sub New()
			InitializeComponent()

		End Sub

Private Sub button1_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
	Dim ofdlg As New OpenFileDialog()
	ofdlg.Multiselect = False
	ofdlg.Filter = "Word 97-2003 Files (*.doc)|*.doc"
	If ofdlg.ShowDialog() = True Then
		image1.Source = Nothing
		textBlock1.Text = String.Empty
'		#Region "#richserverload"
		richServer = New RichEditDocumentServer()
		richServer.CreateNewDocument()
		Try
			richServer.LoadDocument(ofdlg.File.OpenRead(), DocumentFormat.Doc)
			imgs = richServer.Document.GetImages(richServer.Document.Range)
			If imgs.Count > 0 Then
				ShowCurrentImage()
			End If
			textBlock1.Text = richServer.Document.Text
		Catch ex As Exception
			textBlock1.Text = "Exception occurs:" & Constants.vbLf + ex.Message
		End Try
'		#End Region ' #richserverload

		button2.IsEnabled = True
		AddHandler SimpleAnimation.Completed, AddressOf SimpleAnimaton_Completed
		Me.SimpleAnimation.Begin()
	End If
End Sub

Private Sub SimpleAnimaton_Completed(ByVal sender As Object, ByVal e As EventArgs)
	If imageIndex = imgs.Count-1 Then
	imageIndex = 0
	Else
	imageIndex += 1
	End If

	ShowCurrentImage()

	Me.SimpleAnimation.Begin()
End Sub

		Private Sub button2_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
'			#Region "#richserversave"
			Dim doc As SubDocument = richServer.Document.Sections(0).BeginUpdateFooter()
			Dim docRange As DocumentRange = doc.InsertText(doc.Range.Start, textBox1.Text)
			doc.InsertParagraph(docRange.End)
			richServer.Document.Sections(0).EndUpdateFooter(doc)

			Dim sfdlg As New SaveFileDialog()
			sfdlg.DefaultExt = ".docx"
			sfdlg.Filter = "Word Document (*.docx)|*.docx"
			If sfdlg.ShowDialog() = True Then
				Dim fs As Stream = sfdlg.OpenFile()
				richServer.SaveDocument(fs, DocumentFormat.OpenXml)
				fs.Close()
			End If
'			#End Region ' #richserversave
		End Sub
		Private Sub ShowCurrentImage()
			Dim ms As New MemoryStream(imgs(imageIndex).Image.GetImageBytesSafe(OfficeImageFormat.Png))
			Dim bi As New BitmapImage()
			bi.SetSource(ms)
			image1.Source = bi
			ms.Close()
		End Sub
	End Class
End Namespace
