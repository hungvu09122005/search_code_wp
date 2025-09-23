using System;

class Node
{
    public int Key;
    public Node Left;
    public Node Right;

    public Node(int key)
    {
        Key = key;
        Left = null;
        Right = null;
    }
}

class BinaryTree
{
    public void LRN(Node root)
    {
        if (root == null)
            return;

        LRN(root.Left);
        LRN(root.Right);
        Console.Write(root.Key + " ");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Node root = new Node(1);
        root.Left = new Node(2);
        root.Right = new Node(3);
        root.Left.Left = new Node(4);
        root.Left.Right = new Node(5);
        root.Right.Right = new Node(6);

        BinaryTree tree = new BinaryTree();

        Console.Write("Postorder traversal: ");
        tree.LRN(root);

        Console.WriteLine("\nFinish encoding. Press 'Enter' to exit.");
        Console.ReadLine();
    }
}
