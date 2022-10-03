#define CG_Debug
#define CG_OpenGL
// #define CG_DirectX

using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;

namespace gcgcg
{
  internal class CirculoPontoCentral : ObjetoGeometria
  {
    public CirculoPontoCentral(char rotulo, Objeto paiRef, double raio, int qtdPontos, Ponto4D ptoCentral) : base(rotulo, paiRef)
    {
        double angulo = 360 / qtdPontos;
        double anguloPonto = angulo;
        
        for(int i =0; i < qtdPontos; i++){
            ;
            
            base.PontosAdicionar(ajustarPontoAoCentro(ptoCentral, Matematica.GerarPtosCirculo(anguloPonto, raio)));
            anguloPonto += angulo;
        }
    }

    protected Ponto4D ajustarPontoAoCentro(Ponto4D ptCentral, Ponto4D ptBorda){
            ptBorda.X = ptBorda.X + ptCentral.X;
            ptBorda.Y = ptBorda.Y + ptCentral.Y;
    
    return ptBorda;
    }

    protected override void DesenharObjeto(){
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