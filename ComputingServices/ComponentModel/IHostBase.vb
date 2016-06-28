﻿#Region "Microsoft.VisualBasic::2e28920677e297707448e2a8501f6fc4, ..\ComputingServices\ComponentModel\IHostBase.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Abstract

Namespace ComponentModel

    Public MustInherit Class IHostBase : Inherits IMasterBase(Of TcpSynchronizationServicesSocket)

        Sub New(portal As Integer)
            __host = New TcpSynchronizationServicesSocket(portal)
        End Sub

        Sub New()
        End Sub
    End Class

    Public MustInherit Class IMasterBase(Of TSocket As IServicesSocket)

        Public MustOverride ReadOnly Property Portal As IPEndPoint

        Protected Friend __host As TSocket

        Public Shared Narrowing Operator CType(master As IMasterBase(Of TSocket)) As IPEndPoint
            Return master.Portal
        End Operator

        Public Shared Narrowing Operator CType(master As IMasterBase(Of TSocket)) As System.Net.IPEndPoint
            Return master.Portal.GetIPEndPoint
        End Operator

    End Class
End Namespace
