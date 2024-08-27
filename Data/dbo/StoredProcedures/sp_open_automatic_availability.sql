CREATE OR REPLACE PROCEDURE sp_open_automatic_availability()
LANGUAGE plpgsql
AS $$
-- Step 1: Define variables
DECLARE
	current_date DATE := CURRENT_DATE;
    repetition_record RECORD;
    new_available_date DATE;
    existing_availability_count INT;
BEGIN
    
    -- Step 2: Loop through availabilities for today
    FOR repetition_record IN
        SELECT * FROM doctoravailability
        WHERE availabledate = current_date
          AND repetitiondelay IS NOT NULL
    LOOP
        -- Calculate the next start date based on the repetition delay
        new_available_date := current_date + (repetition_record.repetitiondelay * 7);

        -- Check if there is already an availability on the new start date
        SELECT COUNT(*) INTO existing_availability_count
        FROM doctoravailability
        WHERE doctorid = repetition_record.doctorid
          AND availabledate = new_available_date;

        -- If no availability exists for the new start date, create one
        IF existing_availability_count = 0 THEN
            INSERT INTO doctoravailability (availabledate, dayname, starthour, endhour, maxclient, currentreservations, doctorid, repetitiondelay)
            VALUES (new_available_date, repetition_record.dayname, repetition_record.starthour, repetition_record.endhour, 
                    repetition_record.maxclient, 0, repetition_record.doctorid, repetition_record.repetitiondelay);
        END IF;
    END LOOP;
    
END;
$$;