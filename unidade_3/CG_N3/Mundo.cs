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

        private Key lastPressedKey;
        private int indexPonto;
        private Poligono objetoNovo = null;
#if CG_Privado
    private Retangulo obj_Retangulo;
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
            objetoNovo = new Poligono(objetoId, null);
            objetosLista.Add(objetoNovo);
            objetoNovo.PontosAdicionar(new Ponto4D(50, 50));
            objetoNovo.PontosAdicionar(new Ponto4D(350, 50));
            objetoNovo.PontosAdicionar(new Ponto4D(350, 350));
            objetoNovo.PontosAdicionar(new Ponto4D(50, 350));
            objetoSelecionado = objetoNovo;
            objetoNovo = null;

#if CG_Privado
      objetoId = Utilitario.charProximo(objetoId);
      obj_Retangulo = new Retangulo(objetoId, null, new Ponto4D(50, 50, 0), new Ponto4D(150, 150, 0));
      obj_Retangulo.ObjetoCor.CorR = 255; obj_Retangulo.ObjetoCor.CorG = 0; obj_Retangulo.ObjetoCor.CorB = 255;
      objetosLista.Add(obj_Retangulo);
      objetoSelecionado = obj_Retangulo;

      objetoId = Utilitario.charProximo(objetoId);
      obj_SegReta = new Privado_SegReta(objetoId, null, new Ponto4D(50, 150), new Ponto4D(150, 250));
      obj_SegReta.ObjetoCor.CorR = 255; obj_SegReta.ObjetoCor.CorG = 99; obj_SegReta.ObjetoCor.CorB = 71;
      objetosLista.Add(obj_SegReta);
      objetoSelecionado = obj_SegReta;

      objetoId = Utilitario.charProximo(objetoId);
      obj_Circulo = new Privado_Circulo(objetoId, null, new Ponto4D(100, 300), 50);
      obj_Circulo.ObjetoCor.CorR = 177; obj_Circulo.ObjetoCor.CorG = 166; obj_Circulo.ObjetoCor.CorB = 136;
      objetosLista.Add(obj_Circulo);
      objetoSelecionado = obj_Circulo;
