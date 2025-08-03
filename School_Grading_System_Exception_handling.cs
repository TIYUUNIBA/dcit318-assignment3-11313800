using System;
using System.Collections.Generic;
using System.IO;

// Custom Exceptions
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// Student Class
public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        return Score switch
        {
            >= 80 and <= 100 => "A",
            >= 70 and <= 79 => "B",
            >= 60 and <= 69 => "C",
            >= 50 and <= 59 => "D",
            _ => "F"
        };
    }
}

// Student Result Processor
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();

        using var reader = new StreamReader(inputFilePath);
        int lineNumber = 0;

        while (!reader.EndOfStream)
        {
            lineNumber++;
            string line = reader.ReadLine();

            try
            {
                var fields = line.Split(',');

                // Validate field count
                if (fields.Length != 3)
                {
                    throw new MissingFieldException(
                        $"Line {lineNumber}: Expected 3 fields but found {fields.Length}");
                }

                // Parse ID
                if (!int.TryParse(fields[0].Trim(), out int id))
                {
                    throw new InvalidScoreFormatException(
                        $"Line {lineNumber}: Invalid ID format '{fields[0]}'");
                }

                // Parse score
                if (!int.TryParse(fields[2].Trim(), out int score))
                {
                    throw new InvalidScoreFormatException(
                        $"Line {lineNumber}: Invalid score format '{fields[2]}'");
                }

                // Validate score range
                if (score < 0 || score > 100)
                {
                    throw new InvalidScoreFormatException(
                        $"Line {lineNumber}: Score must be between 0-100 (got {score})");
                }

                students.Add(new Student(
                    id,
                    fields[1].Trim(),
                    score));
            }
            catch (Exception ex) when (
                ex is MissingFieldException || 
                ex is InvalidScoreFormatException)
            {
                Console.WriteLine($"Skipping line {lineNumber}: {ex.Message}");
            }
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using var writer = new StreamWriter(outputFilePath);
        
        writer.WriteLine("=== Student Grade Report ===");
        writer.WriteLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}");
        writer.WriteLine("============================");
        writer.WriteLine();

        foreach (var student in students)
        {
            writer.WriteLine(
                $"{student.FullName} (ID: {student.Id}): " +
                $"Score = {student.Score}, Grade = {student.GetGrade()}");
        }

        writer.WriteLine();
        writer.WriteLine($"Total Students Processed: {students.Count}");
    }
}

// Main Program
class Program
{
    static void Main(string[] args)
    {
        const string inputFile = "students.txt";
        const string outputFile = "grade_report.txt";

        var processor = new StudentResultProcessor();

        try
        {
            // Read and validate student data
            var students = processor.ReadStudentsFromFile(inputFile);

            // Generate report
            processor.WriteReportToFile(students, outputFile);

            Console.WriteLine($"Successfully processed {students.Count} students.");
            Console.WriteLine($"Report saved to: {outputFile}");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Error: Input file '{inputFile}' not found.");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Data error: {ex.Message}");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Data error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}