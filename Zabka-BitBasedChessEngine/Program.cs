public class Program
{
    public static void Main(string[] args)
    {
        Board board = new Board();
        MoveGenerator moveGenerator = new MoveGenerator();
        Engine engine = new Engine();
        UCIHandler uciHandler = new UCIHandler(board, moveGenerator, engine);

        uciHandler.HandleInput();
    }
}
