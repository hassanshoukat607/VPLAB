﻿using System;
using System.Collections.Generic;
abstract class User
{
    public string UserId { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    protected User(string userId, string name, string phoneNumber)
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
    }
    public abstract void Register();
    public abstract void Login();
    public virtual void DisplayProfile()
    {
        Console.WriteLine($"User ID: {UserId}, Name: {Name}, Phone: {PhoneNumber}");
    }
}
class Rider : User
{
    public List<Trip> RideHistory { get; set; } = new List<Trip>();
    public Rider(string userId, string name, string phoneNumber) : base(userId, name, phoneNumber) { }
    public override void Register() => Console.WriteLine($"Rider {Name} registered.");
    public override void Login() => Console.WriteLine($"Rider {Name} logged in.");
    public Trip RequestRide(string startLocation, string destination)
    {

        if (string.IsNullOrEmpty(startLocation) || string.IsNullOrEmpty(destination))

             new ArgumentNullException("location is not null or zero.");
        Console.WriteLine($"{Name} requested a ride from {startLocation} to {destination}");

        var trip = new Trip(RideHistory.Count + 1,Name,null,startLocation,destination,0);
        trip.CalculateFare();
        RideHistory.Add(trip);
        return trip;
    }
    public void ViewRideHistory()
    {
        Console.WriteLine($"Ride History for {Name}:");
        foreach (var trip in RideHistory)
        {
            trip.DisplayTripDetails();
        }
    }
}
class Driver : User
{
    public string DriverId { get; set; }
    public string VehicleDetails { get; set; }
    public bool IsAvailable { get; set; } = true;
    public List<Trip> TripHistory { get; set; } = new List<Trip>();
    public Driver(string userId, string name, string phoneNumber, string driverId, string vehicleDetails)
        : base(userId, name, phoneNumber)
    {
        DriverId = driverId ?? throw new ArgumentNullException(nameof(driverId));
        VehicleDetails = vehicleDetails ?? throw new ArgumentNullException(nameof(vehicleDetails));
    }
    public override void Register() => Console.WriteLine($"Driver {Name} registered.");
    public override void Login() => Console.WriteLine($"Driver {Name} logged in.");
    public void AcceptRide(Trip trip)
    {
        if (IsAvailable)
        {
            IsAvailable = false;
            trip.DriverName = Name;
            Console.WriteLine($"{Name} accepted the ride.");
            TripHistory.Add(trip);
            trip.StartTrip();

        }

        else
        {
            Console.WriteLine($"{Name} is not available to accept rides.");

        }

    }

    public void ViewTripHistory()
    {
        Console.WriteLine($"Trip History for {Name}:");
        foreach (var trip in TripHistory)
        {
            trip.DisplayTripDetails();
        }
    }
    public void ToggleAvailability()
    {
        IsAvailable = !IsAvailable;
        Console.WriteLine($"{Name} availability set to {IsAvailable}");
    }

}
class Trip
{
    public int TripId { get; set; }
    public string RiderName { get; set; }
    public string? DriverName { get; set; }
    public string StartLocation { get; set; }
    public string Destination { get; set; }
    public double Fare { get; set; }
    public string Status { get; set; } = "Pending";
    public Trip(int tripId, string riderName, string? driverName, string startLocation, string destination, double fare)
    {

        TripId = tripId;
        RiderName = riderName ?? throw new ArgumentNullException(nameof(riderName));
        DriverName = driverName;
        StartLocation = startLocation ?? throw new ArgumentNullException(nameof(startLocation));
        Destination = destination ?? throw new ArgumentNullException(nameof(destination));
        Fare = fare;
    }
    public void CalculateFare()
    {
        Fare = 20;
    }
    public void StartTrip() => Status = "Ongoing";
    public void EndTrip() => Status = "Completed";
    public void DisplayTripDetails()
    {
        Console.WriteLine($"Trip ID: {TripId}, Rider: {RiderName}, Driver: {DriverName}, Start: {StartLocation}, " +

                          $"Destination: {Destination}, Fare: {Fare}, Status: {Status}");
    }
}
class RideSharingSystem
{
    private List<Rider> RegisteredRiders = new List<Rider>();
    private List<Driver> RegisteredDrivers = new List<Driver>();
    private List<Trip> AvailableTrips = new List<Trip>();
    public Rider? GetRider(string riderId) => RegisteredRiders.FirstOrDefault(r => r.UserId == riderId);
    public Driver? GetDriver(string driverId) => RegisteredDrivers.FirstOrDefault(d => d.UserId == driverId);
    public void RegisterUser(string userType, string userId, string name, string phoneNumber, string? driverId = null, string? vehicleDetails = null)
    {
        if (userType == "Rider")
        {
            var rider = new Rider(userId, name, phoneNumber);
            RegisteredRiders.Add(rider);
            rider.Register();
        }
        else if (userType == "Driver")
        {
            if (driverId == null || vehicleDetails == null)
                throw new ArgumentException("DRIVER ID WOULD BE NOT A NULL");
            var driver = new Driver(userId, name, phoneNumber, driverId, vehicleDetails);
            RegisteredDrivers.Add(driver);
            driver.Register();
        }

    }
    public void RequestRide(string riderId, string startLocation, string destination)
    {
        var rider = GetRider(riderId);
        if (rider == null)
        {
            Console.WriteLine("Not found.");
            return;

        }



        var trip = rider.RequestRide(startLocation, destination);

        AvailableTrips.Add(trip);

    }



