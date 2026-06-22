using System;
using System.Collections.Generic;
using System.Text;

namespace Calculadora.Core
{
    public sealed class ResultadoCalculo
    {
        public string Infixa { get; }

        /// <summary>Sequência pósfixa (RPN) correspondente (ex.: ABC*+).</summary>
        public string Posfixa { get; }

        /// <summary>
        /// Vetor de valores reais.
        /// Valores[0] corresponde à letra A, [1] à letra B etc.
        /// </summary>
        public double[] Valores { get; }

        /// <summary>Resultado numérico final da expressão.</summary>
        public double Resultado { get; }

        // CORREÇÃO: Construtor criado para permitir atribuir valores às propriedades readonly { get; }
        public ResultadoCalculo(string infixa, string posfixa, double[] valores, double resultado)
        {
            Infixa = infixa;
            Posfixa = posfixa;
            Valores = valores;
            Resultado = resultado;
        }

        public static List<string> Tokenizar(string expressao)
        {
            var tokens = new List<string>();
            int i = 0;
            while (i < expressao.Length)
            {
                char c = expressao[i];

                // Pula espaços
                if (c == ' ') { i++; continue; }

                // Número (incluindo ponto decimal)
                if (char.IsDigit(c) || c == '.')
                {
                    var num = new StringBuilder();
                    while (i < expressao.Length && (char.IsDigit(expressao[i]) || expressao[i] == '.'))
                    {
                        num.Append(expressao[i]);
                        i++;
                    }
                    tokens.Add(num.ToString());
                    continue;
                }

                // Sinal negativo unário: aparece após '(' ou no início
                if (c == '-' && (tokens.Count == 0 || tokens[tokens.Count - 1] == "("))
                {
                    var num = new StringBuilder();
                    num.Append(c);
                    i++;
                    while (i < expressao.Length && (char.IsDigit(expressao[i]) || expressao[i] == '.'))
                    {
                        num.Append(expressao[i]);
                        i++;
                    }
                    if (num.Length > 1)
                    {
                        tokens.Add(num.ToString());
                        continue;
                    }
                    else
                    {
                        tokens.Add("-");
                        continue;
                    }
                }

                // Operador ou parêntese
                tokens.Add(c.ToString());
                i++;
            }
            return tokens;
        }

        public static void CriarVetorEInfixa(List<string> tokens, out double[] valores, out string infixaLetras)
        {
            valores = new double[26];
            var sb = new StringBuilder();
            int letraIdx = 0;

            foreach (string tok in tokens)
            {
                double num;
                if (double.TryParse(tok, System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture, out num))
                {
                    if (letraIdx >= 26)
                        throw new Exception("Expressão possui mais de 26 operandos distintos.");
                    valores[letraIdx] = num;
                    sb.Append((char)('A' + letraIdx));
                    letraIdx++;
                }
                else
                {
                    sb.Append(tok);
                }
            }
            infixaLetras = sb.ToString();
        }

        private static bool EhOperador(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '^' ||
                   c == '(' || c == ')';
        }

        private static bool TemPrecedencia(char topo, char lido)
        {
            if (lido == '(') return false;
            if (topo == '(') return false;

            int prioTopo = Prioridade(topo);
            int prioLido = Prioridade(lido);

            if (lido == '^')
                return prioTopo > prioLido;

            return prioTopo >= prioLido;
        }

        private static int Prioridade(char op)
        {
            switch (op)
            {
                case '^': return 3;
                case '*':
                case '/': return 2;
                case '+':
                case '-': return 1;
                default: return 0;
            }
        }

        public static string ConverterInfixaParaPosfixa(string infixaLetras)
        {
            var resultado = new StringBuilder();
            var pilha = new Pilha<char>();

            foreach (char simboloLido in infixaLetras)
            {
                if (!EhOperador(simboloLido))
                {
                    resultado.Append(simboloLido);
                }
                else
                {
                    bool parar = false;
                    while (!parar && !pilha.EstaVazia && TemPrecedencia(pilha.OTopo(), simboloLido))
                    {
                        char operadorTopo = pilha.Desempilhar();
                        if (operadorTopo != '(')
                            resultado.Append(operadorTopo);
                        else
                            parar = true;
                    }

                    if (simboloLido != ')')
                    {
                        pilha.Empilhar(simboloLido);
                    }
                    else
                    {
                        if (!pilha.EstaVazia && pilha.OTopo() == '(')
                            pilha.Desempilhar();
                    }
                }
            }

            while (!pilha.EstaVazia)
            {
                char op = pilha.Desempilhar();
                if (op != '(')
                    resultado.Append(op);
            }

            return resultado.ToString();
        }

        private static double ValorDaSubExpressao(double op1, char operador, double op2)
        {
            switch (operador)
            {
                case '+': return op1 + op2;
                case '-': return op1 - op2;
                case '*': return op1 * op2;
                case '/':
                    if (op2 == 0) throw new DivideByZeroException("Divisão por zero.");
                    return op1 / op2;
                case '^': return Math.Pow(op1, op2);
                default: throw new Exception("Operador desconhecido: " + operador);
            }
        }

        public static double ValorDaExpressaoPosfixa(string cadeiaPosfixa, double[] valores)
        {
            var pilha = new Pilha<double>();

            foreach (char simbolo in cadeiaPosfixa)
            {
                if (!EhOperador(simbolo))
                {
                    pilha.Empilhar(valores[simbolo - 'A']);
                }
                else
                {
                    double operando2 = pilha.Desempilhar();
                    double operando1 = pilha.Desempilhar();
                    double parcial = ValorDaSubExpressao(operando1, simbolo, operando2);
                    pilha.Empilhar(parcial);
                }
            }

            return pilha.Desempilhar();
        }

        public static ResultadoCalculo Calcular(string expressaoDigitada)
        {
            var tokens = Tokenizar(expressaoDigitada);

            double[] valores;
            string infixaLetras;
            CriarVetorEInfixa(tokens, out valores, out infixaLetras);

            string posfixa = ConverterInfixaParaPosfixa(infixaLetras);
            double resultado = ValorDaExpressaoPosfixa(posfixa, valores);

            // CORREÇÃO: Agora passa os parâmetros via construtor com segurança
            return new ResultadoCalculo(infixaLetras, posfixa, valores, resultado);
        }
    }
}   