/**
  Autor: Dalton Solano dos Reis
**/

//#define CG_Privado // código do professor.
#define CG_Gizmo  // debugar gráfico.
#define CG_Debug // debugar texto.
#define CG_OpenGL // render OpenGL.
//#define CG_DirectX // render DirectX.

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK.Input;
using CG_Biblioteca;

namespace gcgcg
{
  class Mundo : GameWindow
  {
    private static Mundo instanciaMundo = null;

    private Mundo(int width, int height) : base(width, height) { }

    public static Mundo GetInstance(int width, int height)
    {
      if (instanciaMundo == null)
        instanciaMundo = new Mundo(width, height);
      return instanciaMundo;
    }

    private CameraOrtho camera = new CameraOrtho();
    protected List<Objeto> objetosLista = new List<Objeto>();
    private ObjetoGeometria objetoSelecionado = null;
    private char objetoId = '@';
    private bool bBoxDesenhar = false;
    int mouseX, mouseY;   //TODO: achar método MouseDown para não ter variável Global
    private bool mouseMoverPto = false;
    private int qtySplinePoint;
#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      camera.xmin = 0; camera.xmax = 600; camera.ymin = 0; camera.ymax = 600;

      Console.WriteLine(" --- Ajuda / Teclas: ");
      Console.WriteLine(" [  H     ] mostra teclas usadas. ");

      objetoId = Utilitario.charProximo(objetoId);

      Poligono poligon = new Poligono(objetoId, null);
      poligon.PontosAdicionar(new Ponto4D(50, 50));
      poligon.PontosAdicionar(new Ponto4D(350, 350));
      poligon.PontosAdicionar(new Ponto4D(350, 50));
      poligon.PontosAdicionar(new Ponto4D(50, 350));

      objetosLista.Add(poligon);

      objetoSelecionado = poligon;

#if CG_Privado
      objetoId = Utilitario.charProximo(objetoId);
      obj_SegReta = new Privado_SegReta(objetoId, null, new Ponto4D(50, 150), new Ponto4D(150, 250));
      obj_SegReta.ObjetoCor.CorR = 255; obj_SegReta.ObjetoCor.CorG = 255; obj_SegReta.ObjetoCor.CorB = 0;
      objetosLista.Add(obj_SegReta);
      objetoSelecionado = obj_SegReta;

      objetoId = Utilitario.charProximo(objetoId);
      obj_Circulo = new Privado_Circulo(objetoId, null, new Ponto4D(100, 300), 50);
      obj_Circulo.ObjetoCor.CorR = 0; obj_Circulo.ObjetoCor.CorG = 255; obj_Circulo.ObjetoCor.CorB = 255;
      obj_Circulo.PrimitivaTipo = PrimitiveType.Points;
      obj_Circulo.PrimitivaTamanho = 5;
      objetosLista.Add(obj_Circulo);
      objetoSelecionado = obj_Circulo;
#endif
#if CG_OpenGL
      GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
#endif
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);
#if CG_OpenGL
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadIdentity();
      GL.Ortho(camera.xmin, camera.xmax, camera.ymin, camera.ymax, camera.zmin, camera.zmax);
#endif
    }
    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);
#if CG_OpenGL
      GL.Clear(ClearBufferMask.ColorBufferBit);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();
#endif
#if CG_Gizmo      
      Sru3D();
#endif
      for (var i = 0; i < objetosLista.Count; i++)
        objetosLista[i].Desenhar();
#if CG_Gizmo
      if (bBoxDesenhar && (objetoSelecionado != null))
        objetoSelecionado.BBox.Desenhar();
