using System;
using System.Collections.Generic;
using System.Text;
using tabuleiro;

namespace xadrez
{
    class PartidaDeXadrez
    {
        public Tabuleiro tab { get; private set; }

        public int turno { get; private set; }

        public Cor jogadorAtual { get; private set; }

        public bool terminada { get; private set; }

        public bool xeque { get; private set; }

        public Peca vulneravelEnPassant { get; private set; }

        private HashSet<Peca> pecas; //Conjunto que guarda todas as peças da partida

        private HashSet<Peca> capturadas; //Conjunto que guarda todas as peças capturadas da partida

        //Inicio de partida
        public PartidaDeXadrez()
        {
            tab = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branca; //O Jogo sempre inicia pelas peças brancas
            terminada = false;
            vulneravelEnPassant = null;
            xeque = false;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();

            colocarPecas();

        }

        //Deslocamento da peça
        public Peca executaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = tab.retirarPeca(origem);
            p.incrementarQteMovimentos();
            Peca pecaCapturada = tab.retirarPeca(destino);
            tab.colocarPeca(p, destino);

            //Adicionando as peças capturadas ao conjunto
            if (pecaCapturada != null)
            {
                capturadas.Add(pecaCapturada);
            }

            // #jogadaespecial roque pequeno
            if(p is Rei && destino.coluna == origem.coluna + 2)
            {
                Posicao origemT = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna + 1);
                Peca T = tab.retirarPeca(origemT);
                T.incrementarQteMovimentos();
                tab.colocarPeca(T, destinoT);
            }

