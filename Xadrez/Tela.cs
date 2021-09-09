using System;
using System.Collections.Generic;
using System.Text;
using tabuleiro;
using xadrez;

namespace Xadrez
{
    class Tela
    {
        public static void imprimirTabuleiro(Tabuleiro tab)
        {   //Populando tabuleiro
            for(int i=0; i<tab.linhas; i++)
            {
                Console.Write(8 - i + " ");

                for(int j=0; j < tab.colunas; j++)
                {
                    if (tab.peca(i, j) == null)
                    {   //Caso não tenha peça alocada imprimir -
                        Console.Write("- ");
                    }
                    else
                    {   //Endereçamento de peça no tabuleiro
                        Tela.imprimirPeca(tab.peca(i, j));
                        Console.Write(" ");
                    }
                    
                }
                
                //Quebra de linha para formato de tabuleiro
                Console.WriteLine();
            }

            Console.WriteLine("  a b c d e f g h");

        }
            
        public static PosicaoXadrez lerPosicaoXadrez()
        {
            string s = Console.ReadLine();
            char coluna = s[0];
            int linha = int.Parse(s[1] + "");
            return new PosicaoXadrez(coluna, linha);
        }





        //Definindo a cor ao imprimir a peça
            public static void imprimirPeca(Peca peca)
        {   
            if(peca.cor == Cor.Branca)
            {
                Console.Write(peca);
            }
            else
            {
                ConsoleColor aux = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(peca);
                Console.ForegroundColor = aux;
            }

        }

    }
}
