﻿Imports Microsoft.VisualBasic.Linq.Statements.TokenIcer

Namespace LDM.Parser

    Module Extensions

        Public ReadOnly Property Primitive As Tokens = Tokens.String
        Public ReadOnly Property OpenParens As Tokens = Tokens.LPair
        Public ReadOnly Property CloseParens As Tokens = Tokens.RPair

    End Module
End Namespace