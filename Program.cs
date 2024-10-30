using System;
using System.Collections.Generic;

class User
{
    public string UserId { get; }
    public string Name { get; }
    public string PhoneNo { get; }

    public User(string userId, string name, string phoneNo)
    {
        UserId = userId;
        Name = name;
        PhoneNo = phoneNo;
    }

    public void Register()
    {
        Console.WriteLine($"User {Name} registered successfully.");
    }
}

class Rider : User
{
    public List<string> RideHistory { get; }

    public Rider(string userId, string name, string phoneNo) : base(userId, name, phoneNo)
    {
        RideHistory = new List<string>();
    }

    public void RequestRide(string startLocation, string destination)
    {
        Console.WriteLine($"Ride requested from {startLocation} to {destination}.");
        RideHistory.Add($"Requested ride from {startLocation} to {destination}.");
    }

    public void ViewRideHistory()
    {
        Console.WriteLine("Ride History:");
        foreach (var ride in RideHistory)
        {
            Console.WriteLine(ride);
        }
    }
}

class Driver : User
{
    public string DriverId { get; }
    public string VehicleDetails { get; }
    public bool IsAvailable { get; private set; }
    public List<string> TripHistory { get; }

    public Driver(string userId, string name, string phoneNo, string driverId, string vehicleDetails)
        : base(userId, name, phoneNo)
    {
        DriverId = driverId;
        VehicleDetails = vehicleDetails;
        IsAvailable = true;
        TripHistory = new List<string>();
    }

    public void AcceptRide(Trip trip)
    {
        if (IsAvailable)
        {
            IsAvailable = false;
            TripHistory.Add(trip.TripId);
            Console.WriteLine($"Ride accepted for trip ID: {trip.TripId}.");
        }
        else
        {
            Console.WriteLine("Driver is not available.");
        }
    }

    public void ViewTripHistory()
    {
        Console.WriteLine("Trip History:");
        foreach (var trip in TripHistory)
        {
            Console.WriteLine(trip);
        }
    }

    public void ToggleAvailability()
    {
        IsAvailable = !IsAvailable;
        Console.WriteLine($"Driver is now {(IsAvailable ? "available" : "not available")}.");
    }
}

class Trip
{
    public string TripId { get; }
    public Rider Rider { get; }
    public Driver? Driver { get; set; } // Mark as nullable
    public string StartLocation { get; }
    public string Destination { get; }
    public double Fare { get; private set; }
    public string Status { get; private set; }

    public Trip(string tripId, Rider rider, string startLocation, string destination)
    {
        TripId = tripId;
        Rider = rider;
        StartLocation = startLocation;
        Destination = destination;
        Fare = 0;
        Status = "pending";
    }

    public void CalculateFare(double distance)
    {
        Fare = distance * 1.5; // Example fare calculation
        Console.WriteLine($"Fare calculated: {Fare}");
    }

    public void StartTrip()
    {
        Status = "in progress";
        Console.WriteLine($"Trip {TripId} started.");
    }

    public void EndTrip()
    {
        Status = "completed";
        Console.WriteLine($"Trip {TripId} ended.");
    }

    public void DisplayTripDetails()
    {
        Console.WriteLine($"Trip ID: {TripId}, Rider: {Rider.Name}, Driver: {(Driver != null ? Driver.Name : "None")}, " +
                          $"From: {StartLocation}, To: {Destination}, Fare: {Fare}, Status: {Status}");
    }
}

class RideSharingSystem
{
    public List<Rider> RegisteredRiders { get; } // Make public
    public List<Driver> RegisteredDrivers { get; } // Make public
    public List<Trip> Trips { get; } // Make public

    public RideSharingSystem()
    {
        RegisteredRiders = new List<Rider>();
        RegisteredDrivers = new List<Driver>();
        Trips = new List<Trip>();
    }

    public void RegisterUser(User user)
    {
        if (user is Rider rider)
        {
            RegisteredRiders.Add(rider);
        }
        else if (user is Driver driver)
        {
            RegisteredDrivers.Add(driver);
        }
        user.Register();
    }

    public Trip RequestRide(Rider rider, string startLocation, string destination)
    {
        var trip = new Trip((Trips.Count + 1).ToString(), rider, startLocation, destination);
        rider.RequestRide(startLocation, destination);
        Trips.Add(trip);
        return trip;
    }

