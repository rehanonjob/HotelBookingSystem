 Hotel Room Booking System

 1. Project Overview

The Hotel Room Booking System is an ASP.NET Core MVC web application developed using C#, Entity Framework Core, and SQL Server.

The application allows guests to browse hotel rooms, search for room availability, make bookings, and view their booking history. Hotel administrators can manage rooms, view guest bookings, update booking statuses, and create additional administrator accounts.

2. Technology Stack

- **Backend:** C#, ASP.NET Core MVC
- **Frontend:** HTML, CSS, JavaScript, Bootstrap 5
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core
- **Authentication:** Cookie-based Authentication
- **IDE:** Visual Studio

 3. Features Implemented

 Guest Features
- Guest registration and login.
- Browse available hotel rooms.
- View room details.
- Search rooms by check-in date, check-out date, and room type.
- Check room availability.
- Book hotel rooms.
- View booking confirmation.
- View personal booking history.

 Admin Features
- Secure administrator login.
- Admin dashboard with booking and room statistics.
- Add, edit, activate, and deactivate rooms.
- View all guest bookings.
- View individual booking details.
- Confirm and cancel bookings.
- Create additional administrator accounts.

 Booking Rules
- Check-out date must be after check-in date.
- Booking dates must be valid.
- Inactive rooms cannot be booked.
- Pending and Confirmed bookings block overlapping reservations.
- Cancelled bookings do not block room availability.
- Booking amount is calculated using the room price stored in the database.

 4. Project Structure

```text
HotelBookingSystem/
├── src/
│   └── HotelBookingSystem/
├── database/
│   ├── HotelBookingDB.bak
│   ├── HotelBookingDB.sql
│   └── ER-Diagram.png
├── screenshots/
│   ├── Desktop/
│   └── Mobile/
├── README.md
└── AI_TOOLS_USED.md
```

 5. Database Setup

1. Open SQL Server Management Studio.
2. Restore `HotelBookingDB.bak`, or execute `HotelBookingDB.sql` to create the database.
3. Update the connection string in `appsettings.json` according to your SQL Server instance.
4. Ensure the application can connect to the restored database.

 6. How to Run the Application

1. Open the `.sln` file from the `src` folder using Visual Studio.
2. Restore the NuGet packages if required.
3. Verify the SQL Server connection string.
4. Build the solution.
5. Run the application using Visual Studio.

 7. Test Login Credentials

 Guest Account
- Email: testguest@example.com
- Password: Test@12345
 Admin Account
- Email: cr7@gmail.com
- Password: 123456789
 8. Design Decisions

- Used ASP.NET Core MVC to separate application logic, models, and views.
- Used Entity Framework Core for database operations.
- Implemented two roles: Guest and Admin.
- Used cookie authentication and role-based authorization.
- Used server-side validation for booking dates and room availability.
- Calculated booking amounts using database room prices rather than trusting browser-submitted values.
- Used Bootstrap for responsive desktop and mobile layouts.
- Used Pending, Confirmed, and Cancelled booking statuses.

AI Tools Used
- ChatGPT for debugging assistance.
- Copilot for code suggestions and refactoring.
- Claude for planning and design discussions.