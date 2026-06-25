using System;
using System.Collections.Generic;
using System.Text;

public class CalcularResultado
{
    // Propriedades da classe 
    public string Infixa { get; }
    public string Posfixa { get; }
    public double[] Valores { get; }
    public double Resultado { get; }


    // Construtor da classe
    public CalcularResultado(string infixa, string posfixa, double[] valores, double resultado)
    {
        Infixa = infixa;
        Posfixa = posfixa;
        Valores = valores;
        Resultado = resultado;
    }


    // Método que percorre toda a expressão recebida
    public List<string> LerCadeia(string cadeia)
    {
        var dados = new List<string>();
        int indice = 0;

        while (indice < cadeia.Length)   // Enquanto não percorreu a expressão inteira
        {
            char caractere = cadeia[indice];

            if (caractere == ' ')   // Se houver espaço, anda para o próximo caractere
                indice++;

            if (char.IsDigit(caractere) || caractere == '.')    // Verifica se o caractere é um decimal (está entre 0 e 9) e se é um ponto (.)
            {
                var resultado = "";
                while (indice < cadeia.Length && (char.IsDigit(cadeia[indice]) || cadeia[indice] == '.'))  // Enquanto a expressão não chegou ao fim e é um caratere válido
                {
                    resultado += cadeia[indice];   // Adiciona o dado lido à variável numero
                    indice++;
                }
                dados.Add(resultado.ToString());   // Adiciona o número à lista de dados
                continue;
            }

            if (caractere == '-' && (dados.Count == 0 || dados[dados.Count - 1] == "("))    // Verifica se o caractere é '-' e se está antes de um número
            {
                var numero = "-";
                indice++;

                while (indice < cadeia.Length && (char.IsDigit(cadeia[indice]) || cadeia[indice] == '.'))  // Enquanto a expressão não chegou ao fim e é um caratere válido
                {
                    numero += cadeia[indice];   // Adiciona o dado lido à variável numero
                    indice++;
                }

                if (numero.Length > 1)  // Se há valores inseridos na variável
                {
                    dados.Add(numero.ToString());   // Adicionamos na lista de dados
                    continue;
                }

                else
                {
                    dados.Add("-");     // Caso contrário, adicionamos apenas o sinal de menos
                    continue;
                }
            }

            dados.Add(caractere.ToString());    // Adiciona o caractere à lista de dados
            indice++;
        }
        return dados;   // Retorna a lista de dados
    }


    // Método que cria o vetor de valores e substitui os operandos por letras
    public static void CriarVetorEInfixa(List<string> dados, out double[] valores, out string letrasInfixa)
    {
        valores = new double[26];   // Cria um vetor de 26 posições para armazenar os valores dos operandos
        var texto = "";             // String que armazenará a expressão usando letras
        int indice = 0;             // Índice para percorrer o vetor de valores

        foreach (string dado in dados)
        {
            double num;
            if (double.TryParse(dado, out num))     // Verifica se o dado é um número
            {
                if (indice >= 26)
                    throw new Exception("Expressão possui mais de 26 operandos");

                valores[indice] = num;      // Armazena o valor do operando no vetor
                texto += (char)('A' + indice);  // Substitui o valor do operando por uma letra 
                indice++;
            }
            else
            {
                texto += dado;  // Adiciona o operador ou parêntese à expressão
            }
        }
        letrasInfixa = texto;   // Retorna a expressão com letras
    }


    // Método que verifica se o caractere é um operador
    private static bool EhOperador(char caractere)
    {
        return caractere == '+' || caractere == '-' || caractere == '*' || caractere == '/' || caractere == '^' ||
                caractere == '(' || caractere == ')';
    }


    // Método que verifica se o operador que está no topo da pilha deve ser executado 
    // antes do operador que acabou de ser lido da expressão
    private bool DeveDesempilhar(char topo, char lido)
    {
        if (lido == '(' || topo == ')')    // Se o operador lido ou no topo for '(', não desempilha nada, apenas empilha
            return false;

        if (lido == '^')
            return false; // Se o operador lido for '^', não desempilha nada, apenas empilha

        // Obtém o nível de prioridade dos operadores
        int prioridadeTopo = Prioridade(topo);
        int prioridadeLido = Prioridade(lido);

        return prioridadeTopo >= prioridadeLido;    // Se o operador no topo da pilha tiver prioridade maior ou igual ao operador lido, desempilha
    }


