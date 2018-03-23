namespace Logic

    open Infrastructure
    open Domain
    open System

    type Entity() = 

        static let validateAge age =  if age <= 0 || age >= 100 then Failure "Invalid Age." else Success age
    
        static let validateSpecialty specialty = 
            match specialty with
            |Geriatrics | Pediatrics | General -> Success specialty
            | _ -> Failure "Invalid Specialty."    

        static let createClient (firstName, lastName, age) = 
            let result = Validate.Entity<int>(firstName, lastName, age, validateAge)
            match result with 
            | Success age ->
                printfn "Creating Client: %s %s"  firstName lastName 
                Success (new Client(new FullName(firstName, lastName), age))
            | Failure message ->
                Failure message
    
        static let createStaff (firstName, lastName, specialty) = 
            let result = Validate.Entity<Specialty>(firstName, lastName, specialty, validateSpecialty)
            match result with 
            | Success spec ->
                printfn "Creating Staff: %s %s"  firstName lastName 
                Success (new Staff(new FullName(firstName, lastName), spec))
            | Failure message ->
                Failure message

        static let createSchedule(startDate: DateTime, endDate: DateTime) = 
            let numOfDays =  (startDate - endDate).TotalDays
            match numOfDays with
            | 0.0 -> 
                seq ([|new Schedule(startDate, endDate)|]) 
            
            | 1.0 ->
                let date = startDate.Date.AddDays(1.0).AddSeconds(-1.0)
                let dates = [(startDate, date); (date.AddMinutes(1.0), endDate)]
                dates 
                    |> Seq.map(fun(startDate, endDate) -> new Schedule(startDate, endDate))

            | _ -> Seq.empty 

        static let createVisit(client, staff, startDate:DateTime, endDate:DateTime) = 
        
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

            let schedules = createSchedule(startDate, endDate)
        
            new Visit(client, staff,schedules)

        static member CreateClient(firstName, lastName, age) = 
            createClient(firstName, lastName, age)

        static member CreateStaff(firstName, lastName, specialty) = 
            createStaff(firstName, lastName, specialty)   

        static member ScheduleVisit(client, staff, startDate, endDate) = 
            createVisit(client, staff, startDate, endDate)