using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System.Collections.Generic;
using System;
using System.Threading;

namespace gcgcg
{
    internal class Cubo : ObjetoGeometria
    {
        private bool exibeVetorNormal = false;
        private bool animacao = false;
        public Cubo(char rotulo, Objeto paiRef) : base(rotulo, paiRef)
        {
            base.PontosAdicionar(new Ponto4D(-0.5, -0.5, 0.5)); // PtoA listaPto[0]
            base.PontosAdicionar(new Ponto4D(0.5, -0.5, 0.5)); // PtoB listaPto[1]
            base.PontosAdicionar(new Ponto4D(0.5, 0.5, 0.5)); // PtoC listaPto[2]
            base.PontosAdicionar(new Ponto4D(-0.5, 0.5, 0.5)); // PtoD listaPto[3]
            base.PontosAdicionar(new Ponto4D(-0.5, -0.5, -0.5)); // PtoE listaPto[4]
            base.PontosAdicionar(new Ponto4D(0.5, -0.5, -0.5)); // PtoF listaPto[5]
            base.PontosAdicionar(new Ponto4D(0.5, 0.5, -0.5)); // PtoG listaPto[6]
            base.PontosAdicionar(new Ponto4D(-0.5, 0.5, -0.5)); // PtoH listaPto[7]
        }

        protected override void DesenharObjeto()
        {       // Sentido anti-horário
            GL.Begin(PrimitiveType.Quads);
            // Face da frente (vermelho)
            //GL.Color3(1.0f,0.0f,0.0f);
            GL.Normal3(0, 0, 1);
            GL.Vertex3(base.pontosLista[0].X, base.pontosLista[0].Y, base.pontosLista[0].Z);    // PtoA
            GL.Vertex3(base.pontosLista[1].X, base.pontosLista[1].Y, base.pontosLista[1].Z);    // PtoB
            GL.Vertex3(base.pontosLista[2].X, base.pontosLista[2].Y, base.pontosLista[2].Z);    // PtoC
            GL.Vertex3(base.pontosLista[3].X, base.pontosLista[3].Y, base.pontosLista[3].Z);    // PtoD
                                                                                                // Face do fundo (verde)
                                                                                                //GL.Color3(0.0f,1.0f,0.0f);
            GL.Normal3(0, 0, -1);
            GL.Vertex3(base.pontosLista[4].X, base.pontosLista[4].Y, base.pontosLista[4].Z);    // PtoE
            GL.Vertex3(base.pontosLista[7].X, base.pontosLista[7].Y, base.pontosLista[7].Z);    // PtoH
            GL.Vertex3(base.pontosLista[6].X, base.pontosLista[6].Y, base.pontosLista[6].Z);    // PtoG
            GL.Vertex3(base.pontosLista[5].X, base.pontosLista[5].Y, base.pontosLista[5].Z);    // PtoF
                                                                                                // Face de cima (azul)
                                                                                                //GL.Color3(0.0f,0.0f,1.0f);
            GL.Normal3(0, 1, 0);
            GL.Vertex3(base.pontosLista[3].X, base.pontosLista[3].Y, base.pontosLista[3].Z);    // PtoD
            GL.Vertex3(base.pontosLista[2].X, base.pontosLista[2].Y, base.pontosLista[2].Z);    // PtoC
            GL.Vertex3(base.pontosLista[6].X, base.pontosLista[6].Y, base.pontosLista[6].Z);    // PtoG
            GL.Vertex3(base.pontosLista[7].X, base.pontosLista[7].Y, base.pontosLista[7].Z);    // PtoH
                                                                                                // Face de baixo (amarelo)
                                                                                                // GL.Color3(1.0f,1.0f,0.0f);
            GL.Normal3(0, -1, 0);
            GL.Vertex3(base.pontosLista[0].X, base.pontosLista[0].Y, base.pontosLista[0].Z);    // PtoA
            GL.Vertex3(base.pontosLista[4].X, base.pontosLista[4].Y, base.pontosLista[4].Z);    // PtoE
            GL.Vertex3(base.pontosLista[5].X, base.pontosLista[5].Y, base.pontosLista[5].Z);    // PtoF
            GL.Vertex3(base.pontosLista[1].X, base.pontosLista[1].Y, base.pontosLista[1].Z);    // PtoB
                                                                                                // Face da direita (ciano)
                                                                                                //.Color3(0.0f,1.0f,1.0f);
            GL.Normal3(1, 0, 0);
            GL.Vertex3(base.pontosLista[1].X, base.pontosLista[1].Y, base.pontosLista[1].Z);    // PtoB
            GL.Vertex3(base.pontosLista[5].X, base.pontosLista[5].Y, base.pontosLista[5].Z);    // PtoF
            GL.Vertex3(base.pontosLista[6].X, base.pontosLista[6].Y, base.pontosLista[6].Z);    // PtoG
            GL.Vertex3(base.pontosLista[2].X, base.pontosLista[2].Y, base.pontosLista[2].Z);    // PtoC
                                                                                                // Face da esquerda (magenta)
                                                                                                //GL.Color3(1.0f,0.0f,1.0f);
            GL.Normal3(-1, 0, 0);
            GL.Vertex3(base.pontosLista[0].X, base.pontosLista[0].Y, base.pontosLista[0].Z);    // PtoA
            GL.Vertex3(base.pontosLista[3].X, base.pontosLista[3].Y, base.pontosLista[3].Z);    // PtoD
            GL.Vertex3(base.pontosLista[7].X, base.pontosLista[7].Y, base.pontosLista[7].Z);    // PtoH
            GL.Vertex3(base.pontosLista[4].X, base.pontosLista[4].Y, base.pontosLista[4].Z);    // PtoE
            GL.End();

            // if (exibeVetorNormal) //TODO: acho que não precisa.
            //   ajudaExibirVetorNormal(); //TODO: acho que não precisa.
        }

        public void Animar(Ponto4D ptBaixoDir, Ponto4D ptCimaDir, Ponto4D ptCimaEsq, Ponto4D ptBaixoEsq, int qtdPontos)
        {
            Random rd = new Random();
            List<Ponto4D> pontos = Matematica.GerarPtosSpline(ptBaixoDir, ptCimaDir, ptCimaEsq, ptBaixoEsq, qtdPontos);
            foreach (Ponto4D ponto in pontos)
            {
                // ver como corrigir
                this.AtribuirIdentidade();
                this.Translacao(ponto.X, 'x');
                this.Translacao(ponto.Y, 'y');
                this.Translacao(ponto.Z, 'z');

                this.Rotacao(rd.Next(5, 20), 'z');
                this.Rotacao(rd.Next(1, 10), 'x');
                this.Rotacao(rd.Next(1, 10), 'y');
                Thread.Sleep(0050);
            }
        }

        public void setAnimacao(bool animacao)
        {
            this.animacao = animacao;
        }

        public bool getAnimacao()
        {
            return this.animacao;
        }

        //TODO: melhorar para exibir não só a lista de pontos (geometria), mas também a topologia ... poderia ser listado estilo OBJ da Wavefrom
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Cubo: " + base.rotulo + "\n";
            for (var i = 0; i < pontosLista.Count; i++)
            {
                retorno += "P" + i + "[" + pontosLista[i].X + "," + pontosLista[i].Y + "," + pontosLista[i].Z + "," + pontosLista[i].W + "]" + "\n";
            }
            return (retorno);
        }

    }
}