    // Método que devolve a prioridade de um operando
    private static int Prioridade(char operador)
    {
        switch (operador)
        {
            case '^': return 3;
            case '*': return 2;
            case '/': return 2;
            case '+': return 1;
            case '-': return 1;
            default: return 0;
        }
    }


    string ConverterInfixaParaPosfixa(string cadeiaLida)
    {
        string resultado = "";
        var umaPilha = new PilhaLista<char>(); // Instancia a Pilha

        for (int indice = 0; indice < cadeiaLida.Length; indice++)
        {
            char simboloLido = cadeiaLida[indice];

            if (!EhOperador(simboloLido)) // Se for operando (letras A-Z)
            {
                resultado += simboloLido; // adiciona no resultado
            }
            else // símbolo é operador ou parêntese
            {
                if (simboloLido == ')')
                {
                    // Desempilha tudo até achar o '(' correspondente
                    while (!umaPilha.EstaVazia && umaPilha.oTopo() != '(')
                    {
                        resultado += umaPilha.Desempilhar();
                    }

                    if (umaPilha.EstaVazia)
                        throw new Exception("Parênteses fechados incorretamente.");

                    umaPilha.Desempilhar(); // Remove o '(' da pilha
                }
                else
                {
                    while (!umaPilha.EstaVazia && DeveDesempilhar(umaPilha.oTopo(), simboloLido))
                    {
                        resultado += umaPilha.Desempilhar();
                    }
                    umaPilha.Empilhar(simboloLido);
                }
            }
        } // fim do for

        // Descarrega o restante da Pilha para a Saída
        while (!umaPilha.EstaVazia)
        {
            char operadorRestante = umaPilha.Desempilhar();
            if (operadorRestante != '(')
            {
                resultado += operadorRestante;
            }
        }

        return resultado;
    }


    // Método que calcula o valor de uma subexpressão (ex: operando1 + operando2)
    private static double ValorDaSubExpressao(double op1, char operador, double op2)
    {
        switch (operador)
        {
            case '+': return op1 + op2;
            case '-': return op1 - op2;
            case '*': return op1 * op2;
            case '/':
                if (op2 == 0)
                    throw new Exception("Não é possível dividir por zero.");
                return op1 / op2;
            case '^': return Math.Pow(op1, op2);
            default: throw new Exception("Operador desconhecido: " + operador);
        }
    }

    double ValorDaExpressaoPosfixa(string cadeiaPosfixa, double[] valoresOperandos)
    {
        var umaPilha = new PilhaLista<double>();

        for (int atual = 0; atual < cadeiaPosfixa.Length; atual++)
        {
            char simbolo = cadeiaPosfixa[atual];

            if (!EhOperador(simbolo)) // É Operando (Letra de 'A' a 'Z')
            {
                // Subtrai o caractere 'A' para descobrir o índice correto no vetor
                int indice = simbolo - 'A';
                umaPilha.Empilhar(valoresOperandos[indice]);
            }
            else // É Operador (+, -, *, /, ^)
            {
                // O primeiro elemento desempilhado é o segundo operando da conta
                double operando2 = umaPilha.Desempilhar();
                double operando1 = umaPilha.Desempilhar();

                // Calcula o resultado da subexpressão (ex: operando1 + operando2)
                double valorParcial = ValorDaSubExpressao(operando1, simbolo, operando2);

                // Empilha o resultado parcial para ser usado nos próximos operadores
                umaPilha.Empilhar(valorParcial);
            }
        }

        // O último elemento que restar na pilha é o resultado final da expressão inteira
        return umaPilha.Desempilhar();
    }

    public static CalcularResultado Calcular(string expressaoDigitada)
    {
        // Criamos uma instância temporária da classe para poder acessar os métodos que não são estáticos
        var calc = new CalcularResultado("", "", new double[0], 0);

        // Lê a expressão usando o método da instância
        var dados = calc.LerCadeia(expressaoDigitada);

        // Cria o vetor de valores reais e substitui por letras 
        double[] valores;
        string infixaLetras;
        CriarVetorEInfixa(dados, out valores, out infixaLetras);

        // Converte a cadeia de letras infixa para pós-fixa usando a instância
        string posfixa = calc.ConverterInfixaParaPosfixa(infixaLetras);

        // Avalia o resultado final da expressão pós-fixa usando a instância
        double resultado = calc.ValorDaExpressaoPosfixa(posfixa, valores);

        // Retorna o objeto final com todos os dados calculados com sucesso
        return new CalcularResultado(infixaLetras, posfixa, valores, resultado);
    }
}