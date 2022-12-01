#define CG_Gizmo
// #define CG_Privado

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
    private float fovy, aspect, near, far;
    private Vector3 eye, at, up;
    private Mundo(int width, int height) : base(width, height) { }

    public static Mundo GetInstance(int width, int height)
    {
      if (instanciaMundo == null)
        instanciaMundo = new Mundo(width, height);
      return instanciaMundo;
    }

    private CameraPerspective camera = new CameraPerspective();
    protected List<Objeto> objetosLista = new List<Objeto>();
    private ObjetoGeometria objetoSelecionado = null;
    private char objetoId = '@';
    private String menuSelecao = "";
    private char menuEixoSelecao = 'z';
    private float deslocamento = 0;
    private bool bBoxDesenhar = false;

#if CG_Privado
    private Cilindro obj_Cilindro;
    private Esfera obj_Esfera;
    private Cone obj_Cone;
#endif

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
      GL.Enable(EnableCap.DepthTest);
      GL.Enable(EnableCap.CullFace);
      // ___ parâmetros da câmera sintética
      fovy = (float)Math.PI / 4;
      aspect = Width / (float)Height;
      near = 1.0f;
      far = 50.0f;
      eye = new Vector3(5, 20, 40);
      // eye = new Vector3(20, 30, 20);
      at = new Vector3(0, 0, 0);
      up = new Vector3(0, 1, 0);

      Console.WriteLine(" --- Ajuda / Teclas: ");
      Console.WriteLine(" [  H     ] mostra teclas usadas. ");

      objetoId = Utilitario.charProximo(objetoId);
      const double LARGURA_CHAO = 10;
      const double PROFUNDIDADE_CHAO = 12;
      drawFloor(LARGURA_CHAO, PROFUNDIDADE_CHAO, objetoId);

      objetoId = Utilitario.charProximo(objetoId);
      objetoSelecionado = new Cubo(objetoId, null);
      objetosLista.Add(objetoSelecionado);
      objetoSelecionado.ObjetoCor.CorR = 255;
      objetoSelecionado.ObjetoCor.CorG = 255;
      objetoSelecionado.ObjetoCor.CorB = 255;
      Objeto objeto_dado = (Objeto)objetoSelecionado;
      objeto_dado.EscalaXYZBBox(5, 5, 5);
      objeto_dado.Translacao(2.5, 'x');
      objeto_dado.Translacao(2.5, 'y');
      objeto_dado.Translacao(2.5, 'z');


      objetoId = Utilitario.charProximo(objetoId);
      Circulo obj_Circulo = new Circulo(objetoId, null, 0.03, 1);
      obj_Circulo.ObjetoCor.CorR = 0; obj_Circulo.ObjetoCor.CorG = 0; obj_Circulo.ObjetoCor.CorB = 0;
      obj_Circulo.PrimitivaTipo = PrimitiveType.Points;
      obj_Circulo.PrimitivaTamanho = 5;
      objetoSelecionado.FilhoAdicionar(obj_Circulo);

      obj_Circulo.Translacao(0.5, 'z');

#if CG_Privado  //FIXME: arrumar os outros objetos
      objetoId = Utilitario.charProximo(objetoId);
      obj_Cilindro = new Cilindro(objetoId, null);
      obj_Cilindro.ObjetoCor.CorR = 177; obj_Cilindro.ObjetoCor.CorG = 166; obj_Cilindro.ObjetoCor.CorB = 136;
      objetosLista.Add(obj_Cilindro);
      obj_Cilindro.Translacao(2, 'x');

      objetoId = Utilitario.charProximo(objetoId);
      obj_Esfera = new Esfera(objetoId, null);
      obj_Esfera.ObjetoCor.CorR = 177; obj_Esfera.ObjetoCor.CorG = 166; obj_Esfera.ObjetoCor.CorB = 136;
      objetosLista.Add(obj_Esfera);
      obj_Esfera.Translacao(4, 'x');

      objetoId = Utilitario.charProximo(objetoId);
      obj_Cone = new Cone(objetoId, null);
      obj_Cone.ObjetoCor.CorR = 177; obj_Cone.ObjetoCor.CorG = 166; obj_Cone.ObjetoCor.CorB = 136;
      objetosLista.Add(obj_Cone);
      obj_Cone.Translacao(6, 'x');
