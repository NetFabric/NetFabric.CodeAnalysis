Namespace TestData

    Public Structure MappedEnumerator(Of T)
        Implements IEnumerator(Of T)

        Private ReadOnly _source As T()
        Private _index As Integer

        Public Sub New(source As T())
            _source = source
            _index = -1
        End Sub

        Public ReadOnly Property Current As T Implements IEnumerator(Of T).Current
            Get
                Current = _source(_index)
            End Get
        End Property

        Public ReadOnly Property IEnumerator_Current As Object Implements IEnumerator.Current
            Get
                IEnumerator_Current = _source(_index)
            End Get
        End Property

        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            _index = _index + 1
            MoveNext = _index < _source.Length
        End Function

        Public Sub Reset() Implements IEnumerator.Reset
            Throw New NotSupportedException
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
        End Sub

    End Structure

End Namespace

