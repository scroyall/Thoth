namespace Thoth.Utils;

public class AtomStack<TAtom>
{
    public int Cursor { private set; get; }

    private readonly IReadOnlyList<TAtom> _items;

    public AtomStack(IReadOnlyList<TAtom> items)
    {
        _items = items;
        
        Reset();
    }

    public TAtom? Consume() => Consume<TAtom>();

    public TConsumedAtom? Consume<TConsumedAtom>()
        where TConsumedAtom : TAtom
    {
        switch (Peek())
        {
            case TConsumedAtom atom:
                Cursor++;
                return atom;
            case { } other:
                Console.Error.WriteLine($"Failed to consume {typeof(TConsumedAtom).Name}; unexpected {other}.");
                return default;
            case null:
                Console.Error.WriteLine($"Failed to consume {typeof(TConsumedAtom).Name}; unexpected end of input.");
                return default;
        }
    }

    private bool IsValidIndex(int index)
    {
        return (index >= 0 && index < _items.Count);
    }

    public TAtom? Peek(int offset = 0)
    {
        var index = Cursor + offset;
        return IsValidIndex(index) ? _items[index] : default;
    }

    public void Reset()
    {
        Cursor = 0;
    }
}
