using System;
using UnityEngine;

public class CircularLinkedList<T>
{
    public int Count
    {
        get;
        private set;
    }
    public int MaxCount;

    private T[] _items;
    private int _currentStartIndex;

    public CircularLinkedList(int maxCount)
    {
        MaxCount = maxCount;
        Count = 0;
        _currentStartIndex = 0;
        
        _items = new T[maxCount];
    }

    public void Add(T item)
    {
        int newIndex = _currentStartIndex + Count;
        if (newIndex < MaxCount)
        {
            _items[newIndex] = item;
        }
        else
        {
            newIndex -= MaxCount;
            _items[newIndex] = item;
        }

        Count++;
        
        if (Count > MaxCount)
        {
            Count = MaxCount;
            _currentStartIndex++;
            if (_currentStartIndex >= MaxCount)
                _currentStartIndex = 0;
        }
    }
    
    public T Get(int index)
    {
        if (Count <= index)
            throw new IndexOutOfRangeException();
        
        int indexToGet = _currentStartIndex + index;
        if (indexToGet >= MaxCount)
            indexToGet -= MaxCount;
        return _items[indexToGet];
    }

    public void Clear()
    {
        Count = 0;
        _currentStartIndex = 0;
    }
}
