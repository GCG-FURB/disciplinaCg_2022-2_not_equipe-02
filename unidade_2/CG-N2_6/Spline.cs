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
    public Spline(char rotulo, Objeto paiRef, Ponto4D ptoCimaEsq, Ponto4D ptoCimaDir, Ponto4D ptoBaixoEsq, Ponto4D ptoBaixoDir, int qtdPontos) : base(rotulo, paiRef)
    {
      CalculateSplinePoints(ptoCimaEsq, ptoCimaDir, ptoBaixoEsq, ptoBaixoDir, qtdPontos);
    }

    public void CalculateSplinePoints(Ponto4D ptoCimaEsq, Ponto4D ptoCimaDir, Ponto4D ptoBaixoEsq, Ponto4D ptoBaixoDir, int qtdPontos)
    {
      base.PontosRemoverTodos();
      double t = 1.0 / qtdPontos;

      for (double i = 0; i <= 1; i += t)
      {
        double x = (Math.Pow((1 - i), 3) * ptoCimaEsq.X) + (3 * i * Math.Pow((1 - i), 2) * ptoCimaDir.X) + (3 * Math.Pow(i, 2) * (1 - i) * ptoBaixoEsq.X) + (Math.Pow(i, 3) * ptoBaixoDir.X);
        double y = (Math.Pow((1 - i), 3) * ptoCimaEsq.Y) + (3 * i * Math.Pow((1 - i), 2) * ptoCimaDir.Y) + (3 * Math.Pow(i, 2) * (1 - i) * ptoBaixoEsq.Y) + (Math.Pow(i, 3) * ptoBaixoDir.Y);
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