    public void AcceptRide(string driverId)

    {

        var driver = GetDriver(driverId);
        var availableTrip = AvailableTrips.FirstOrDefault();
        if (driver == null)
        {
            Console.WriteLine("Driver not found.");
            return;
        }
        if (availableTrip == null)

        {
            Console.WriteLine("No available trips.");
            return;
        }
        driver.AcceptRide(availableTrip);
    }

    public void CompleteTrip(string driverId)
    {
       var driver = GetDriver(driverId);
        if (driver != null && driver.TripHistory.Any())
        {

            var trip = driver.TripHistory.Last();

            trip.EndTrip();

            driver.ToggleAvailability();

            AvailableTrips.Remove(trip);

            Console.WriteLine("Trip completed:");

            trip.DisplayTripDetails();

        }

    }

    public void DisplayAllTrips()
    {
        Console.WriteLine("All Trips:");
        foreach (var trip in AvailableTrips)
        {
            trip.DisplayTripDetails();
        }
    }
}

class Program
{
    static void Main()
    {
        var rideSharingSystem = new RideSharingSystem();
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nWelcome to ride sharing system\n1. Register As rider  \n2. Register as driver  \n3. Request a Ride (Rider) \n" +
                              "4. Accept a Ride DRIVER  \n5. Complete a Trip (DRIVER) \n6. View Ride History (RIDE)\n" +
                              "7. View Trip History (Driver)\n8. Display All Trips\n9. Exit\n");
            Console.Write("Choose an option: ");
            var choice = Console.ReadLine();


            switch (choice)
            {
                case "1":

                    Console.Write("Enter Rider ID: ");
                    var riderId = Console.ReadLine()!;
                   // readline();
                    Console.Write("Enter Name: ");
                    var riderName = Console.ReadLine()!;
                    Console.Write("Enter Phone Number: ");
                    var riderPhone = Console.ReadLine()!;
                    rideSharingSystem.RegisterUser("Rider", riderId, riderName, riderPhone);
                    break;
                case "2":

                    Console.Write("Enter Driver ID: ");
                    var driverId = Console.ReadLine()!;
                    Console.Write("Enter Name: ");
                    var driverName = Console.ReadLine()!;
                    Console.Write("Enter Phone Number: ");
                    var driverPhone = Console.ReadLine()!;
                    Console.Write("Enter Vehicle Details: ");
                    var vehicleDetails = Console.ReadLine()!;
                    rideSharingSystem.RegisterUser("Driver", driverId, driverName, driverPhone, driverId, vehicleDetails);
                    break;
                case "3":

                    Console.Write("Enter Rider ID: ");
                    var requestRiderId = Console.ReadLine()!;
                    Console.Write("Enter Start Location: ");
                    var startLocation = Console.ReadLine()!;
                    Console.Write("Enter Destination: ");
                    var destination = Console.ReadLine()!;
                    rideSharingSystem.RequestRide(requestRiderId, startLocation, destination);
                    break;

                case "4":

                    Console.Write("Enter Driver ID: ");
                    var acceptDriverId = Console.ReadLine()!;
                    rideSharingSystem.AcceptRide(acceptDriverId);
                    break;
                case "5":

                    Console.Write("Enter Driver ID: ");
                    var completeDriverId = Console.ReadLine()!;
                    rideSharingSystem.CompleteTrip(completeDriverId);
                    break;
                case "6":

                    Console.Write("Enter Rider ID: ");
                    var viewRiderId = Console.ReadLine()!;
                    var rider = rideSharingSystem.GetRider(viewRiderId);
                    rider?.ViewRideHistory();
                    break;
                case "7":

                    Console.Write("Enter Driver ID: ");
                    var viewDriverId = Console.ReadLine()!;
                    var driver = rideSharingSystem.GetDriver(viewDriverId);
                    break;
            }
        }
    }
}
