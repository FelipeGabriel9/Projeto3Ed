using System;

public class NoLista<T> where T : IComparable<T>, new()
{
    private T info;
    private NoLista<T> prox;

    public T Info
    {
        get => info;
        set => info = value;
    }

    public NoLista<T> Prox
    {
        get => prox;
        set => prox = value;
    }

    public NoLista(T info, NoLista<T> prox)
    {
        this.info = info;
        this.prox = prox;
    }

    public NoLista(T info)
    {
        this.info = info;
        this.prox = null;
    }
}