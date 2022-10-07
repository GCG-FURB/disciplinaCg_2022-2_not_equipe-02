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
        private Ponto4D pto1;
        private Ponto4D pto2;
        private Ponto4D pto3;
        private Ponto4D pto4;
        private SegReta sr1;
        private SegReta sr2;
        private SegReta sr3;
        private Spline spline;

        private Ponto4D pontoSelecionado;
        private int indicePonto;
        private int qtdPontosSpline;

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

            objetoId = Utilitario.charProximo(objetoId);


            pto1 = new Ponto4D(-100, 100, 0);
            pto2 = new Ponto4D(100, -100, 0);
            pto3 = new Ponto4D(-100, -100, 0);
            pto4 = new Ponto4D(100, 100, 0);

            sr1 = new SegReta(objetoId, null, pto1, pto3);
            sr1.ObjetoCor.CorR = 0; sr1.ObjetoCor.CorG = 255; sr1.ObjetoCor.CorB = 255;
            sr1.PrimitivaTipo = PrimitiveType.Lines;
            sr1.PrimitivaTamanho = 2;
            objetosLista.Add(sr1);

            sr2 = new SegReta(objetoId, null, pto2, pto4);
            sr2.ObjetoCor.CorR = 0; sr2.ObjetoCor.CorG = 255; sr2.ObjetoCor.CorB = 255;
            sr2.PrimitivaTipo = PrimitiveType.LineStrip;
            sr2.PrimitivaTamanho = 2;
            objetosLista.Add(sr2);

            sr3 = new SegReta(objetoId, null, pto1, pto4);
            sr3.ObjetoCor.CorR = 0; sr3.ObjetoCor.CorG = 255; sr3.ObjetoCor.CorB = 255;
            sr3.PrimitivaTipo = PrimitiveType.Lines;
            sr3.PrimitivaTamanho = 2;
            objetosLista.Add(sr3);

            qtdPontosSpline = 10;
            spline = new Spline(objetoId, null, pto3, pto1, pto4, pto2, qtdPontosSpline);
            spline.ObjetoCor.CorR = 255; spline.ObjetoCor.CorG = 255; spline.ObjetoCor.CorB = 0;
            spline.PrimitivaTipo = PrimitiveType.LineStrip;
            spline.PrimitivaTamanho = 5;
            objetosLista.Add(spline);
            objetoSelecionado = spline;

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
            GL.ClearColor(0.55f, 0.55f, 0.55f, 1f);
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

            else if (e.Key == Key.Number1)
            {
                objetoSelecionado = sr1;
                pontoSelecionado = pto1;
                indicePonto = 0;
            }

            else if (e.Key == Key.Number2)
            {
                objetoSelecionado = sr1;
                pontoSelecionado = pto3;
                indicePonto = 1;
            }

            else if (e.Key == Key.Number3)
            {
                objetoSelecionado = sr2;
                pontoSelecionado = pto2;
                indicePonto = 0;
            }

            else if (e.Key == Key.Number4)
            {
                objetoSelecionado = sr2;
                pontoSelecionado = pto4;
                indicePonto = 1;
            }

            else if (e.Key == Key.C && pontoSelecionado != null)
            {
                pontoSelecionado.Y = pontoSelecionado.Y + 10;
                objetoSelecionado.PontosAlterar(pontoSelecionado, indicePonto);
                spline.CalculateSplinePoints(pto3, pto1, pto4, pto2, qtdPontosSpline);
            }

            else if (e.Key == Key.B && pontoSelecionado != null)
            {
                pontoSelecionado.Y = pontoSelecionado.Y - 10;
                objetoSelecionado.PontosAlterar(pontoSelecionado, indicePonto);
                spline.CalculateSplinePoints(pto3, pto1, pto4, pto2, qtdPontosSpline);
            }

            else if (e.Key == Key.D && pontoSelecionado != null)
            {
                pontoSelecionado.X = pontoSelecionado.X + 10;
                objetoSelecionado.PontosAlterar(pontoSelecionado, indicePonto);
                spline.CalculateSplinePoints(pto3, pto1, pto4, pto2, qtdPontosSpline);
            }

            else if (e.Key == Key.E && pontoSelecionado != null)
            {
                pontoSelecionado.X = pontoSelecionado.X - 10;
                objetoSelecionado.PontosAlterar(pontoSelecionado, indicePonto);
                spline.CalculateSplinePoints(pto3, pto1, pto4, pto2, qtdPontosSpline);
            }

            else if (e.Key == Key.R)
            {
                pto1.X = -100; pto1.Y = 100;
                pto2.X = 100; pto2.Y = -100;
                pto3.X = -100; pto3.Y = -100;
                pto4.X = 100; pto4.Y = 100;
                spline.CalculateSplinePoints(pto3, pto1, pto4, pto2, qtdPontosSpline);
            }

            else if (e.Key == Key.Plus || e.Key == Key.KeypadPlus)
            {
                qtdPontosSpline++;
                spline.CalculateSplinePoints(pto3, pto1, pto4, pto2, qtdPontosSpline);
            }

            else if ((e.Key == Key.Minus || e.Key == Key.KeypadMinus) && qtdPontosSpline > 5)
            {
                qtdPontosSpline--;
                spline.CalculateSplinePoints(pto3, pto1, pto4, pto2, qtdPontosSpline);
            }

            else if (e.Key == Key.Escape)
                Exit();

#if CG_Gizmo


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
            window.Title = "CG_N2";
            window.Run(1.0 / 60.0);
        }
    }
}
