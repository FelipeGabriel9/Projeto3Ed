using System;
using System.Collections.Generic;

public class PilhaLista<T> : IStack<T> 
            where T: IComparable<T>, new()
{
    NoLista<T> topo;    // primeiro nó da lista/pilha
    int tamanho;        // quantidade de elementos na pilha

    public PilhaLista()     // construtor
    {
        topo = null;
        tamanho = 0;
    }

    public int Tamanho => tamanho;              // get do tamanho da pilha
    public bool EstaVazia => topo == null;      // get que retorna true se a pilha estiver vazia
    
    public void Empilhar(T dado)    // Método que adiciona um novo dado à pilha
    {
        var novoNo = new NoLista<T>(dado, topo);    // instancia um novo nó que armazenará o dado recebido
        topo = novoNo;      // o topo passa a apontar para o novo nó
        tamanho++;          // incrementamos 1 no tamanho da pilha
    }

    public T oTopo()    // Método que devolve o elemento no topo da pilha sem removê-lo
    {
        if (EstaVazia)
            throw new Exception("Underflow da pilha");  // se a pilha estiver vazia, é disparada uma exceção

        T objTopo = topo.Info;  // guarda o dado em um objeto genérico
        return objTopo;         // retorna esse objeto
    }

    public T Desempilhar()  // Método que devolve o elemento do topo e o exclui
    {
        if (EstaVazia)
            throw new Exception("Underflow da pilha");  // se a pilha estiver vazia, é disparada uma exceção

        T dadoDesempilhado = topo.Info; // guarda o dado em um objeto genérico
        topo = topo.Prox;               // avança o topo para o nó seguinte
        tamanho--;                      // atualiza o número de elementod na pilha

        return dadoDesempilhado;        // devolve o objeto que estava no topo
    }

    public List<T> Conteudo()   // Método que devolve uma lista com os dados da pilha
    {
        var conteudo = new List<T>();   // criamos uma lista que armazenará os dados
        NoLista<T> atual = topo;        // ponteiro que auxiliará na inserção de dados na lista

        while(atual != null)            // enquanto a pilha não chegou ao fim
        {
            conteudo.Add(atual.Info);   // adiciona o elemento atual à lista
            atual = atual.Prox;         // avança para o próximo item da pilha
        }

        return conteudo;                // retorna a lista de dados
    }


}