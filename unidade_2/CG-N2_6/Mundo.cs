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

    private Ponto4D point1;
    private Ponto4D point2;
    private Ponto4D point3;
    private Ponto4D point4;

    private SegReta segReta1;
    private SegReta segReta2;
    private SegReta segReta3;

    private Ponto4D selectedPoint;

    private int qtySplinePoint;
    private Spline spline;
#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      camera.xmin = -400; camera.xmax = 400; camera.ymin = -400; camera.ymax = 400;

      Console.WriteLine(" --- Ajuda / Teclas: ");
      Console.WriteLine(" [  H     ] mostra teclas usadas. ");

      DrawScene(true);

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

    private void DrawScene(bool shouldStartFromBeginning = false)
    {
      bool isInitilization = objetosLista.Count <= 0;
      if (shouldStartFromBeginning)
      {
        point1 = new Ponto4D(-100, -100, 0);
        point2 = new Ponto4D(-100, 100, 0);
        point3 = new Ponto4D(100, 100, 0);
        point4 = new Ponto4D(100, -100, 0);

        selectedPoint = point1;
        qtySplinePoint = 10;
      }

      objetosLista = new List<Objeto>();

      objetoId = Utilitario.charProximo(objetoId);
      segReta1 = new SegReta(objetoId, null, point1, point2);
      segReta1.ObjetoCor = new Cor(0, 255, 255, 255);
      segReta1.PrimitivaTipo = PrimitiveType.Lines;
      segReta1.PrimitivaTamanho = 2;
      objetosLista.Add(segReta1);

      objetoId = Utilitario.charProximo(objetoId);
      segReta2 = new SegReta(objetoId, null, point2, point3);
      segReta2.ObjetoCor = new Cor(0, 255, 255, 255);
      segReta2.PrimitivaTipo = PrimitiveType.Lines;
      segReta2.PrimitivaTamanho = 2;
      objetosLista.Add(segReta2);

      objetoId = Utilitario.charProximo(objetoId);
      segReta3 = new SegReta(objetoId, null, point3, point4);
      segReta3.ObjetoCor = new Cor(0, 255, 255, 255);
      segReta3.PrimitivaTipo = PrimitiveType.Lines;
      segReta3.PrimitivaTamanho = 2;
      objetosLista.Add(segReta3);

      objetoId = Utilitario.charProximo(objetoId);
      spline = new Spline(objetoId, null, point1, point2, point3, point4, qtySplinePoint);
      spline.ObjetoCor = new Cor(255, 255, 0, 255);
      spline.PrimitivaTipo = PrimitiveType.LineStrip;
      spline.PrimitivaTamanho = 5;
      objetosLista.Add(spline);
      objetoSelecionado = spline;
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
      else if (e.Key == Key.E)
      {
        MovePoint(selectedPoint, -5, 0);
      }
      else if (e.Key == Key.D)
      {
        MovePoint(selectedPoint, 5, 0);
      }
      else if (e.Key == Key.C)
      {
        MovePoint(selectedPoint, 0, 5);
      }
      else if (e.Key == Key.B)
      {
        MovePoint(selectedPoint, 0, -5);
      }
      else if (e.Key == Key.R)
      {
        DrawScene(true);
      }
      else if (e.Key == Key.Plus || e.Key == Key.KeypadPlus)
      {
        IncSplinePoints(1);
      }
      else if (e.Key == Key.Minus || e.Key == Key.KeypadMinus)
      {
        IncSplinePoints(-1);
      }
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
      else if (e.Key == Key.Number1)
      {
        objetoSelecionado = segReta1;
        selectedPoint = point1;
      }
      else if (e.Key == Key.Number2)
      {
        objetoSelecionado = segReta1;
        selectedPoint = point2;
      }
      else if (e.Key == Key.Number3)
      {
        objetoSelecionado = segReta2;
        selectedPoint = point3;
      }
      else if (e.Key == Key.Number4)
      {
        objetoSelecionado = segReta3;
        selectedPoint = point4;
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

    private void MovePoint(Ponto4D point, int displacementInX, int displacementInY)
    {
      point.X += displacementInX;
      point.Y += displacementInY;

      DrawScene();
    }

    private bool IncSplinePoints(int pointIncrease)
    {
      if (pointIncrease < 0 && qtySplinePoint <= 2)
      {
        return false;
      }
      else if (pointIncrease > 1 && qtySplinePoint >= 15)
      {
        return false;
      }

      this.qtySplinePoint += pointIncrease;
      spline.CalculateSplinePoints(point1, point2, point3, point4, qtySplinePoint);

      return true;
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
      Mundo window = Mundo.GetInstance(600, 600);
      window.Title = "CG_N2_6";
      window.Run(1.0 / 60.0);
    }
  }
}
