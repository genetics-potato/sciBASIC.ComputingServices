﻿Imports System.IO
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Protocol
Imports Microsoft.VisualBasic.Net.Protocol.Reflection
Imports Microsoft.VisualBasic.Serialization

Namespace FileSystem.Protocols

    Public Class FileURI

        Public Property EntryPoint As IPEndPoint
        Public Property File As String

        Sub New(uri As String)
            Dim addr As String = Regex.Match(uri, "^\d+@\d+(\.\d+){3}[:]\/\/").Value
            If String.IsNullOrEmpty(addr) Then  '本地文件
                File = uri
                EntryPoint = Nothing
            Else ' 远程文件
                File = uri.Replace(addr, "")
                Dim Tokens As String() = addr.Split(":"c).First.Split("@"c)
                EntryPoint = New IPEndPoint(Tokens(1), Scripting.CTypeDynamic(Of Integer)(Tokens(Scan0)))
            End If
        End Sub

        Sub New(path As String, portal As IPEndPoint)
            File = path
            EntryPoint = portal
        End Sub

        Sub New()
        End Sub

        ''' <summary>
        ''' {port}@{ipaddress}://C:\xxx\xxx.file
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            If EntryPoint Is Nothing Then
                Return File
            Else
                Return $"{EntryPoint.Port}@{EntryPoint.IPAddress}://{File}"
            End If
        End Function
    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    Public Module API

        Public ReadOnly Property ProtocolEntry As Long =
            New Protocol(GetType(FileSystemAPI)).EntryPoint

        Public Function OpenHandle(file As String, mode As FileMode, access As FileAccess) As RequestStream
            Dim params As New FileOpen With {
                .FileName = file,
                .Mode = mode,
                .Access = access
            }
            Return RequestStream.CreateProtocol(ProtocolEntry, FileSystemAPI.OpenHandle, params)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pos">-100表示set</param>
        ''' <param name="handle"></param>
        ''' <returns></returns>
        Public Function GetSetReadPosition(pos As Long, handle As FileHandle) As RequestStream
            Dim args As New FileStreamPosition(handle) With {.Position = pos}
            Return RequestStream.CreateProtocol(ProtocolEntry, FileSystemAPI.FilePosition, args)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="handle"></param>
        ''' <returns></returns>
        Public Function GetSetLength(length As Long, handle As FileHandle) As RequestStream
            Dim args As New FileStreamPosition(handle) With {.Position = length}
            Return RequestStream.CreateProtocol(ProtocolEntry, FileSystemAPI.FileStreamLength, args)
        End Function
    End Module
End Namespace