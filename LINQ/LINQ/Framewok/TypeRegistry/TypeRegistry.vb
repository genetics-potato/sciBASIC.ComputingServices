﻿Imports System.Reflection
Imports Microsoft.VisualBasic.LINQ.Framework.Reflection

Namespace Framework

    ''' <summary>
    ''' Type registry table for loading the external LINQ entity assembly module.
    ''' (起始这个模块就是相当于一个类型缓存而已，因为程序可以直接读取dll文件里面的内容，但是直接读取的方法会造成性能下降，所以需要使用这个对象来缓存所需要的类型数据) 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TypeRegistry : Implements System.IDisposable

        Public Property ExternalModules As List(Of RegistryItem)

        Dim File As String

        Public Overrides Function ToString() As String
            Return File
        End Function

        ''' <summary>
        ''' 返回包含有该类型的目标模块的文件路径
        ''' </summary>
        ''' <param name="Name">LINQ Entity集合中的元素的简称或者别称，即Item中的Name属性</param>
        ''' <returns>If the key is not exists in this object, than the function will return a empty string.</returns>
        ''' <remarks></remarks>
        Public Function FindAssemblyPath(Name As String) As String
            Dim Item = Find(Name)
            If Item Is Nothing Then
                Return ""
            Else
                Return Item.AssemblyFullPath
            End If
        End Function

        ''' <summary>
        ''' Return a registry item in the table using its specific name property.
        ''' (返回注册表中的一个指定名称的项目)
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Find(Name As String) As RegistryItem
            For i As Integer = 0 To ExternalModules.Count - 1
                If String.Equals(Name, ExternalModules(i).Name, StringComparison.OrdinalIgnoreCase) Then
                    Return ExternalModules(i)
                End If
            Next
            Return Nothing
        End Function

        ''' <summary>
        ''' Registry the external LINQ entity assembly module in the LINQFramework
        ''' </summary>
        ''' <param name="AssemblyPath">DLL file path</param>
        ''' <returns></returns>
        ''' <remarks>查询出目标元素的类型定义并获取信息</remarks>
        Public Function Register(AssemblyPath As String) As Boolean
            Dim assm As Assembly = Assembly.LoadFrom(IO.Path.GetFullPath(AssemblyPath)) 'Load external module
            Dim ILINQEntityTypes As TypeInfo() =
                LQueryFramework.LoadAssembly(assm, LINQEntity.ILINQEntity) 'Get type define informations of LINQ entity

            If ILINQEntityTypes.IsNullOrEmpty Then Return False

            Dim LQuery As IEnumerable(Of RegistryItem) =
                    From Type As Type In ILINQEntityTypes
                    Select New RegistryItem With {
                        .Name = LINQEntity.GetEntityType(Type),
                        .AssemblyPath = AssemblyPath,
                        .TypeId = Type.FullName
                    }        'Generate the resitry item for each external type

            For Each Item As RegistryItem In LQuery     'Update exists registry item or insrt new item into the table
                Dim Item2 As RegistryItem = Find(Item.Name)         '在注册表中查询是否有已注册的类型
                If Item2 Is Nothing Then
                    Call ExternalModules.Add(Item)  'Insert new record.(添加数据)
                Else                                'Update exists data.(更新数据)
                    Item2.AssemblyPath = Item.AssemblyPath
                    Item2.TypeId = Item.TypeId
                End If
            Next
            Return True
        End Function

        Public Shared Function Load(Path As String) As TypeRegistry
            If FileIO.FileSystem.FileExists(Path) Then
                Dim TypeRegistry As TypeRegistry = Path.LoadXml(Of TypeRegistry)()
                TypeRegistry.File = Path
                Return TypeRegistry
            Else
                Return New TypeRegistry With {
                    .ExternalModules = New List(Of RegistryItem),
                    .File = Path
                }
            End If
        End Function

        Public Sub Save(Optional Path As String = "")
            If String.IsNullOrEmpty(Path) Then
                Path = Me.File
            End If
            Call Me.GetXml.SaveTo(Path, System.Text.Encoding.Unicode)
        End Sub

        Public Shared Widening Operator CType(Path As String) As TypeRegistry
            Return TypeRegistry.Load(Path)
        End Operator

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  释放托管状态(托管对象)。
                    Call Me.Save()
                End If

                ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO:  将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO:  仅当上面的 Dispose(ByVal disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose(ByVal disposing As Boolean)中。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' Visual Basic 添加此代码是为了正确实现可处置模式。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace