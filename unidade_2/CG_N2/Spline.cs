/**
  Autor: Dalton Solano dos Reis
**/

#define CG_Debug
#define CG_OpenGL
// #define CG_DirectX

using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System;

namespace gcgcg
{
    internal class Spline : ObjetoGeometria
    {
        public Spline(char rotulo, Objeto paiRef, Ponto4D ptoEsq, Ponto4D ptoCentro, Ponto4D ptoDir, int qtdPontos) : base(rotulo, paiRef)
        {
            CalculateSplinePoints(ptoEsq, ptoCentro, ptoDir, qtdPontos);
        }

        public void CalculateSplinePoints(Ponto4D ptoEsq, Ponto4D ptoCentro, Ponto4D ptoDir, int qtdPontos)
        {
            base.PontosRemoverTodos();
            double t = 1.0 / qtdPontos;

            for (double i = 0; i <= 1; i += t)
            {
                //funcao bezier P(t) = P0*t^2 + P1*2*t*(1-t) + P2*(1-t)^2
                double x = (ptoEsq.X * Math.Pow(i, 2)) + (ptoCentro.X * 2 * i * (1 - i)) + (ptoDir.X * Math.Pow(1 - i, 2));
                double y = (ptoEsq.Y * Math.Pow(i, 2)) + (ptoCentro.Y * 2 * i * (1 - i)) + (ptoDir.Y * Math.Pow(1 - i, 2));
                base.PontosAdicionar(new Ponto4D(x, y, 0));
            }
        }

        protected override void DesenharObjeto()
        {
#if CG_OpenGL && !CG_DirectX
            GL.Begin(base.PrimitivaTipo);
            foreach (Ponto4D pto in pontosLista)
            {
                GL.Vertex2(pto.X, pto.Y); ;
            }


            GL.End();
#elif CG_DirectX && !CG_OpenGL
    Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
    Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
        }

        //TODO: melhorar para exibir não só a lista de pontos (geometria), mas também a topologia ... poderia ser listado estilo OBJ da Wavefrom
#if CG_Debug
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Retangulo: " + base.rotulo + "\n";
            for (var i = 0; i < pontosLista.Count; i++)
            {
                retorno += "P" + i + "[" + pontosLista[i].X + "," + pontosLista[i].Y + "," + pontosLista[i].Z + "," + pontosLista[i].W + "]" + "\n";
            }
            return (retorno);
        }
#endif

    }
}