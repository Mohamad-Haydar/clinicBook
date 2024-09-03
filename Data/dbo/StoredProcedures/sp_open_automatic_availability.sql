CREATE OR REPLACE PROCEDURE sp_open_automatic_availability()
LANGUAGE plpgsql
AS $$
-- Step 1: Define variables
DECLARE
	current_date DATE := CURRENT_DATE;
    doctor_availability_row RECORD;
    new_available_date DATE;
    nb_of_open_availability INT;
BEGIN
    -- Step 2: Loop through availabilities for today
    FOR doctor_availability_row IN
        SELECT * FROM doctoravailability
        WHERE availabledate = current_date
          AND repetitiondelay IS NOT NULL
    LOOP
        -- Get the number of open availability
        nb_of_open_availability := doctor_availability_row.nbofopenavailability;

        -- loop over the number of needed open availability
        FOR i IN 1..nb_of_open_availability LOOP
             new_available_date := current_date + (doctor_availability_row.repetitiondelay * i * 7);

            -- If no availability exists for the new date, create one
            IF NOT EXISTS (
                SELECT 1
                FROM doctoravailability
                WHERE doctorid = doctor_availability_row.doctorid
                AND availabledate = new_available_date
            ) THEN
                INSERT INTO doctoravailability (availabledate, dayname, starthour, endhour, maxclient, currentreservations, doctorid, repetitiondelay, nbofopenavailability)
                VALUES (new_available_date, doctor_availability_row.dayname, doctor_availability_row.starthour, doctor_availability_row.endhour, 
                        doctor_availability_row.maxclient, 0, doctor_availability_row.doctorid, doctor_availability_row.repetitiondelay, doctor_availability_row.nbofopenavailability);
            END IF;
        END LOOP;
    END LOOP;
    
END;
$$;