using System;
using System.Collections.Generic;
using System.Text;
using tabuleiro;

namespace Xadrez
{
    class Tela
    {
        public static void imprimirTabuleiro(Tabuleiro tab)
        {   //Populando tabuleiro
            for(int i=0; i<tab.linhas; i++)
            {
                for(int j=0; j < tab.colunas; j++)
                {
                    if (tab.peca(i, j) == null)
                    {   //Caso não tenha peça alocada imprimir -
                        Console.Write("- ");
                    }
                    else
                    {   //Endereçamento de peça no tabuleiro
                        Console.Write(tab.peca(i, j) + " ");
                    }
                    
                }
                
                //Quebra de linha para formato de tabuleiro
                Console.WriteLine();
            }
            
        

        }


    }
}
