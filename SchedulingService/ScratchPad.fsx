open System

module Infrastructure = 

    type Result<'TSuccess, 'TFailure> = 
        | Success of 'TSuccess
        | Failure of 'TFailure
    
    type Validate() = 
        static member Instance obj = if (obj <> null) then Success obj else Failure "Value cannot be null."
        
        static member Entity<'TType>(firstName:string, lastName:string, prop:'TType, validatePropFunc: ('TType -> Result<'TType,string>)) = 
            if String.IsNullOrWhiteSpace(firstName) then Failure "First Name cannot be empty."
            else if String.IsNullOrWhiteSpace(lastName) then Failure "Last Name cannot be empty."
            else
                let result = validatePropFunc prop
                match result with
                    | Success s ->
                        Success prop
                    | Failure f ->
                        Failure f

module Domain = 
    
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

module Logic = 
    open Infrastructure  
    open Domain   

    let private validateAge age =  if age <= 0 || age >= 100 then Failure "Invalid Age." else Success age
    
    let private validateSpecialty specialty = 
        match specialty with
        |Geriatrics | Pediatrics | General -> Success specialty
        | _ -> Failure "Invalid Specialty."    

    let private CreateClient (firstName, lastName, age) = 
        let result = Validate.Entity<int>(firstName, lastName, age, validateAge)
        match result with 
        | Success age ->
            printfn "Creating Client: %s %s"  firstName lastName 
            Success (new Client(new FullName(firstName, lastName), age))
        | Failure message ->
            Failure message
    
    let private CreateStaff (firstName, lastName, specialty) = 
        let result = Validate.Entity<Specialty>(firstName, lastName, specialty, validateSpecialty)
        match result with 
        | Success spec ->
            printfn "Creating Staff: %s %s"  firstName lastName 
            Success (new Staff(new FullName(firstName, lastName), spec))
        | Failure message ->
            Failure message

    let private CreateSchedule(startDate: DateTime, endDate: DateTime) = 
        let numOfDays =  (startDate - endDate).TotalDays
        match numOfDays with
        | 0.0 -> 
            seq ([|new Schedule(startDate, endDate)|]) 
            
        | 1.0 ->
            let date = startDate.Date.AddDays(1.0).AddSeconds(-1.0)
            let dates = [(startDate, date); (date.AddMinutes(1.0), endDate)]
            dates 
                |> Seq.map(fun(startDate, endDate) -> new Schedule(startDate, endDate))

        | _ -> Seq.empty //raise(new InvalidOperationException("Schedules cannot span more than 1 day.")

    let private CreateVisit(client, staff, startDate:DateTime, endDate:DateTime) = 
        
        let result = Validate.Instance(client) 
        match result with 
        | Failure message -> 
            raise(new ArgumentNullException("Client cannot be null.")) |> ignore
        | _-> () |> ignore

        let result' = Validate.Instance(staff)
        match result' with 
        | Failure message -> raise(new ArgumentNullException("Staff cannot be null.")) |> ignore
        | _-> () |> ignore
        
        if startDate > endDate then raise(new ArgumentOutOfRangeException("Invalid Dates."))

        let schedules = CreateSchedule(startDate, endDate)
        
        new Visit(client, staff,schedules)


    type Entity() = 

        static member CreateClient(firstName, lastName, age) = 
            CreateClient(firstName, lastName, age)

        static member CreateStaff(firstName, lastName, specialty) = 
            CreateStaff(firstName, lastName, specialty)   

        static member ScheduleVisit(client, staff, startDate, endDate) = 
            CreateVisit(client, staff, startDate, endDate)


module Client =
    open Logic
    open Domain
    open Infrastructure
    
    let clientResult = Entity.CreateClient("Tony", "Stark", 40)
    let client = 
        match clientResult with
        | Success client -> 
            let staffResult = Entity.CreateStaff("Pepper", "Potts", Specialty.General)
            match staffResult with 
            | Success staff ->                
                let visit = Entity.ScheduleVisit(client, staff, DateTime.Now, DateTime.Now.AddHours(1.0))
                printfn "Visit Scheduled: Client: %s, %s - Staff: %s, %s - Date:" 
                        (visit.Client.Name.FirstName) (visit.Client.Name.LastName) (visit.Staff.Name.FirstName) (visit.Staff.Name.LastName) 
            | Failure message ->
                raise (new Exception(message))
                

        | Failure m ->  
            raise (new Exception(m))
    