    public Driver? FindAvailableDriver()
    {
        foreach (var driver in RegisteredDrivers)
        {
            if (driver.IsAvailable)
            {
                return driver;
            }
        }
        Console.WriteLine("No available drivers.");
        return null;
    }

    public void CompleteTrip(Trip trip)
    {
        trip.EndTrip();
    }

    public void DisplayAllTrips()
    {
        foreach (var trip in Trips)
        {
            trip.DisplayTripDetails();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var system = new RideSharingSystem();
        bool running = true;

        while (running)
        {
            Console.WriteLine("\nRide Sharing System Menu:");
            Console.WriteLine("1. Register as Rider");
            Console.WriteLine("2. Register as Driver");
            Console.WriteLine("3. Request a Ride");
            Console.WriteLine("4. Accept a Ride");
            Console.WriteLine("5. Complete Trip");
            Console.WriteLine("6. View Ride History");
            Console.WriteLine("7. Display All Trips");
            Console.WriteLine("8. Exit");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter User ID: ");
                    string riderId = Console.ReadLine() ?? string.Empty;
                    Console.Write("Enter Name: ");
                    string riderName = Console.ReadLine() ?? string.Empty;
                    Console.Write("Enter Phone No: ");
                    string riderPhone = Console.ReadLine() ?? string.Empty;
                    var rider = new Rider(riderId, riderName, riderPhone);
                    system.RegisterUser(rider);
                    break;

                case "2":
                    Console.Write("Enter User ID: ");
                    string driverUserId = Console.ReadLine() ?? string.Empty;
                    Console.Write("Enter Name: ");
                    string driverName = Console.ReadLine() ?? string.Empty;
                    Console.Write("Enter Phone No: ");
                    string driverPhone = Console.ReadLine() ?? string.Empty;
                    Console.Write("Enter Driver ID: ");
                    string drvId = Console.ReadLine() ?? string.Empty;
                    Console.Write("Enter Vehicle Details: ");
                    string vehicleDetails = Console.ReadLine() ?? string.Empty;
                    var driver = new Driver(driverUserId, driverName, driverPhone, drvId, vehicleDetails);
                    system.RegisterUser(driver);
                    break;

                case "3":
                    Console.Write("Enter Rider User ID: ");
                    string requestingRiderId = Console.ReadLine() ?? string.Empty;
                    var requestingRider = system.RegisteredRiders.Find(r => r.UserId == requestingRiderId);
                    if (requestingRider != null)
                    {
                        Console.Write("Enter Start Location: ");
                        string startLocation = Console.ReadLine() ?? string.Empty;
                        Console.Write("Enter Destination: ");
                        string destination = Console.ReadLine() ?? string.Empty;
                        system.RequestRide(requestingRider, startLocation, destination);
                    }
                    else
                    {
                        Console.WriteLine("Rider not found.");
                    }
                    break;

                case "4":
                    Console.Write("Enter Driver User ID: ");
                    string acceptingDriverUserId = Console.ReadLine() ?? string.Empty;
                    var acceptingDriver = system.RegisteredDrivers.Find(d => d.UserId == acceptingDriverUserId);
                    if (acceptingDriver != null)
                    {
                        var tripToAccept = system.Trips.Find(t => t.Driver == null);
                        if (tripToAccept != null)
                        {
                            acceptingDriver.AcceptRide(tripToAccept);
                            tripToAccept.Driver = acceptingDriver; // Assign driver to trip
                            tripToAccept.CalculateFare(10); // Example distance
                            tripToAccept.StartTrip();
                        }
                        else
                        {
                            Console.WriteLine("No ride requests available.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Driver not found.");
                    }
                    break;

                case "5":
                    Console.Write("Enter Trip ID to complete: ");
                    string tripIdToComplete = Console.ReadLine() ?? string.Empty;
                    var tripToComplete = system.Trips.Find(t => t.TripId == tripIdToComplete);
                    if (tripToComplete != null)
                    {
                        system.CompleteTrip(tripToComplete);
                    }
                    else
                    {
                        Console.WriteLine("Trip not found.");
                    }
                    break;

                case "6":
                    Console.Write("Enter Rider User ID to view history: ");
                    string riderIdToView = Console.ReadLine() ?? string.Empty;
                    var riderToView = system.RegisteredRiders.Find(r => r.UserId == riderIdToView);
                    riderToView?.ViewRideHistory();
                    break;

                case "7":
                    system.DisplayAllTrips();
                    break;

                case "8":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
}
