﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectList
using FlyTickets2025.web.Data.Entities; // To reference Flight, ApplicationUser, Seat entities
using FlyTickets2025.Web.ValidationAttributes; // If you have custom validation attributes

namespace FlyTickets2025.web.Models
{
    public class TicketViewModel
    {
        //// Properties corresponding to your Ticket entity, but adjusted for UI needs

        //// Flight Selection
        //[Display(Name = "Flight")]
        //[Required(ErrorMessage = "Please select a flight.")]
        //public int FlightId { get; set; }
        //public SelectList? Flights { get; set; } // For the dropdown

        //// User (Client) Selection
        //[Display(Name = "Client")]
        //[Required(ErrorMessage = "Client is required.")]
        //public string? ApplicationUserId { get; set; } // Use string for Identity User Id
        //public SelectList? ApplicationUsers { get; set; } // For the dropdown

        //// Seat Selection
        //[Display(Name = "Seat")]
        //[Required(ErrorMessage = "Please select a seat.")]
        //public int SeatId { get; set; }
        //public SelectList? Seats { get; set; } // For the dropdown

        //[Display(Name = "Passenger Type")]
        //[Required(ErrorMessage = "Passenger type is required.")]
        //public PassengerType PassengerType { get; set; } 

        //[Display(Name = "Booked")]
        //public bool IsBooked { get; set; } // Will be pre-set in controller, read-only in view

        //[Display(Name = "Purchase Date")]
        //[Required(ErrorMessage = "Purchase date is required.")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public DateTime PurchaseDate { get; set; }

        //[Display(Name = "Ticket Price")]
        //[Required(ErrorMessage = "Ticket price is required.")]
        //[Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        //[DataType(DataType.Currency)]
        //public decimal TicketPrice { get; set; }

        //[Display(Name = "Client Name")]
        //[Required(ErrorMessage = "Client name is required.")]
        //[StringLength(100, ErrorMessage = "Client name cannot exceed 100 characters.")]
        //public string? ClientName { get; set; }

        //[Display(Name = "Flight Date")]
        //[Required(ErrorMessage = "Flight date is required.")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public DateTime FlightDate { get; set; }

        //// Optionally, add properties to display related data for selected flight/user/seat
        //public Flight? SelectedFlightDetails { get; set; }

        // --- Data Submitted by the Form ---

        public int FlightId { get; set; }

        [Display(Name = "Seat")]
        [Required(ErrorMessage = "Please select a seat.")]
        public int SeatId { get; set; }

        [Display(Name = "Passenger Type")]
        [Required(ErrorMessage = "Passenger type is required.")]
        public PassengerType PassengerType { get; set; }

        [Display(Name = "Ticket Price")]
        [DataType(DataType.Currency)]
        public decimal FinalPrice { get; set; }

        [Display(Name = "Passenger Full Name")]
        [Required(ErrorMessage = "Passenger's full name is required.")]
        [StringLength(200)]
        public string PassengerFullName { get; set; }

        [Display(Name = "Passenger Email")]
        [Required(ErrorMessage = "Passenger's email is required.")]
        [EmailAddress]
        public string PassengerEmail { get; set; }

        [Display(Name = "Document Type")]
        [Required(ErrorMessage = "Passenger's document type is required.")]
        public DocumentType PassengerDocumentType { get; set; }

        [Display(Name = "Document Number")]
        [Required(ErrorMessage = "Passenger's document number is required.")]
        [StringLength(50)]
        public string PassengerDocumentNumber { get; set; }

        // --- Data for Displaying Info on the Page ---

        public Flight? SelectedFlightDetails { get; set; }
        public List<Seat>? AvailableSeats { get; set; }
        public ApplicationUser? CurrentUser { get; set; }
    }
}