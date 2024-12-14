> [!NOTE]  
> Documentation Part 1 in Readme in SWK5-NextStop Sub-Project/-Directory 
> 
> **Status: Up to Date w/ 1st Hand-In November 2024**

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

The main focus of this part is adding the controllers and services in subdirectories to the main project, reflecting the main functionality subdivided in terms of Model and Controller, and also more sophisticated DTOs (Data Transfer Objects), 
in the appropriate subdirectory to the Domain (project), building out the solution in a scalable and flexible manner.

### Main Project
- **Controller/**  
  Handles HTTP requests and maps them to services.

- **Service/**  
  Contains business logic and orchestrates interactions with repositories.

This project ends up with all the higher level code in this way, whereas the others have the lower-level functionality like reposiory, domain (see the following), infrastructure. Test remains in a project of its own, reflecting (logically) the cross-layer concern.

### Domain Project
- also contains **DTOs/**  
  Stores DTOs for transferring data between layers.

### Concept Questions Answered

- 

**On-going documentation**:

## Outside Resources

- https://www.youtube.com/watch?v=yGwGLcqcJ6Q