#endif

      GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
      GL.Enable(EnableCap.DepthTest);
      GL.Enable(EnableCap.CullFace);
    }

    private ObjetoGeometria drawFloor(double width, double depth, char objetoId)
    {
      Cubo cube = new Cubo(objetoId, null);

      Objeto objetoCube = (Objeto)cube;
      objetoCube.EscalaXYZBBox(width, 1, depth);
      objetoCube.Translacao(width / 2, 'x');
      objetoCube.Translacao(-0.5, 'y');
      objetoCube.Translacao(depth / 2, 'z');

      ObjetoGeometria objetoGeometriaCube = (ObjetoGeometria)cube;

      objetoGeometriaCube.ObjetoCor.CorR = 40;
      objetoGeometriaCube.ObjetoCor.CorG = 40;
      objetoGeometriaCube.ObjetoCor.CorB = 40;

      objetosLista.Add(cube);

      return objetoGeometriaCube;
    }
    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);

      GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
      Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, near, far);
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadMatrix(ref projection);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);
    }
    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      Matrix4 modelview = Matrix4.LookAt(eye, at, up);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadMatrix(ref modelview);
#if CG_Gizmo      
      Sru3D();
#endif
      for (var i = 0; i < objetosLista.Count; i++)
        objetosLista[i].Desenhar();
      if (bBoxDesenhar && (objetoSelecionado != null))
        objetoSelecionado.BBox.Desenhar();
      this.SwapBuffers();
    }

    protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
    {
      // Console.Clear(); //TODO: não funciona.
      if (e.Key == Key.H) Utilitario.AjudaTeclado();
      else if (e.Key == Key.Escape) Exit();
      //--------------------------------------------------------------
      else if (e.Key == Key.Number9)
        objetoSelecionado = null;                     // desmacar objeto selecionado
      else if (e.Key == Key.B)
        bBoxDesenhar = !bBoxDesenhar;     //FIXME: bBox não está sendo atualizada.
      else if (e.Key == Key.E)
      {
        Console.WriteLine("--- Objetos / Pontos: ");
        for (var i = 0; i < objetosLista.Count; i++)
        {
          Console.WriteLine(objetosLista[i]);
        }
      }
      //--------------------------------------------------------------
      else if (e.Key == Key.X) menuEixoSelecao = 'x';
      else if (e.Key == Key.Y) menuEixoSelecao = 'y';
      else if (e.Key == Key.Z) menuEixoSelecao = 'z';
      else if (e.Key == Key.Minus) deslocamento--;
      else if (e.Key == Key.Plus) deslocamento++;
      else if (e.Key == Key.C) menuSelecao = "[menu] C: Câmera";
      else if (e.Key == Key.O) menuSelecao = "[menu] O: Objeto";

      // Menu: seleção
      else if (menuSelecao.Equals("[menu] C: Câmera")) camera.MenuTecla(e.Key, menuEixoSelecao, deslocamento);
      else if (menuSelecao.Equals("[menu] O: Objeto"))
      {
        if (objetoSelecionado != null) objetoSelecionado.MenuTecla(e.Key, menuEixoSelecao, deslocamento, bBoxDesenhar);
        else Console.WriteLine(" ... Objeto NÃO selecionado.");
      }

      else
        Console.WriteLine(" __ Tecla não implementada.");

      if (!(e.Key == Key.LShift)) //FIXME: não funciona.
        Console.WriteLine("__ " + menuSelecao + "[" + deslocamento + "]");
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
    }

#if CG_Gizmo
    private void Sru3D()
    {
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
    }
#endif    
  }
  class Program
  {
    static void Main(string[] args)
    {
      ToolkitOptions.Default.EnableHighResolution = false;
      Mundo window = Mundo.GetInstance(600, 600);
      window.Title = "CG_N4";
      window.Run(1.0 / 60.0);
    }
  }
}
