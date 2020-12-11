using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnePortal.Models.ViewModels
{
    public class Complain
    {
        public string Priority { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }
    }
    

    public class Suggestion
    {
        public string Title { get; set; }
    }

    public enum Resolve
    {
        NotResolved, AdminResolved, Resolved
    }

    public enum Priority
    {
        High, Medium, Low
    }

    public class Policy
    {
        public string DisplayName { get; set; }
        public string FilePath { get; set; }
        public string CreateDate { get; set; }
    }

    public class CoreValues
    {
        public int id { get; set; }
        public string title { get; set; }
        public string definition { get; set; }
        public string body { get; set; }
        public string howHeader { get; set; }
        public string how { get; set; }
    }


    public class Room
    {
        public int id { get; set; }
        public string title { get; set; }
        public string definition { get; set; }
        public string body { get; set; }
        public string howHeader { get; set; }
        public string how { get; set; }
    }

    public class Laundry
    {
        public int Id { get; set; }

        public string ServiceType { get; set; }
        public string  GuestEmail{ get; set; }
        public string Description { get; set; }
        public DateTime CollectionDate { get; set; }
        public  DateTime  DeliveryDate { get; set; }
        public string GuestName { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string Deleted { get; set; }
        public string EmployeeName { get; set; }


    }
}
