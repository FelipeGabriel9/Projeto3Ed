using System.Collections.Generic;

public interface IStack<T>
{
    void Empilhar(T item);      // Insere um novo item na pilha
    T Desempilhar();            // Remove e devolve o último dado da pilha
    T oTopo();                  // Devolve o último elemento inserido sem removê-lo
    int Tamanho { get; }        // Propriedade que devolve a quantidade atual de elementos da pilha
    bool EstaVazia { get; }     // Propriedade que indica se a pilha não possui elementos
    List<T> Conteudo();         // Retorna uma lista com os dados empilhados
}