using System;
using System.Collections.Generic;
using System.IO;

public class ListaSimples<T> where T : IComparable<T>,
                                       new()
{
    protected NoLista<T>    primeiro,       // ponteiro que aponta para o primeiro nó
                            ultimo,         // ponteiro que aponta para o último nó
                            atual,          // usado para percorrer a lista sequencialmente
                            anterior;       // usado como auxiliar do atual

    protected int quantosNos;               // contador de nós da lista

    public ListaSimples()
    {
        primeiro = ultimo = atual = anterior = null;
        quantosNos = 0;
    }

    public int Tamanho => quantosNos;       // get de quantosNos
    public NoLista<T> Primeiro => primeiro; // get do primeiro elemento da lista
    public NoLista<T> Ultimo => ultimo;     // get do ultimo elemento da lista
    public bool EstaVazia => primeiro == null;  // retorna true se a lista estiver vazia
    public NoLista<T> Atual => atual;       // get do elemento atual da lista


    public void InserirAposFim(T novoDado)  // Método de inserção ao fim da lista
    {
        var novoNo = new NoLista<T>(novoDado);  // criamos um novo nó que armazenará o novoDado
        if (EstaVazia)
            primeiro = novoNo;                  // se a lista está vazia, o novo dado será inserido na primeira posição
        else
            ultimo.Prox = novoNo;               // caso contrário, o novo dado será o último da lista

        ultimo = novoNo;    // passa a ser o último da lista
        quantosNos++;       // incrementamos 1 na quantidade de nós
    }


    public void InserirAposFim(NoLista<T> novoNo)   // sobrecarga no método de inserir após o fim
    {
        novoNo.Prox = null;         
        if (EstaVazia)      
            primeiro = novoNo;      // se estiver vazia, o novo nó é o primeiro elemento da lista
        else
            ultimo.Prox = novoNo;   // caso contrário, o novo nó será o último da lista

        ultimo = novoNo;    // passa a ser o último da lista
        quantosNos++;       // incrementamos 1 na quantidade de nós
    }   

    public void InserirAntesDoInicio(T novoDado)    // Método de inserção antes do início
    {
        var novoNo = new NoLista<T>(novoDado);      // criamos um novo nó que armazenará o novoDado
        if (EstaVazia)
            ultimo = novoNo;                        // se a lista está vazia, o novo nó será inserido na última posição
        else
            novoNo.Prox = primeiro;                 // caso contrário, será inserido antes do primeiro nó

        primeiro = novoNo;  // passa a ser o primeiro da lista
        quantosNos++;       // incrementamos 1 na quantidade de nós
    }

    public void InserirAntesDoInicio(NoLista<T> novoNo) // sobrecarga do método de inserir antes do início
    {
        novoNo.Prox = primeiro;                     // se a lista não está vazia, inserimos antes do primeiro elemento
        if (EstaVazia)
            ultimo = novoNo;                        // caso contrário, o novo nó será inserido na última posição
       
        primeiro = novoNo;  // passa a ser o primeiro da lista
        quantosNos++;       // incrementamos 1 na quantidade de nós
    }

    public void InserirEntreDoisNos(T dados)    // método que insere entre dois nós
    {
        var novoNo = new NoLista<T>(dados);       // criamos um novo nó que armazenará o dado a ser inserido
        anterior.Prox = novoNo;                   // o nó anterior passa a apontar para o novo nó
        novoNo.Prox = atual;                      // o novo nó passa a apontar para o nó atual

        if(anterior == ultimo)     // se o nó anterior for o último, o novo nó passa a ser o último
            ultimo = novoNo;

        quantosNos++;       // incrementamos 1 na quantidade de nós
    }
    public List<T> Listar() // método que exibe os elementos presentes na lista
    {
        var osDados = new List<T>();        // criamos uma nova lista onde armazenaremos os dados

        // inicializamos os ponteiros
        anterior = null;
        atual = primeiro;

        while(atual != null)
        {
            osDados.Add(atual.Info);        // adicionamos a informação desse nó atual na lista de resultados
           
            // atualizamos os ponteiros para apontar para andarmos na lista
            anterior = atual;
            atual = atual.Prox;
        }

        return osDados;     // retornamos uma lista com as informações
    }

    public bool Existe(T dadoProcurado) // método que busca um dado na lista
    {
        anterior = null;
        atual = primeiro;

        if (EstaVazia)      // se a lista está vazia, retorna false
            return false;

        if (dadoProcurado.CompareTo(primeiro.Info) < 0)     // se o dadoProcurado é menor que o menor dado da lista
            return false;

        if (dadoProcurado.CompareTo(ultimo.Info) > 0)       // se o dadoProcurado é maior que o maior dado da lista
        {
            anterior = ultimo;      // indica inclusão após o último    
            atual = null;
            return false;
        }
            
        while (atual != null)       // percorremos a lista procurando o dado
        {
            if (dadoProcurado.CompareTo(atual.Info) == 0)   // se achou o dado, retorna true
                return true;

            if (dadoProcurado.CompareTo(atual.Info) < 0)    // se o dado procurado for menor que o menor dado,
                return false;                               // retorna false

            // avança os ponteiros para o próximo nó
            anterior = atual;
            atual = atual.Prox;
        }

        // se nao encontrou o dado, retorna false
        return false;   
    }

    public bool InseriuEmOrdem(T dados) // método que insere em ordem
    {
        if (!Existe(dados))     // Existe() ajusta ponteiros anterior e atual 
        {                       // aqui temos certeza de que a chave não existe 
                                // guarda dados no novo nó 
            if (EstaVazia)                    // se a lista está vazia, então o   
                InserirAntesDoInicio(dados);  // dado ficará como primeiro da lista 
            else
                if (anterior == null && atual != null) // testa se nova chave < primeira chave 
                    // (dados.Info.CompareTo(primeiro.Info) < 0)
                    InserirAntesDoInicio(dados); // liga novo nó antes do primeiro 
                else
                    if (anterior != null && atual == null)  // testa se nova chave > última chave 
                        // (dados.Info.CompareTo(ultimo.Info) > 0)
                        InserirAposFim(dados);
                    else
                        InserirEntreDoisNos(dados);  // insere entre os nós anterior e atual 

            return true;        // foi incluído em ordem
        }

        return false;       // se o dado já existe, não insere
    }

    public bool Excluiu(T dadoAExcluir) // método que exclui um dado da lista
    {
        if (EstaVazia)      // se a lista está vazia, não temos dados para excluir
            return false;

        if (!Existe(dadoAExcluir))  // se não existe não existe na lista, apenas ajustamos os ponteiros
            return false;

        if (atual == primeiro)          // se o item a ser excluído é o primeiro:
        {
            primeiro = primeiro.Prox;   // o primeiro aponta para o segundo
            if (atual == ultimo)        // se a lista possui apenas um elemento:
                ultimo = null;          // o ultimo aponta para null
                
            atual = primeiro;           // atual (item excluído) é o primeiro elemento da lista
        }
        else
            if (atual == ultimo)        // se o item a ser excluído é igual ao último:
            {
                anterior.Prox = null;   // o penúltimo elemento aponta para null
                ultimo = anterior;      // e passa a ser o último elemento da lista
                atual = ultimo;         // o dado excluído é o último
            }
            else
            {
                anterior.Prox = atual.Prox;     // se o dado a ser excluído está entre dois nós, o anterior aponta 
                                                // para o sucessor do atual
                atual = atual.Prox;             // e o atual aponta para o elemento seguinte
            }

        quantosNos--;       // decementamos 1 de quantos nós
        return true;        
    }

    public ListaSimples<T> Unir(ListaSimples<T> outraLista)     // método que devolve a união em ordem de duas listas
    {   
        var novaLista = new ListaSimples<T>();  // criamos uma lista que armazerá as duas listas
        
        // inicializamos os ponteiros para percorrer as listas
        this.atual = this.primeiro;
        outraLista.atual = outraLista.primeiro;

        // percorremos ambas as listas em paralelo
        while(this.atual != null && outraLista.atual != null)
        {
            if (this.atual.Info.CompareTo(outraLista.atual.Info) < 0)   // verificamos se o valor da lista this
                                                                        // é menor que o da lista recebida
            {
                var noSeguinteAoAtual = this.atual.Prox;
                novaLista.InserirAposFim(this.atual);   // se for, inserimos após o fim o dado da lista this
                this.atual = noSeguinteAoAtual;         // avança na lista this
            }
            else
                if (outraLista.atual.Info.CompareTo(this.atual.Info) < 0)   // verifica se o valor da lista recebida
                                                                            // é menor que o da lista this
                {
                    var noSeguinteAoAtual = outraLista.atual.Prox;
                    novaLista.InserirAposFim(outraLista.atual);     // se for, inserimos após fim o dado da lista recebida
                    outraLista.atual = noSeguinteAoAtual;           // avança na outra lista
                }
                else
                {
                    // caso os elementos sejam iguais
                    var noSeguinteAoAtual = this.atual.Prox;
                    novaLista.InserirAposFim(this.atual);       // inserimos um dos dados na lista que será retornada
                    this.atual = noSeguinteAoAtual;             // avança na lista this
                    outraLista.atual = outraLista.atual.Prox;   // avança na lista recebida
                }
        }

        while (this.atual != null)    // se a lista this não foi lida por completo
        {
            var noSeguinteAoAtual = this.atual.Prox;
            novaLista.InserirAposFim(this.atual);   // adicionamos os dados ao fim da lista que será retornada
            this.atual = noSeguinteAoAtual;
        }

        while (outraLista.atual != null)    // se a lista recebida não chegou ao fim
        {
            var noSeguinteAoAtual = outraLista.atual.Prox;
            novaLista.InserirAposFim(outraLista.atual); // adicionamos os dados ao fim da lista que será retornada
            outraLista.atual = noSeguinteAoAtual;
        }

        // resetamos os ponteiros, já que não temos mais elementos em nenhuma lista 
        this.primeiro = this.ultimo = null;   
        this.quantosNos = 0;
        outraLista.primeiro = outraLista.ultimo = null;
        outraLista.quantosNos = 0;

        return novaLista;   // devolve uma única lista de união
    }

    public void Inverter()  // Método responsável por inverter uma lista
    {
        if (EstaVazia || primeiro == ultimo) // se a lista vazia ou com apenas um nó
            return; 
        
        // ponteiros que serão usados na inversão
        anterior = null;
        atual = primeiro;
        ultimo = primeiro; // o primeiro atual passará a ser o último

        while (atual != null)
        {
            var proximo = atual.Prox;   // guarda o próximo nó

            atual.Prox = anterior;      // inverte o ponteiro

            anterior = atual;           // avança anterior
            atual = proximo;            // avança atual
        }

        primeiro = anterior;            // o último elemento passa a ser o primeiro
    }
}