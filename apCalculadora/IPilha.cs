using System;

public interface IPilha<T>
{ 
    void Empilhar(T elemento);      // Empilha um elemento no topo
    T Desempilhar();                // Remove e retorna o elemento do topo
    T OTopo();                      // Consulta o topo sem remover
    bool EstaVazia { get; }         // Indica se a pilha está vazia
    int Tamanho { get; }         // Número de elementos presentes
}