            // #jogadaespecial roque grande
            if (p is Rei && destino.coluna == origem.coluna - 2)
            {
                Posicao origemT = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna - 1);
                Peca T = tab.retirarPeca(origemT);
                T.incrementarQteMovimentos();
                tab.colocarPeca(T, destinoT);
            }

            // #jogadaespecial en passant
            if(p is Peao)
            {
                if(origem.coluna != destino.coluna)
                {
                    Posicao posP;
                    if(p.cor == Cor.Branca)
                    {
                        posP = new Posicao(destino.linha +1, destino.coluna);
                    }
                    else
                    {
                        posP = new Posicao(destino.linha - 1, destino.coluna);
                    }
                    pecaCapturada = tab.retirarPeca(posP);
                    capturadas.Add(pecaCapturada);
                }
            }


            return pecaCapturada;
        }

        public void desfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = tab.retirarPeca(destino);
            p.decrementarQteMovimentos();
            if (pecaCapturada != null)
            {
                tab.colocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }
            tab.colocarPeca(p, origem);

            // #jogadaespecial roque pequeno
            if (p is Rei && destino.coluna == origem.coluna + 2)
            {
                Posicao origemT = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna + 1);
                Peca T = tab.retirarPeca(destinoT);
                T.decrementarQteMovimentos();
                tab.colocarPeca(T, origemT);
            }

            // #jogadaespecial roque grande
            if (p is Rei && destino.coluna == origem.coluna - 2)
            {
                Posicao origemT = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna - 1);
                Peca T = tab.retirarPeca(destinoT);
                T.incrementarQteMovimentos();
                tab.colocarPeca(T, origemT);
            }

            // #jogadaespecial en passant
            if(p is Peao)
            {
                if(origem.coluna != destino.coluna && pecaCapturada == vulneravelEnPassant)
                {
                    Peca peao = tab.retirarPeca(destino);
                    Posicao posP;

                    if(p.cor == Cor.Branca)
                    {
                        posP = new Posicao(3, destino.coluna);
                    }
                    else
                    {
                        posP = new Posicao(4, destino.coluna);
                    }
                    
                    tab.colocarPeca(peao, posP);
                }
            }


        }

        public void realizaJogada(Posicao origem, Posicao destino)
        {

            Peca pecaCapturada = executaMovimento(origem, destino);

            if (estaEmXeque(jogadorAtual))
            {
                desfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque!");
            }

            if (estaEmXeque(adversaria(jogadorAtual)))
            {
                xeque = true;
            }
            else
            {
                xeque = false;
            }


            if (testeXequemate(adversaria(jogadorAtual)))
            {
                terminada = true; //Fim da partida
            }
            else
            {
                turno++;//Incrementa o turno

                mudajogador();//Chama o metodo de alterar jogador
            }

            Peca p = tab.peca(destino);

            // #jogadaespecial en passant
            if(p is Peao && (destino.linha == origem.linha - 2 || destino.linha == origem.linha + 2))
            {
                vulneravelEnPassant = p;
            }
            else
            {
                vulneravelEnPassant = null;
            }
        }

        //Controle de seleção da posição de origem
        public void validarPosicaoDeOrigem(Posicao pos)
        {

            if (tab.peca(pos) == null)
            {
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");
            }

            if (jogadorAtual != tab.peca(pos).cor)
            {
                throw new TabuleiroException("A peça de origem escolhida não é sua");
            }

            if (!tab.peca(pos).existeMovimentosPossiveis())
            {
                throw new TabuleiroException("Não há movimentos possiveis para a peça de origem escolhida!");
            }
        }


        public void validarPosicaoDeDestino(Posicao origem, Posicao destino)
        {
            if (!tab.peca(origem).movimentoPossivel(destino))
            {
                throw new TabuleiroException("Posição de destino inválida!");
            }
        }

        //Seleciona o jogador
        private void mudajogador()
        {
            if (jogadorAtual == Cor.Branca)
            {
                jogadorAtual = Cor.Preta;
            }
            else
            {
                jogadorAtual = Cor.Branca;
            }
        }

        public HashSet<Peca> pecasCapturadas(Cor cor) //Retorna todas as peças da cor informada
        {
            HashSet<Peca> aux = new HashSet<Peca>();

            foreach (Peca x in capturadas)
            {
                if (x.cor == cor)
                {
                    aux.Add(x);
                }
            }
            return aux;
        }

        public HashSet<Peca> pecasEmJogo(Cor cor) //Retorna as peças em jogo de determinada cor
        {
            HashSet<Peca> aux = new HashSet<Peca>();

            foreach (Peca x in pecas)
            {
                if (x.cor == cor)
                {
                    aux.Add(x);
                }
            }
            aux.ExceptWith(pecasCapturadas(cor)); //Retira as peças capturadas
            return aux;
        }

        //Selecionando peça adversaria
        private Cor adversaria(Cor cor)
        {
            if (cor == Cor.Branca)
            {
                return Cor.Preta;
            }
            else
            {
                return Cor.Branca;
            }

        }

        private Peca rei(Cor cor)
        {
            foreach (Peca x in pecasEmJogo(cor))
            {
                if (x is Rei)
                {
                    return x;
                }
            }
            return null;
        }

        //Teste para verificar se o rei está em Xeque
        public bool estaEmXeque(Cor cor)
        {
            Peca R = rei(cor);

            if (R == null)
            {
                throw new TabuleiroException("Não existe um rei da cor " + cor + " em jogo!");
            }

            foreach (Peca x in pecasEmJogo(adversaria(cor)))
            {
                bool[,] mat = x.movimentosPossiveis();
                if (mat[R.posicao.linha, R.posicao.coluna])
                {
                    return true;
                }
            }
            return false;
        }

        //Teste Xequemate
        public bool testeXequemate(Cor cor)
        {
            if (!estaEmXeque(cor))
            {
                return false;
            }

            foreach (Peca x in pecasEmJogo(cor))
            {
                bool[,] mat = x.movimentosPossiveis();

                for (int i = 0; i < tab.linhas; i++)
                {
                    for (int j = 0; j < tab.colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao origem = x.posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = executaMovimento(origem, destino);

                            bool testeXeque = estaEmXeque(cor);
                            desfazMovimento(origem, destino, pecaCapturada);

                            if (!testeXeque)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            //Todas as tentativas de sair do xeque falham então:
            //Return true = Xequemate
            return true;
        }


        public void colocarNovapeca(char coluna, int linha, Peca peca)
        {
            tab.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca);
        }

        private void colocarPecas()
        {
            colocarNovapeca('a', 1, new Torre(tab, Cor.Branca));
            colocarNovapeca('b', 1, new Cavalo(tab, Cor.Branca));
            colocarNovapeca('c', 1, new Bispo(tab, Cor.Branca));
            colocarNovapeca('d', 1, new Dama(tab, Cor.Branca));
            colocarNovapeca('e', 1, new Rei(tab, Cor.Branca, this));
            colocarNovapeca('f', 1, new Bispo(tab, Cor.Branca));
            colocarNovapeca('g', 1, new Cavalo(tab, Cor.Branca));
            colocarNovapeca('h', 1, new Torre(tab, Cor.Branca));
            colocarNovapeca('a', 2, new Peao(tab, Cor.Branca, this));
            colocarNovapeca('b', 2, new Peao(tab, Cor.Branca, this));
            colocarNovapeca('c', 2, new Peao(tab, Cor.Branca, this));
            colocarNovapeca('d', 2, new Peao(tab, Cor.Branca, this));
            colocarNovapeca('e', 2, new Peao(tab, Cor.Branca, this));
            colocarNovapeca('f', 2, new Peao(tab, Cor.Branca, this));
            colocarNovapeca('g', 2, new Peao(tab, Cor.Branca, this));
            colocarNovapeca('h', 2, new Peao(tab, Cor.Branca, this));


            colocarNovapeca('a', 8, new Torre(tab, Cor.Preta));
            colocarNovapeca('b', 8, new Cavalo(tab, Cor.Preta));
            colocarNovapeca('c', 8, new Bispo(tab, Cor.Preta));
            colocarNovapeca('d', 8, new Dama(tab, Cor.Preta));
            colocarNovapeca('e', 8, new Rei(tab, Cor.Preta, this));
            colocarNovapeca('f', 8, new Bispo(tab, Cor.Preta));
            colocarNovapeca('g', 8, new Cavalo(tab, Cor.Preta));
            colocarNovapeca('h', 8, new Torre(tab, Cor.Preta));
            colocarNovapeca('a', 7, new Peao(tab, Cor.Preta, this));
            colocarNovapeca('b', 7, new Peao(tab, Cor.Preta, this));
            colocarNovapeca('c', 7, new Peao(tab, Cor.Preta, this));
            colocarNovapeca('d', 7, new Peao(tab, Cor.Preta, this));
            colocarNovapeca('e', 7, new Peao(tab, Cor.Preta, this));
            colocarNovapeca('f', 7, new Peao(tab, Cor.Preta, this));
            colocarNovapeca('g', 7, new Peao(tab, Cor.Preta, this));
            colocarNovapeca('h', 7, new Peao(tab, Cor.Preta, this));

        }


    }
}
