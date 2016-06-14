﻿Imports System.Reflection
Imports Microsoft.VisualBasic.ComputingServices.ComponentModel
Imports Microsoft.VisualBasic.ComputingServices.FileSystem
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Net.Http

Namespace TaskHost

    <Protocol(GetType(TaskProtocols))>
    Public Class TaskInvoke : Inherits IHostBase
        Implements IRemoteSupport
        Implements IDisposable

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="port">You can suing function <see cref="GetFirstAvailablePort"/> to initialize this server object.</param>
        Sub New(Optional port As Integer = 1234)
            Call MyBase.New(port)
            __host.Responsehandler = AddressOf New ProtocolHandler(Me).HandleRequest
            FileSystem = New FileSystemHost(GetFirstAvailablePort)
        End Sub

        Public Function Run() As Integer
            Return __host.Run()
        End Function

        Public Overrides ReadOnly Property Portal As IPEndPoint
            Get
                Return Me.GetPortal
            End Get
        End Property

        Public ReadOnly Property FileSystem As FileSystemHost Implements IRemoteSupport.FileSystem
        Public ReadOnly Property LinqProvider As LinqPool = New LinqPool

        ''' <summary>
        ''' Invoke the function on the remote server.(远程服务器上面通过这个方法执行函数调用)
        ''' </summary>
        ''' <param name="params"></param>
        ''' <returns></returns>
        Public Shared Function Invoke(params As InvokeInfo) As Rtvl
            Dim rtvl As Rtvl

            Try
                Dim rtvlType As Type = Nothing
                Dim value As Object = __invoke(params, rtvlType)
                rtvl = New Rtvl(value, rtvlType)
            Catch ex As Exception
                ex = New Exception(params.GetJson, ex)
                rtvl = New Rtvl(ex)
            End Try

            Return rtvl
        End Function

        ''' <summary>
        ''' A common function of invoke the method on the remote machine
        ''' </summary>
        ''' <param name="params">远程主机上面的函数指针</param>
        ''' <param name="value">value's <see cref="system.type"/></param>
        ''' <returns></returns>
        Private Shared Function __invoke(params As InvokeInfo, ByRef value As Type) As Object
            Dim func As MethodInfo = params.GetMethod
            Dim paramsValue As Object() = params.Parameters.ToArray(Function(arg) arg.GetValue)
            Dim x As Object = func.Invoke(Nothing, paramsValue)
            value = func.ReturnType
            Return x
        End Function

        Public Shared Function TryInvoke(info As InvokeInfo) As Object
            Return __invoke(info, Nothing)
        End Function

        <Protocol(TaskProtocols.Invoke)>
        Private Function Invoke(CA As Long, args As RequestStream, remote As System.Net.IPEndPoint) As RequestStream
            Dim params As InvokeInfo = Serialization.LoadObject(Of InvokeInfo)(args.GetUTF8String)
            Dim value As Rtvl = Invoke(params)
            Return New RequestStream(value.GetJson)
        End Function

        <Protocol(TaskProtocols.Free)>
        Private Function Free(CA As Long, args As RequestStream, remote As System.Net.IPEndPoint) As RequestStream
            Dim uid As String = args.GetUTF8String
            Call LinqProvider.Free(uid)
            Return NetResponse.RFC_OK  ' HTTP/200
        End Function

        ''' <summary>
        ''' 执行远程Linq代码
        ''' </summary>
        ''' <param name="CA">SSL证书编号</param>
        ''' <param name="args"></param>
        ''' <param name="remote"></param>
        ''' <returns></returns>
        <Protocol(TaskProtocols.InvokeLinq)>
        Private Function InvokeLinq(CA As Long, args As RequestStream, remote As System.Net.IPEndPoint) As RequestStream
            Dim params As InvokeInfo = Serialization.LoadObject(Of InvokeInfo)(args.GetUTF8String) ' 得到远程函数指针信息
            Dim type As Type = Nothing
            Dim value As Object = __invoke(params, type)
            Dim source As IEnumerable = DirectCast(value, IEnumerable)
            Dim svr As String = LinqProvider.OpenQuery(source, type).GetJson   ' 返回数据源信息
            Return New RequestStream(svr)
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call _LinqProvider.Free
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace