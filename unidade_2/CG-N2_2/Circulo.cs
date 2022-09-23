//#define CG_Privado // código do professor.
#define CG_Gizmo  // debugar gráfico.
#define CG_Debug // debugar texto.
#define CG_OpenGL // render OpenGL.
//#define CG_DirectX // render DirectX.

using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;

namespace gcgcg
{
  class Circulo : ObjetoGeometria
  {
    public Circulo(char rotulo, Objeto paiRef, int qtdePontos, double raio) : base(rotulo, paiRef)
    {
      Ponto4D ponto;

      double intervaloAngulo = 360 / qtdePontos;
      double angulo = 0;

      for (int i = 0; i < qtdePontos; i++)
      {
        ponto = Matematica.GerarPtosCirculo(angulo, raio);

        base.PontosAdicionar(ponto);

        angulo += intervaloAngulo;
      }
    }

    protected override void DesenharObjeto()
    {
#if CG_OpenGL && !CG_DirectX
      GL.Begin(base.PrimitivaTipo);
      foreach (Ponto4D pto in pontosLista)
      {
        GL.Vertex2(pto.X, pto.Y);
      }
      GL.End();
#elif CG_DirectX && !CG_OpenGL
    Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif      
    }
  }
}