#endif
            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(camera.xmin, camera.xmax, camera.ymin, camera.ymax, camera.zmin, camera.zmax);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
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
            if (e.Key == Key.H)
                Utilitario.AjudaTeclado();
            else if (e.Key == Key.Escape)
                Exit();
            else if (e.Key == Key.E)
            {
                Console.WriteLine("--- Objetos / Pontos: ");
                for (var i = 0; i < objetosLista.Count; i++)
                {
                    Console.WriteLine(objetosLista[i]);
                }
            }
            else if (e.Key == Key.O)
                bBoxDesenhar = !bBoxDesenhar;

            else if (e.Key == Key.S)
            { // questão 7
                objetoSelecionado = null;
                foreach (Objeto obj in objetosLista)
                { // filtra pela bbox
                    SelectObject(obj);
                }
            }
            else if (e.Key == Key.Enter)
            {

                // questão 3
                if (lastPressedKey == Key.P)
                {
                    objetoSelecionado.PontosAlterar(new Ponto4D(mouseX, mouseY), indexPonto);
                }
                else if (lastPressedKey == Key.A)
                {
                    objetoSelecionado.FilhoAdicionar(objetoNovo);
                }

                if (objetoNovo != null)
                {
                    objetoNovo.PontosRemoverUltimo();   // N3-Exe6: "truque" para deixar o rastro
                    objetoSelecionado = objetoNovo;
                    objetoNovo = null;
                }
            }
            else if (e.Key == Key.Space) // questão 5
            {
                if (objetoNovo == null)
                {
                    objetoId = Utilitario.charProximo(objetoId);
                    objetoNovo = new Poligono(objetoId, null);
                    objetosLista.Add(objetoNovo);
                    objetoNovo.PontosAdicionar(new Ponto4D(mouseX, mouseY));
                    objetoNovo.PontosAdicionar(new Ponto4D(mouseX, mouseY));  // N3-Exe6: "troque" para deixar o rastro
                }
                else
                    objetoNovo.PontosAdicionar(new Ponto4D(mouseX, mouseY));
            }
            else if (objetoSelecionado != null)
            {
                if (e.Key == Key.M)
                    Console.WriteLine(objetoSelecionado.Matriz);
                else if (e.Key == Key.D) // questão 2
                {
                    objetosLista.Remove(objetoSelecionado);
                }
                else if (e.Key == Key.P) // questão 3
                {
                    Ponto4D mouse = new Ponto4D(mouseX, mouseY);
                    indexPonto = GetClosestPoint(mouse);
                    lastPressedKey = Key.P;
                }
                else if (e.Key == Key.R) // questão 6
                {
                    objetoSelecionado.ObjetoCor.CorR = 255;
                    objetoSelecionado.ObjetoCor.CorG = 0;
                    objetoSelecionado.ObjetoCor.CorB = 0;
                }

                else if (e.Key == Key.G)// questão 6
                {
                    objetoSelecionado.ObjetoCor.CorR = 0;
                    objetoSelecionado.ObjetoCor.CorG = 255;
                    objetoSelecionado.ObjetoCor.CorB = 0;
                }

                else if (e.Key == Key.B) // questão 6
                {
                    objetoSelecionado.ObjetoCor.CorR = 0;
                    objetoSelecionado.ObjetoCor.CorG = 0;
                    objetoSelecionado.ObjetoCor.CorB = 255;
                }

                else if (e.Key == Key.I)
                    objetoSelecionado.AtribuirIdentidade();
                //TODO: não está atualizando a BBox com as transformações geométricas
                else if (e.Key == Key.Left) // questão 8
                    objetoSelecionado.TranslacaoXYZ(-10, 0, 0);
                else if (e.Key == Key.Right) // questão 8
                    objetoSelecionado.TranslacaoXYZ(10, 0, 0);
                else if (e.Key == Key.Up) // questão 8
                    objetoSelecionado.TranslacaoXYZ(0, 10, 0);
                else if (e.Key == Key.Down) // questão 8
                    objetoSelecionado.TranslacaoXYZ(0, -10, 0);
                else if (e.Key == Key.PageUp) // questão 9
                    objetoSelecionado.EscalaXYZ(2, 2, 2);
                else if (e.Key == Key.PageDown) // questão 9
                    objetoSelecionado.EscalaXYZ(0.5, 0.5, 0.5);
                else if (e.Key == Key.Home) // questão 9
                    objetoSelecionado.EscalaXYZBBox(0.5, 0.5, 0.5);
                else if (e.Key == Key.End) // questão 9
                    objetoSelecionado.EscalaXYZBBox(2, 2, 2);
                else if (e.Key == Key.Number1) // questão 10
                    objetoSelecionado.Rotacao(10);
                else if (e.Key == Key.Number2) // questão 10
                    objetoSelecionado.Rotacao(-10);
                else if (e.Key == Key.Number3) // questão 10
                    objetoSelecionado.RotacaoZBBox(10);
                else if (e.Key == Key.Number4) // questão 10
                    objetoSelecionado.RotacaoZBBox(-10);
                else if (e.Key == Key.Number9)
                    objetoSelecionado = null; // desmacar objeto selecionado
                else if (e.Key == Key.A) // questão 11
                {
                    lastPressedKey = Key.A;
                    if (objetoNovo == null)
                    {
                        objetoId = Utilitario.charProximo(objetoId);
                        objetoNovo = new Poligono(objetoId, null);
                        objetoNovo.PontosAdicionar(new Ponto4D(mouseX, mouseY));
                        objetoNovo.PontosAdicionar(new Ponto4D(mouseX, mouseY));  // N3-Exe6: "troque" para deixar o rastro
                    }
                    else
                    {
                        objetoNovo.PontosAdicionar(new Ponto4D(mouseX, mouseY));
                    }
                }

                else
                    Console.WriteLine(" __ Tecla não implementada.");
            }
            else
                Console.WriteLine(" __ Tecla não implementada.");
        }

        private int GetClosestPoint(Ponto4D pt)
        {
            double lowestDistance = Double.MaxValue;
            int point = -1;


            for (int i = 0; i < objetoSelecionado.Pontos().Count; i++)
            {
                double distance = getDistance(pt, objetoSelecionado.Pontos()[i]);
                if (distance <= lowestDistance)
                {
                    lowestDistance = distance;
                    point = i;
                }
            }
            return point;
        }

        private void SelectObject(Objeto obj)
        {
            if ((mouseX >= obj.BBox.obterMenorX) && (mouseX <= obj.BBox.obterMaiorX) &&
                    (mouseY >= obj.BBox.obterMenorY) && (mouseY <= obj.BBox.obterMaiorY))
            { // verifica se o ponto é interno à ela
                objetoSelecionado = (ObjetoGeometria)obj;
                bBoxDesenhar = true;

            }

            if (objetoSelecionado == null)
            {
                foreach (Objeto filho in obj.Filhos())
                {
                    if (objetoSelecionado == null)
                    {
                        SelectObject(filho);
                    }
                }
            }

        }

        private double getDistance(Ponto4D pt1, Ponto4D pt2)
        {
            return Math.Sqrt(Math.Pow((pt1.X - pt2.X), 2) + Math.Pow((pt1.Y - pt2.Y), 2));
        }

        //TODO: não está considerando o NDC
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            mouseX = e.Position.X; mouseY = 600 - e.Position.Y; // Inverti eixo Y
            if (objetoNovo != null)
            {
                objetoNovo.PontosUltimo().X = mouseX;
                objetoNovo.PontosUltimo().Y = mouseY;
            }
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
            window.Title = "CG_N3";
            window.Run(1.0 / 60.0);
        }
    }
}