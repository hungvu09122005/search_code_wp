using System;

class Program
{
    
    /// <summary>
    /// Encodes the given number using a custom bitwise algorithm for a specified number of steps.
    /// The encoding process involves bitwise negation, masking, left rotation, and modulo operation.
    /// </summary>
    /// <param name="number">
    /// The integer to encode. This parameter is passed by reference and will be updated with the encoded value.
    /// </param>
    /// <param name="steps">
    /// The number of encoding steps to perform. If zero, the method returns without modifying the number.
    /// </param>
    static void encode(ref int number, int steps)
    {
        if (steps == 0)
        {
            return;
        }

        int encoded = (~number) & 0b1111;

        encoded = ((encoded << 1) & 0b1111) | (encoded >> 3);

        number = encoded % 10;

        encode(ref number, steps - 1);
    }

    static void Main()
    {
        int encodeNum = 7;
        int stepEncode = 2;

        encode(ref encodeNum, stepEncode);

        Console.WriteLine($"The encoding result of {encodeNum} after {stepEncode} times is :" + encodeNum);

        Console.WriteLine("Finish encoding. Press 'Enter' to exit.");
        Console.ReadLine();
    }
}
