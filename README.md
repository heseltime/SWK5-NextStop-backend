> [!NOTE]  
> Documentation Part 1 in Readme in SWK5-NextStop Sub-Project/-Directory 
> 
> **Status: Up to Date w/ 2nd Hand-In December 2024**

![bus-display-sketch.png](img/bus-display-sketch.png)

# SWK5 NextStop (Jack Heseltine)

This is a GitHub (Classroom) managed project.

[As documented in Part 1](https://github.com/swk5-2024ws/nextstop-bb-g2-heseltine/tree/main/SWK5-NextStop), this project aims to fulfill requirements adapting the project to a use case from real life, a sort of event management system using bus stops symbolically for showing what is going on inside a larger institution (its physical site), in this case the Katholische Hochschulgemeinde in Linz, located at Johannes Kepler University.

## Part 1 (Ausbaustufe 1)

[Documented in SWK5-NextStop](https://github.com/swk5-2024ws/nextstop-bb-g2-heseltine/tree/main/SWK5-NextStop): Here just the headings (overview).
- **Project Structure**

- **Data Model**
    - *Entity Relationships*
    - *Additional Info About Relationships*
    - *Table-Creation Script*
    - *Initial Testdata*

- **Data Access Layer (DAL)**
    - *Initial DAL-Testing*
    - *Database Technology Choice*

## Part 2 

The main focus of this part is adding the controllers, DTOs services in subdirectories to the main project, reflecting the main functionality subdivided in terms of Model and Controller, and also more sophisticated DTOs (Data Transfer Objects), building out the solution in a scalable and flexible manner.

### Main Project
- **Controller/**  
  Handles HTTP requests and maps them to services.

- **Service/**  
  Contains business **logic** and orchestrates interactions with repositories.
- 
- also contains **DTO/**  
  Stores DTOs for transferring data between layers.

This project ends up with all the higher level code in this way, whereas the others have the lower-level functionality like reposiory, domain (see the following), infrastructure. Test remains in a project of its own, reflecting (logically) the cross-layer concern.

Example DTO for a Domain class: **Holiday**, here the company (registering the holiday) is abstracted away by the DTO.

```csharp
public class HolidayDTO
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public bool IsSchoolBreak { get; set; }
}
```

```csharp
public class Holiday
{
    public int Id  { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public bool IsSchoolBreak { get; set; }
    public int CompanyId { get; set; }
}
```

A Mapper class (containing `ToDTO`/`ToDomain` methods), now also added to the main project (thus containing all the core API functionality), mediates between Domain and DTO: default values are not set, as a side-note.

### Testing Project

**Controller-integration tests** (thereby also covering Services, DTOs and Mappers) were developed as part of Part 2: interesting here, since the resources are accessed over the network, this could be a separately developed solution as well, but is submitted as part of the one-solution hand-in for this class.

### Authentication

With JWT, OAuth & Open ID connect, using Auth0 Token Authority. Backend testing workflow: test the endpoint unauthenticated in Swagger, compare to token-authorized call in Postman.

### Route vs Schedule

Schedule is what is needed for task number #4: schedules contain routes and route-stop-schedules.

### Submission Status "Ausbaustufe 2"

Everything up to and including CheckIns: displaying these and making statistics was not implemented. Some debugging and testing might need to be added post-submission (not for credit).

# Projekt-Dokumentation

## 1. Für welches Datenmodell haben Sie sich entschieden? ER-Diagramm, etwaige Besonderheiten erklären.
We used an ER model centered around entities like `Schedule`, `Route`, `Stop`, and `CheckIn`. Key relationships include:
- `Route` has many `Stops` (via `RouteStopSchedule`).
- `Schedule` links `Routes` with specific dates and times.
- `CheckIn` validates bus stops for schedules and routes using API keys.
  Unique features include modular handling of `RouteStopSchedule` for route-specific timings.

---

## 2. Dokumentieren Sie auf Request-Ebene den gesamten Workflow anhand eines möglichst durchgängigen Beispiels.
1. **Add Stops**:
  - **POST** `/api/Stop`
  - **Body**:
    ```json
    { "name": "Stop A", "shortName": "A", "gpsCoordinates": "48.210033,16.363449" }
    ```

2. **Add Holidays**:
  - **POST** `/api/Holiday`
  - **Body**:
    ```json
    { "description": "Christmas", "date": "2024-12-25", "isSchoolBreak": true }
    ```

3. **Add Schedule**:
  - **POST** `/api/Schedule`
  - **Body**:
    ```json
    { "routeId": 1, "validityStart": "2024-01-01", "validityStop": "2024-12-31", "date": "2024-06-15", "routeStopSchedules": [ { "stopId": 1, "sequenceNumber": 1, "time": "08:00" } ] }
    ```

4. **Check In**:
  - **POST** `/api/Schedule/checkIn`
  - **Body**:
    ```json
    { "scheduleId": 1, "routeId": 1, "stopId": 1, "dateTime": "2024-06-15T08:05:00", "apiKey": "valid-key" }
    ```

---

## 3. Wie stellen Sie sicher, dass das Einchecken der Busse nur mit einem gültigen API-Key möglich ist?
API keys are validated using the `IApiKeyValidator` service. Only preconfigured valid keys stored in the system (e.g., via `appsettings.json`) are accepted.

---

## 4. Ein Angreifer hat Zugriff auf ein Datenbank-Backup Ihres Produktivsystems bekommen. Welchen Schaden kann er anrichten?
The attacker could:
- Extract schedule and route data.
- Access check-in data, including API keys if not encrypted.
  Mitigation: Sensitive data like API keys is stored encrypted.

---

## 5. Bei welchen Teilen Ihres Systems ist eine korrekte Funktionsweise aus Ihrer Sicht am kritischsten? Welche Maßnahmen haben Sie getroffen, um sie zu gewährleisten?
Critical parts:
- API Key validation: Ensured with unit tests and proper key management.
- Schedule and route logic: Validated with integration tests and plausibility checks on database queries.

---

## 6. Wie haben Sie die Berechnung passender Routen bei Fahrplanabfragen modular umgesetzt?
The `RouteStopSchedule` is decoupled from `Schedule`. Adding or updating route calculation logic only requires changes in the repository and query methods.

---

## 7. Welche Prüfungen führen Sie bei einem Check-in (der Standortdaten eines Busses) durch, um die Korrektheit der Daten zu gewährleisten?
- Validates `ScheduleId`, `RouteId`, and `StopId` against the database.
- Ensures `DateTime` is within the `Schedule.ValidityStart` and `ValidityStop` range.
- Confirms the API key's validity.

---

## 8. Wie stellen Sie sicher, dass Ihre API auch in außergewöhnlichen Konstellationen stets korrekte Fahrplandaten liefert?
Edge cases like year transitions, daylight savings, and holidays are tested:
- Time handling uses `TimeOnly` and `DateOnly` to prevent ambiguity.
- Holidays are flagged and linked with schedules.

---

## 9. Bei der Übertragung eines API-Keys auf einen Bus ist etwas schiefgelaufen.
**a)** Monitoring tracks invalid API key errors and alerts the team.  
**b)** Logs store the last successful check-in, enabling issue traceability.

---

## 10. Denken Sie an die Skalierbarkeit Ihres Projekts. Was macht Ihnen am meisten Kopfzerbrechen?
Handling high-frequency requests for real-time data like check-ins or schedule queries could strain the database. Solutions include caching frequently accessed data and using distributed databases.

---

## 11. Wenn Sie das Projekt neu anfangen würden – was würden Sie anders machen?
- Introduce GraphQL for flexible queries (group part however)
- Implement more extensive test coverage earlier in the project.

---

**On-going documentation**:

## Outside Resources

- https://www.youtube.com/watch?v=yGwGLcqcJ6Q
- https://dev.to/kasuken/securing-net-6-minimal-api-with-auth0-4h5f