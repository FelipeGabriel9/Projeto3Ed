public class No<T>
{
    public T Valor { get; }
    public No<T> Proximo { get; set; }

    public No(T valor, No<T> proximo)
    {
        Valor = valor;
        Proximo = proximo;
    }
}