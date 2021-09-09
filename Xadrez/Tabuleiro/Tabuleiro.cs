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

        public Peca peca(Posicao pos)
        {
            return pecas[pos.linha, pos.coluna];
        }

        public bool existePeca(Posicao pos)
        {
            validarPosicao(pos);
            return peca(pos) != null;
        }

        //Endereçando uma peça a uma posição do tabuleiro
        public void colocarPeca(Peca p, Posicao pos)
        {   
            //Teste para saber se a posição está ocupada
            if (existePeca(pos))
            {
                throw new TabuleiroException("Já existe uma peça nessa posição!");
            }

            //Coloca peça no tabuleiro
            pecas[pos.linha, pos.coluna] = p;
            p.posicao = pos;
        }

        //Retirando uma peça do tabuleiro
        public Peca retirarPeca(Posicao pos)
        {
            if (peca(pos)== null)
            {
                return null;
            }
            
            Peca aux = peca(pos);
            aux.posicao = null;
            pecas[pos.linha, pos.coluna] = null;
            return aux;
        }
        

        //Avaliando se a posição digitada está correta.
        public bool posicaoValida(Posicao pos)
        {
            if (pos.linha < 0 || pos.linha >= linhas || pos.coluna < 0 || pos.coluna >= colunas)
            {
                return false;
            }
            return true;
        }

        //Exception - Posição de peça inválida
        public void validarPosicao(Posicao pos)
        {
            if (!posicaoValida(pos))
            {
                throw new TabuleiroException("Posição inválida!");
            }
        }

    }
}
