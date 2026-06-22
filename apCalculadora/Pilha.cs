using System;

namespace Calculadora.Core
{
    public sealed class Pilha<T> : IPilha<T>
    {
        private No<T> _topo;
        private int _quantidade;

        public bool EstaVazia => _topo is null;
        public int Tamanho => _quantidade;

        public void Empilhar(T elemento)
        {
            _topo = new No<T>(elemento, _topo);
            _quantidade++;
        }

       
        public T Desempilhar()
        {
            if (EstaVazia)
                throw new InvalidOperationException("Operação inválida: pilha está vazia.");

            T valor = _topo.Valor;
            _topo = _topo.Proximo;
            _quantidade--;
            return valor;
        }

        public T OTopo()
        {
            if (EstaVazia)
                throw new InvalidOperationException("Operação inválida: pilha está vazia.");

            return _topo.Valor;
        }
    }
}
