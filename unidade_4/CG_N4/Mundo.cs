#define CG_Gizmo
// #define CG_Privado

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK.Input;
using CG_Biblioteca;
using System.Threading;

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
    private Cubo dado;
    private bool movingCamera = false;
    private const double LARGURA_CHAO = 50;
    private const double PROFUNDIDADE_CHAO = 50;
    const double COMPRIMENTO_ARESTA_DADO = 2;
    private float previousMouseX = 0;
    private int actualDegreeHorizontal = 0;
    private float previousMouseY = 0;
    private int actualDegreeVertical = 0;
    private int cameraRadius = 20;
    private bool ligaLuz = true;

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
      far = 100.0f;
      eye = new Vector3(15, 25, 40);
      at = new Vector3(0, 0, 0);
      up = new Vector3(0, 1, 0);

      //TODO entender melhor o proposito e funcionamento da luz
      // Enable Light 0 and set its parameters.
      GL.Light(LightName.Light0, LightParameter.Position, new float[] { 0.0f, 2.0f, 0.0f });
      GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
      GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
      GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
      GL.Light(LightName.Light0, LightParameter.SpotExponent, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
      GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0.2f, 0.2f, 0.2f, 1.0f });
      GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
      GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);

      Console.WriteLine(" --- Ajuda / Teclas: ");
      Console.WriteLine(" [  H     ] mostra teclas usadas. ");

      objetosLista.Add(Floor(LARGURA_CHAO, PROFUNDIDADE_CHAO));

      objetoId = Utilitario.charProximo(objetoId);

      dado = Dice();
      objetosLista.Add(dado);
      objetoSelecionado = dado;
      Thread thread = new Thread(new ThreadStart(AnimarDado));
      thread.Start();


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

    private Cubo Dice()
    {
      if (ligaLuz)
      {
        GL.Enable(EnableCap.Lighting);
        GL.Enable(EnableCap.Light0);
        GL.Enable(EnableCap.ColorMaterial);
      }

      objetoId = Utilitario.charProximo(objetoId);

      Cubo dice = new Cubo(objetoId, null);

      dice.EscalaXYZBBox(COMPRIMENTO_ARESTA_DADO, COMPRIMENTO_ARESTA_DADO, COMPRIMENTO_ARESTA_DADO);
      dice.Translacao(COMPRIMENTO_ARESTA_DADO / 2, 'x');
      dice.Translacao(COMPRIMENTO_ARESTA_DADO / 2, 'y');
      dice.Translacao(COMPRIMENTO_ARESTA_DADO / 2, 'z');

      dice.ObjetoCor.CorR = 255;
      dice.ObjetoCor.CorG = 255;
      dice.ObjetoCor.CorB = 255;

      dice.FilhoAdicionar(DiceCircle(0, 0, -0.5));

      dice.FilhoAdicionar(DiceCircle(-0.5, -0.3, -0.3));
      dice.FilhoAdicionar(DiceCircle(-0.5, 0.3, 0.3));

      dice.FilhoAdicionar(DiceCircle(-0.3, 0.5, -0.3));
      dice.FilhoAdicionar(DiceCircle(0, 0.5, 0));
      dice.FilhoAdicionar(DiceCircle(0.3, 0.5, 0.3));

      dice.FilhoAdicionar(DiceCircle(-0.3, -0.5, 0.3));
      dice.FilhoAdicionar(DiceCircle(-0.3, -0.5, -0.3));
      dice.FilhoAdicionar(DiceCircle(0.3, -0.5, 0.3));
      dice.FilhoAdicionar(DiceCircle(0.3, -0.5, -0.3));

      dice.FilhoAdicionar(DiceCircle(0.5, 0.3, 0.3));
      dice.FilhoAdicionar(DiceCircle(0.5, -0.3, 0.3));
      dice.FilhoAdicionar(DiceCircle(0.5, 0, 0));
      dice.FilhoAdicionar(DiceCircle(0.5, 0.3, -0.3));
      dice.FilhoAdicionar(DiceCircle(0.5, -0.3, -0.3));

      dice.FilhoAdicionar(DiceCircle(0.3, -0.3, 0.5));
      dice.FilhoAdicionar(DiceCircle(0.3, 0, 0.5));
      dice.FilhoAdicionar(DiceCircle(0.3, 0.3, 0.5));
      dice.FilhoAdicionar(DiceCircle(-0.3, -0.3, 0.5));
      dice.FilhoAdicionar(DiceCircle(-0.3, 0, 0.5));
      dice.FilhoAdicionar(DiceCircle(-0.3, 0.3, 0.5));

      if (ligaLuz)
      {
        GL.Disable(EnableCap.Lighting);
        GL.Disable(EnableCap.Light0);
      }

      return dice;
    }

    private void AnimarDado()
    {
      Random rd = new Random();
      while (true)
      {
        if (dado.getAnimacao())
        {
          Ponto4D ptBaixoEsq = new Ponto4D(0, 0, 0);
          Ponto4D ptBaixoDir = new Ponto4D(rd.Next(5, Convert.ToInt32(LARGURA_CHAO) - 10), 1, rd.Next(5, Convert.ToInt32(PROFUNDIDADE_CHAO) - 10));
          Ponto4D ptCimaEsq = new Ponto4D(rd.Next(10, 30), 15, rd.Next(10, 30));
          Ponto4D ptCimaDir = new Ponto4D(rd.Next(10, 30), 15, rd.Next(10, 30));

          dado.AtribuirIdentidade();
          dado.EscalaXYZBBox(COMPRIMENTO_ARESTA_DADO, COMPRIMENTO_ARESTA_DADO, COMPRIMENTO_ARESTA_DADO);
          dado.Translacao(COMPRIMENTO_ARESTA_DADO / 2, 'x');
          dado.Translacao(COMPRIMENTO_ARESTA_DADO / 2, 'y');
          dado.Translacao(COMPRIMENTO_ARESTA_DADO / 2, 'z');
          dado.Animar(ptBaixoDir, ptCimaDir, ptCimaEsq, ptBaixoEsq, 20);
          dado.setAnimacao(false);
        }
      }
    }

    private Circulo DiceCircle(double translationX, double translationY, double translationZ)
    {
      objetoId = Utilitario.charProximo(objetoId);

      Circulo circle = new Circulo(objetoId, null, 0.03, 20);
      circle.PrimitivaTipo = PrimitiveType.Points;
      circle.PrimitivaTamanho = 5;

      circle.Translacao(translationX, 'x');
      circle.Translacao(translationY, 'y');
      circle.Translacao(translationZ, 'z');

      circle.ObjetoCor.CorR = 0; circle.ObjetoCor.CorG = 0; circle.ObjetoCor.CorB = 0;

      return circle;
    }

    private ObjetoGeometria Floor(double width, double depth)
    {
      objetoId = Utilitario.charProximo(objetoId);

      Cubo floor = new Cubo(objetoId, null);

      Objeto objetoFloor = (Objeto)floor;
      objetoFloor.EscalaXYZBBox(width, 1, depth);
      objetoFloor.Translacao(width / 2, 'x');
      objetoFloor.Translacao(-0.5, 'y');
      objetoFloor.Translacao(depth / 2, 'z');

      ObjetoGeometria objetoGeometriaFloor = (ObjetoGeometria)floor;

      objetoGeometriaFloor.ObjetoCor.CorR = 40;
      objetoGeometriaFloor.ObjetoCor.CorG = 40;
      objetoGeometriaFloor.ObjetoCor.CorB = 40;

      return objetoGeometriaFloor;
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
      else if (e.Key == Key.Number0)
      {
        eye = new Vector3(-20, 10, -20);
      }
      else if (e.Key == Key.Number1)
      {
        eye = new Vector3(Convert.ToInt32(LARGURA_CHAO) + 10, 5, 40);
      }
      else if (e.Key == Key.Number2)
      {
        eye = new Vector3(1, 5, 20);
      }
      else if (e.Key == Key.Number3)
      {
        eye = new Vector3(20, 5, 1);
      }
      else if (e.Key == Key.Number4)
      {
        eye = new Vector3(Convert.ToInt32(LARGURA_CHAO) + 10, 30, Convert.ToInt32(PROFUNDIDADE_CHAO) + 10);
      }
      else if (e.Key == Key.Number5)
      {
        eye = new Vector3(1, 20, 1);
      }
      else if (e.Key == Key.Space)
      {
        dado.setAnimacao(true);
      }
      else if (e.Key == Key.L)
      {
        ligaLuz = !ligaLuz;
      }

      else
        Console.WriteLine(" __ Tecla não implementada.");

      if (!(e.Key == Key.LShift)) //FIXME: não funciona.
        Console.WriteLine("__ " + menuSelecao + "[" + deslocamento + "]");
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
      if (movingCamera)
      {
        int newAngleDegreeHorizontal = GetNewDegreeForCameraHorizontal(e.X, this.previousMouseX, actualDegreeHorizontal);
        int newAngleDegreeVertical = GetNewDegreeForCameraVertical(e.Y, this.previousMouseY, actualDegreeVertical);

        Ponto4D newPoint = Matematica.GerarPtosCirculo(newAngleDegreeHorizontal, cameraRadius);
        eye = new Vector3((float)newPoint.X, eye.Y, (float)newPoint.Y);

        Ponto4D newPointVertical = Matematica.GerarPtosCirculo(newAngleDegreeVertical, cameraRadius);
        eye = new Vector3(eye.X, (float)(newPointVertical.Y * -1), eye.Z);

        this.previousMouseX = e.X;
        this.actualDegreeHorizontal = newAngleDegreeHorizontal;

        this.previousMouseY = e.Y;
        this.actualDegreeVertical = newAngleDegreeVertical;
      }
    }

    private int GetNewDegreeForCameraHorizontal(float actualX, float previousX, int actualDegree)
    {
      if (actualX == previousX)
      {
        return actualDegree;
      }

      bool isMovingToRightSide = (actualX - previousX > 0);

      if (isMovingToRightSide && actualDegree == 359) return 0;
      else if (isMovingToRightSide == false && actualDegree == 0) return 359;

      return isMovingToRightSide ? actualDegree += 1 : actualDegree -= 1;
    }

    private int GetNewDegreeForCameraVertical(float actualY, float previousY, int actualDegree)
    {
      if (actualY == previousY)
      {
        return actualDegree;
      }

      bool isMovingUp = (actualY - previousY > 0);

      if (isMovingUp && actualDegree == 359) return 0;
      else if (isMovingUp == false && actualDegree == 0) return 359;

      return isMovingUp ? actualDegree += 1 : actualDegree -= 1;
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);
      movingCamera = true;
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
      base.OnMouseUp(e);
      movingCamera = false;
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
      base.OnMouseWheel(e);
      cameraRadius = (e.Value * -1) + 20; //20 cause its the initial value
      Ponto4D newPoint = Matematica.GerarPtosCirculo(actualDegreeHorizontal, cameraRadius);
      eye = new Vector3((float)newPoint.X, eye.Y, (float)newPoint.Y);

      // Ideia de desprender o at por hora fora abandonada
      // Ponto4D newPointAt = Matematica.GerarPtosCirculo(actualDegreeHorizontal + 180, cameraRadius);
      // at = new Vector3((float)newPointAt.X, at.Y, (float)newPointAt.Y);
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
