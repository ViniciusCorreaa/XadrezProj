using System;
using System.Collections.Generic;
using System.Text;

namespace tabuleiro
{
    class Tabuleiro
    {
        //Linhas e colunas configuraveis para mais opções de jogos*.
        public int linhas { get; set; }

        public int colunas { get; set; }

        public Peca[,] pecas;

        public Tabuleiro(int linhas, int colunas)
        {
            this.linhas = linhas;
            this.colunas = colunas;
            pecas = new Peca[linhas, colunas];
        }

        public Peca peca(int linha, int coluna)
        {
            return pecas[linha, coluna];
        }




    }
}