#endif
      this.SwapBuffers();
    }

    protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
    {
      if (e.Key == Key.H)
        Utilitario.AjudaTeclado();
      else if (e.Key == Key.Escape)
        Exit();
      else if (e.Key == Key.I)
      {
        if (camera.xmax - camera.xmin >= 300)
        {
          camera.ZoomIn();
        }
        else
        {
          Console.WriteLine("Limite de zoom in atingido");
        }
      }
      else if (e.Key == Key.O)
      {
        if (camera.xmax - camera.xmin <= 700)
        {
          camera.ZoomOut();
        }
        else
        {
          Console.WriteLine("Limite de zoom out atingido");
        }
      }
#if CG_Gizmo
      else if (e.Key == Key.O)
        bBoxDesenhar = !bBoxDesenhar;
#endif
      else if (e.Key == Key.V)
        mouseMoverPto = !mouseMoverPto;   //TODO: falta atualizar a BBox do objeto
      else
        Console.WriteLine(" __ Tecla não implementada.");
    }

    //TODO: não está considerando o NDC
    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
      mouseX = e.Position.X; mouseY = 600 - e.Position.Y; // Inverti eixo Y
      if (mouseMoverPto && (objetoSelecionado != null))
      {
        objetoSelecionado.PontosUltimo().X = mouseX;
        objetoSelecionado.PontosUltimo().Y = mouseY;
      }
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      Console.WriteLine("Mouse X: " + mouseX);
      Console.WriteLine("Mouse Y: " + mouseY);
      bool foundAnObject = DidSelectAnObject(this.mouseX, this.mouseY);

      if (!foundAnObject)
      {
        bBoxDesenhar = false;
      }
    }

    private bool DidSelectAnObject(int mouseX, int mouseY)
    {
      Ponto4D clickPoint = new Ponto4D(mouseX, mouseY);

      ObjetoGeometria objetoGeometria;

      foreach (Objeto objeto in objetosLista)
      {
        objetoGeometria = (ObjetoGeometria)objeto;

        if (IsPointInsideObject(objetoGeometria, clickPoint))
        {
          bBoxDesenhar = true;
          objeto.BBox.Desenhar();
          return true;
        }
      }

      return false;
    }

    private bool IsPointInsideObject(ObjetoGeometria objetoGeometria, Ponto4D clickPoint)
    {
      int qtyIntersections = 0;

      Ponto4D intersectionPoint, objCurrentVertex, objNextVertex;

      for (int i = 0; i < objetoGeometria.pontosLista.Count; i++)
      {
        if (i == (objetoGeometria.pontosLista.Count - 1)) // no ultimo indice, current é o ultimo elemento e next é o primeiro
        {
          objCurrentVertex = objetoGeometria.pontosLista[i];
          objNextVertex = objetoGeometria.pontosLista[0];
        }
        else
        {
          objCurrentVertex = objetoGeometria.pontosLista[i];
          objNextVertex = objetoGeometria.pontosLista[i + 1];
        }

        if (objCurrentVertex.Y != objNextVertex.Y)
        {
          intersectionPoint = GetIntersectionPointWithLineL(clickPoint, objCurrentVertex, objNextVertex);

          if (intersectionPoint == null)
          { // não faz interseccao na horizontal com esse lado em questao
            continue;
          }

          else if (intersectionPoint.X == clickPoint.X)
          { //Ponto sobre um dos lados
            return true;
          }


          else if (intersectionPoint.X > clickPoint.X) // a interseccao esta a direita?
          {
            qtyIntersections++;
          }

        }
        else if
        (
          clickPoint.Y == objCurrentVertex.Y
          && clickPoint.X >= Math.Min(objCurrentVertex.X, objNextVertex.X)
          && clickPoint.X <= Math.Max(objCurrentVertex.X, objNextVertex.X)
        ) // ponto selecionado está sobre o lado horizontal
        {
          return true;
        }
      }

      Console.WriteLine("Intersections: " + qtyIntersections);

      if (qtyIntersections % 2 == 1)
      {
        return true;
      }

      return false;
    }

    private Ponto4D GetIntersectionPointWithLineL(Ponto4D clickPoint, Ponto4D point1, Ponto4D point2)
    {
      if (clickPoint.Y > Math.Max(point1.Y, point2.Y))
      {
        return null;
      }

      if (clickPoint.Y < Math.Min(point1.Y, point2.Y))
      {
        return null;
      }

      double y = clickPoint.Y;

      double x = GetIntersectionXGivenIntersectionYAnd2PointsOfTheLine(point1, point2, y);

      return new Ponto4D(x, y);
    }

    private double GetIntersectionXGivenIntersectionYAnd2PointsOfTheLine(Ponto4D point1, Ponto4D point2, double y)
    {
      double x1 = point1.X;
      double y1 = point1.Y;

      double x2 = point2.X;
      double y2 = point2.Y;

      return ((x1 * y2) + (x2 * y) + (-1 * y1 * x2) + (-1 * x1 * y)) / (y2 - y1);
    }

#if CG_Gizmo
    private void Sru3D()
    {
#if CG_OpenGL
      GL.LineWidth(1);
      GL.Begin(PrimitiveType.Lines);
      // GL.Color3(1.0f,0.0f,0.0f);
      GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
      GL.Vertex3(0, 0, 0); GL.Vertex3(200, 0, 0);
      // GL.Color3(0.0f,1.0f,0.0f);
      GL.Color3(Convert.ToByte(0), Convert.ToByte(255), Convert.ToByte(0));
      GL.Vertex3(0, 0, 0); GL.Vertex3(0, 200, 0);
      // GL.Color3(0.0f,0.0f,1.0f);
      GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(255));
      GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 200);
      GL.End();
#endif
    }

#endif    
  }

  class Program
  {
    static void Main(string[] args)
    {
      ToolkitOptions.Default.EnableHighResolution = false;
      Mundo window = Mundo.GetInstance(600, 600);
      window.Title = "CG_N3";
      window.Run(1.0 / 60.0);
    }
  }
}
