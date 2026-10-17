using SmartNav.Domain.Buildings;

namespace SmartNav.Infrastructure.Seeding;

/// <summary>Global, tenant-agnostic defaults. Tenants may add their own categories on top.</summary>
internal static class ReferenceData
{
    public static IEnumerable<LocationCategory> GlobalCategories()
    {
        string[][] items =
        [
            ["ENTRY","Entrance","door-open"], ["EXIT","Exit","door-closed"], ["RECEPTION","Reception","concierge-bell"],
            ["LOBBY","Lobby","sofa"], ["DEPARTMENT","Department","building"], ["OFFICE","Office","briefcase"],
            ["ROOM","Room","door"], ["CABIN","Cabin","square"], ["SHOP","Shop","shopping-bag"],
            ["RESTAURANT","Restaurant","utensils"], ["CAFE","Cafe","coffee"], ["PHARMACY","Pharmacy","pill"],
            ["MEDICAL_STORE","Medical Store","pill"], ["LAB","Lab","flask"], ["EMERGENCY","Emergency","siren"],
            ["BILLING","Billing","receipt"], ["WASHROOM","Washroom","toilet"], ["LIFT","Lift","arrow-up-down"],
            ["STAIR","Stairs","stairs"], ["ESCALATOR","Escalator","trending-up"], ["CORRIDOR","Corridor","move"],
            ["PARKING","Parking","car"], ["ATM","ATM","banknote"], ["INFORMATION_DESK","Information Desk","info"],
            ["CLASSROOM","Classroom","school"], ["LECTURE_HALL","Lecture Hall","presentation"], ["LIBRARY","Library","library"],
            ["ADMINISTRATION","Administration","landmark"], ["HOTEL_ROOM","Hotel Room","bed"],
            ["CONFERENCE_ROOM","Conference Room","users"], ["GOVERNMENT_DEPARTMENT","Government Department","landmark"],
            ["SECURITY","Security","shield"], ["OTHER","Other","map-pin"],
        ];
        var order = 0;
        foreach (var i in items)
            yield return new LocationCategory { Code = i[0], Name = i[1], Icon = i[2], SortOrder = order += 10 };
    }
}
