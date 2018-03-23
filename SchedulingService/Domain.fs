namespace Domain
    
    open System

    type FullName (firstName:string, lastName:string) = 
        member this.FirstName = firstName
        member this.LastName = lastName

    type Specialty = Geriatrics | Pediatrics | General

    [<AllowNullLiteral>]
    type Client (name:FullName, age:int) = 
        member this.Name = name
        member this.Age = age 
        
    [<AllowNullLiteral>]
    type Staff (name:FullName, specialty:Specialty) = 
        member this.Name = name
        member this.Specialty = specialty
    
    type Person = Client | Staff 

    type Schedule(startDate:DateTime, endDate:DateTime) =
        member this.StartDate = startDate
        member this.EndDate = endDate

    [<AllowNullLiteral>]
    type Visit(client:Client, staff:Staff, schedule:Schedule seq) = 
        member this.Client = client
        member this.Staff = staff
        member this.Schedules  = schedule
