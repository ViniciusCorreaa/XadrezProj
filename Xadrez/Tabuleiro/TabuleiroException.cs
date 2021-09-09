using System;

namespace tabuleiro
{
    //Criando Exception personalizado 
    class TabuleiroException : Exception
    {
        public TabuleiroException(string msg) : base(msg)
        {

        }

    }
}
