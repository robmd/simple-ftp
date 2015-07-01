Imports System.Net
Imports System.IO

Public Class Form1
    Private Sub listFTP(URL As String, username As String, password As String)
        Dim request As FtpWebRequest = Nothing
        Dim response As FtpWebResponse = Nothing
        Dim reader As StreamReader = Nothing
        Try
            request = CType(WebRequest.Create(URL), WebRequest)
            request.Credentials = New NetworkCredential(username, password)
            request.Method = WebRequestMethods.Ftp.ListDirectory
            response = CType(request.GetResponse(), FtpWebResponse)
            reader = New StreamReader(response.GetResponseStream)
            While reader.Peek > -1
                ListBox1.Items.Add(reader.ReadLine())
            End While
            ToolStripStatusLabel1.Text = "Listing complete!"
        Catch ex As UriFormatException
            ToolStripStatusLabel1.Text = ex.Message
        Catch ex As WebException
            ToolStripStatusLabel2.Text = ex.Message
        Finally
            If reader IsNot Nothing Then reader.Close()
        End Try
    End Sub

    Private Sub downloadFTP(URL As String, username As String, password As String)
        Dim request As FtpWebRequest = Nothing
        Dim response As FtpWebResponse = Nothing
        Dim responseStream As Stream = Nothing
        Dim fileStream As FileStream = Nothing
        Try
            request = CType(WebRequest.Create(URL), FtpWebRequest)
            request.Credentials = New NetworkCredential(username, password)
            request.Method = WebRequestMethods.Ftp.DownloadFile
            response = CType(request.GetResponse(), FtpWebResponse)
            responseStream = response.GetResponseStream
            SaveFileDialog1.FileName = Path.GetFileName(request.RequestUri.LocalPath)
            If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                fileStream = File.Create(SaveFileDialog1.FileName)
                Dim buff(1024) As Byte
                Dim bytesRead As Integer = 0
                While True
                    bytesRead = responseStream.Read(buff, 0, buff.Length)
                    If bytesRead = 0 Then Exit While
                    fileStream.Write(buff, 0, bytesRead)
                End While
                ToolStripStatusLabel1.Text = "Download complete!"
            End If
        Catch ex As UriFormatException
            ToolStripStatusLabel1.Text = ex.Message
        Catch ex As WebException
            ToolStripStatusLabel2.Text = ex.Message
        Catch ex As IOException
            ToolStripStatusLabel2.Text = ex.Message
        Finally
            If responseStream IsNot Nothing Then responseStream.Close()
            If fileStream IsNot Nothing Then fileStream.Close()
        End Try
    End Sub

    Private Sub uploadFTP(URL As String, filename As String, username As String, password As String)
        Dim request As System.Net.FtpWebRequest = DirectCast(WebRequest.Create(URL), FtpWebRequest)
        request.Credentials = New NetworkCredential(username, password)
        request.Method = WebRequestMethods.Ftp.UploadFile

        Dim file() As Byte = System.IO.File.ReadAllBytes(filename)

        Dim strz As System.IO.Stream = request.GetRequestStream()
        strz.Write(file, 0, file.Length)
        strz.Close()
        strz.Dispose()
    End Sub

    Private Sub deleteFTP(URL As String, username As String, password As String)
        Dim request As FtpWebRequest = Nothing
        Dim response As FtpWebResponse = Nothing
        Try
            request = CType(WebRequest.Create(URL), FtpWebRequest)
            request.Credentials = New NetworkCredential(username, password)
            request.Method = WebRequestMethods.Ftp.DeleteFile
            response = CType(request.GetResponse(), FtpWebResponse)
            ToolStripStatusLabel1.Text = "Delete complete!"
        Catch ex As UriFormatException
            ToolStripStatusLabel1.Text = ex.Message
        Catch ex As WebException
            ToolStripStatusLabel2.Text = ex.Message
        Finally
            If response IsNot Nothing Then response.Close()
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ListBox1.Items.Clear()
        listFTP(txtServer.Text, txtUsername.Text, txtPassword.Text)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        downloadFTP(TextBox1.Text, txtUsername.Text, txtPassword.Text)
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        TextBox1.Text = txtServer.Text & "/" & ListBox1.SelectedItems(0).ToString()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim request As System.Net.FtpWebRequest = DirectCast(System.Net.WebRequest.Create(txtServer.Text), System.Net.FtpWebRequest)
            request.Credentials = New System.Net.NetworkCredential("user", "password")
            request.Method = System.Net.WebRequestMethods.Ftp.UploadFile

            Dim file() As Byte = System.IO.File.ReadAllBytes("c:\file.txt")

            Dim strz As System.IO.Stream = request.GetRequestStream()
            strz.Write(file, 0, file.Length)
            strz.Close()
            strz.Dispose()
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If MessageBox.Show("Are you sure you want to delete the selected file?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = Windows.Forms.DialogResult.Yes Then
            deleteFTP(TextBox1.Text, txtUsername.Text, txtPassword.Text)
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        ListBox1.Items.Clear()
        listFTP(txtServer.Text, txtUsername.Text, txtPassword.Text)
    End Sub
End Class
