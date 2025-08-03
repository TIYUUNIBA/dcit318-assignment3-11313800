using System;
using System.Collections.Generic;
using System.Linq;

// Generic Repository
public class Repository<T>
{
    private readonly List<T> _items = new List<T>();

    public void Add(T item) => _items.Add(item);

    public List<T> GetAll() => new List<T>(_items);

    public T? GetById(Func<T, bool> predicate) => _items.FirstOrDefault(predicate);

    public bool Remove(Func<T, bool> predicate)
    {
        var item = _items.FirstOrDefault(predicate);
        if (item != null)
        {
            return _items.Remove(item);
        }
        return false;
    }
}

// Patient class
public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }

    public override string ToString() => $"[Patient {Id}] {Name}, {Age}yrs ({Gender})";
}

// Prescription class
public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

    public override string ToString() => $"[Prescription {Id}] {MedicationName} (Issued: {DateIssued:yyyy-MM-dd})";
}

// Main Health System Application
public class HealthSystemApp
{
    private readonly Repository<Patient> _patientRepo = new Repository<Patient>();
    private readonly Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

    public void SeedData()
    {
        // Add patients
        _patientRepo.Add(new Patient(1, "John Doe", 35, "Male"));
        _patientRepo.Add(new Patient(2, "Jane Smith", 28, "Female"));
        _patientRepo.Add(new Patient(3, "Robert Johnson", 42, "Male"));

        // Add prescriptions
        _prescriptionRepo.Add(new Prescription(1, 1, "Ibuprofen", new DateTime(2023, 1, 15)));
        _prescriptionRepo.Add(new Prescription(2, 1, "Amoxicillin", new DateTime(2023, 2, 1)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Lisinopril", new DateTime(2023, 1, 20)));
        _prescriptionRepo.Add(new Prescription(4, 2, "Metformin", new DateTime(2023, 2, 5)));
        _prescriptionRepo.Add(new Prescription(5, 3, "Atorvastatin", new DateTime(2023, 1, 10)));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();
        foreach (var prescription in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            }
            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        return _prescriptionMap.TryGetValue(patientId, out var prescriptions) 
            ? prescriptions 
            : new List<Prescription>();
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("=== All Patients ===");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine(patient);
        }
        Console.WriteLine();
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        var patient = _patientRepo.GetById(p => p.Id == patientId);
        if (patient == null)
        {
            Console.WriteLine($"Patient with ID {patientId} not found.");
            return;
        }

        Console.WriteLine($"=== Prescriptions for {patient.Name} ===");
        var prescriptions = GetPrescriptionsByPatientId(patientId);
        if (prescriptions.Count == 0)
        {
            Console.WriteLine("No prescriptions found.");
            return;
        }

        foreach (var prescription in prescriptions)
        {
            Console.WriteLine(prescription);
        }
    }

    public static void Main()
    {
        var app = new HealthSystemApp();
        
        // Initialize data
        app.SeedData();
        app.BuildPrescriptionMap();

        // Display all patients
        app.PrintAllPatients();

        // Display prescriptions for a specific patient (using first patient in this example)
        var firstPatientId = app._patientRepo.GetAll().First().Id;
        app.PrintPrescriptionsForPatient(firstPatientId);
    }
}