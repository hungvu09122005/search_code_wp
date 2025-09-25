using System;
class EscapeRoom {
    
    /// <summary>
    /// Hiển thị câu hỏi đầu tiên liên quan đến nguồn gốc tên gọi "Kaito Kid" trong manga và anime "Detective Conan".
    /// Hàm này in ra thông tin thú vị về ý nghĩa tên "Kaito Kid" và yêu cầu người dùng nhập câu trả lời cho một câu hỏi.
    /// </summary>
    static void FirstQuestion()
    {
        Console.WriteLine("First Question: ");
        Console.WriteLine(
            "\tIn the manga and anime series \"Detective Conan\", the famous thief Kaito Kid " +
            "has a very special name. \"Kaito\" means \"phantom thief\" in Japanese, " +
            "while \"Kid\" is actually a fascinating mistake. " +
            "It originated from a series of numbers that the author, Gosho Aoyama, " +
            "misread as \"KID\", and from there the name Kaito Kid was born. ");
        Console.WriteLine("\tDo you know what those numbers are?");
        Console.Write("\tInput your answer: ");
    }
    
    /// <summary>
    /// Hiển thị câu hỏi thứ hai liên quan đến kết quả của một đoạn mã C#.
    /// Hàm này sẽ in ra đoạn mã sử dụng phép toán XOR giữa hai số nguyên, sau đó chuyển kết quả thành ký tự và nối với chuỗi.
    /// Người dùng được yêu cầu dự đoán kết quả in ra của đoạn mã này.
    /// </summary>
    static void SecondQuestion()
    {
        Console.WriteLine("\n\nSecond Question: ");
        Console.WriteLine("\tstring s = \"oder\";");
        Console.WriteLine("\tint value1 = 75;");
        Console.WriteLine("\tint value2 = 8;");
        Console.WriteLine("\tint temp = value1 ^ value2;");
        Console.WriteLine("\tchar ch = (char)temp;");
        Console.WriteLine("\tstring result = ch + s;");
        Console.WriteLine("\tConsole.WriteLine(result);");
        Console.WriteLine("\tWhat do you think the outcome of the program will be?");
        Console.Write("\tInput your answer: ");
    }

    /// <summary>
    /// Hiển thị câu hỏi cuối cùng liên quan đến thuật toán sắp xếp.
    /// Hàm này in ra đoạn mã giả của một thuật toán sắp xếp mảng và yêu cầu người dùng đoán tên thuật toán đó.
    /// </summary>
    static void FinalQuestion()
    {
        Console.WriteLine("\n\nFinal Question: ");
        string code = @"
        int n = arr.Length;
        for (int i = 1; i < n; i++)
        {
            int key = arr[i];
            int j = i - 1;
        
            while (j >= 0 && arr[j] > key)
            {
                arr[j + 1] = arr[j];
                j--;
            }
        
            arr[j + 1] = key;
        }";

        Console.WriteLine(code);
        Console.WriteLine("\tWhich sorting algorithm is this?");
        Console.Write("\tInput your answer: ");
    }
    
    /// <summary> 
    /// Hàm này tạo ra một địa chỉ email đặc biệt dựa trên câu trả lời của người dùng cho ba câu hỏi.
    /// - Chuyển tất cả các chuỗi đầu vào thành chữ thường.
    /// - Nếu chuỗi đầu tiên rỗng, thêm "68@student.h" vào sau "23120", ngược lại thêm ký tự cuối của chuỗi đầu tiên rồi mới đến "68@student.h".
    /// - Nếu chuỗi thứ hai rỗng, thêm "mu", ngược lại thêm ký tự đầu của chuỗi thứ hai rồi mới đến "mu".
    /// - Nếu chuỗi thứ ba có ít nhất 3 ký tự, thêm ký tự thứ ba của chuỗi này.
    /// Cuối cùng, in ra địa chỉ email đã tạo.
    /// </summary>
    static void haveMail(ref string fir, ref string se, ref string final)
    {
        int firLeg = fir.Length;
        int seLeg = se.Length;
        int finalLeg = final.Length;
        
        fir = fir.ToLower();
        se = se.ToLower();
        final = final.ToLower();
        string mail = "23120";
        
        if (firLeg == 0)
        {
            mail += "68@student.h";
        }
        else
        {  
            mail = mail + fir[firLeg - 1] + "68@student.h";
        }
        if (seLeg == 0)
        {
            mail += "mu";
        }
        else 
        {
            mail = mail + se[0]+ "mu";
        }
        if (finalLeg >= 3)
        {
            mail = mail + final[2];
        }
        Console.WriteLine("Hay gui loi moi toi: " + mail);
    }

    static void Main() {
        Console.WriteLine("Program EscapeRoom: ");
        FirstQuestion();
        string firQues = Console.ReadLine();
        SecondQuestion();
        string seQues = Console.ReadLine();
        FinalQuestion();
        string finalQues = Console.ReadLine();
        Console.WriteLine(firQues + " " + seQues + " " + finalQues);
        haveMail(ref firQues, ref seQues, ref finalQues);
        Console.WriteLine("Finish program. Press 'Enter' to exit.");
        Console.ReadLine();
    }
}