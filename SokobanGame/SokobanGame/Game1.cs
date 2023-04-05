using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection.Emit;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace SokobanGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private int nrLinhas = 0;
    private int nrColunas = 0;
    private SpriteFont font;
    private Texture2D dot, box, wall;
    private Player sokoban;

    public const int tileSize = 64;

    private char[,] level;
    public List<Point> boxes;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        LoadLevel("level1.txt");
        _graphics.PreferredBackBufferHeight = tileSize * (1 + level.GetLength(1)); //definição da altura
        _graphics.PreferredBackBufferWidth = tileSize * level.GetLength(0); //definição da largura
        _graphics.ApplyChanges(); //aplica a atualização da janela
        sokoban.LoadContents();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        font = Content.Load<SpriteFont>("File");
        //player = Content.Load<Texture2D>("Character4");
        dot = Content.Load<Texture2D>("EndPoint_Blue");
        box = Content.Load<Texture2D>("Crate_Brown");
        wall = Content.Load<Texture2D>("Wall_Brown");

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        // TODO: Add your update logic here

        if (Keyboard.GetState().IsKeyDown(Keys.R)) Initialize(); // Game restart

        if (Victory()) Exit(); // FIXME: Change current level

        // TODO: Add your update logic here
        sokoban.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        base.Draw(gameTime);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        //_spriteBatch.DrawString(font, $"Numero de Linhas = {nrLinhas} -- Numero de Colunas = {nrColunas}", new Vector2(0, 0), Color.Black);

        _spriteBatch.DrawString(font, "O texto que quiser", new Vector2(0, 40), Color.Black);
        _spriteBatch.DrawString(font, $"Numero de Linhas = {nrLinhas}", new Vector2(0, 0), Color.Black);
        _spriteBatch.DrawString(font, $"Numero de Colunas = {nrColunas}", new Vector2(0, 20), Color.Black);

        Rectangle position = new Rectangle(0, 0, tileSize, tileSize);
        for (int x = 0; x < level.GetLength(0); x++) //pega a primeira dimensão
        {
            for (int y = 0; y < level.GetLength(1); y++) //pega a segunda dimensão
            {
                position.X = x * tileSize; // define o position
                position.Y = y * tileSize; // define o position
                switch (level[x, y])
                {
                    //case 'Y':
                    //    _spriteBatch.Draw(player, position, Color.White);
                    //    break;
                    //case '#':
                    //    _spriteBatch.Draw(box, position, Color.White);
                    //    break;
                    case '.':
                        _spriteBatch.Draw(dot, position, Color.White);
                        break;
                    case 'X':
                        _spriteBatch.Draw(wall, position, Color.White);
                        break;
                }
            }
        }
        // Draw UI
        _spriteBatch.DrawString(font,// Tipo de letra
        "Tempo Decorrido = ", // Texto
        new Vector2(5, level.GetLength(1) * tileSize + 5), // Posição do texto
        Color.White, // Cor da letra
        0f, //Rotação
        Vector2.Zero, // Origem
        2f, // Escala
        SpriteEffects.None, //Sprite effect (FlipHorizontally)
        0); // Ordenar sprites

        sokoban.Draw(_spriteBatch);

        foreach (Point b in boxes)
        {
            position.X = b.X * tileSize;
            position.Y = b.Y * tileSize;
            _spriteBatch.Draw(box, position, Color.White);
        }


        //case 'Y':
        // _spriteBatch.Draw(player, position, Color.White);
        // break;
        //position.X = sokoban.Position.X * tileSize; //posição do Player
        //position.Y = sokoban.Position.Y * tileSize; //posição do Player
        //_spriteBatch.Draw(player, position, Color.White); //desenha o Player
        _spriteBatch.End();
    }
    public bool HasBox(int x, int y)
    {
        foreach (Point b in boxes)
        {
            if (b.X == x && b.Y == y) return true; // se a caixa tiver a mesma posição do Player
        }
        return false;
    }
    public bool FreeTile(int x, int y)
    {
        if (level[x, y] == 'X') return false;  // se for uma parede está ocupada
        if (HasBox(x, y)) return false; // verifica se é uma caixa
        return true;

        /* The same as:    return level[x,y] != 'X' && !HasBox(x,y);   */
    }

    public bool Victory()
    {
        foreach (Point b in boxes) // pecorrer a lista das caixas
        {
            if (level[b.X, b.Y] != '.') return false; // verifica se há caixas sem pontos
        }
        return true;
    }

    void LoadLevel(string levelFile)
    {
        boxes = new List<Point>();
        string[] linhas = File.ReadAllLines($"Content/{levelFile}"); // "Content/" + level
        nrLinhas = linhas.Length;
        nrColunas = linhas[0].Length;
        level = new char[nrColunas, nrLinhas];


        for (int x = 0; x < nrColunas; x++)
        {
            for (int y = 0; y < nrLinhas; y++)
            {
                if (linhas[y][x] == '#')
                {
                    boxes.Add(new Point(x, y));
                    level[x, y] = ' '; // put a blank instead of the box '#'
                }
                else if (linhas[y][x] == 'Y')
                {
                    sokoban = new Player(this, x, y);
                    level[x, y] = ' '; // put a blank instead of the sokoban 'Y'
                }
                else
                {
                    level[x, y] = linhas[y][x];
                }
            }
        }



    }

}
