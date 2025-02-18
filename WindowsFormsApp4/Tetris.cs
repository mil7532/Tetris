using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace WindowsFormsApp4
{
    public partial class Tetris : Form
    {
        private static System.Timers.Timer timer1;

        private int[,] grid = new int[,]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //1
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //2
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //3
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //4
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //5
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //6
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //7
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //8
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //9
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //10
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //11
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //12
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //13
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //14
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //15
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //16
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //17
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //18
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //19
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, //20
        };

        private int[,] block_O = new int[,]
        {
            {0, 1, 0},
            {1, 1, 1},
        };

        private int[,] block_1 = new int[,]
        {
            {1 ,1},
            {1 ,1},
        };

        private int[,] block_2 = new int[,]
        {
            {1, 1, 1, 1},
        };

        private int[,] block_3 = new int[,]
        {
            {1, 1, 0,},
            {0, 1, 1,},
        };

        private int[,] block_4 = new int[,]
        {
            {0, 1, 1,},
            {1, 1, 0,},
        };

        private int[,] block_5 = new int[,]
        {
            {0 ,1},
            {0 ,1},
            {1 ,1},
        };

        private int[,] block_6 = new int[,]
        {
            {1 ,0},
            {1 ,0},
            {1 ,1},
        };

        int positionX = 5;
        int positionY = 3;
        bool start_new_block = true;
        int[,] currentBlock;

        Random rnd_block = new Random(DateTime.Now.Millisecond);

        public Tetris()
        {
            //300x600
            InitializeComponent();

            //[width, height] 
            pictureBox1.BackColor = Color.White;
            this.DoubleBuffered = true;
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            this.KeyDown += new KeyEventHandler(Form_KeyDown);
            this.KeyPreview = true;
            pictureBox1.Invalidate();
            button1.Text = "START GAME (F1)";

            timer1 = new System.Timers.Timer(1010);
            timer1.Elapsed += OnTimerElapsed;
            timer1.AutoReset = true;
            timer1.Enabled = false;
        }

        private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            MoveDown();
            pictureBox1.Invalidate(); // Automatyczny ruch klocka w dół
        }

        int value_pkt = 0;
        public void TetrisClearGameBoard()
        {
            for (int i = grid.GetLength(0) - 1; i > 0; i--)
            {
                if (TCGB_IsFullLineBoardGame(i))
                {
                    value_pkt++;
                    if (label2.InvokeRequired)
                        label2.Invoke(new Action(() => label2.Text = value_pkt.ToString()));
                    else
                        label2.Text = value_pkt.ToString();

                    TCGB_RemoveLineInBoard(i);
                    TCGB_MoveDown(i);
                    i++;
                }
            }
            pictureBox1.Invalidate();
        }

        private bool TCGB_IsFullLineBoardGame(int row)
        {
            //funkcja sprawdza czy podana linia jest pusta (row)
            for (int i = 0; i < grid.GetLength(1); i++)
            {
                if (grid[row, i] == 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void TCGB_RemoveLineInBoard(int row)
        {
            //funkcja usuwa znalezioną linie
            for (int i = 0; i < grid.GetLength(1); i++)
                grid[row, i] = 0;
        }

        private void TCGB_MoveDown(int row)
        {
            //funkcja przesuwa górną cześć nad usuniętą linie o 1 w dół.
            for (int i = row; i > 0; i--)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = grid[i - 1, j];
                }
            }

            for (int t = 0; t < grid.GetLength(1); t++)
                grid[0, t] = 0;
        }

        private void GenerateNewBlock()
        {
            positionX = 5;
            positionY = 0;

            start_new_block = false;
            int select_block = rnd_block.Next(1, 6);
            switch (select_block)
            {
                case 1:
                    currentBlock = block_O;
                    break;
                case 2:
                    currentBlock = block_1;
                    break;
                case 3:
                    currentBlock = block_2;
                    break;
                case 4:
                    currentBlock = block_3;
                    break;
                case 5:
                    currentBlock = block_4;
                    break;
                case 6:
                    currentBlock = block_5;
                    break;
                default:
                    MessageBox.Show("error:");
                    break;
            }

            CreateBlockOnGrid();
            pictureBox1.Invalidate();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            pictureBox1.Invalidate();

            if (e.KeyCode == Keys.F1)
            {
                timer1.Enabled = true;
                GenerateNewBlock();
            }

            //rotate block
            if (e.KeyCode == Keys.W)
            {
                CleanBlockOnGrid();
                int[,] rotated_current_block = rotation90(currentBlock);

                if (!CheckCollision(rotated_current_block, positionX, positionY))
                {
                    currentBlock = rotated_current_block;
                }

                CreateBlockOnGrid();
                pictureBox1.Invalidate();
            }
            //block position in right 
            if (e.KeyCode == Keys.D)
            {
                if (!CheckCollision(currentBlock, positionX + 1, positionY))
                {
                    CleanBlockOnGrid();
                    positionX++;
                    CreateBlockOnGrid();
                }
            }
            //block position in left
            if (e.KeyCode == Keys.A)
            {
                if (!CheckCollision(currentBlock, positionX - 1, positionY))
                {
                    CleanBlockOnGrid();
                    positionX--;
                    CreateBlockOnGrid();
                }
            }

            if (e.KeyCode == Keys.S)
            {
                MoveDown();
            }

            pictureBox1.Invalidate();
        }

        private void MoveDown()
        {
            if (!CheckCollision(currentBlock, positionX, positionY + 1))
            {
                CleanBlockOnGrid();
                positionY++;
                CreateBlockOnGrid();
            }
            else
            {
                if (StackedBlock(currentBlock))
                {
                    start_new_block = true;
                    // Zatrzymaj timer, jeśli klocek dotarł do górnej części planszy
                    if (positionY == 0)
                    {
                        timer1.Enabled = false;
                        MessageBox.Show("Game Over");
                        return;
                    }

                    GenerateNewBlock();
                    return;
                }
            }
        }

        private bool StackedBlock(int[,] block)
        {
            for (int i = 0; i < block.GetLength(0); i++)
            {
                for (int j = 0; j < block.GetLength(1); j++)
                {
                    if (block[i, j] == 1)
                    {
                        int newX = positionX + j;
                        int newY = positionY + i + 1;

                        if (newY >= grid.GetLength(0) || grid[newY, newX] != 0)
                        {
                            TetrisClearGameBoard();
                            return true;
                        }
                    }
                }
            }
            for (int j = 0; j < block.GetLength(1); j++)
            {
                if (block[0, j] == 1 && positionY == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public int[,] rotation90(int[,] rotation)
        {
            int[,] rotated = new int[rotation.GetLength(1), rotation.GetLength(0)];
            for (int i = 0; i < rotation.GetLength(0); i++)
            {
                for (int j = 0; j < rotation.GetLength(1); j++)
                {
                    rotated[j, rotation.GetLength(0) - 1 - i] = rotation[i, j];
                }
            }
            return rotated;
        }

        private void CreateBlockOnGrid()
        {
            for (int i = 0; i < currentBlock.GetLength(0); i++)
            {
                for (int j = 0; j < currentBlock.GetLength(1); j++)
                {
                    if (currentBlock[i, j] == 1)
                    {
                        grid[positionY + i, positionX + j] = 1;
                    }
                }
            }
        }

        private void CleanBlockOnGrid()
        {
            for (int i = 0; i < currentBlock.GetLength(0); i++)
            {
                for (int j = 0; j < currentBlock.GetLength(1); j++)
                {
                    if (currentBlock[i, j] == 1)
                    {
                        grid[positionY + i, positionX + j] = 0;
                    }
                }
            }
        }

        private bool CheckCurrentBlockPosition(int cPosX, int cPosY)
        {
            for (int i = 0; i < currentBlock.GetLength(0); i++)
            {
                for (int j = 0; j < currentBlock.GetLength(1); j++)
                {
                    if (currentBlock[i, j] == 1)
                    {
                        if (positionX + j == cPosX && positionY + i == cPosY)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool CheckCollision(int[,] block, int offsetX, int offsetY)
        {
            for (int i = 0; i < block.GetLength(0); i++)
            {
                for (int j = 0; j < block.GetLength(1); j++)
                {
                    if (block[i, j] == 1)
                    {
                        int newX = j + offsetX;
                        int newY = i + offsetY;

                        if (newX < 0 || grid.GetLength(1) <= newX || newY >= grid.GetLength(0))
                        {
                            return true;
                        }
                        if (newY >= 0 && grid[newY, newX] != 0)
                        {
                            if (!CheckCurrentBlockPosition(newX, newY))
                            {
                                return true; // Pozycja należy do aktualnego klocka
                            }
                        }
                    }
                }
            }
            return false;
        }

        private Color GetColorForValue(int value)
        {
            switch (value)
            {
                case 1: return Color.Red;
                case 2: return Color.Blue;
                case 3: return Color.Green;
                default: return Color.White;
            }
        }

        private void DrawBlock(Graphics g)
        {
            for (int row = 0; row < 20; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    // Wybierz kolor na podstawie wartości w 'grid'
                    Color kolorKomorki = GetColorForValue(grid[row, col]);

                    // Użyj SolidBrush do wypełnienia kwadratu kolorem
                    using (SolidBrush brush = new SolidBrush(kolorKomorki))
                    {
                        g.FillRectangle(brush, col * 30, row * 30, 30, 30);
                    }
                }
            }
        }

        private void GridPaint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.LightGray, 2);

            // Rysuj klocki
            DrawBlock(e.Graphics);

            // Rysuj siatkę
            for (int i = 0; i <= 300; i += 30)
                e.Graphics.DrawLine(pen, i, 0, i, 600);
            for (int j = 0; j <= 600; j += 30)
                e.Graphics.DrawLine(pen, 0, j, 300, j);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            GenerateNewBlock();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }
    }
}