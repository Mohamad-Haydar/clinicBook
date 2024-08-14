CREATE OR REPLACE PROCEDURE sp_delete_old_availability()
LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM doctoravailability
    WHERE availabledate < CURRENT_DATE;
END;
$$;