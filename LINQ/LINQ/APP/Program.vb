﻿Imports Microsoft.VisualBasic.LINQ.Framework.Provider

Module Program

    ''' <summary>
    ''' DO_NOTHING
    ''' </summary>
    ''' <remarks></remarks>
    Public Function Main() As Integer

        Dim s As String = "From var As Type In ""$source->parallel"" Let x = var -> aa(d,""g ++ "") Let y as string = var -> aa(d,""g ++ "") where x -> test2(test3(xx),var) is true Let x = var -> aa(d,""g ++ "") Let x = var -> aa(d,""g ++ "") Let x = var -> aa(d,""g ++ "") select new varType(var,x), x+3"
        Dim expr = LINQ.Statements.LINQStatement.TryParse(s)

        Dim r As TypeRegistry = TypeRegistry.LoadDefault
        Dim h = r.GetHandle("typedef")
        Dim c = h("E:\Microsoft.VisualBasic.Parallel\trunk\LINQ\LINQ\bin\Debug\Settings\LinqRegistry.xml")
        Dim tt = r.Find("typedef")
        Dim t = tt.GetType

        Return GetType(CLI).RunCLI(App.CommandLine, AddressOf __exeEmpty)
    End Function

    Private Function __exeEmpty() As Integer
        Call Console.WriteLine("{0}!{1}", GetType(Program).Assembly.Location, GetType(LINQ.Framework.LQueryFramework).FullName)
        Return 0
    End Function
End Module
