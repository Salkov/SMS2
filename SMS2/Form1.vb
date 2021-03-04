Imports System.IO
Imports System.IO.Ports
Imports System.Text

Public Class Form1
    Dim SerialPort1 As New SerialPort
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Skicka.Click
        Try
            Dim Result As String
            Dim Telefonnummer As String = "+46730383163"
            Dim meddelande As String = "hål öl gäle"
            Dim ArrayOFBytes() As Byte = System.Text.Encoding.Unicode.GetBytes(meddelande)
            Dim UTF16 As String
            Dim v As Integer
            For v = 0 To ArrayOFBytes.Length - 1
                If v Mod 2 = 0 Then
                    Dim t As Integer = ArrayOFBytes(v)
                    ArrayOFBytes(v) = ArrayOFBytes(v + 1)
                    ArrayOFBytes(v + 1) = t
                End If
            Next
            For v = 0 To ArrayOFBytes.Length - 1
                Dim c As String = Hex$(ArrayOFBytes(v))
                If c.Length = 1 Then
                    c = "0" & c
                End If
                UTF16 = UTF16 & c
            Next
            'While meddelande.Length > 0
            '    sVal = Conversion.Hex(Strings.Asc(meddelande.Substring(0, 1).ToString()))
            '    meddelande = meddelande.Substring(1, meddelande.Length - 1)
            '    sHex &= sVal
            'End While
            With SerialPort1
                .WriteLine("AT" & vbCrLf)
                .WriteLine("AT+CSMP=1,167,0,8" & vbCrLf)
                .WriteLine("AT+CSCS=""HEX""" & Chr(34) & vbCrLf)
                .WriteLine("AT+CMGS=" & Telefonnummer & vbCr) 'sender ko no. rakhne ho tyo txtnumber ma 
                .WriteLine(UTF16 & vbCrLf & Chr(26)) 'txtmessage automatic huna parchha haina?
            End With
            Dim Input As String = ReadResponse(SerialPort1, 400)


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Public Function ReadResponse(ByVal Port As SerialPort, ByVal TimeOut As Integer)

        Dim Buff As String = ""
        Try
            Do
                Dim t As String = Port.ReadExisting()
                Buff = Buff & t
            Loop Until Buff.EndsWith(vbCrLf & "Ok" & vbCrLf) Or Buff.EndsWith(vbCrLf & "> ") Or Buff.EndsWith(vbCrLf & "ERROR" & vbCrLf)

            RichTextBox1.Text = Buff
        Catch ex As Exception
            Throw ex
        End Try
        Return Buff
    End Function
    Private Sub Anslut_Click(sender As Object, e As EventArgs) Handles Anslut.Click
        If SerialPort1.IsOpen Then
            SerialPort1.Close()
            Anslut.Text = "Anslut"
        Else
            Try
                With SerialPort1
                    .PortName = "COM1"
                    .BaudRate = 115200
                    .Parity = IO.Ports.Parity.None
                    .DataBits = 8
                    .StopBits = Ports.StopBits.One
                    .Handshake = Ports.Handshake.None
                    .RtsEnable = True
                    .DtrEnable = True
                    .Open()
                    .WriteLine("AT+CNMI=1,2,0,0,0" & vbCrLf) 'send whatever data that it receives to serial port
                End With
                Anslut.Text = "Disconnect"
            Catch ex As Exception
                Anslut.Text = "Anslut"
                MsgBox(ex.Message)
            End Try
        End If
    End Sub
    Public Function StrToHex(ByRef meddelande As String) As String
        Dim sVal As String
        Dim sHex As String = ""
        While meddelande.Length > 0
            sVal = Conversion.Hex(Strings.Asc(meddelande.Substring(0, 1).ToString()))
            meddelande = meddelande.Substring(1, meddelande.Length - 1)
            sHex = sHex & sVal
        End While
        Return sHex
    End Function
    Public Function ConvertToUTF16(ByVal meddelande As String) As String
        Dim ArrayOFBytes() As Byte = System.Text.Encoding.Unicode.GetBytes(meddelande)
        Dim UTF16 As String
        Dim v As Integer
        For v = 0 To ArrayOFBytes.Length - 1
            If v Mod 2 = 0 Then
                Dim t As Integer = ArrayOFBytes(v)
                ArrayOFBytes(v) = ArrayOFBytes(v + 1)
                ArrayOFBytes(v + 1) = t
            End If
        Next
        For v = 0 To ArrayOFBytes.Length - 1
            Dim c As String = Hex$(ArrayOFBytes(v))
            If c.Length = 1 Then
                c = "0" & c
            End If
            UTF16 = UTF16 & c
        Next
        meddelande = UTF16
        Return meddelande
    End Function

End Class
