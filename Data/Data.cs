using App1.Models;
using System.Collections.Generic;
using Windows.System;

namespace App1.Data
{
    public static class QuestionRepository
    {
        public static List<QuestionData> Questions { get; } = new List<QuestionData>
        {
            CreateQuestion1(),
            CreateQuestion2(),
            CreateQuestion3()
        };

        private static QuestionData CreateQuestion1()
        {
            var body = string.Join("\n", new List<string>
{
                "Given the following code:",
                "\tfor (int i = 0; i < n - 1; i++) {",
                "\t    for (int j = 0; j < n - i - 1; j++) {",
                "\t        if (arr[j] > arr[j + 1]) {",
                "\t            swap(arr[j], arr[j + 1]);",
                "\t        }",
                "\t    }",
                "\t}",
                "Given the array [3, 6, 21, 12, 5, 63, 4, 67] with n being the length of the array, what will the array look like when i = 3 and j = 3?"
});

            var q = new QuestionData("Question 1", body);
            q.AddAnswer("[3, 5, 4, 6, 12, 21, 63, 67]", true);
            q.AddAnswer("[3, 6, 12, 5, 21, 4, 63, 67]", false);
            q.AddAnswer("[3, 5, 6, 4, 12, 21, 63, 67]", false);
            return q;
        }


        private static QuestionData CreateQuestion2()
        {
            var body = string.Join("\n", new List<string>
            {
                "Given the following code:",
                "\tstring s = \"oder\";",
                "\tint value1 = 75;",
                "\tint value2 = 8;",
                "\tint temp = value1 ^ value2;",
                "\tchar ch = (char)temp;",
                "\tstring result = ch + s;",
                "\tConsole.WriteLine(result);",
                "",
                "What will be the output of the program?"
            });

            var q = new QuestionData("Question 2", body);


            q.AddAnswer("Koder", false);
            q.AddAnswer("Coder", true);
            q.AddAnswer("Loder", false);

            return q;
        }


        private static QuestionData CreateQuestion3()
        {
            var body = string.Join("\n", new List<string>
            {
                "Given the following code:",
                "\tint[] arr = { 2, 3, 5, 1, 4 };",
                "\tint result = 0;",
                "\tfor (int i = 0; i < arr.Length; i++){",
                "\t\tfor (int j = i; j < arr.Length; j++){",
                "\t\t\tif (arr[j] % 2 == 0)",
                "\t\t\t\tresult += arr[j];",
                "\t\t\telse",
                "\t\t\t\tresult -= arr[j];",
                "\t\t}",
                "\t}",
                "\tConsole.WriteLine(result);",
                "Question: What will be printed on the console?"
            });

            var q = new QuestionData("Question 3", body);

            q.AddAnswer("-3", true);
            q.AddAnswer("0", false);
            q.AddAnswer("10", false);

            return q;
        }
    }
    public static class UserState
    {
        public static string Name = "";
    }
}
