Namespace TestData

    Public Structure MappedEnumerable(Of T)
        Implements IEnumerable(Of T)

        Private ReadOnly _source As T()

        Public Sub New(source As T())
            _source = source
        End Sub

        Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            GetEnumerator = New MappedEnumerator(Of T)(_source)
        End Function

        Public Function IEnumerable_ExplicitGetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            IEnumerable_ExplicitGetEnumerator = New MappedEnumerator(Of T)(_source)
        End Function

    End Structure
    
    
    Public Class ValidateEnumerables
        Public Sub ValidEnumerables()
            
            For Each item As Integer In New MappedEnumerable(Of Integer)
            Next
            
        End Sub
    End Class

End Namespace

