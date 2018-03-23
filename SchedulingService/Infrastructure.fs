namespace Infrastructure 
    
    open System

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

